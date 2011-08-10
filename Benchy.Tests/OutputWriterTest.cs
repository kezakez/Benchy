using System.IO;
using Moq;
using NUnit.Framework;

namespace Benchy.Tests
{
    public class OutputWriterTest
    {
        [Test]
        public void WriteResults_Should_WriteAnEntry_When_NoFileExists()
        {
            var streamFactory = new Mock<IStreamFactory>();
            var memoryStream = new TestStreamContents();
            streamFactory.Setup(s => s.GetStream("c:\\dir\\testname.json")).Returns(memoryStream);

            var target = new OutputWriter("c:\\dir", streamFactory.Object);
            target.WriteResults("testname", "label" , 42);

            Assert.AreEqual(@"{ ""benchmarkname"": ""testname"", ""data"": [[""label"", 42]] }", memoryStream.Contents);
        }

        [Test]
        public void WriteResults_Should_AppendAnEntry_When_FileExists()
        {
            var streamFactory = new Mock<IStreamFactory>();
            var memoryStream = new TestStreamContents();
            var writer = new StreamWriter(memoryStream);
            writer.Write(@"{ ""benchmarkname"": ""testname"", ""data"": [[""label"", 42]] }");
            writer.Flush();
            memoryStream.Position = 0;

            streamFactory.Setup(s => s.GetStream("c:\\dir\\testname.json")).Returns(memoryStream);

            var target = new OutputWriter("c:\\dir", streamFactory.Object);
            target.WriteResults("testname", "label", 40);


            Assert.AreEqual(@"{ ""benchmarkname"": ""testname"", ""data"": [[""label"", 42], [""label"", 40]] }", memoryStream.Contents);
        }

        public class TestStreamContents : MemoryStream
        {
            public override void Close()
            {
                Position = 0;
                var reader = new StreamReader(this);
                Contents = reader.ReadToEnd();

                base.Close();
            }

            public string Contents { get; set; }
        }
    }
}
