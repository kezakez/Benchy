using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Benchy.Results.Models
{
    public class BenchmarkItem
    {
        public BenchmarkItem(string filename)
        {
            FileName = filename;
        }
        public string FileName { get; set; }

        public string ClassName { get; set; }
        public string TestName { get; set; }

        public string Data { get; set; }

        internal void Load()
        {

        }
    }
}
