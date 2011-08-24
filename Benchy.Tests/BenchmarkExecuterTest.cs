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
        public void RunBenchmarks_Should_ErrorOut_When_MethodSuppliedWithNoClassName()
        {
            var assemblyInterrogator = new Mock<AssemblyInterrogator>();
            assemblyInterrogator.Setup(p => p.AssemblyName).Returns("TestBenchmark.dll");
            var items = new List<Object>();
            var testObject = new TestBenchmarkClass1();
            testObject.SetUpThrowException = true;
            items.Add(testObject);
            string errors;
            assemblyInterrogator.Setup(p => p.PopulateItemsToBench(out errors)).Returns(true);
            assemblyInterrogator.Setup(p => p.ItemsToBench).Returns(items);
            var outputWriter = new Mock<OutputWriter>();
            var processStarter = new Mock<ProcessStarterFactory>();

            var target =
                new BenchmarkExecuter(
                    new Settings {BuildLabel = "Build1", BenchmarkClass = "", BenchmarkMethod = "TestBenchy2"},
                    assemblyInterrogator.Object, outputWriter.Object, processStarter.Object);
            bool result = target.RunBenchmarks(out errors);

            Assert.IsFalse(result);
            Assert.IsFalse(testObject.SetUpCalled);
            Assert.IsFalse(testObject.TestBenchy1Called);
            Assert.IsFalse(testObject.TestBenchy2Called);
            Assert.IsFalse(testObject.TearDownCalled);
        }

        [Test]
        public void
            RunBenchmarks_Should_LogExceptionsButNotResults_When_ClassAndMethodNameSpecifiedAndExceptionsAreThrown()
        {
            var assemblyInterrogator = new Mock<AssemblyInterrogator>();
            assemblyInterrogator.Setup(p => p.AssemblyName).Returns("TestBenchmark.dll");
            var items = new List<Object>();
            var testObject = new TestBenchmarkClass1();
            testObject.TestBenchy2ThrowException = true;
            items.Add(testObject);
            string errors;
            assemblyInterrogator.Setup(p => p.PopulateItemsToBench(out errors)).Returns(true);
            assemblyInterrogator.Setup(p => p.ItemsToBench).Returns(items);

            var outputWriter = new Mock<OutputWriter>();

            var processStarter = new Mock<ProcessStarterFactory>();

            var target =
                new BenchmarkExecuter(
                    new Settings
                        {BuildLabel = "Build1", BenchmarkClass = "TestBenchmarkClass1", BenchmarkMethod = "TestBenchy2"},
                    assemblyInterrogator.Object, outputWriter.Object, processStarter.Object);
            bool result = target.RunBenchmarks(out errors);

            Assert.IsFalse(result);
            Assert.IsTrue(testObject.SetUpCalled);
            Assert.IsTrue(testObject.TearDownCalled);
            Assert.IsFalse(testObject.TestBenchy1Called);
            Assert.IsTrue(testObject.TestBenchy2Called);
            outputWriter.Verify(o => o.WriteResults("TestBenchy2", "Build1", It.IsAny<long>()), Times.Never());
            outputWriter.Verify(o => o.WriteError(It.IsAny<string>(), "TestBenchy2", "Build1"), Times.Once());
        }

        [Test]
        public void RunBenchmarks_Should_NotRunBenchmark_When_SetupThrowsExceptions()
        {
            var assemblyInterrogator = new Mock<AssemblyInterrogator>();
            assemblyInterrogator.Setup(p => p.AssemblyName).Returns("TestBenchmark.dll");
            var items = new List<Object>();
            var testObject = new TestBenchmarkClass1();
            testObject.SetUpThrowException = true;
            items.Add(testObject);
            string errors;
            assemblyInterrogator.Setup(p => p.PopulateItemsToBench(out errors)).Returns(true);
            assemblyInterrogator.Setup(p => p.ItemsToBench).Returns(items);
            var outputWriter = new Mock<OutputWriter>();
            var processStarter = new Mock<ProcessStarterFactory>();

            var target =
                new BenchmarkExecuter(
                    new Settings
                        {BuildLabel = "Build1", BenchmarkClass = "TestBenchmarkClass1", BenchmarkMethod = "TestBenchy2"},
                    assemblyInterrogator.Object, outputWriter.Object, processStarter.Object);
            bool result = target.RunBenchmarks(out errors);

            Assert.IsFalse(result);
            Assert.IsFalse(testObject.TestBenchy1Called);
            Assert.IsFalse(testObject.TestBenchy2Called);
            Assert.IsTrue(testObject.TearDownCalled);
        }

        [Test]
        public void RunBenchmarks_Should_RunAllTestsInAExe_When_NoTestClassOrMethodSpecified()
        {
            var settings = new Settings
                               {
                                   BuildLabel = "Build1",
                                   BenchmarkDll = "TestBenchmark.dll",
                                   BenchmarkClass = "",
                                   BenchmarkMethod = ""
                               };

            var assemblyInterrogator = new Mock<AssemblyInterrogator>();
            assemblyInterrogator.Setup(p => p.AssemblyName).Returns("TestBenchmark.dll");
            var items = new List<Object>();
            items.Add(new TestBenchmarkClass1());
            items.Add(new TestBenchmarkClass2());
            string errors;
            assemblyInterrogator.Setup(p => p.PopulateItemsToBench(out errors)).Returns(true);
            assemblyInterrogator.Setup(p => p.ItemsToBench).Returns(items);

            var outputWriter = new Mock<OutputWriter>();

            var processStarter = new Mock<ProcessStarterFactory>();
            processStarter.Setup(p => p.Execute(It.IsAny<string>(), out errors)).Returns(true);

            var target = new BenchmarkExecuter(settings, assemblyInterrogator.Object, outputWriter.Object,
                                               processStarter.Object);
            bool result = target.RunBenchmarks(out errors);

            Assert.AreEqual(true, result);
            string t0;
            processStarter.Verify(p => p.Execute(It.IsAny<string>(), out t0), Times.Exactly(3));
        }

        [Test]
        public void RunBenchmarks_Should_RunBenchmarkAndCallSetUpAndTearDown_When_ClassAndMethodNameSpecified()
        {
            var assemblyInterrogator = new Mock<AssemblyInterrogator>();
            assemblyInterrogator.Setup(p => p.AssemblyName).Returns("TestBenchmark.dll");
            var items = new List<Object>();
            var testObject = new TestBenchmarkClass1();
            items.Add(testObject);
            string errors;
            assemblyInterrogator.Setup(p => p.PopulateItemsToBench(out errors)).Returns(true);
            assemblyInterrogator.Setup(p => p.ItemsToBench).Returns(items);

            var outputWriter = new Mock<OutputWriter>();

            var processStarter = new Mock<ProcessStarterFactory>();

            var target =
                new BenchmarkExecuter(
                    new Settings
                        {BuildLabel = "Build1", BenchmarkClass = "TestBenchmarkClass1", BenchmarkMethod = "TestBenchy1"},
                    assemblyInterrogator.Object, outputWriter.Object, processStarter.Object);
            string result;

            target.RunBenchmarks(out result);

            outputWriter.Verify(o => o.WriteResults("TestBenchy1", "Build1", It.IsAny<long>()), Times.Once());
            Assert.IsTrue(testObject.SetUpCalled);
            Assert.IsTrue(testObject.TearDownCalled);
            Assert.IsTrue(testObject.TestBenchy1Called);
            Assert.IsFalse(testObject.TestBenchy2Called);
        }

        [Test]
        public void RunBenchmarks_Should_RunTestsInClassInAExe_When_JustTheClassIsSpecified()
        {
            var settings = new Settings
                               {
                                   BuildLabel = "Build1",
                                   BenchmarkDll = "TestBenchmark.dll",
                                   BenchmarkClass = "TestBenchmarkClass1",
                                   BenchmarkMethod = ""
                               };

            var assemblyInterrogator = new Mock<AssemblyInterrogator>();
            assemblyInterrogator.Setup(p => p.AssemblyName).Returns("TestBenchmark.dll");
            var items = new List<Object>();
            items.Add(new TestBenchmarkClass1());
            items.Add(new TestBenchmarkClass2());
            string errors;
            assemblyInterrogator.Setup(p => p.PopulateItemsToBench(out errors)).Returns(true);
            assemblyInterrogator.Setup(p => p.ItemsToBench).Returns(items);

            var outputWriter = new Mock<OutputWriter>();

            var processStarter = new Mock<ProcessStarterFactory>();
            processStarter.Setup(p => p.Execute(It.IsAny<string>(), out errors)).Returns(true);

            var target = new BenchmarkExecuter(settings, assemblyInterrogator.Object, outputWriter.Object,
                                               processStarter.Object);
            bool result = target.RunBenchmarks(out errors);

            Assert.AreEqual(true, result);
            string t0;
            processStarter.Verify(p => p.Execute(It.IsAny<string>(), out t0), Times.Exactly(2));
        }
    }
}