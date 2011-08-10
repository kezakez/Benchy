using System;

namespace Benchy.Framework
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class TearDownAttribute : Attribute
    {
    }
}