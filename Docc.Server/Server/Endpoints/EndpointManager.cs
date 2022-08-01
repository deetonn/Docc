using Docc.Common;
using Docc.Common.Data;
using Docc.Server.Data;

namespace Docc.Server.Endpoints;

/*
 * 
 */

internal delegate Request ContentCallback(Dictionary<string, string> args, Connection conn);

internal record DirectoryListing(string Location, ContentCallback Responder);

internal class DirectoryListingManager
{
    public List<DirectoryListing> ActiveListings { get; set; }
        = new List<DirectoryListing>();
    public int Count()
        => ActiveListings.Count;

    public void MapGet(string Location, ContentCallback callback)
    {
        ActiveListings.Add(new DirectoryListing(Location, callback));
    }
    public void MapSet(string Location, ContentCallback callback)
    {
        ActiveListings.Add(new DirectoryListing(Location, callback));
    }

    public void Map(string Location, ContentCallback callback)
        => MapSet(Location, callback);

    /*
     * TODO:
     * 
     * The below functions are kind of redundant.
     * They could be implemented much better, their names also make no sense.
     */

    /// <summary>
    /// automatically serves <paramref name="info"/> to the specified <paramref name="location"/>
    /// </summary>
    /// <param name="location">The mapped callback to search for</param>
    /// <param name="info">The sender information, the person to serve <paramref name="location"/> to.</param>
    public void CallMappedOnline(string location, Dictionary<string, string> args, Connection info)
    {
        if (!ActiveListings.Any(ActiveListings.Contains))
        {
            info.Socket?.SendRequest(new Request { Location = "/404" });
            return;
        }

        var response = ActiveListings
            .Where(x => x.Location == location)
            .First()
            .Responder
            .Invoke(args, info);

        info.Socket?.SendRequest(response);
    }

    public Request CallMappedLocal(string location, Dictionary<string, string> args)
    {
        if (!ActiveListings.Any(x => x.Location == location))
        {
            return new Request { Location = "/404" };
        }

        var response = ActiveListings
            .Where(x => x.Location == location)
            .First()
            .Responder
            .Invoke(args, null!);

        return response;
    }
}
