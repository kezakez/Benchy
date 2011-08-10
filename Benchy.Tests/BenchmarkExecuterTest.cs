using System;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using PEL.Benchmark;

namespace Benchy.Tests
{
    [TestFixture]
    public class BenchmarkExecuterTest
    {
        [Test]
        public void RunBenchmarks_Should_RunEachTestInAExe_When_NoTestNameSpecified()
        {
            var settings = new Settings { BenchmarkDll = "TestBenchmark.dll", BuildLabel = "Build1", BenchmarkMethod = "" };

            var assemblyInterrogator = new Mock<AssemblyInterrogator>();
            assemblyInterrogator.Setup(p => p.AssemblyName).Returns("TestBenchmark.dll");
            var items = new List<Object>();
            items.Add(new TestBenchmarkClass());
            string errors;
            assemblyInterrogator.Setup(p => p.PopulateItemsToBench(out errors)).Returns(true);
            assemblyInterrogator.Setup(p => p.ItemsToBench).Returns(items);

            var outputWriter = new Mock<OutputWriter>();

            var processStarter = new Mock<ProcessStarterFactory>();
            processStarter.Setup(p => p.Execute(It.IsAny<string>(), out errors)).Returns(true);

            var target = new BenchmarkExecuter(settings, assemblyInterrogator.Object, outputWriter.Object, processStarter.Object);
            bool result = target.RunBenchmarks(out errors);
            
            Assert.AreEqual(true, result);
            string t1;
            var expectedString1 = settings.WriteParameterString("TestBenchy1");
            processStarter.Verify(p => p.Execute(expectedString1, out t1), Times.Once());
            string t2;
            var expectedString2 = settings.WriteParameterString("TestBenchy2");
            processStarter.Verify(p => p.Execute(expectedString2, out t2), Times.Once());
            string t3;
            processStarter.Verify(p=>p.Execute(It.IsAny<string>(), out t3), Times.Exactly(2));
        }

        [Test]
        public void RunBenchmarks_Should_RunBenchmarkAndCallSetUpAndTearDown_When_TestNameSpecified()
        {
            var assemblyInterrogator = new Mock<AssemblyInterrogator>();
            assemblyInterrogator.Setup(p => p.AssemblyName).Returns("TestBenchmark.dll");
            var items = new List<Object>();
            var testObject = new TestBenchmarkClass();
            items.Add(testObject);
            string errors;
            assemblyInterrogator.Setup(p => p.PopulateItemsToBench(out errors)).Returns(true);
            assemblyInterrogator.Setup(p => p.ItemsToBench).Returns(items);

            var outputWriter = new Mock<OutputWriter>();

            var processStarter = new Mock<ProcessStarterFactory>();

            var target = new BenchmarkExecuter(new Settings { BuildLabel = "Build1", BenchmarkMethod = "TestBenchy1" }, assemblyInterrogator.Object, outputWriter.Object, processStarter.Object);
            string result;

            target.RunBenchmarks(out result);

            outputWriter.Verify(o => o.WriteResults("TestBenchy1", "Build1", It.IsAny<long>()), Times.Once());
            Assert.AreEqual(true, testObject.SetUpCalled);
            Assert.AreEqual(true, testObject.TearDownCalled);
            Assert.AreEqual(true, testObject.TestBenchy1Called);
            Assert.AreEqual(false, testObject.TestBenchy2Called);
        }

        [Test]
        public void RunBenchmarks_Should_LogExceptionsButNotResults_When_TestNameSpecifiedAndExceptionsAreThrown()
        {
            var assemblyInterrogator = new Mock<AssemblyInterrogator>();
            assemblyInterrogator.Setup(p => p.AssemblyName).Returns("TestBenchmark.dll");
            var items = new List<Object>();
            var testObject = new TestBenchmarkClass();
            testObject.TestBenchy2ThrowException = true;
            items.Add(testObject);
            string errors;
            assemblyInterrogator.Setup(p => p.PopulateItemsToBench(out errors)).Returns(true);
            assemblyInterrogator.Setup(p => p.ItemsToBench).Returns(items);

            var outputWriter = new Mock<OutputWriter>();

            var processStarter = new Mock<ProcessStarterFactory>();

            var target = new BenchmarkExecuter( new Settings { BuildLabel = "Build1", BenchmarkMethod = "TestBenchy2" }, assemblyInterrogator.Object, outputWriter.Object, processStarter.Object);

            var result = target.RunBenchmarks(out errors);

            Assert.AreEqual(false, result);
            Assert.AreEqual(true, testObject.SetUpCalled);
            Assert.AreEqual(true, testObject.TearDownCalled);
            Assert.AreEqual(false, testObject.TestBenchy1Called);
            Assert.AreEqual(true, testObject.TestBenchy2Called);
            outputWriter.Verify(o => o.WriteResults("TestBenchy2", "Build1", It.IsAny<long>()), Times.Never());
            outputWriter.Verify(o => o.WriteError(It.IsAny<string>(), "TestBenchy2", "Build1"), Times.Once());
        }

    }
}