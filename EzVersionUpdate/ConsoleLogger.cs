public class ConsoleLogger : ILogger
{
    private readonly string categoryName;

    public ConsoleLogger(string categoryName)
    {
        this.categoryName = categoryName;
    }

    public IDisposable BeginScope<TState>(TState state)
    {
        return null; // Not used in this example
    }

    public bool IsEnabled(LogLevel logLevel)
    {
        // You can filter log levels here based on your requirements.
        return true;
    }

    public void Log<TState>(LogLevel logLevel, TState state, Exception exception, Func<TState, Exception, string> formatter)
    {
        Log(logLevel, state, exception, formatter); 
    }

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
    {
        if (!IsEnabled(logLevel))
        {
            return;
        }

        var message = formatter(state, exception);

        // Set the default console color
        var consoleColor = ConsoleColor.White;

        // Determine the console color based on log level
        switch (logLevel)
        {
            case LogLevel.Trace:
            case LogLevel.Debug:
                consoleColor = ConsoleColor.White;
                break;
            case LogLevel.Information:
                consoleColor = ConsoleColor.Green;
                break;
            case LogLevel.Warning:
                consoleColor = ConsoleColor.Yellow;
                break;
            case LogLevel.Error:
                consoleColor = ConsoleColor.Red;
                break;
            case LogLevel.Critical:
                consoleColor = ConsoleColor.DarkRed;
                break;
        }

        // Change the console color
        Console.ForegroundColor = consoleColor;

        // Log to the console
        Console.WriteLine($"{logLevel}: {categoryName} - {message}");

        // Reset the console color
        Console.ResetColor();

        // Log to the IDE output window for debug messages
        if (logLevel == LogLevel.Debug)
        {
            System.Diagnostics.Debug.WriteLine($"{categoryName} - {message}");
        }
    }
}
