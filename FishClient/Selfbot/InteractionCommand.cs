using Discord.WebSocket;
using FishClient.Reflect;

namespace FishClient.Selfbot;

public class InteractionCommand
{
    public InteractionCommand(DiscordSocketClient client, SocketApplicationCommand cmd, ISocketMessageChannel chan, ulong version)
    {
        ApplicationId = cmd.ApplicationId;
        GuildId = cmd.Guild.Id;
        ChannelId = chan.Id;
        SessionId = Accessor.GetPrivateField<string>(client, "_sessionId");
        Version = version;
        Id = cmd.Id;
        Name = cmd.Name;
        Type = (int) cmd.Type;
    }
    
    public InteractionCommand(ulong applicationId, ulong guildId, ulong channelId, string sessionId, ulong version, ulong id, string name, int type)
    {
        ApplicationId = applicationId;
        GuildId = guildId;
        ChannelId = channelId;
        SessionId = sessionId;
        Version = version;
        Id = id;
        Name = name;
        Type = type;
    }

    public ulong ApplicationId { get; }
    public ulong GuildId { get; }
    public ulong ChannelId { get; }
    public string SessionId { get; }
    public ulong Version { get; }
    public ulong Id { get; }
    public string Name { get; }
    public int Type { get; }
}