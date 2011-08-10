using System.IO;

namespace PEL.Benchmark
{
    public class Settings
    {
        public bool ProcessParameters(string[] args, out string result)
        {
            BenchmarkMethod = string.Empty;
            OutputDirectory = Directory.GetCurrentDirectory();

            result = string.Empty;

            if (args.Length == 0)
            {
                result = @"Benchy.exe -benchmarkdll:""YourAssembly.dll"" -buildlabel:""build42"" [-benchmarkmethod:""MethodNameMarkedWithBenchmarkAttribute""] [-outputdirectory:""c:\buildoutput""] [-pause]";
                return false;
            }

            foreach (var arg in args)
            {
                var sep = arg.IndexOf(':');

                string key;
                var value = string.Empty;

                if (sep != -1)
                {
                    key = arg.Substring(0, sep);
                    value = arg.Substring(sep + 1);
                }
                else
                {
                    key = arg;
                }

                key = key.TrimStart(new[] {'/', '-'}).ToLower();
                value = value.Trim(new[] {'"'}).Trim();

                switch (key)
                {
                    case "benchmarkdll":
                        BenchmarkDll = value;
                        break;
                    case "buildlabel":
                        BuildLabel = value;
                        break;
                    case "benchmarkmethod":
                        BenchmarkMethod = value;
                        break;
                    case "outputdirectory":
                        OutputDirectory = value;
                        break;
                    case "pause":
                        Pause = true;
                        break;
                }
            }

            if (string.IsNullOrWhiteSpace(BenchmarkDll))
            {
                result = "The benchmarkdll file name is required.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(BuildLabel))
            {
                result = "The buildlabel parameter is required.";
                return false;
            }

            return true;
        }

        public string WriteParameterString(string benchmarkMethod = "")
        {
            var outMethodName = (!string.IsNullOrWhiteSpace(benchmarkMethod)) ? benchmarkMethod : BenchmarkMethod;
            var result = string.Format("-benchmarkdll:\"{0}\" -buildlabel:\"{1}\" -benchmarkmethod:\"{2}\" -outputdirectory:\"{3}\"", BenchmarkDll, BuildLabel, outMethodName, OutputDirectory);
            if (Pause)
            {
                result += result + " -pause";
            }
            return result;
        }

        public string BuildLabel { get; set; }

        public string BenchmarkDll { get; set; }

        public string BenchmarkMethod { get; set; }

        public string OutputDirectory { get; set; }

        public bool Pause { get; set; }
    }
}
