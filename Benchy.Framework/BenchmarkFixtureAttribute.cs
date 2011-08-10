using System;

namespace Benchy.Framework
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public class BenchmarkFixtureAttribute : Attribute
    {
    }
}
