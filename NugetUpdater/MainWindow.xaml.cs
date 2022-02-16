using NugetUpdater.Extensions;
using NugetUpdater.Helpers;
using NugetUpdater.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Xml;
using System.Xml.Linq;

namespace NugetUpdater
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public string WorkStationPath => @"C:\Workstation";
        private List<Button> ProjectButtons;
        //public TextBox ConsoleBox;
        public MainWindow()
        {
            InitializeComponent();

            var solutions = SolutionHelper.GetSolutions(WorkStationPath);
            // Create the application's main window
            this.Title = "Grid Sample";

            // Create the Grid
            //var directories = Directory.GetDirectories(WorkStationPath);
            var projectList = solutions.SelectMany(solution => solution.Projects).ToList();
                
            //    new List<string>
            //{
            //    @"C:\Workstation\orbitax-file-store-api_fork-shahadat\Orbitax.FileStore.Api\Orbitax.FileStore.Business",
            //    @"C:\Workstation\orbitax-treaty-api_fork-shahadat\Orbitax.Treaty.Api\Orbitax.Treaty.Business",
            //    @"C:\Workstation\orbitax-core-lib_fork-shahadat\Standard\Orbitax.Core.Business",
            //    @"C:\Workstation\orbitax-file-store-api_fork-shahadat\Orbitax.FileStore.Api\Orbitax.FileStore.Api"
            //};

            var projectIndex = 0;
            var projectStackPanel = new StackPanel();
            //projectStackPanel.Style = new Style
            //{

            //}
            ProjectButtons = new List<Button>();
            foreach (var item in projectList)
            {
                var button = new Button///
                {
                    Name = $"select{projectIndex}",
                    Content = Path.GetFileName(item.Name),
                    Tag = new Dictionary<string, object> { { "ProjectInfo", item }}
                };
                button.Click += SelectProject;
                ProjectButtons.Add(button);
                projectStackPanel.Children.Add(button);
            }
            grid.Children.Add(projectStackPanel);
            Grid.SetRow(projectStackPanel, 0);
            Grid.SetColumn(projectStackPanel, 0);
            
            //AddLabels(canvas, directories.ToList(), 0, 0);
            //ProjectShow(projectDirecotry, 0);
        }

        private void SelectProject(object sender, RoutedEventArgs e)
        {
            foreach (var button in ProjectButtons)
            {
                button.Background = new SolidColorBrush(Colors.White);
                button.Foreground = new SolidColorBrush(Colors.Black);
            }
            var currentButton = sender as Button;
            currentButton.Background = (SolidColorBrush)new BrushConverter().ConvertFrom("#fbeee0");
            currentButton.Foreground = (SolidColorBrush)new BrushConverter().ConvertFrom("#422800");
            var tags = currentButton.Tag as Dictionary<string, object>;
            var projectInfo = tags["ProjectInfo"] as ProjectInfo;
            ProjectShow(projectInfo, 0);

        }

        private void ProjectShow(ProjectInfo projectInfo, int row)
        {
            grid.Children.RemoveRange(1, grid.Children.Count - 1);
            projectInfo.Packages = GetPackageInfo(projectInfo.RelativePath);
            var label = new Label
            {
                Content = projectInfo.Name,
            };

            var updateVersionStackPanel = new StackPanel();
            var textBox = new TextBox
            {
                Name = "version",
                Text = projectInfo.Version,
                Width = 100,
                Height = 30,
                VerticalAlignment = VerticalAlignment.Center,
                Background = new SolidColorBrush(Colors.White),
                BorderThickness = new Thickness(2),
                BorderBrush = new SolidColorBrush(Colors.Gray),
            };
            updateVersionStackPanel.Children.Add(textBox);

            //ConsoleBox = new TextBox
            //{
            //    Name = "console",
            //    Text = string.Empty,
            //    Width = 400,
            //    Height = 500,
            //    Background = new SolidColorBrush(Colors.Black),
            //    Foreground = new SolidColorBrush(Colors.White),
            //    BorderThickness = new Thickness(2),
            //    BorderBrush = new SolidColorBrush(Colors.Gray),
            //};
            //grid.Children.Add(ConsoleBox);
            //Grid.SetRow(ConsoleBox, 0);
            //Grid.SetColumn(ConsoleBox, 2);

            var button = new Button///
            {
                Name = "update",
                Content = "Update Version",
                Tag = new Dictionary<string, object> { { "ProjectInfo", projectInfo }, { "TextBox", textBox } }
            };
            button.Click += Button_Click;
            updateVersionStackPanel.Children.Add(button);
            grid.Children.Add(updateVersionStackPanel);
            Grid.SetRow(updateVersionStackPanel, 0);
            Grid.SetColumn(updateVersionStackPanel, 1);

            
            //Grid.SetRow(button, 0);
            //Grid.SetColumn(button, 1);

            var stackPanel = new StackPanel();
            var updatePackageIndex = 0;
            foreach (var package in projectInfo.Packages)
            {
                var pklabel = new Label
                {
                    Content = $"{package.Name}  {package.Version}"
                };
                var updatePackageButton = new Button
                {
                    Name = $"updatepackage{updatePackageIndex++}",
                    Content = "Update Package",
                    Tag = new Dictionary<string, object> { { "PackageInfo", package }, { "Directory", projectInfo.Directory } },
                };
                updatePackageButton.Click += UpdatePackage;
                stackPanel.Children.Add(pklabel);
                stackPanel.Children.Add(updatePackageButton);
            }
            grid.Children.Add(stackPanel);
            Grid.SetRow(stackPanel, 0);
            Grid.SetColumn(stackPanel, 2);


        }

        private void UpdatePackage(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var dict = button.Tag as Dictionary<string, object>;
            var packageInfo = dict["PackageInfo"] as PackageInfo;
            var directory = dict["Directory"] as string;

            var orbitaxNuget = "https://www.myget.org/F/orbitax-3-1/auth/a5388e02-c5ad-4ad9-a0eb-092deaee327e/api/v3/index.json";
            using (Process process = new Process())
            {
                process.StartInfo = new ProcessStartInfo
                {
                    FileName = "CMD.exe",
                    WorkingDirectory = $"{directory}",
                    Arguments = string.Format(@$"/C dotnet add package {packageInfo.Name} --source {orbitaxNuget} --prerelease")
                };
                process.Start();
                process.WaitForExit();
                 
                //RedirectStandardOutput = true,
                //CreateNoWindow = true
            };

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var dict = button.Tag as Dictionary<string,object>;
            var projectInfo = dict["ProjectInfo"] as ProjectInfo;
            var textBox = dict["TextBox"] as TextBox;
           // var consoleBox = dict["ConsoleBox"] as TextBox;

            var projectRelativePath = System.IO.Path.Combine(projectInfo.Directory, projectInfo.RelativePath);
            XDocument projDefinition = XDocument.Load(projectRelativePath, LoadOptions.PreserveWhitespace);
            var projectElement = projDefinition.ElementByTag("Project");
            var propertyGroup = projectElement.ElementByTag("PropertyGroup");

            var newVersion = textBox.Text;
            propertyGroup.SetElementValue("Version", textBox.Text);
            using (var writer = XmlWriter.Create(projectRelativePath, new XmlWriterSettings { OmitXmlDeclaration = true, Indent = true }))
            {
                projDefinition.Save(writer);
            }

            var process1 = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "CMD.exe",
                    Arguments = string.Format(@$"/C dotnet build -c Release"),
                    WorkingDirectory = $"{projectInfo.Directory}",
                    UseShellExecute = false,
                    CreateNoWindow = false,
                    RedirectStandardOutput = false
                }
            };

            string temp = string.Empty;
            //process1.OutputDataReceived += (sender, e) => DoSomething(e); // use this for synchronization
            process1.Start();
            
            //process1.BeginOutputReadLine();
            process1.WaitForExit();

            var nugetFileName = $"{projectInfo.Id}.{newVersion}.nupkg";
            var nugetFileDirectory = $"{projectInfo.Directory}/bin/Release";
            var nugetPath = System.IO.Path.Combine(nugetFileDirectory, nugetFileName);

            if (File.Exists(nugetPath) == false)
            {
                throw new Exception($"{nugetFileName} file not found.");
            }
            using (Process process = new Process())
            {
                process.StartInfo.FileName = "CMD.exe";
                process.StartInfo.WorkingDirectory = $"{projectInfo.Directory}/bin/Release";
                process.StartInfo.Arguments = string.Format(@$"/C nuget push {nugetFileName} a5388e02-c5ad-4ad9-a0eb-092deaee327e -Source https://www.myget.org/F/orbitax-3-1/api/v2/package");
                process.Start();
                //process.StartInfo.ArgumentList.Add(string.Format("git push {0} {1}", remoteName, branch));
                process.WaitForExit();
            }


        }

        private int AddLabels(Canvas canvas, List<string> directories, int topPosition, int level)
        {
            for (int i = 0; i < directories.Count; i++)
            {
                var label = new Label
                {
                    Content = System.IO.Path.GetFileName(directories[i]),
                };
                Canvas.SetLeft(label, level * 10);
                Canvas.SetTop(label, topPosition);
                topPosition += 20;
                canvas.Children.Add(label);
                var subDirectories = Directory.GetDirectories(directories[i]).ToList();
                if (level < 1 && subDirectories.Count < 3)
                {
                    topPosition = AddLabels(canvas, subDirectories, topPosition, level + 1);
                }
            }
            return topPosition;
        }

        private List<PackageInfo> GetPackageInfo(string projectPath)
        {
            if (File.Exists(projectPath) == false)
            {
                throw new Exception("File not found.");
            }
            XDocument projDefinition = XDocument.Load(projectPath, LoadOptions.PreserveWhitespace);
            var projectElement = projDefinition.ElementByTag("Project");
            var propertyGroup = projectElement.ElementByTag("PropertyGroup");
            var projectId = propertyGroup.ElementByTag("PackageId")?.Value;
            var version = propertyGroup.ElementByTag("Version")?.Value;

            var itemGroups = projectElement.ElementsByTag("ItemGroup");
            var packageReferences = itemGroups.SelectMany(itemGroup => itemGroup.ElementsByTag("PackageReference"));
            var orbitaxPackageReferences = packageReferences.Where(package => package.Attribute("Include").Value.Contains("Orbitax"));
            var orbitaxPackageNames = orbitaxPackageReferences.Select(x => x.Attribute("Include").Value).ToList();

            var packages = orbitaxPackageReferences.Select(packageInfo => new PackageInfo
            {
                Name = packageInfo.Attribute("Include").Value,
                Version = packageInfo.Attribute("Version").Value
            }).ToList();
            return packages;
        }
    }

    
}
