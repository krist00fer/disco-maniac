using System.ComponentModel.Design;
using Discord;
using Discord.WebSocket;

Console.WriteLine("Disco Maniac Starting!");

var client = new DiscordSocketClient();
client.Log += LogAsync;
client.Ready += ReadyAsync;
client.MessageReceived += MessageReceivedAsync;

var token = Environment.GetEnvironmentVariable("DISCO_BOT_TOKEN");

Console.WriteLine($"Token: {token}");

await client.LoginAsync(TokenType.Bot, token);
await client.StartAsync(); 
await Task.Delay(Timeout.Infinite);







async Task MessageReceivedAsync(SocketMessage message)
{
    if (message.Author.Id == client.CurrentUser.Id)
        return;

    Console.WriteLine(message.Content);

    if (message.Content == "!ping")
        await message.Channel.SendMessageAsync("pong!");
}

Task LogAsync(LogMessage log)
{
    Console.WriteLine("Nu loggar jag");
    Console.WriteLine(log.ToString());
    return Task.CompletedTask;
}

Task ReadyAsync()
{
    Console.WriteLine($"{client.CurrentUser} is connected!");

    return Task.CompletedTask;
}