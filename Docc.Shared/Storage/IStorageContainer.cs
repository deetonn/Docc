using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Docc.Common.Storage;

public interface IStorageContainer
{
    IList<StorageItem> SavedItems { get; }

    public bool Add(StorageItem item);

    public int Contains(StorageItem item);

    public StorageItem? Get(string userName);

    public void Save();
}
