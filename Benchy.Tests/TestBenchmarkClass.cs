using System;
using Benchy.Framework;

namespace Benchy.Tests
{
    [BenchmarkFixture]
    public class TestBenchmarkClass
    {
        public bool SetUpCalled { get; set; }
        public bool SetUpThrowException { get; set; }

        [SetUp]
        public void SetupData()
        {
            SetUpCalled = true;
            if (SetUpThrowException)
            {
                throw new Exception();
            }
        }

        public bool TearDownCalled { get; set; }
        public bool TearDownThrowException { get; set; }

        [TearDown]
        public void TearDownData()
        {
            TearDownCalled = true;
            if (TearDownThrowException)
            {
                throw new Exception();
            }
        }

        public bool TestBenchy1Called { get; set; } 

        [Benchmark]
        public void TestBenchy1()
        {
            TestBenchy1Called = true;
            System.Threading.Thread.Sleep(200);
        }

        public bool TestBenchy2Called { get; set; }
        public bool TestBenchy2ThrowException { get; set; }

        [Benchmark]
        public void TestBenchy2()
        {
            TestBenchy2Called = true;
            System.Threading.Thread.Sleep(300);
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