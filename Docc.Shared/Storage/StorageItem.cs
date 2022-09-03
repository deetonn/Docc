using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Docc.Common.Storage;

// interact with the users storage item without having access to 
// their password
public interface ISafeItem
{
    public string? Name { get; set; }
    public Guid UserId { get; set; }
    public SortedSet<string> Permissions { get; set; }
    public List<string> Tags { get; set; }
}

public class StorageItem : ISafeItem
{
    public StorageItem(string? name, string? hashedPassword)
    {
        Name = name;
        HashedPassword = hashedPassword;
    }

    public virtual string? Name { get; set; }
    public virtual string? HashedPassword { get; set; }
    public virtual Guid UserId { get; set; } = Guid.NewGuid();

    public virtual SortedSet<string> Permissions { get; set; } = new() { "send_message" };
    public virtual List<string> Tags { get; set; } = new() { "User" };
}
