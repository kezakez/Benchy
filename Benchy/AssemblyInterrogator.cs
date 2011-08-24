using System;
using System.Collections.Generic;
using System.Reflection;
using Benchy.Framework;

namespace Benchy
{
    public class AssemblyInterrogator
    {
        private List<object> _itemsToBench = new List<object>();

        public AssemblyInterrogator()
        {
        }

        public AssemblyInterrogator(string assemblyName)
        {
            AssemblyName = assemblyName;
        }

        public virtual string AssemblyName { get; private set; }

        public virtual List<object> ItemsToBench
        {
            get { return _itemsToBench; }
            set { _itemsToBench = value; }
        }

        public virtual bool PopulateItemsToBench(out string result)
        {
            try
            {
                AssemblyName name = System.Reflection.AssemblyName.GetAssemblyName(AssemblyName);
                Assembly assembly = Assembly.Load(name.FullName);
                Type[] types = assembly.GetTypes();
                foreach (Type type in types)
                {
                    object[] attributes = type.GetCustomAttributes(true);
                    foreach (object attribute in attributes)
                    {
                        if (attribute.GetType() == typeof (BenchmarkFixtureAttribute))
                        {
                            object benchObject = assembly.CreateInstance(type.FullName);
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