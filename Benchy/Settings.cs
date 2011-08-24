using System.IO;

namespace PEL.Benchmark
{
    public class Settings
    {
        public string BuildLabel { get; set; }

        public string BenchmarkDll { get; set; }

        public string BenchmarkClass { get; set; }

        public string BenchmarkMethod { get; set; }

        public string OutputDirectory { get; set; }

        public bool Pause { get; set; }

        public bool ProcessParameters(string[] args, out string result)
        {
            BenchmarkMethod = string.Empty;
            OutputDirectory = Directory.GetCurrentDirectory();

            result = string.Empty;

            if (args.Length == 0)
            {
                result =
                    @"Benchy.exe -benchmarkdll:""YourAssembly.dll"" -buildlabel:""build42"" [-benchmarkmethod:""MethodNameMarkedWithBenchmarkAttribute""] [-outputdirectory:""c:\buildoutput""] [-pause]";
                return false;
            }

            foreach (string arg in args)
            {
                int sep = arg.IndexOf(':');

                string key;
                string value = string.Empty;

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
                    case "buildlabel":
                        BuildLabel = value;
                        break;
                    case "benchmarkdll":
                        BenchmarkDll = value;
                        break;
                    case "benchmarkclass":
                        BenchmarkClass = value;
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

        public string WriteParameterString(string benchmarkClass, string benchmarkMethod)
        {
            string result =
                string.Format(
                    "-buildlabel:\"{0}\" -benchmarkdll:\"{1}\" -benchmarkclass:\"{2}\" -benchmarkmethod:\"{3}\" -outputdirectory:\"{4}\"",
                    BuildLabel, BenchmarkDll, benchmarkClass, benchmarkMethod, OutputDirectory);
            if (Pause)
            {
                result += result + " -pause";
            }
            return result;
        }
    }
}