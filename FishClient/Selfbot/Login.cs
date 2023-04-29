using Discord;
using FishClient.Core;
using FishClient.Reflect;
using Discord.WebSocket;
using Discord.Net.Rest;

namespace FishClient.Selfbot;

public class Login
{
    public static async Task LoginSelfbotAsync(string TOKEN)
    {
        DiscordSocketClient client = FishClientBot.Client!;
        object logManager = Accessor.GetPrivateProperty<object>(client, "LogManager");
        object apiClient = Accessor.GetPrivateProperty<object>(client, "ApiClient");
        object requestQueue = Accessor.GetPrivateProperty<object>(apiClient, "RequestQueue");
        IRestClient restClient = Accessor.GetPrivateProperty<IRestClient>(apiClient, "RestClient");
        if (Accessor.GetPrivateField<bool>(client, "_isFirstLogin"))
        {
            Accessor.SetPrivateField(client, "_isFirstLogin", false);
            await Invoker.PrivateMethod<Task> (logManager, "WriteInitialLog", new object[] {});
        }
        if (Accessor.GetPrivateProperty<LoginState>(client, "LoginState") != LoginState.LoggedOut)
        {
            await Invoker.PrivateMethod<Task>(client, "LogoutInternalAsync", new object[] {});
        }
        Accessor.SetPrivateProperty(client, "LoginState", LoginState.LoggingIn);
        if (Accessor.GetPrivateProperty<LoginState>(apiClient, "LoginState") != LoginState.LoggedOut)
        {
            await Invoker.PrivateMethod<Task>(apiClient, "LogoutInternalAsync", new object[] { });
        }
        Accessor.SetPrivateProperty(apiClient, "LoginState", LoginState.LoggingIn);
        try
        {
            Accessor.GetPrivateField<CancellationTokenSource>(apiClient, "_loginCancelToken").Dispose();
        }
        catch (Exception)
        {
            //ignore
        }
        CancellationTokenSource cToken = new CancellationTokenSource();
        Accessor.SetPrivateField(apiClient, "_loginCancelToken", cToken);
        Accessor.SetPrivateProperty(apiClient, "AuthToken", null!);
        Task task = Invoker.PrivateMethod<Task>(requestQueue, "SetCancelTokenAsync", new object[] { cToken.Token });
        restClient.SetCancelToken(cToken.Token);
        Accessor.SetPrivateProperty(apiClient, "AuthTokenType", TokenType.Bot);
        Accessor.SetPrivateProperty(apiClient, "AuthToken", TOKEN.TrimEnd());
        restClient.SetHeader("authorization", TOKEN.TrimEnd());
        await task;
        Accessor.SetPrivateProperty(apiClient, "LoginState", LoginState.LoggedIn);
        await Invoker.PrivateMethod<Task>(client, "OnLoginAsync", new object[] { TokenType.Bot, TOKEN });
    }
}