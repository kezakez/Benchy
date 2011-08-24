using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

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

        private List<BenchmarkItem> _benchmarkItems = new List<BenchmarkItem>();
        public List<BenchmarkItem> BenchmarkItems
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
                BenchmarkItems.Add(new BenchmarkItem(file.FullName));
            }
        }
    }
}
