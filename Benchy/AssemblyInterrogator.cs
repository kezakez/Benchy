using System;
using System.Collections.Generic;
using System.Reflection;

namespace Benchy
{
    public class AssemblyInterrogator
    {
        public AssemblyInterrogator() { }

        public AssemblyInterrogator(string assemblyName)
        {
            AssemblyName = assemblyName;
        }

        public virtual string AssemblyName { get; private set; }

        private List<object> _itemsToBench = new List<object>();
        public virtual List<object> ItemsToBench { get { return _itemsToBench; } set { _itemsToBench = value; } }

        public virtual bool PopulateItemsToBench(out string result)
        {
            try
            {
                AssemblyName name = System.Reflection.AssemblyName.GetAssemblyName(AssemblyName);
                Assembly assembly = Assembly.Load(name.FullName);
                Type[] types = assembly.GetTypes();
                foreach (var type in types)
                {
                    var attributes = type.GetCustomAttributes(true);
                    foreach (var attribute in attributes)
                    {
                        if (attribute.GetType() == typeof(Framework.BenchmarkFixtureAttribute))
                        {
                            var benchObject = assembly.CreateInstance(type.FullName);
                            ItemsToBench.Add(benchObject);
                        }
                    }
                }
                result = "";
                return true;
            }
            catch (Exception e)
            {
                result = e.ToString();
                return false;
            }
        }
    }
}
