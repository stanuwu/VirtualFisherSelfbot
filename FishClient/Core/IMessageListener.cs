using Discord.WebSocket;

namespace FishClient.Core;

public interface IMessageListener
{
    public static abstract void OnMessage(SocketMessage msg);
}