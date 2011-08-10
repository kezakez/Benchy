using System;
using System.Collections.Generic;
using System.Reflection;
using Benchy.Framework;
using PEL.Benchmark;

namespace Benchy
{
    public class BenchmarkExecuter
    {
        private readonly Settings _settings;
        private readonly AssemblyInterrogator _interrogator;
        private readonly OutputWriter _writer;
        private readonly ProcessStarterFactory _processStarter;
        public BenchmarkExecuter(Settings settings, AssemblyInterrogator interrogator, OutputWriter writer, ProcessStarterFactory processStarter)
        {
            _settings = settings;
            _interrogator = interrogator;
            _writer = writer;
            _processStarter = processStarter;
        }

        public bool RunBenchmarks(out string result)
        {
            var success = _interrogator.PopulateItemsToBench(out result);
            if (!success)
            {
                return false;
            }
            foreach (var item in _interrogator.ItemsToBench)
            {
                MethodInfo setUpMethod = null;
                var benchMethods = new List<MethodInfo>();
                MethodInfo tearDownMethod = null;
                var methods = item.GetType().GetMethods();
                foreach (var method in methods)
                {
                    var attributes = method.GetCustomAttributes(true);
                    foreach (var attribute in attributes)
                    {
                        if (attribute.GetType() == typeof(BenchmarkAttribute))
                        {
                            benchMethods.Add(method);
                        }
                        else if (attribute.GetType() == typeof(SetUpAttribute))
                        {
                            setUpMethod = method;
                        }
                        else if (attribute.GetType() == typeof(TearDownAttribute))
                        {
                            tearDownMethod = method;
                        }
                    }
                }

                foreach (var method in benchMethods)
                {
                    var foundTestName = method.Name;
                    if (string.IsNullOrWhiteSpace(_settings.BenchmarkMethod))
                    {
                        // start a process for each item to benchmark
                        if (!_processStarter.Execute(_settings.WriteParameterString(foundTestName), out result))
                        {
                            return false;
                        }
                    }
                    else
                    {
                        if (string.Equals(_settings.BenchmarkMethod, foundTestName, StringComparison.OrdinalIgnoreCase))
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
                                var output = e.ToString();
                                if (e is TargetInvocationException && e.InnerException is System.IO.FileNotFoundException)
                                {
                                    var fileNotFound = e.InnerException as System.IO.FileNotFoundException;
                                    output += fileNotFound.FusionLog;
                                }
                                _writer.WriteError(output, method.Name, _settings.BuildLabel);
                                return false;
                            }
                        }
                    }
                }
            }

            return true;
        }

        private long invokeBenchmarkMethod(object item, MethodInfo method)
        {
            var stopwatch = new System.Diagnostics.Stopwatch();
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