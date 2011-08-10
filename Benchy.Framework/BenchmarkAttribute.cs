using System;

namespace Benchy.Framework
{

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class BenchmarkAttribute : Attribute
    {
    }
}
