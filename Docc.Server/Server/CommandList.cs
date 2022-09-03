using Docc.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Docc.Server;

public delegate void ServerCommand(string[] args, ILogger logger);

internal class CommandList
{
    private readonly Dictionary<string, ServerCommand> _commands = new();
    private readonly ILogger _logger;

    public CommandList(ILogger logger)
    {
        _logger = logger;

        Add("help", (args, _) =>
        {
            Console.Write("Listing commands\n\n");
            foreach (var command in _commands)
            {
                Console.WriteLine($"{command.Key}");
            }
        });
    }

    public bool Exists(string name)
    {
        return _commands.ContainsKey(name);
    }
    public void Add(string name, ServerCommand command)
    {
        _commands.Add(name, command);
    }
    public void Execute(string name, string[] args)
    {
        if (!Exists(name))
        {
            _logger.Log(this, $"no such command '{name}'");
            return;
        }

        _commands[name].Invoke(args, _logger);
    }
}
