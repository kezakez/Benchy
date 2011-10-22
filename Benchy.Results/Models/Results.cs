using System.Collections.Generic;

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

        public Benchmark FindItem(string projId, string testId)
        {
            var project = this.ProjectItems.Find(item => item.Name == projId);
            var testItem = project.BenchmarkItems.Find(item => item.Name == testId);

            return testItem;
        }
    }
}