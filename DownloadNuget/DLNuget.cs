using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.XPath;
using NuGet;

/********************************************************************************************************************************* 
 * 
 * 
 * Copyright and Licensing Message

This code is copyright 2018 Gary Cole Jr.

This code is licensed by Gary Cole to others under the GPLv.3 https://opensource.org/licenses/GPL-3.0 

If you find the code useful or just feel generous a donation is appreciated.

Donate with this link: paypal.me/GColeJr Please choose Friends and Family

Alternative Licensing Options

If you prefer to license under the LGPL for a project, https://opensource.org/licenses/LGPL-3.0 
Single Developers working on their own project can do so with a donation of $20 or more. 
Small and medium companies can do so with a donation of $50 or more. 
Corporations can do so with a donation of $1000 or more.

If you prefer to license under the MS-RL for a project, https://opensource.org/licenses/MS-RL 
Single Developers working on their own project can do so with a donation of $40 or more. 
Small and medium companies can do so with a donation of $100 or more. 
Corporations can do so with a donation of $2000 or more.

if you prefer to license under the MS-PL for a project, https://opensource.org/licenses/MS-PL 
Single Developers working on their own project can do so with a donation of $1000 or more. 
Small and medium companies can do so with a donation of $2000 or more. 
Corporations can do so with a donation of $10000 or more.

If you use the code in more than one project, a separate license is required for each project.

Any modifications to this code must retain this message. 

 * *************************************************************************************************************************************/

namespace DownloadNuget
{
    public class DLNuget
    {
        /// <summary>
        /// Dir- The directory where the package will be downloaded
        /// </summary>
        public string Dir = "";
        /// <summary>
        /// DLL - All the dlls paths will be placed in this list
        /// </summary>
        public List<string> DLLs = new List<string>();
        /// <summary>
        /// AllFiles - All the paths of the files in the package will be palced in this list
        /// </summary>
        public List<string> AllFiles = new List<string>();
        /// <summary>
        /// ErrorMessage - If an error occurrs it will be placed here to explain why.
        /// </summary>
        public string ErrorMessage = "";
        /// <summary>
        /// UniqueDir- Whether DIR needs to be an empty directory or not
        /// </summary>
        public bool UniqueDir = true;
        /// <summary>
        /// PackageID-The Package id on nuget to download.
        /// </summary>
        public string PackageID = "";
        public SemanticVersion version;
        private IPackageRepository repo = PackageRepositoryFactory.Default.CreateRepository("https://packages.nuget.org/api/v2");
        /// <summary>
        /// Constructor for setting up an empty project
        /// </summary>
        public DLNuget()
        { }

        /// <summary>
        /// This constructor sets up the package to be downloaded and placed on the hard drive for use
        /// </summary>
        /// <param name="packageID">This is the Nuget id of the project to be downloaded</param>
        /// <param name="vers">This is the version that should be downloaded. If left null, it will download the latest version.</param>
        /// <param name="dir">This is where the package will be downloaded to. If left null, it will place it in the AppData directory with the packageid as the name of the directory</param>
        /// <param name="uniquedir">This is used to make sure that the directory where the package is downloaded is only in use for this project. If set to false, it will not force
        /// the directory to be new. It also will not allow the directory to be deleted.</param>
        public DLNuget(string packageID, string vers = null, string dir = null, bool uniquedir = true)
        {
            PackageID = packageID;
            UniqueDir = uniquedir;
            SetDirectory(dir);
            SetVersion(vers);
        }
              
        /// <summary>
        /// This method initiates the download of the package.
        /// It also will place the dlls and the files in the package into the two lists: AllFiles and DLLs.
        /// </summary>
        public int RetrievePackage()
        {
            PackageManager packageManager = new PackageManager(repo, Dir);
            packageManager.InstallPackage(PackageID, version);
            GetDLLs();
            return DLLs.Count;
        }
        /// <summary>
        /// An attempt to write an Async method for retrieving the package
        /// </summary>
        /// <returns></returns>
        public async Task<int> RetrievePackageAsync()
        {
            return await Task.Factory.StartNew(() => RetrievePackage());          
        }

        public async Task<int> RetriveLocalPackageAsync(string dir)
        {
            return await Task.Factory.StartNew(() => RetrieveLocalPackage(dir));
        }
        public int RetrieveLocalPackage(string dir)
        {
            UniqueDir = false;
            SetDirectory(dir);
            GetDLLs();
            return DLLs.Count;
        }
        /// <summary>
        /// This will remove the directory where the package was downloaded
        /// </summary>
        /// <returns>Returns the success of the operation. If there is an error, it is stored in the public ErrorMessage variable.</returns>
        public bool Remove()
        {
            try
            {
                if (UniqueDir)
                {
                    DeleteFilesAndFoldersRecursively(Dir);
                    return true;
                }
                else
                {
                    ErrorMessage = "Directory is not unique to this project so it cannot be removed.";
                    return false;
                }
            }
            catch (Exception e)
            {
                ErrorMessage = e.Message;
                return false;
            }
        }
        /// <summary>
        /// This allows the version to be set outside the constructor if needed.
        /// </summary>
        /// <param name="vers">A string representing the version to be targeted for download.</param>
        public void SetPackageVersion(string vers = null)
        {
            SetVersion(vers);
        }
        /// <summary>
        /// Changes the DIR from being in the AppData folder to a temporary folder
        /// It will also replace if the Directory has been set elsewhere.
        /// </summary>
        public void UseTempDirectory()
        {
            string path = Path.GetRandomFileName();
            path = (Path.Combine(Path.GetTempPath(), path));
            string dir = $@"{path}{PackageID}";
            dir = dir.Replace('.', '_');
            Directory.CreateDirectory(dir);
            Dir = dir;
        }
        private void SetVersion(string vers)
        {
            if (vers == null)
                version = GetLatestVersion(repo, PackageID);
            else
                version = SemanticVersion.Parse(vers);
        }
        private void SetDirectory(string dir)
        {
            if (dir == null)
                Dir = $@"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\{PackageID}";
            else
                Dir = dir;
            int count = 0;
            do
            {
                if (count > 0)
                    Dir += count;
                count++;
            }
            while ((Directory.Exists(Dir)) && (UniqueDir));
        }
        private SemanticVersion GetLatestVersion(IPackageRepository repo, string packageID)
        {
            return SemanticVersion.Parse(GetLatestVersion());
            //List<IPackage> packages = repo.FindPackagesById(packageID).ToList();
            //packages = packages.Where(item => (item.IsReleaseVersion() == true)).ToList();
            //packages.Sort();
            //return packages[packages.Count - 1].Version;
        }
        public List<string> GetAllVersions()
        {
            List<string> vers = new List<string>();
            List<IPackage> packages = repo.FindPackagesById(PackageID).ToList();
            packages = packages.Where(item => (item.IsReleaseVersion() == true)).ToList();
            foreach (var p in packages)
                vers.Add(p.Version.ToString());
            vers.Sort();
            return vers;
        }
        public string GetLatestVersion()
        {
            List<string> vers = GetAllVersions();
            return vers[vers.Count - 1];
        }
        private void GetDLLs()
        {
            SearchDirectory(Dir);        
        }
        private void SearchDirectory(string directory)
        {
            if (Directory.Exists(directory))
            {
                string[] directories = Directory.GetDirectories(directory);
                foreach (var d in directories)
                {
                    SearchFiles(d);
                    SearchDirectory(d);
                }
            }
        }
        private void SearchFiles(string directory)
        {
            string[] files = Directory.GetFiles(directory);
            foreach (var f in files)
            {
                if (f.EndsWith("dll"))
                    DLLs.Add(f);
                AllFiles.Add(f);
            }
        }
        private static void DeleteFilesAndFoldersRecursively(string target_dir)
        {
            foreach (string file in Directory.GetFiles(target_dir))
            {
                File.Delete(file);
            }

            foreach (string subDir in Directory.GetDirectories(target_dir))
            {
                DeleteFilesAndFoldersRecursively(subDir);
            }
            Thread.Sleep(1);
            Directory.Delete(target_dir);
        }
        /// <summary>
        /// Gets all the DLLs i nthe Nuget Package as an Array
        /// </summary>
        public string[] DLLsArray
        {
            get
            {
                return DLLs.ToArray();
            }
        }
        /// <summary>
        /// Gets all the files in the nuget package as an array
        /// </summary>
        public string[] AllFilesArray
        {
            get
            {
                return AllFiles.ToArray();
            }
        }
        public string GetDirectory()
        {
            return Dir;
        }
    }
}
