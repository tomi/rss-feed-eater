
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
    private readonly string _partitionKey = nameof(Feed.FeedId);

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

    public async Task<IEnumerable<FeedItem>> CreateFeedItems(IEnumerable<FeedItem> items)
    {
      var container = await this.GetContainer();

      // Assuming your have your data available to be inserted or read
      var concurrentTasks = new List<Task<ItemResponse<FeedItem>>>();
      foreach (var itemToInsert in items)
      {
        concurrentTasks.Add(container.CreateItemAsync(itemToInsert, new PartitionKey(itemToInsert.FeedId)));
      }

      var createdItems = await Task.WhenAll(concurrentTasks);

      return createdItems.Select(response => response.Resource);
    }

    public async Task<Feed> GetFeedByUri(string feedUri)
    {
      var container = await this.GetContainer();

      try
      {
        QueryDefinition query = new QueryDefinition($"SELECT * FROM Feeds c WHERE c.{nameof(Feed.RssUrl)} = @RssUrl")
            .WithParameter("@RssUrl", feedUri);

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
      var db = await _db.GetDatabase();

      return await db.DefineContainer(name: "Feeds", partitionKeyPath: $"/{_partitionKey}")
        .WithIndexingPolicy()
          .WithIncludedPaths()
            .Path("/")
            .Path($"/{nameof(Feed.Type)}/*")
            .Path($"/{nameof(Feed.RssUrl)}/*")
            .Path($"/{nameof(FeedItem.ItemId)}/*")
            .Path($"/{_partitionKey}/*")
            .Attach()
          .Attach()
        .CreateIfNotExistsAsync();
    }
  }
}