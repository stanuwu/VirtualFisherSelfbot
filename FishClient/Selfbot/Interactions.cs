using System.Text;
using Discord.Net.Rest;
using Discord.WebSocket;
using FishClient.Core;
using FishClient.Reflect;
using FishClient.Util;

namespace FishClient.Selfbot;

public class Interactions
{
    public static async void SendInteraction(InteractionCommand cmd, List<InteractionCommandOption> opts)
    {
        DiscordSocketClient client = FishClientBot.Client!;
        object apiClient = Accessor.GetPrivateProperty<object>(client, "ApiClient");
        IRestClient restClient = Accessor.GetPrivateProperty<IRestClient>(apiClient, "RestClient");

        StringBuilder options = new StringBuilder();
        foreach (InteractionCommandOption o in opts)
        {
            options.Append($"{{\"type\": {o.Type}, \"name\": \"{o.Name}\", \"value\": \"{o.Value}\"}}");
            if (!opts.Last().Equals(o)) options.Append(",");
        }

        string jsonData = $"\"type\": 2," +
                          $"\"application_id\": \"{cmd.ApplicationId}\"," +
                          $"\"guild_id\": \"{cmd.GuildId}\"," +
                          $"\"channel_id\": \"{cmd.ChannelId}\"," +
                          $"\"session_id\": \"{cmd.SessionId}\"," +
                          $"\"data\": {{" +
                          $"\"version\": \"{cmd.Version}\"," +
                          $"\"id\": \"{cmd.Id}\"," +
                          $"\"name\": \"{cmd.Name}\"," +
                          $"\"type\": {cmd.Type}," +
                          $"\"options\": [" +
                          $"{options}" +
                          $"]"+
                        $"}}";

        await restClient.SendAsync("POST", "interactions", "{"+jsonData+"}", new CancellationToken());
    }
}