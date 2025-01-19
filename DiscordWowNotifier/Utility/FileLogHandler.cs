using System.IO;

namespace DiscordWowNotifier.Utility;

public class FileLogHandler : ILogHandler
{
    public FileLogHandler()
    {
        if (!Path.Exists("logs")) Directory.CreateDirectory("logs");
    }

    public void Log(object message)
    {
        var logPath = $"logs/{DateTime.Now:yyyy-MM-dd}.txt";
        File.AppendAllText(logPath, $"[{DateTime.Now:HH:mm:ss}] {message}\n");
    }
}