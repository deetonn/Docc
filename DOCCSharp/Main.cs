global using static Docc.Common.StaticHelpers;
using Docc.Client;
using Docc.Common;
using System.Diagnostics;

Client Client = new("@admin.root");

Client.UseLogger<ClientConsoleLogger>();

Stopwatch Stopwatch = Stopwatch.StartNew();

new Thread(() =>
{
    while (true)
    {
        var version = Client.MakeRequest(
            new RequestBuilder()
            .WithLocation("/api/v1/version")
            .Build()
        );

        Console.Title = $"Docc Client - (server: {version?.Content.First()})";

        Thread.Sleep(TimeSpan.FromSeconds(5));
    }
}).Start();

while (true)
{

}