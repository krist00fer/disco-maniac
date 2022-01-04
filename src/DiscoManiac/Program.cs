using System.Runtime.Serialization.Json;
using System.Security.AccessControl;
using System.Text;
using System.ComponentModel.Design;
using Discord;
using Discord.WebSocket;
using System.Text.Json;

Console.WriteLine("Disco Maniac Starting!");

var highScores = new Dictionary<string, int>();

var client = new DiscordSocketClient();
client.Log += LogAsync;
client.Ready += ReadyAsync;
client.MessageReceived += MessageReceivedAsync;

var token = Environment.GetEnvironmentVariable("DISCO_BOT_TOKEN");

Console.WriteLine($"Token: {token}");

await client.LoginAsync(TokenType.Bot, token);
await client.StartAsync(); 
await Task.Delay(Timeout.Infinite);



(bool isCommand, string command, string[] args) GetCommand(char commandPrefix, string message)
{
    if (string.IsNullOrWhiteSpace(message)) 
    {
        return (false, "", null);
    }

    if (message.Length < 2 || message[0] != commandPrefix)
    {
        return (false, "", null);
    }

    var messageWithoutPrefix = message.Substring(1);
    var messageAsWords = messageWithoutPrefix.Split();
    var command = messageAsWords[0].ToLower();
    var args = messageAsWords[1..];
    
    return (true, command, args);
}

async Task MessageReceivedAsync(SocketMessage message)
{
    if (message.Author.Id == client.CurrentUser.Id)
        return;

    string author = message.Author.ToString();

    Console.WriteLine($"DEBUG - Message: {message.Content}, From: {author}");
    
    var newScore = message.Content.Length;

    // Check if user exists
    if (highScores.ContainsKey(author))
    {
        var currentScore = highScores[author];
        highScores[author] = currentScore + newScore;
    }
    else
    {
        highScores.Add(author, newScore);
    }


    var (isCommand, command, args) = GetCommand('!', message.Content);

    if (!isCommand)
        return;

    if (command == "show-scores")
    {
        var sb = new StringBuilder();

        sb.AppendLine("High Score");

        foreach (var item in highScores)
        {
            sb.AppendLine($"{item.Key} - {item.Value}");
        }

        message.Channel.SendMessageAsync(sb.ToString());
    }
    else if (command == "save-scores")
    {
        Console.WriteLine("Saving scores");
        using var file = File.Create("saved-scores.json");
        var options = new JsonSerializerOptions { WriteIndented = true };
        await JsonSerializer.SerializeAsync(file, highScores, options);
    }
    else if (command == "load-scores")
    {
        Console.WriteLine("Loading scores...");
        var json = File.ReadAllText("saved-scores.json");
        highScores = JsonSerializer.Deserialize<Dictionary<string, int>>(json);
        Console.WriteLine("New scores loaded");
    }
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