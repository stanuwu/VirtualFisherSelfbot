using System.Diagnostics;
using System.Drawing;
using System.Net;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using FishClient.Core;
using FishClient.Reflect;
using FishClient.Selfbot;
using FishClient.Solver;
using FishClient.Util;

namespace FishClient.Commands;

public class Fishing : ModuleBase<SocketCommandContext>, IInit
{
    private const int DELAY = 2000;
    private const int BASE_BET = 5;
    private static int CURR_BET = 5;
    private static bool _runCe;
    private static bool _runFish;
    private static ISocketMessageChannel? _channel;
    private static TextCaptcha _captcha = new TextCaptcha();
    
    [Command("setChannel")]
    [Summary("Set the channel.")]
    public async Task SetChannelAsync(string? args = null)
    {
        _channel = Context.Channel;
    }
    
    [Command("ceStart")]
    [Summary("Start the coinflip exploit.")]
    public async Task CeStartAsync(string? args = null)
    {
        _runCe = true;
        _channel = Context.Channel;
        CURR_BET = BASE_BET;
    }
    
    [Command("ceStop")]
    [Summary("Stop the coinflip exploit.")]
    public async Task CeStopAsync([Remainder] string? args = null)
    {
        _runCe = false;
    }
    
    [Command("fishStart")]
    [Summary("Start the autofish.")]
    public async Task FishStartAsync(string? args = null)
    {
        _runFish = true;
        _channel = Context.Channel;
    }
    
    [Command("fishStop")]
    [Summary("Stop the autofish.")]
    public async Task StopStopAsync([Remainder] string? args = null)
    {
        _runFish = false;
    }

    public static void Init()
    {
        Task.Run(async () =>
        {
            while (true)
            {
                await Task.Delay(DELAY + new Random().Next(0, 500));
                if(_channel == null) continue;
                IMessage message = (await  _channel.GetMessagesAsync(10).LastAsync()).First();
                if (message.Embeds.Count < 1) continue;
                IEmbed embed = message.Embeds.First();
                if (embed.Title == "Coinflip" && embed.Author!.Value.Name == FishClientBot.Client!.CurrentUser.Username)
                {
                    if (embed.Description.Contains("YOU WIN")) CURR_BET = BASE_BET;
                    else CURR_BET *= 2;
                }
                if (!_runCe) continue;
                InteractionCommand cmd = new InteractionCommand(574652751745777665,
                    ((SocketTextChannel) _channel).Guild.Id,
                    _channel.Id,
                    Accessor.GetPrivateField<string>(FishClientBot.Client!, "_sessionId"),
                    1090392072328052752,
                    912432961067028512,
                    "coinflip",
                    1);
                List<InteractionCommandOption> options = new List<InteractionCommandOption>()
                {
                    new ((int)ApplicationCommandOptionType.String, "selection", "h"),
                    new ((int)ApplicationCommandOptionType.String, "amount", CURR_BET+"")
                };
                Interactions.SendInteraction(cmd, options);
            }
        });

        Task.Run(async () =>
        {
            while (true)
            {
                Random random = new Random();
                int chance = random.Next(0, 500);
                int d = 3750;
                if (chance > 250) d += random.Next(100, 300);
                if (chance > 400) d += random.Next(500, 2000);
                if (chance > 498) d += random.Next(200000, 400000);
                await Task.Delay(d);
                if(!_runFish) continue;
                if(_channel == null) continue;
                InteractionCommand cmd = new InteractionCommand(574652751745777665,
                    ((SocketTextChannel) _channel).Guild.Id,
                    _channel.Id,
                    Accessor.GetPrivateField<string>(FishClientBot.Client!, "_sessionId"),
                    1090392072281919609,
                    912432960643416115,
                    "fish",
                    1);
                List<InteractionCommandOption> options = new List<InteractionCommandOption>();
                Interactions.SendInteraction(cmd, options);
                if (chance > 450)
                {
                    InteractionCommand cmd2 = new InteractionCommand(574652751745777665,
                        ((SocketTextChannel) _channel).Guild.Id,
                        _channel.Id,
                        Accessor.GetPrivateField<string>(FishClientBot.Client!, "_sessionId"),
                        1090392072281919610,
                        912432960643416116,
                        "sell",
                        1);
                    List<InteractionCommandOption> options2 = new List<InteractionCommandOption>()
                    {
                        new((int)ApplicationCommandOptionType.String, "amount", "all")
                    };
                    Interactions.SendInteraction(cmd2, options2);
                }
            }
        });
        
        Task.Run(async () =>
        {
            while (true)
            {
                if(_channel == null) continue;
                await Task.Delay(5000);
                List<IMessage> messages = new List<IMessage>();
                foreach (IReadOnlyCollection<IMessage> msgs in await _channel.GetMessagesAsync(25).ToListAsync())
                {
                    messages.AddRange(msgs);
                }
                foreach (var message in messages)
                {
                    if (message.Embeds.Count < 1) continue;
                    IEmbed embed = message.Embeds.First();
                    if (embed.Title.Contains("Anti-bot") && embed.Author!.Value.Name == FishClientBot.Client!.CurrentUser.Username)
                    {
                        WebClient client = new WebClient();
                        Stream stream = client.OpenRead(embed.Image!.Value.Url);
                        Bitmap image = new Bitmap(stream);
                        string solved = _captcha.Solve(image);
                        await stream.FlushAsync();
                        stream.Close();
                        client.Dispose();
                        InteractionCommand cmd = new InteractionCommand(574652751745777665,
                            ((SocketTextChannel) _channel!).Guild.Id,
                            _channel.Id,
                            Accessor.GetPrivateField<string>(FishClientBot.Client, "_sessionId"),
                            1090392072428720230,
                            912432961222238220,
                            "verify",
                            1);
                        List<InteractionCommandOption> options = new List<InteractionCommandOption>()
                        {
                            new((int)ApplicationCommandOptionType.String, "answer", solved)
                        };
                        Interactions.SendInteraction(cmd, options);
                        await Task.Delay(50000);
                        break;
                    }
                }
            }
        });
    }
}