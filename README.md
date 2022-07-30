# Docc :newspaper:
An example of client-server connections built from scratch. This project contains multi-threaded networking, login system, session ids to track who the server is talking to, requests sent in json format that are able to contain anything and complete freedom in adding new endpoints.

## Client example

```cs
using Docc.Client;
using Docc.Common;
using Docc.Common.Storage;

Client client;

if (!DoccClient.Create("user", StorageUtil.Sha256Hash("pass"), out client){
	Console.WriteLine("failed to connect to the server.");
	Environment.Exit(-1);
}

client.UseLogger<ClientConsoleLogger>();

// make any requests to server endpoints you've defined.

var status = client.MakeRequest(
	new RequestBuilder()
		.WithLocation("/api/status")
		.WithResult(RequestResult.Ok)
		.Build()
);

if (status.Result != RequestResult.Ok){
	Console.WriteLine("failed to fetch status...");
	Environment.Exit(-2);
}

var text = status.Content[0];

Console.WriteLine($"status: `{text}`");
```

## Server example

```cs
using Docc.Common;
using Docc.Server;
using System.Text;
using System.ComponentModel;
using System.Runtime.InteropServices;
using Docc.Server.Server;
using Docc.Server.Server.Auth;
using Docc.Common.Storage;
using Docc.Server.Endpoints;

// contains all endpoints
DirectoryListingManager manager = new();
// encapsulates the servers connections
ServerConnection connection = new(ServerType.Local);
// server commands, that can be typed into the console
CommandList context = new(new CommandLogger());

string status = "Online";

// connection.UseAuthorization<PrivateServerAuthorization>();

// serve the status to any client thats after it
manager.MapGet("/api/status", (args, conn) =>
{
	return new RequestBuilder()
		.WithResult(RequestResult.OK)
		.AddContent(status)
		.Build();
});

// change the status from the server console
context.Add("status.set", (args, logger) => {
	if (args.Count() != 1){
		logger.Log("usage: status.set <status>");
		return;	
	}

	status = args[0];
}

// handle all messages sent from clients 
connection.OnMessage = (req, client) =>
{
	//connection.Logger.Log($"received request for '{req.Location}'");
	var response = manager.CallMappedLocal(req.Location, req.Arguments);
	client.Socket?.SendRequest(response);
};

// handle commands typed into the console
while (true)
{
	// for dramatic effect
	Thread.Sleep(TimeSpan.FromSeconds(.5));
	
	Console.Write(">> ");
	var input = Console.ReadLine();
	
	if (string.IsNullOrEmpty(input))
	{
		Console.Clear();
		continue;
	}
	var shattered = input.Split(' ');
	
	var command = shattered[0];
	List<string> arguments = new();
	
	if (shattered.Length > 0)
		arguments = shattered[1..].ToList();
		
	context.Execute(command, arguments.ToArray());
}
```
FYI: This isn't a library, these are just examples of how dynamic it is.

# Work in progress
This is currently implemented to be used locally, it also does not encrypt packets being sent across sockets.
