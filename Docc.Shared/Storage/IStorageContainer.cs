using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Docc.Common.Storage;

public interface IStorageContainer
{
    IList<IStorageItem> SavedItems { get; }

    public bool Add(IStorageItem item);

    public int Contains(IStorageItem item);

    public void Save();
}
