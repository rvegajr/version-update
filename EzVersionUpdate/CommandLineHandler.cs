public interface ICommandLineHandler
{
    void Run(Options options);
}
class CommandLineHandler : ICommandLineHandler
{
    private readonly IVersionUpdater versionUpdater;

    public CommandLineHandler(IVersionUpdater versionUpdater)
    {
        this.versionUpdater = versionUpdater;
    }
    public void Run(Options options)
    {
        versionUpdater.Run(options);
    }
}