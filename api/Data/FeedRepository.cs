
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;

namespace RssFeedEater.Data
{
  public class FeedRepository
  {
    private readonly RssFeedEaterDb _db;
    // private static readonly string containerName = "Feeds";

    public FeedRepository(RssFeedEaterDb db)
    {
      _db = db;
    }

    public async Task<Feed> CreateFeed(Feed feed)
    {
      var container = await this.GetContainer();

      var response = await container.CreateItemAsync<Feed>(feed);

      return response.Resource;
    }

    public async Task<Feed> GetFeedByUri(string feedUri)
    {
      var container = await this.GetContainer();

      try
      {
        QueryDefinition query = new QueryDefinition($"SELECT * FROM Feeds c WHERE c.Uri = @Uri")
            .WithParameter("@Uri", feedUri);

        List<Feed> results = new List<Feed>();
        using (FeedIterator<Feed> resultSetIterator = container.GetItemQueryIterator<Feed>(
            query,
            requestOptions: new QueryRequestOptions()
            {
              PartitionKey = new PartitionKey(feedUri)
            }))
        {
          while (resultSetIterator.HasMoreResults)
          {
            FeedResponse<Feed> response = await resultSetIterator.ReadNextAsync();
            results.AddRange(response);
            if (response.Diagnostics != null)
            {
              System.Console.WriteLine($"\nQueryWithSqlParameters Diagnostics: {response.Diagnostics.ToString()}");
            }
          }
        }

        return results.FirstOrDefault();
      }
      catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
      {
        return null;
      }
    }

    private async Task<Container> GetContainer()
    {
      return await _db.database.DefineContainer(name: "Feeds", partitionKeyPath: "/Uri")
        .WithUniqueKey().Path("/Uri").Attach()
        .WithIndexingPolicy()
          .WithIncludedPaths().Path("/").Path("/Uri/*").Attach()
          .Attach()
        .CreateIfNotExistsAsync();
    }
  }
}