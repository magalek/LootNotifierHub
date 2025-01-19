using DiscordWowNotifier.Utility;

namespace DiscordWowNotifier.Data;

[Serializable]
public class Configuration
{
    public string DiscordWebhook { get; set; }
    public string GamePath { get; set; }
    public ItemQuality QualityFilter { get; set; } = ItemQuality.Rare;
}