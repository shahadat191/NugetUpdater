using System.Collections.Generic;
using System.IO;

namespace NugetUpdater.Models
{
    public class SolutionInfo
    {
        public string Name { get; set; }
        public string RelativePath { get; set; }
        public List<ProjectInfo> Projects { get; set; }
        public SolutionInfo()
        {
            Projects = new List<ProjectInfo>();
        }
    }
    public class ProjectInfo
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string RelativePath { get; set; }
        public List<PackageInfo> Packages { get; set; }
        public string Version { get; set; }

        //public string ParentRelativePath { get; set; }
        public ProjectInfo()
        {
            Packages = new List<PackageInfo>();
        }
        public bool IsAllowed()
        {
            var allowedTypes = new HashSet<string>() { ".Contracts", ".Business", ".Proxy" };
            var projectType = Path.GetExtension(Name);
            return allowedTypes.Contains(projectType);
        }
    }
    public class PackageInfo
    {
        public string Name { get; set; }
        public string Version { get; set; }
    }
}
