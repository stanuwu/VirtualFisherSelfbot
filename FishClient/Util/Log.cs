using Discord;
using FishClient.Core;

namespace FishClient.Util;

public class Log
{
    public static async void Debug(string msg)
    {
        await FishClientBot.Log(new LogMessage(LogSeverity.Debug, "Debugging", msg));
    }

    public static async void Info(string source, string msg)
    {
        await FishClientBot.Log(new LogMessage(LogSeverity.Info, source, msg));
    }
}