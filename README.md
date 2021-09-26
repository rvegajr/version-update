# EZ Version Update

A simple dotnet tool to update previously existing version numbers. 

## Description

This application will default to the path it is running from.  It will enumerate across all non hidden paths for each file mask passed through the -m parameter (currently *.csproj).   
It will then extract the version number,  and if there are no command line parameters, it will increment the REVISION number.
The version number format is MAJOR.MINOR.REVISION.BUILD  [0.1.2.3]

Note that the projects nust have the one of the following tags for it to change it already in the file.
<Version>15.1.36</Version>
<AssemblyVersion>15.1.36</AssemblyVersion>
<FileVersion>15.1.36</FileVersion>

## Getting Started

### Dependencies

* .net 5.0


### Executing program

* Install by typing in the following in the command line in the directory that has itself or a subdirectory of the projects you wish to change
```
dotnet tool install -g EzVersionUpdate
```
* After installing, you can have it increment the REVISION number by one each time you type the following and press enter
```
ezver
```
* To set a specific version number, use the following
```
ezver -v 1.0.1
```
* To increment the MAJOR Release by 1
```
ezver -i 0
```
* To increment the MINOR Release by 1
```
ezver -i 1
```
* To increment the BUILD Release by 1
```
ezver -i 3
```
* To see what will be change as a test
```
ezver -t
```

## Authors



Ricky Vega [@rvegajr](https://github.com/rvegajr/)

## License

This project is licensed under the MIT License - see the LICENSE.md file for details

## Acknowledgments

Thank you!
* [commandlineparser](https://github.com/commandlineparser/commandline)
