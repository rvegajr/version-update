public interface IVersionUpdater
{
    void Run(Options options);
}

class VersionUpdater : IVersionUpdater
{
    private readonly IProjectFileEditor projectFileParser;
    private readonly IFileSystemScanner fileSystemScanner;
    private readonly IStringUtilites stringUtilites;
    private readonly ILogger logger; // Inject an ILogger for logging

    public VersionUpdater(IProjectFileEditor projectFileParser, IFileSystemScanner fileSystemScanner, IStringUtilites stringUtilites, ILogger logger)
    {
        this.projectFileParser = projectFileParser;
        this.fileSystemScanner = fileSystemScanner;
        this.stringUtilites = stringUtilites;
        this.logger = logger;
    }

    public void Run(Options o)
    {
        int MAJOR = 0; int MINOR = 1; int REVISION = 2; int BUILD = 3; //Version Segments

        // Logging the version of the utility
        var assembly = System.Reflection.Assembly.GetExecutingAssembly();
        var fvi = System.Diagnostics.FileVersionInfo.GetVersionInfo(assembly.Location);
        string version = fvi.FileVersion;
        logger.LogInformation($"EzVersionUpdate Utility - Version: {version} - https://github.com/rvegajr/EzVersionUpdate");

        // Determine the path for searching project files
        var path = Environment.CurrentDirectory;
        if (!Path.EndsInDirectorySeparator(path)) path += Path.DirectorySeparatorChar;
        if (!string.IsNullOrEmpty(o.path)) path = o.path;

        // Logging the search for project files
        logger.LogInformation("Searching for project files...");

        // Splitting file masks and searching for project files
        var masks = o.fileMasks.Split(",");
        var list = fileSystemScanner.FindFiles(path, masks);

        // Iterating through the found project files
        foreach (var file in list)
        {
            logger.LogInformation($"Project File {file}");
            projectFileParser.Load(file);
            string filever = projectFileParser.Version;

            if (!string.IsNullOrEmpty(filever))
            {
                var Version = stringUtilites.VersionStringIncrement(filever, o.increment);
                if (!string.IsNullOrEmpty(o.versionNumberForce)) Version = o.versionNumberForce;
                Match match1 = Regex.Match(Version, @"\d{1,3}\.\d{1,3}\.\d{1,3}");
                Match match2 = Regex.Match(Version, @"\d{1,3}\.\d{1,3}\.\d{1,5}\.\d{1,5}");

                if (!((match1.Success) || (match2.Success)))
                {
                    logger.LogError("Nuget version number should be ##9.##9.####9.####9 - thus 1.1.1, 1.1.1.0, or 999.999.9999 are valid");
                    throw new Exception("Invalid NuGet version number format.");
                }

                logger.LogInformation($"File Version in file is {filever}.. will change to {Version}" + (o.test ? ".. remove -t to make it happen" : ""));

                if (!o.test)
                {
                    var VersionAttribute = stringUtilites.VersionStringParts(Version, MAJOR, MINOR, REVISION);
                    var AssemblyVersionAttribute = stringUtilites.VersionStringParts(Version, MAJOR, MINOR, REVISION, BUILD);
                    projectFileParser.Version = VersionAttribute;
                    projectFileParser.AssemblyVersion = AssemblyVersionAttribute;
                    projectFileParser.FileVersion = AssemblyVersionAttribute;
                    projectFileParser.Save();
                }
            }
            else
            {
                logger.LogInformation($"No File version in file.. skipping");
            }
        }

        // Logging completion of the process
        logger.LogInformation("Finished!");
    }
}
