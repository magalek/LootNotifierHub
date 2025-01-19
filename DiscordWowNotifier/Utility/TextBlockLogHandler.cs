using System.Windows;
using System.Windows.Controls;

namespace DiscordWowNotifier.Utility;

public class TextBlockLogHandler(TextBlock textBlock) : ILogHandler
{
    public void Log(object message)
    {
        Application.Current.Dispatcher.Invoke(() => { textBlock.Text += message + "\n"; });
    }
}