using System.Collections.Generic;
using System.IO;

namespace Benchy.Results.Models
{
    public class ProjectItem
    {
        public ProjectItem(string directory)
        {
            Directory = directory;
        }

        public string Directory { get; set; }
        public string Name { get; set; }

        private List<Benchmark> _benchmarkItems = new List<Benchmark>();
        public List<Benchmark> BenchmarkItems
        {
            get { return _benchmarkItems; }
            set { _benchmarkItems = value; }
        }

        internal void Load()
        {
            var directory = new DirectoryInfo(Directory);
            Name = directory.Name;
            
            FileInfo[] files = directory.GetFiles();
            foreach (var file in files)
            {
                var item = new Benchmark
                               {
                                   Name = Path.GetFileNameWithoutExtension(file.FullName),
                                   Data = File.ReadAllText(file.FullName)
                               };
                BenchmarkItems.Add(item);
            }
        }
    }
}
