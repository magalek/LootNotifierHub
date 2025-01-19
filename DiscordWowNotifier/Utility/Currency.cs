using System.Text.RegularExpressions;

namespace DiscordWowNotifier.Utility;

public readonly struct Currency
{
    private readonly Regex regex = new Regex(@"(\d\d[gsc])");
    
    private readonly string Gold;
    private readonly string Silver;
    private readonly string Copper;
    
    public Currency(string text)
    {
        var match = regex.Match(text);
        Gold = match.Groups[0].Value;
        Silver = match.Groups[1].Value;
        Copper = match.Groups[2].Value;
    }
    
    public override string ToString()
    {
        return $"{Gold} {Silver} {Copper}";
    }
}