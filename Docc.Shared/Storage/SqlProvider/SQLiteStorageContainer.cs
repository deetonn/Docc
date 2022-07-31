using System.Data.SQLite;

namespace Docc.Common.Storage;

/*
 * Just a start
 */

public class SQLiteStorageContainer : IStorageContainer
{
    private readonly string _configDirectory;

    protected SQLiteConnection SQLiteConnection { get; set; }

    public IList<StorageItem> SavedItems => throw new NotImplementedException("SQLiteStorageContainer cannot implement SavedItems.");

    public SQLiteStorageContainer(string configDirectory)
    {
        _configDirectory = configDirectory;
        SQLiteConnection = new SQLiteConnection($"data source={configDirectory}");
    }

    public bool Add(StorageItem item)
    {
        throw new NotImplementedException();
    }

    public int Contains(StorageItem item)
    {
        throw new NotImplementedException();
    }

    public StorageItem? Get(string userName)
    {
        throw new NotImplementedException();
    }

    public void Save()
    {
        throw new NotImplementedException();
    }
}
