#DownloadNuget

This project allows you to download a nuget package based on the nuget PackageID from code. 
It makes it simpler to used a nuget package if you compile code from within your project. 

It builds off the Nuget.Core package.

The original plan was to incorporate this ability in my EasyCompiler project 
but the Nuget.Core apparently is not Net Standard 2.0 compliant.
So I decided to separate it out and it can be used on the desktop.

It may also work in Net Standard because the error indicates that it may not be compatible 
outside the Windows Desktop so its not definite.

Simple Use
```csharp
 DLNuget nugetpackage = new DLNuget("newtonsoft.json");
nugetpackage.RetrivePackage();
string[] dlls = nugetpackage.DLLsArray;
``` 