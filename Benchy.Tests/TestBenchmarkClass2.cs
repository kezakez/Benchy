using System;
using System.Threading;
using Benchy.Framework;

namespace Benchy.Tests
{
    [BenchmarkFixture]
    public class TestBenchmarkClass2
    {
        public bool SetUpCalled { get; set; }

        public bool TearDownCalled { get; set; }

        public bool TestBenchy1Called { get; set; }

        [SetUp]
        public void SetupData()
        {
            SetUpCalled = true;
        }

        [TearDown]
        public void TearDownData()
        {
            TearDownCalled = true;
        }

        [Benchmark]
        public void TestBenchy1()
        {
            TestBenchy1Called = true;
            Thread.Sleep(200);
        }

        public void SomeOtherMethod()
        {
            throw new Exception();
        }
    }
}