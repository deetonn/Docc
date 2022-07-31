using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Docc.Client.Connection;

public static class Global
{
    public static Client? Connection { get; set; }

    public static bool TryLogin(string user, string pass)
    {
        Client conn;
        var res = DoccClient.Create(user, pass, out conn);

        Connection = conn;
        return res;
    }
}
