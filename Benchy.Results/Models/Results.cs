using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Benchy.Results.Models
{
    public class Projects
    {
        private string _dataDirectory = string.Empty;
        public Projects(string dataDirectory)
        {
            _dataDirectory = dataDirectory;
        }

        private List<ProjectItem> _projectItems = new List<ProjectItem>();
        public List<ProjectItem> ProjectItems
        {
            get { return _projectItems; }
            set { _projectItems = value; }
        }

        public void Load()
        {
            var dirs = System.IO.Directory.GetDirectories(_dataDirectory);
            foreach (var dir in dirs)
            {
                var projectItem = new ProjectItem(dir);
                projectItem.Load();
                ProjectItems.Add(projectItem);
            }
        }
    }
}