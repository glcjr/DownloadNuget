using System;
using System.Threading.Tasks;
using DownloadNuget;

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

namespace ConsoleApp1
{
    class Program
    {
       
        static void Main(string[] args)
        {
            // sets the nuget package to be downloaded
            DLNuget pkg = new DLNuget("Wix");

            //makes it use the temporary directory for malipulation
            pkg.UseTempDirectory();
            Console.WriteLine(pkg.Dir);
            Console.WriteLine(pkg.version);

            //downloads the nuget package
            Console.WriteLine("Wait while package downloaded");
            // pkg.RetrievePackage();
            //changed to an async method
            pkg = RetrievePackage(pkg).Result;
            
            //Can access only the dlls in the package
            Console.WriteLine("All the dlls in the package");
            foreach (var dll in pkg.DLLs)
                Console.WriteLine(dll);

            //Or it can access all the files in the package
            Console.WriteLine("All the files in the package");
            foreach (var f in pkg.AllFiles)
                Console.WriteLine(f);

            //Removes the package from the computer
            if (pkg.Remove())
                Console.WriteLine("The package has been removed.");
            else
                Console.WriteLine(pkg.ErrorMessage);


            //pkg.RetrievePackage();

            
        }
        // needed to break this off into its own async method because I don't think main can be async with a console program. 
        private static async Task<DLNuget> RetrievePackage(DLNuget pkg)
        {
            await pkg.RetrievePackageAsync();
            return pkg;
        }

    }
}
