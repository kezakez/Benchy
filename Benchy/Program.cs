using System;
using Benchy;

namespace PEL.Benchmark
{
    class Program
    {
        static void Main(string[] args)
        {
            // get the settings
            var settings = new Settings();
            string errors;
            bool success = settings.ProcessParameters(args, out errors);
            var outputWriter = new OutputWriter(settings.OutputDirectory);
            if (success)
            {
                // load up the required assembly and run the benchmarks and output the results
                var executer = new BenchmarkExecuter(settings, new AssemblyInterrogator(settings.BenchmarkDll), outputWriter, new ProcessStarterFactory());
                if (executer.RunBenchmarks(out errors))
                {
                    return;
                }
            }

            outputWriter.WriteError(errors, settings.BenchmarkMethod, settings.BuildLabel);

            if (settings.Pause)
            {
                Console.WriteLine("Press Enter to continue.");
                Console.ReadLine();
            }
        }
    }
}
