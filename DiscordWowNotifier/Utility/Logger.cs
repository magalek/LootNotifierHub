namespace DiscordWowNotifier.Utility;

public static class Logger
{
    private static List<ILogHandler> handlers = new List<ILogHandler>();
    
    public static void AddLogHandler(ILogHandler handler)
    {
        handlers.Add(handler);    
    }

    public static void Log(object message)
    {
        foreach (var logHandler in handlers)
        {
            logHandler.Log(message.ToString());
        }
    }
}