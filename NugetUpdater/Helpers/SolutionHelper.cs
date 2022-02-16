using NugetUpdater.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace NugetUpdater.Helpers
{
    public class SolutionHelper
    {
        public static List<SolutionInfo> GetSolutions(string workStationPath)
        {
            var solutions = new List<SolutionInfo>();
            foreach (var directory in Directory.GetDirectories(workStationPath))
            {
                if (IsApiDirectory(directory))
                {
                    var solutionInfo = GetSolutionInfo(directory);
                    solutions.Add(solutionInfo);
                }
            }
            return solutions;
        }
        private static bool IsApiDirectory(string directory)
        {
            return Directory.GetFiles(directory).Any(x => x.Contains("start.bat"));
        }
        private static SolutionInfo GetSolutionInfo(string directory)
        {
            var solutionPath = Directory.GetDirectories(directory).FirstOrDefault(relativePath => Path.GetExtension(relativePath).Equals(".Api"));
            var solutionInfo = new SolutionInfo
            {
                Name = Path.GetFileName(solutionPath),
                RelativePath = solutionPath
            };

            foreach (var subDirectory in Directory.GetDirectories(solutionInfo.RelativePath))
            {
                var projectFiles = Directory.GetFiles(subDirectory, "*.csproj");
                foreach (var projectFilePath in projectFiles)
                {
                    if (projectFilePath != null && projectFilePath.Any())
                    {
                        var projectInfo = new ProjectInfo
                        {
                            Name = Path.GetFileNameWithoutExtension(projectFilePath),
                            RelativePath = projectFilePath
                        };
                        //var (packageId, dependentPackages) = GetPackages(subDirectory);
                        projectInfo.Id = GetProjectId(projectFilePath) ?? projectInfo.Name;
                        //projectInfo.Packages = dependentPackages;

                        var projectType = Path.GetExtension(subDirectory);
                        if (projectInfo.IsAllowed())
                        {
                            solutionInfo.Projects.Add(projectInfo);
                        }
                    }
                }
                
            }
            return solutionInfo;

        }
        private static string GetProjectId(string projectPath)
        {
            if (File.Exists(projectPath) == false)
            {
                throw new Exception("File not found.");
            }
            XDocument projDefinition = XDocument.Load(projectPath, LoadOptions.PreserveWhitespace);
            var projectElement = projDefinition.Elements().FirstOrDefault(x => x.Name.LocalName == "Project");
            var temp = projectElement.Elements().Select(x => x.Name).ToList();
            var propertyGroup = projectElement.Elements().FirstOrDefault(x => x.Name.LocalName == "PropertyGroup");

            var projectId = propertyGroup.Elements().FirstOrDefault(x => x.Name.LocalName == "PackageId")?.Value;
            return projectId;
            //var itemGroups = projectElement.Elements().Where(x => x.Name.LocalName == "ItemGroup");
            //var packageReferences = itemGroups.SelectMany(itemGroup => itemGroup.Elements().Where(item => item.Name == "PackageReference"));
            //var orbitaxPackageReferences = packageReferences.Where(res => res.Attribute("Include").Value.Contains("Orbitax"));

            //var orbitaxPackageNames = orbitaxPackageReferences.Select(x => x.Attribute("Include").Value).ToList();
            //return Tuple.Create(projectId, orbitaxPackageNames);
        }

    }
}
