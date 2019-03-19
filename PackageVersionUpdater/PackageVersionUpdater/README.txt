(TFS) Branches can use PackageVersionUpdater to keep all projects & solutions in the branch aligned in terms of package versions.

README

This tool updates nuget package versions in

- CSPROJ files
- packages.config files

This tool runs the update across all files in specified directory and all subdirectories.

1. if it does not exist, create input file "versions.txt" in the tool's current directory
2. populate the file with package id and the required version, EXAMPLE is below
  - other packages will not be touched, eg. if you specify MyApp.Models 1.0.3 then only MyApp.Models package version will be set to 1.0.3
run the tool
3. as a result, all projects within the specified directory that reference MyApp.Models will now be referencing v1.0.3

EXAMPLE for the versions.txt file:

<package id><space><version>
MyApp.Models 1.0.3
MyApp.Client 1.0.2

