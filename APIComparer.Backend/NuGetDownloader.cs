namespace APIComparer.Backend
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using APIComparer.Shared;
    using NuGet;

    class NuGetDownloader
    {
        readonly string package;
        PackageManager packageManager;

        public NuGetDownloader(string nugetName, IEnumerable<string> repositories)
        {
            package = nugetName;

            //var nugetCacheDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "NuGet", "Cache");
            var nugetCacheDirectory = Path.Combine(AzureEnvironment.GetTempPath(), "packages");


            var reposToUse = new List<IPackageRepository>
            {
                PackageRepositoryFactory.Default.CreateRepository(nugetCacheDirectory)
            };

            reposToUse.AddRange(repositories.ToList().Select(r => PackageRepositoryFactory.Default.CreateRepository(r)));
            var repo = new AggregateRepository(reposToUse);

            packageManager = new PackageManager(repo, /*"packages"*/nugetCacheDirectory);
        }

        public List<string> DownloadAndExtractVersion(string version, string target)
        {
            var semVer = SemanticVersion.Parse(version);

            packageManager.InstallPackage(package, semVer, true, false);

            var dirPath = Path.Combine(AzureEnvironment.GetTempPath(), "packages", string.Format("{0}.{1}", package, version), "lib");

            var netVersionDir = Directory.EnumerateDirectories(dirPath)
                .FirstOrDefault(x => x.EndsWith(target));

            if (netVersionDir == null)
            {
                netVersionDir = Directory.EnumerateDirectories(dirPath)
                    .OrderByDescending(name => name)
                    .FirstOrDefault();
            }

            if (netVersionDir != null)
            {
                dirPath = netVersionDir;
            }

            var files = Directory.EnumerateFiles(dirPath)
                .Where(f => f.EndsWith(".dll") || f.EndsWith(".exe"))
                .ToList();


            if (!files.Any())
            {
                throw new Exception("Couldn't find any assemblies in  " + dirPath);
            }

            return files;
        }
    }
}