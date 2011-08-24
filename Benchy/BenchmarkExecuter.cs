using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using Benchy.Framework;
using PEL.Benchmark;

namespace Benchy
{
    public class BenchmarkExecuter
    {
        private readonly AssemblyInterrogator _interrogator;
        private readonly ProcessStarterFactory _processStarter;
        private readonly Settings _settings;
        private readonly OutputWriter _writer;

        public BenchmarkExecuter(Settings settings, AssemblyInterrogator interrogator, OutputWriter writer,
                                 ProcessStarterFactory processStarter)
        {
            _settings = settings;
            _interrogator = interrogator;
            _writer = writer;
            _processStarter = processStarter;
        }

        public bool RunBenchmarks(out string result)
        {
            bool success = _interrogator.PopulateItemsToBench(out result);
            if (!success)
            {
                return false;
            }
            foreach (object item in _interrogator.ItemsToBench)
            {
                MethodInfo setUpMethod = null;
                var benchMethods = new List<MethodInfo>();
                MethodInfo tearDownMethod = null;
                MethodInfo[] methods = item.GetType().GetMethods();
                foreach (MethodInfo method in methods)
                {
                    object[] attributes = method.GetCustomAttributes(true);
                    foreach (object attribute in attributes)
                    {
                        if (attribute.GetType() == typeof (BenchmarkAttribute))
                        {
                            benchMethods.Add(method);
                        }
                        else if (attribute.GetType() == typeof (SetUpAttribute))
                        {
                            setUpMethod = method;
                        }
                        else if (attribute.GetType() == typeof (TearDownAttribute))
                        {
                            tearDownMethod = method;
                        }
                    }
                }

                foreach (MethodInfo method in benchMethods)
                {
                    string foundTestClassName = item.GetType().Name;
                    string foundTestMethodName = method.Name;

                    if (!string.IsNullOrWhiteSpace(_settings.BenchmarkClass) &&
                        !string.IsNullOrWhiteSpace(_settings.BenchmarkMethod))
                    {
                        if (
                            string.Equals(_settings.BenchmarkClass, foundTestClassName,
                                          StringComparison.OrdinalIgnoreCase) &&
                            string.Equals(_settings.BenchmarkMethod, foundTestMethodName,
                                          StringComparison.OrdinalIgnoreCase))
                        {
                            // run the test
                            return RunBenchmarkMethodInCurrentProcess(setUpMethod, tearDownMethod, item, method);
                        }
                    }
                    else if (!string.IsNullOrWhiteSpace(_settings.BenchmarkClass) &&
                             string.IsNullOrWhiteSpace(_settings.BenchmarkMethod))
                    {
                        // run all tests in class
                        if (string.Equals(_settings.BenchmarkClass, foundTestClassName,
                                          StringComparison.OrdinalIgnoreCase))
                        {
                            if (
                                !_processStarter.Execute(
                                    _settings.WriteParameterString(foundTestClassName, foundTestMethodName),
                                    out result))
                            {
                                return false;
                            }
                        }
                    }
                    else if (string.IsNullOrWhiteSpace(_settings.BenchmarkClass) &&
                             string.IsNullOrWhiteSpace(_settings.BenchmarkMethod))
                    {
                        // run all tests in dll
                        if (
                            !_processStarter.Execute(
                                _settings.WriteParameterString(foundTestClassName, foundTestMethodName),
                                out result))
                        {
                            return false;
                        }
                    }
                    else
                    {
                        result = "MethodName provided without ClassName";
                        return false;
                    }
                }
            }

            return true;
        }

        private bool RunBenchmarkMethodInCurrentProcess(MethodInfo setUpMethod, MethodInfo tearDownMethod, object item,
                                                        MethodInfo method)
        {
            try
            {
                try
                {
                    if (setUpMethod != null)
                    {
                        setUpMethod.Invoke(item, null);
                    }

                    long benchTime = invokeBenchmarkMethod(item, method);
                    _writer.WriteResults(method.Name, _settings.BuildLabel, benchTime);
                }
                finally
                {
                    if (tearDownMethod != null)
                    {
                        tearDownMethod.Invoke(item, null);
                    }
                }

                // one test per process only
                return true;
            }
            catch (Exception e)
            {
                string output = e.ToString();
                if (e is TargetInvocationException && e.InnerException is FileNotFoundException)
                {
                    // write out additional binding debug info when we cant load a dll
                    var fileNotFound = e.InnerException as FileNotFoundException;
                    output += fileNotFound.FusionLog;
                }
                _writer.WriteError(output, method.Name, _settings.BuildLabel);
                return false;
            }
        }

        private long invokeBenchmarkMethod(object item, MethodInfo method)
        {
            var stopwatch = new Stopwatch();
            try
            {
                stopwatch.Start();

                // benchmark the specified test
                method.Invoke(item, null);
            }
            finally
            {
                stopwatch.Stop();
            }
            return stopwatch.ElapsedMilliseconds;
        }
    }
}