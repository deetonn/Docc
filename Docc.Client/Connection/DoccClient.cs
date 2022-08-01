using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Docc.Client;

public class DoccClient
{
    public static (bool, string) Create(string userName, string password, out Client client, bool raw = false)
    {
        client = new Client(userName, password, raw);
        return (client.Connection.Connected, client.Connection.LastMessage);
    }
}
