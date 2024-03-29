﻿using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;

using Newtonsoft.Json;

namespace Docc.Common;

public static class StaticHelpers
{
    public static RequestResult NotFound()
        => RequestResult.ContentNotFound;
    public static RequestResult Okay()
        => RequestResult.OK;
    public static RequestResult DeadSocket()
        => RequestResult.SockedDied;
    public static RequestResult BadContent()
        => RequestResult.BadPacket;
    public static RequestResult FileNotFound()
        => RequestResult.FileNotFound;
    public static RequestResult BadArgs()
        => RequestResult.BadArguments;
    public static RequestBuilder Builder()
        => new();
    public static void Exit(int code)
    {
        Environment.ExitCode = code;
        Environment.Exit(code);
    }

    public static bool ExpectLength<T, T2>(int expected, T real) where T : IEnumerable<T2>
    {
        return real.Count() == expected;
    }

    public static void RunIn(int secs, Action act)
    {
        Task.Run(async () =>
        {
            await Task.Delay(secs * 1000);
            act.Invoke();
        });
    }
}

public static class SocketExtensions
{
    public static Request? Receive(this Socket socket)
    {
        SpinWait.SpinUntil(() =>
        {
            if (socket is null)
                return true;
            return socket.Available != 0;
        });

        if (socket is null)
        {
            return Request.Default;
        }

        byte[] buffer = new byte[1024];
        try
        {
            socket.Receive(buffer);
        }
        catch (SocketException socketException)
        {
            return new RequestBuilder()
                .WithLocation("/internal_error")
                .AddContent(socketException.Message)
                .WithResult(RequestResult.SockedDied)
                .Build();
        }

        var contents = Encoding.Default.GetString(buffer);

        if (!Request.TryDeserialize(contents, out Request? result))
        {
            return null;
        }

        return result;
    }
    public static async Task<Request?> ReceiveAsync(this Socket socket)
    {
        byte[] buffer = new byte[1024];
        _ = await socket.ReceiveAsync(buffer, SocketFlags.None);

        var contents = Encoding.Default.GetString(buffer);

        if (!Request.TryDeserialize(contents, out Request? result))
        {
            return null;
        }

        return result;
    }

    public static void SendRequest(this Socket sock, Request req, ILogger? logger = null)
    {
        var converted = JsonConvert.SerializeObject(req);
        var bytes = Encoding.Default.GetBytes(converted);

        try
        {
            sock.Send(bytes);
        }
        catch (SocketException ex)
        {
            logger?.Log(ex ,$"failed to send request. ({ex.Message})");

            // at this point we assume they've disconnected.
            sock?.Dispose();
        }
    }

    public static void SendEncrypted(this Socket sock, Request req, ILogger? logger = null)
    {
        SendRequest(sock, req, logger);
    }

    public static Request? ReceiveEncrypted(this Socket sock, ILogger? logger = null)
    {
        return Receive(sock);
    }
}
