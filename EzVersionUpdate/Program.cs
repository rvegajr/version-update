class Program
{
    static void Main(string[] args)
    {
        // Create a service collection
        var services = (new ServiceCollection()).RegisterAllClassesWithInterfaces();
        services.AddSingleton<ILogger>(provider => new ConsoleLogger("EzVersionUpdateLog"));

        // Build the service provider
        var serviceProvider = services.BuildServiceProvider();

        // Resolve the service you want
        using (var scope = serviceProvider.CreateScope())
        {
            var commandLineHandler = scope.ServiceProvider.GetRequiredService<ICommandLineHandler>();

            Parser.Default.ParseArguments<Options>(args)
                .WithParsed(o =>
                {
                    commandLineHandler.Run(o);
                });
        }
    }
}

public static class ServiceCollectionExtensions
{
    public static IServiceCollection RegisterAllClassesWithInterfaces(this IServiceCollection services)
    {
        var assembly = Assembly.GetCallingAssembly();

        foreach (var type in assembly.GetTypes())
        {
            if (type.IsClass && !type.IsAbstract)
            {
                var interfaceType = type.GetInterface("I" + type.Name, true);
                if (interfaceType != null)
                {
                    services.AddTransient(interfaceType, type);
                }
            }
        }

        return services;
    }
}