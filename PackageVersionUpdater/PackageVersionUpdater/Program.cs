using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace PackageVersionUpdater
{
    public class Program
    {
        private static List<string> _packagesFilesUpdated = new List<string>();
        private static List<string> _projectFilesUpdated = new List<string>();

        static void Main(string[] args)
        {
            SetWindowSize();

            try
            {
                var baseDir = GetDirectory();
                var projectFiles = GetProjectFiles(baseDir);
                var packagesConfigFiles = GetPackagesConfigFiles(baseDir);
                var newPackageVersions = GetNewPackageVersions();

                foreach (var newVersion in newPackageVersions)
                {
                    Console.WriteLine(Environment.NewLine + $"Updating package '{newVersion.Key}' to version '{newVersion.Value}' . . . ");
                    UpdateProjectFiles(newVersion, projectFiles);
                    UpdatePackagesConfigFiles(newVersion, packagesConfigFiles);
                }

                Console.WriteLine(Environment.NewLine);
                Console.WriteLine($"Project files updated: {_projectFilesUpdated.Count}");
                Console.WriteLine($"Package.config files updated: {_packagesFilesUpdated.Count}");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                Console.WriteLine(Environment.NewLine + "Press any key to continue . . . ");
                Console.ReadKey();
            }
        }

        private static void SetWindowSize()
        {
            try
            {
                Console.SetWindowSize(200, 20);
            }
            catch (Exception)
            {
            }
        }

        private static string GetDirectory()
        {
            Console.WriteLine("Input directory to run in, otherwise the program will run in current directory.");
            Console.Write("Directory: ");
            var dir = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(dir))
            {
                return System.IO.Directory.GetCurrentDirectory();
            }

            var dirInfo = new DirectoryInfo(dir);
            return dirInfo.FullName;
        }

        private static Dictionary<string, string> GetNewPackageVersions()
        {
            if (!System.IO.File.Exists("versions.txt"))
            {
                throw new Exception("File 'versions.txt' is required to run this program. See README.txt for details.");
            }

            var result = new Dictionary<string, string>();
            var lines = System.IO.File.ReadAllLines("versions.txt");

            foreach (var line in lines)
            {
                var split = line.Split(' ');
                result.Add(split[0], split[1]);
            }

            return result;
        }

        private static void UpdatePackagesConfigFiles(KeyValuePair<string, string> newVersion, List<string> packagesConfigFiles)
        {
            foreach (var file in packagesConfigFiles)
            {
                UpdatePackagesConfigFile(file, newVersion.Key, newVersion.Value);
            }
        }

        private static void UpdatePackagesConfigFile(string file, string packageId, string newVersion)
        {
            var content = XDocument.Load(file);
            var elementWithPackage = content.Root.Elements("package").FirstOrDefault(e => e.Attribute("id").Value == packageId);
            if (elementWithPackage != null)
            {
                var currentVersion = elementWithPackage.Attribute("version").Value;

                if (currentVersion != newVersion)
                {
                    elementWithPackage.SetAttributeValue("version", newVersion);
                    content.Save(file);
                    Console.WriteLine($"Updated package '{packageId}' to version '{newVersion}' in '{file}'");
                    if (!_packagesFilesUpdated.Contains(file))
                    {
                        _packagesFilesUpdated.Add(file);
                    }
                }
                else
                {
                    Console.WriteLine($"Package '{packageId}' already at '{newVersion}' in '{file}'");
                }
            }
        }

        private static void UpdateProjectFiles(KeyValuePair<string, string> newVersion, List<string> projectFiles)
        {
            foreach (var file in projectFiles)
            {
                UpdateProjectFile(file, newVersion.Key, newVersion.Value);
            }
        }

        private static void UpdateProjectFile(string file, string packageId, string newVersion)
        {
            const string projectNameSpace = "http://schemas.microsoft.com/developer/msbuild/2003";

            var content = XDocument.Load(file);
            var itemGroupElement = content.Root.Elements($"{{{projectNameSpace}}}ItemGroup").FirstOrDefault(e => e.Elements($"{{{projectNameSpace}}}Reference").Any());
            if (itemGroupElement != null)
            {
                var referenceElement = itemGroupElement.Elements($"{{{projectNameSpace}}}Reference").FirstOrDefault(e => e.Attribute("Include").Value.StartsWith(packageId));
                
                if (referenceElement != null)
                {
                    var hintPathElement = referenceElement.Elements($"{{{projectNameSpace}}}HintPath").FirstOrDefault();
                    if (hintPathElement != null)
                    {
                        var searchvalue = $"\\packages\\{packageId}.";
                        var start = hintPathElement.Value.IndexOf(searchvalue);
                        var end = hintPathElement.Value.IndexOf("\\lib\\", start + searchvalue.Length);
                        var currentVersion = hintPathElement.Value.Substring(start + searchvalue.Length, end - (start + searchvalue.Length));
                        var toReplace = $"{searchvalue}{currentVersion}";
                        var newValue = $"{searchvalue}{newVersion}";

                        if (newValue != toReplace)
                        {
                            hintPathElement.SetValue(hintPathElement.Value.Replace(toReplace, newValue));
                            content.Save(file);
                            Console.WriteLine($"Updated package '{packageId}' to version '{newVersion}' in '{file}'");
                            if (!_projectFilesUpdated.Contains(file))
                            {
                                _projectFilesUpdated.Add(file);
                            }
                        }
                        else
                        {
                            Console.WriteLine($"Package '{packageId}' already at '{newVersion}' in '{file}'");
                        }
                    }
                }
            }
        }

        private static List<string> GetProjectFiles(string baseDir)
        {
            return Directory.GetFiles(baseDir, "*.csproj", SearchOption.AllDirectories).ToList();
        }

        private static List<string> GetPackagesConfigFiles(string baseDir)
        {
            return Directory.GetFiles(baseDir, "packages.config", SearchOption.AllDirectories).ToList();
        }
    }
}
