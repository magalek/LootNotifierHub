using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using DiscordWowNotifier.Browser;
using DiscordWowNotifier.Utility;

namespace DiscordWowNotifier.Data;

[Serializable]
public class DiscordMessageRequest(Configuration configuration)
{
    private readonly Configuration configuration = configuration;

    [JsonIgnore] public string nickname { get; set; }
    [JsonIgnore] public Currency ahPrice { get; set; }
    public string content { get; set; }
    public string username { get; set; } = "WoW Addon";
    public string avatar_url { get; set; } = "https://upload.wikimedia.org/wikipedia/commons/thumb/e/eb/WoW_icon.svg/1200px-WoW_icon.svg.png";

    public async Task SendAsync()
    {
        await SendHeaderRequest();
        await SendDetailsRequest();
    }

    private async Task<HttpResponseMessage> SendHeaderRequest()
    {
        content = $"```\n{nickname} dropped new loot! (Current AH price: {ahPrice.ToString()})\n```";
        
        var multipartContent = new MultipartFormDataContent();
        
        var json = JsonSerializer.Serialize(this);
        var jsonContent = new StringContent(json, Encoding.UTF8, "application/json");
        
        
        var imageContent = new ByteArrayContent(await File.ReadAllBytesAsync(SeleniumUtility.ImgImagePng));
        
        multipartContent.Add(jsonContent, "payload_json");
        multipartContent.Add(imageContent, "file", SeleniumUtility.ImgImagePng);
        
        Logger.Log("Sending message (1/2)...");
        
        return await new HttpClient().PostAsync(configuration.DiscordWebhook, multipartContent);
    }
    
    private async Task<HttpResponseMessage> SendDetailsRequest()
    {
        var multipartContent = new MultipartFormDataContent();

        content = string.Empty;
        var json = JsonSerializer.Serialize(this);
        var jsonContent = new StringContent(json, Encoding.UTF8, "application/json");
        
        var infoContent = new ByteArrayContent(await File.ReadAllBytesAsync(SeleniumUtility.ImgInfoPng));
        
        multipartContent.Add(jsonContent, "payload_json");
        multipartContent.Add(infoContent, "file", SeleniumUtility.ImgInfoPng);
        
        Logger.Log("Sending message (2/2)...");
        
        return await new HttpClient().PostAsync(configuration.DiscordWebhook, multipartContent);
    }
}