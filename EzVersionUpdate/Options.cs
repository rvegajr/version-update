public interface IOptions
{
    string fileMasks { get; set; }
    int increment { get; set; }
    string path { get; set; }
    bool test { get; set; }
    string versionNumberForce { get; set; }
}

public class Options : IOptions
{
    [Option('t', "test", Required = false, HelpText = "Run this as test with no file change.", Default = false)]
    public bool test { get; set; }

    [Option('v', "version", Required = false, HelpText = "Will cause the application to mark all project files with this version number")]
    public string versionNumberForce { get; set; }

    [Option('p', "path", Required = false, HelpText = "Path to file to affect or a directory to recursively search for project files to affect")]
    public string path { get; set; }

    [Option('i', "increment", Required = false, HelpText = "increment type [0.1.2.3]- no value is 2 or 'REVISION' 0.0.X.0, it will bump that version by 1", Default = 2)]
    public int increment { get; set; }

    [Option('f', "filemasks", Required = false, HelpText = "Comma seperate file masks to search for, default will be *.csproj", Default = "*.csproj")]
    public string fileMasks { get; set; }

}