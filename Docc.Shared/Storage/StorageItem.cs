using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Docc.Common.Storage;

public class StorageItem : IStorageItem
{
    public StorageItem(string? name, string? hashedPassword)
        : base(name, hashedPassword)
    {
    }

    public override string? Name { get; init; }

    public override string? HashedPassword { get; init; }
}
