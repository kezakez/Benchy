using System;
using System.Diagnostics;
using System.Reflection;

namespace Benchy
{
    public class ProcessStarterFactory
    {
        public virtual bool Execute(string arguments, out string result)
        {
            try
            {
                Assembly runningAssembly = Assembly.GetEntryAssembly();
                string processName = runningAssembly.ManifestModule.FullyQualifiedName;
                Process process = Process.Start(processName, arguments);
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