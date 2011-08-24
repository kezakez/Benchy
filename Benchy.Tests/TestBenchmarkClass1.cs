using System;
using System.Threading;
using Benchy.Framework;

namespace Benchy.Tests
{
    [BenchmarkFixture]
    public class TestBenchmarkClass1
    {
        public bool SetUpCalled { get; set; }
        public bool SetUpThrowException { get; set; }

        public bool TearDownCalled { get; set; }
        public bool TearDownThrowException { get; set; }

        public bool TestBenchy1Called { get; set; }

        public bool TestBenchy2Called { get; set; }
        public bool TestBenchy2ThrowException { get; set; }

        [SetUp]
        public void SetupData()
        {
            SetUpCalled = true;
            if (SetUpThrowException)
            {
                throw new Exception();
            }
        }

        [TearDown]
        public void TearDownData()
        {
            TearDownCalled = true;
            if (TearDownThrowException)
            {
                throw new Exception();
            }
        }

        [Benchmark]
        public void TestBenchy1()
        {
            TestBenchy1Called = true;
            Thread.Sleep(200);
        }

        [Benchmark]
        public void TestBenchy2()
        {
            TestBenchy2Called = true;
            Thread.Sleep(300);
            if (TestBenchy2ThrowException)
            {
                throw new Exception();
            }
        }

        public void SomeOtherMethod()
        {
            throw new Exception();
        }
    }
}