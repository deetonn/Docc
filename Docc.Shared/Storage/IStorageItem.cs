using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Docc.Common.Storage;

public abstract class IStorageItem
{
    public IStorageItem(string? name, string? hashedPassword)
    {
        Name = name;
        HashedPassword = hashedPassword;
    }

    public virtual string? Name { get; init; }
    public virtual string? HashedPassword { get; init; }
}
