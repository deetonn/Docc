﻿using Docc.Common;

namespace Docc.Client;

// This is my personal client, so the name will always be Admin
// and the userId will always be spoofed.

internal class Client
{
    // this is not a real API key, literally just a random Guid
    const string ApiKey = "249995ce-1683-4fc5-83e0-5628de154fe3";
    protected ClientConnection Connection { get; }
    protected ILogger? _logger;

    public void UseLogger<T>() where T : class, ILogger
    {
        var instance = Activator.CreateInstance(typeof(T));

        if (instance is ILogger logger)
        {
            _logger = logger;
            return;
        }

        throw new InvalidCastException($"failed to create instance of '{typeof(T).Name}'. Does it have a default constructor?");
    }

    public Client()
    {
        var sessionUid = Guid.NewGuid();

        Connection = new ClientConnection(new()
        {
            Name = "@admin.deeton",
            Id = sessionUid
        });

        var rb = new RequestBuilder()
            .WithAgent("Docc AdminClient")
            .WithLocation("/admin/verify")
            .WithArgument("uuid", sessionUid.ToString())
            .WithArgument("apiKey", "1")
            .Build();

        Console.WriteLine($"\nRequest:\n{rb}\n\n");

        var response = Connection.MakeRequest(rb);

        if (response?.Result != RequestResult.OK)
        {
            _logger?.Log($"server did not authorize request.");
            return;
        }
        else
        {
            _logger?.Log($"server authorized you as IsAdmin.");
        }
    }

    public Request? MakeRequest(Request req)
        => Connection.MakeRequest(req);
}