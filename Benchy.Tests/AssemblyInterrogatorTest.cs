using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NUnit.Framework;

namespace Benchy.Tests
{
    [TestFixture]
    public class AssemblyInterrogatorTest
    {
        [Test]
        public void GetObjects_Should_ReturnAnObjectForEachClass_When_ClassesAreMarkedUp()
        {
            // whoa, inception, test using the test assembly that has a class in it marked up.
            string testAssemblyName = Assembly.GetExecutingAssembly().ManifestModule.Name;

            var target = new AssemblyInterrogator(testAssemblyName);
            string errors;
            bool success = target.PopulateItemsToBench(out errors);
            List<Object> list = target.ItemsToBench;

            Assert.AreEqual(true, success);
            Assert.AreEqual(2, list.Count);
            Assert.IsTrue((from item in list where item is TestBenchmarkClass1 select item).Count() == 1);
            Assert.IsTrue((from item in list where item is TestBenchmarkClass2 select item).Count() == 1);
        }
    }
}