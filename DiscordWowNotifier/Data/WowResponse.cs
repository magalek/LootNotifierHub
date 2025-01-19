using DiscordWowNotifier.Utility;

namespace DiscordWowNotifier.Data;

[Serializable]
public class WowResponse
{
    private const string GREEN_FORMAT = "```css\n\"{0}[{1}]\"\n```";
    private const string BLUE_FORMAT = "```ini\n{0}[{1}]\n```";
    private const string YELLOW_FORMAT = "```fix\n{0}[{1}]\n```";

    private string Message => $"{nick} dropped: ";
    
    public string id { get; set; }
    public string name { get; set; }
    public ItemQuality quality {get; set;}
    public string nick {get; set;}
    public int response_id { get; set; }
    
    // public bool TryParseMessage(out string message)
    // {
    //     message = string.Empty;
    //     switch (quality)
    //     {
    //         case ItemQuality.Thrash:
    //         case ItemQuality.Common:
    //             return false;
    //         case ItemQuality.Uncommon: message = string.Format(GREEN_FORMAT, Message, name);
    //             return true;
    //         case ItemQuality.Rare: message = string.Format(BLUE_FORMAT, Message, name);
    //             return true;
    //         case ItemQuality.Epic: message = string.Format(YELLOW_FORMAT, Message, name);
    //             return true;
    //         default:
    //             return false;
    //     }
    // }

    public bool CompareIds(int id) => id == response_id;
}