using Discord.Commands;

namespace FishClient.Commands;

public class Test : ModuleBase<SocketCommandContext>
{
    [Command("test")]
    [Summary("Test the bot.")]
    public async Task TestAsync([Remainder] string? args = null)
    {
        await ReplyAsync("The bot is working!");
    }
}