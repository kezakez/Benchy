using NUnit.Framework;
using PEL.Benchmark;

namespace Benchy.Tests
{
    [TestFixture]
    public class SettingsTest
    {
        [Test]
        public void ProcessParameters_Should_PopulateProperties_When_AllProvided()
        {
            var target = new Settings();
            string result;
            target.ProcessParameters(
                new[]
                    {
                        @"-benchmarkdll:""MyDLL.dll""", @"-outputdirectory:""c:\test\output\""",
                        @"-buildlabel:""testlabel""", @"-benchmarkmethod:""benchname"""
                    }, out result);

            Assert.AreEqual("MyDLL.dll", target.BenchmarkDll);
            Assert.AreEqual("benchname", target.BenchmarkMethod);
            Assert.AreEqual("testlabel", target.BuildLabel);
            Assert.AreEqual(@"c:\test\output\", target.OutputDirectory);
        }

        [Test]
        public void ProcessParameters_Should_ReturnFalse_When_NoDLLGiven()
        {
            var target = new Settings();
            string errors;
            bool result = target.ProcessParameters(new[] {@"-label:""testlabel"""}, out errors);

            Assert.AreEqual(false, result);
        }

        [Test]
        public void ProcessParameters_Should_ReturnFalse_When_NoLabelGiven()
        {
            var target = new Settings();
            string errors;
            bool result = target.ProcessParameters(new[] {@"-benchmarkdll:""test.dll"""}, out errors);

            Assert.AreEqual(false, result);
        }
    }
}