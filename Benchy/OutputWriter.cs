using System;
using System.IO;

namespace Benchy
{
    public class OutputWriter
    {
        private readonly IStreamFactory _streamFactory;

        public OutputWriter()
        {
        }

        public OutputWriter(string directory) : this(directory, new StreamFactory())
        {
        }

        public OutputWriter(string directory, IStreamFactory streamFactory)
        {
            OutputDirectory = directory;
            _streamFactory = streamFactory;
        }

        public string OutputDirectory { get; private set; }

        public virtual void WriteResults(string benchmarkName, string buildLabel, long benchmarkMilliseconds)
        {
            string fileName = Path.Combine(OutputDirectory, benchmarkName + ".json");
            Stream stream = _streamFactory.GetStream(fileName);

            string resultString = @"[""" + buildLabel + @""", " + benchmarkMilliseconds + @"]";

            if (stream.Length == 0)
            {
                // write an empty file
                using (var writer = new StreamWriter(stream))
                {
                    writer.Write(@"{ ""benchmarkname"": """ + benchmarkName + @""", ""data"": [" + resultString + @"] }");
                }
            }
            else
            {
                var reader = new StreamReader(stream);
                string contents = reader.ReadToEnd();

                int dataStart = contents.IndexOf(@"""data"": [");
                int insertPosition = dataStart + GetLastUnmatchedArrayBracketPosition(contents.Substring(dataStart));

                stream.Position = insertPosition;
                using (var writer = new StreamWriter(stream))
                {
                    writer.Write(", " + resultString + contents.Substring(insertPosition));
                }
            }
        }

        public virtual void WriteError(string error, string benchmarkName = "", string buildLabel = "")
        {
            Console.WriteLine(error);

            if (!string.IsNullOrWhiteSpace(benchmarkName))
            {
                string fileName = Path.Combine(OutputDirectory, benchmarkName + "-errors.txt");
                File.AppendAllText(fileName, buildLabel + "\r\n" + error);
            }
        }

        private int GetLastUnmatchedArrayBracketPosition(string searchString)
        {
            int position = 0;
            bool inBracket = false;
            foreach (char dataChar in searchString)
            {
                if (dataChar == '[')
                {
                    inBracket = true;
                }
                else if (dataChar == ']')
                {
                    if (inBracket)
                    {
                        inBracket = false;
                    }
                    else
                    {
                        return position;
                    }
                }
                position++;
            }
            return -1;
        }
    }

    public class StreamFactory : IStreamFactory
    {
        #region IStreamFactory Members

        public Stream GetStream(string fileName)
        {
            return new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite);
        }

        #endregion
    }

    public interface IStreamFactory
    {
        Stream GetStream(string fileName);
    }
}