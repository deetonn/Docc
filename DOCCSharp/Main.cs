global using static Docc.Common.StaticHelpers;
using Docc.Client;
using Docc.Common;
using Docc.Common.Storage;

var user = string.Empty;
var pass = string.Empty;
Client? client = null!;

do
{
    Console.Clear();
    Console.Write("Username: ");
    user = Console.ReadLine();
    Console.Write("\nPassword: ");

    ConsoleKey key;
    do
    {
        var keyInfo = Console.ReadKey(intercept: true);
        key = keyInfo.Key;

        if (key == ConsoleKey.Backspace && pass.Length > 0)
        {
            Console.Write("\b \b");
            pass = pass[0..^1];
        }
        else if (!char.IsControl(keyInfo.KeyChar))
        {
            Console.Write("*");
            pass += keyInfo.KeyChar;
        }
    } while (key != ConsoleKey.Enter);
}
while (!DoccClient.Create(user!, StorageUtil.Sha256Hash(pass!), out client));

client.UseLogger<ClientConsoleLogger>();

Console.WriteLine($"fully connected. (sessionId={client.Connection.SessionId})");

new Thread(() =>
{
    while (true)
    {
        var response = client.MakeRequest(new RequestBuilder()
                .WithLocation("/api/v1/ping")
                .WithResult(Okay())
                .Build()
        );

        Console.WriteLine("/api/v1/ping: " + response);

        Thread.Sleep(TimeSpan.FromSeconds(5));
    }
}).Start();
