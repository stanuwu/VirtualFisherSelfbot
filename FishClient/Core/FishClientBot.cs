using System.Reflection;
using System.Security.AccessControl;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using FishClient.Selfbot;

namespace FishClient.Core;

public class FishClientBot
{
    public static DiscordSocketClient? Client { get; private set; }
    public static CommandService? CommandService { get; private set; }

    private static bool started = false;

    public async Task MainAsync()
    {
        File.Create("log.txt").Close();

        DiscordSocketConfig discordSocketConfig = new DiscordSocketConfig
        {
            UseInteractionSnowflakeDate = false
        };
        
        Client = new DiscordSocketClient(discordSocketConfig);
        Client.Log += Log;

        CommandServiceConfig commandServiceConfig = new CommandServiceConfig
        {
            CaseSensitiveCommands = false,
            DefaultRunMode = RunMode.Async,
            IgnoreExtraArgs = true
        };

        CommandService = new CommandService(commandServiceConfig);

        string TOKEN = File.ReadAllText("token.txt");

        await Login.LoginSelfbotAsync(TOKEN);
        await Client.StartAsync();
        
        Client.MessageReceived += CommandHandler.HandleCommandAsync;
        await CommandService.AddModulesAsync(Assembly.GetEntryAssembly(), null);
        Util.Log.Info("Init", "Loaded Commands");
        
        Client.Connected += Init;

        await Task.Delay(-1);
    }

    public static Task Log(LogMessage msg)
    {
        string txt = $"@{DateTime.Now} [{(msg.Severity + "]").PadRight(9, ' ')} {msg.Source.PadRight(15, ' ').Substring(0, 15)} -> {msg.Message}";

        using (StreamWriter sw = new StreamWriter("log.txt", true))
        {
            sw.WriteLine(txt);
        }
        Console.WriteLine(txt);
        return Task.CompletedTask;
    }

    private static Task Init()
    {
        if (!started)
        {
            started = true;
            Util.Log.Info("Startup", "Bot Started");
            Util.Log.Info("Startup", $"Username: {Client!.CurrentUser.Username}");
        
            foreach (TypeInfo definedType in Assembly.GetEntryAssembly()!.DefinedTypes)
            {
                if ((definedType.IsPublic || definedType.IsNestedPublic))
                {
                    if (definedType.IsAssignableTo(typeof(IInit)) && !definedType.IsInterface)
                    {
                        definedType.GetMethods().First(m => m.Name == "Init").Invoke(null, null);
                    }

                    if (definedType.IsAssignableTo(typeof(IMessageListener)) && !definedType.IsInterface)
                    {
                        CommandHandler.MessageListeners.Add(definedType.GetMethods().First(m => m.Name == "OnMessage"));
                    }
                }
            }
        }
        return Task.CompletedTask;
    }
}