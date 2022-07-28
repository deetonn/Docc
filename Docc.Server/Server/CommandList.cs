using Docc.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Docc.Server;

public delegate void ServerCommand(string[] args, ILogger logger);

public class Redeemable
{
    public string? Name { get; init; }            
    public int Amount { get; init; }
}

internal class CommandList
{
    public Dictionary<Guid, Redeemable> ActiveKeys { get; } = new();
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

        Add("giftcard.gen", (args, logger) =>
        {
            if (args.Length != 1)
            {
                logger.Log("must specify how many wallet funds this key grants.");
                return;
            }

            var generated = Guid.NewGuid();
            ActiveKeys.Add(generated, new() { Name = "Wallet Funds (10 = £1.00)", Amount = int.Parse(args[0]) });
            logger.Log($"generated key: {generated}");
        });
        Add("giftcard.view", (args, logger) =>
        {
            if (!args.Any())
            {
                logger.Log($"must supply the Id of the giftcard to view.");
                return;
            }

            var id = args.FirstOrDefault(Guid.Empty.ToString());

            if (!ActiveKeys.Any(x => x.Key.ToString() == id))
            {
                logger.Log($"no such giftcard exists");
                return;
            }

            var giftcard = ActiveKeys.Where(x => x.Key.ToString() == id).First();

            logger.Log($"giftcard({giftcard.Key}): {giftcard.Value.Name} (x{giftcard.Value.Amount}) [£{giftcard.Value.Amount / 10} eqiv]");
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
            _logger.Log($"no such command '{name}'");
            return;
        }

        _commands[name].Invoke(args, _logger);
    }
}
