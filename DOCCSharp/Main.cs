global using static Docc.Common.StaticHelpers;
using Docc.Client;
using Docc.Common;

Client Client = new("123", "123");

Client.UseLogger<ClientConsoleLogger>();

new Thread(() =>
{
    while (true)
    {
        Thread.Sleep(TimeSpan.FromSeconds(5));
    }
}).Start();

while (true)
{

}
