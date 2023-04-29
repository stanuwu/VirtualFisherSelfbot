using System.Reflection;
using Discord.Commands;
using Discord.WebSocket;

namespace FishClient.Core;

public class CommandHandler
{
    public const char PREFIX = '.';
    public static List<MethodInfo> MessageListeners = new ();
    private static DiscordSocketClient? Client => FishClientBot.Client;
    private static CommandService? CommandService => FishClientBot.CommandService;
    
    public static async Task HandleCommandAsync(SocketMessage msg)
    {
        if (Client == null || CommandService == null)
        {
            return;
        }

        int argPos = 0;
        SocketUserMessage message = (SocketUserMessage)msg;
        if (!(message.HasCharPrefix(PREFIX, ref argPos) && message.Author.Id == Client.CurrentUser.Id))
        {
            foreach (MethodInfo listener in MessageListeners)
            {
                listener.Invoke(null, new object[] { msg });
            }

            return;
        }

        SocketCommandContext ctx = new SocketCommandContext(FishClientBot.Client, message);
        await CommandService.ExecuteAsync(ctx, argPos, null);
    }
}