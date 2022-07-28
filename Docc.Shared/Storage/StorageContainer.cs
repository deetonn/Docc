using Newtonsoft.Json;

namespace Docc.Common.Storage;

public class StorageContainer : IStorageContainer
{
    private readonly string _configDirectory;

    public StorageContainer(string configDir)
    {
        _configDirectory = configDir;
        SavedItems = new List<IStorageItem>();

        var fileContents = string.Empty;

        try
        {
            fileContents = File.ReadAllText(_configDirectory);
        }
        catch (DirectoryNotFoundException)
        {
            // this is fine, it just means the config file has either
            // been deleted, or it's the first time running.
            // report it though.
            Console.WriteLine($"failed to find config file '{_configDirectory}'");
        }

        if (fileContents != string.Empty)
        {
            try
            {
                SavedItems = JsonConvert.DeserializeObject<List<IStorageItem>>(fileContents)
                    ?? SavedItems;
            }
            catch (JsonSerializationException jse)
            {
                Console.WriteLine($"failed to deserialize saved config. [{jse.Message}]");
            }
        }
    }

    public IList<IStorageItem> SavedItems { get; set; }

    public bool Add(IStorageItem item)
    {
        if (Contains(item) != 0)
        {
            return false;
        }

        SavedItems.Add(item);
        return true;
    }

    public int Contains(IStorageItem item)
    {
        if (SavedItems.Where(x => item.Name == x.Name && item.HashedPassword == x.HashedPassword).Any())
        {
            return 0;
        }

        return -1;
    }

    public void Save()
    {
        var serialized = JsonConvert.SerializeObject(SavedItems);
        try
        {
            File.WriteAllText(serialized, _configDirectory);
        }
        catch (DirectoryNotFoundException e)
        {
            Console.WriteLine($"failed to save config. [{e.Message}]");
        }
    }
}
