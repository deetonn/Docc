using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Docc.Client;

public class DoccClient
{
    public static bool Create(string userName, string password, out Client client)
    {
        client = new Client(userName, password);
        return client.Connection.Connected;
    }
}
