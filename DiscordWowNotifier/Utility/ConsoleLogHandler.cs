namespace DiscordWowNotifier.Utility;

public class ConsoleLogHandler : ILogHandler
{
    public void Log(object message)
    {
        Console.WriteLine(message);
    }
}