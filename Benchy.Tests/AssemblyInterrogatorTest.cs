using System;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;

namespace Benchy.Tests
{
    [TestFixture]
    public class AssemblyInterrogatorTest
    {
        [Test]
        public void GetObjects_Should_ReturnAnObjectForEachMethod_When_MethodsAreMarkedUp()
        {
            // whoa, inception, test using the test assembly that has a class in it marked up.
            var testAssemblyName = Assembly.GetExecutingAssembly().ManifestModule.Name;

            var target = new AssemblyInterrogator(testAssemblyName);
            string errors;
            bool success = target.PopulateItemsToBench(out errors);
            List<Object> list = target.ItemsToBench;

            Assert.AreEqual(true, success);
            Assert.AreEqual(1, list.Count);
            Assert.AreEqual(typeof(TestBenchmarkClass), list[0].GetType());
        }
    }
}
