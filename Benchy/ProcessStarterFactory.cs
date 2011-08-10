using System;
using System.Reflection;

namespace Benchy
{
    public class ProcessStarterFactory
    {
        public virtual bool Execute(string arguments, out string result)
        {
            try
            {
                var runningAssembly = Assembly.GetEntryAssembly();
                var processName = runningAssembly.ManifestModule.FullyQualifiedName;
                var process = System.Diagnostics.Process.Start(processName, arguments);
                if (process != null)
                {
                    result = "Started " + processName + " " + arguments;
                    process.WaitForExit();
                    return true;
                }
                result = "Failed to start " + processName + " " + arguments;
                return false;
            }
            catch (Exception e)
            {
                result = "Failed to start Benchy " + arguments + " " + e;
                return false;
            }
        }
    }
}
