

using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;

namespace RssFeedEater.Data
{
  public class RssFeedEaterDb
  {
    public CosmosClient client;
    public Database database;

    private static RssFeedEaterDb _instance;

    private static readonly string _dbName = "RssFeedReaderDB";

    private RssFeedEaterDb(string connectionString)
    {
      client = new CosmosClient(connectionString);
    }

    public async Task<Container> GetContainer(ContainerProperties properties)
    {
      return await this.database.CreateContainerIfNotExistsAsync(properties);
    }

    public static async Task<RssFeedEaterDb> Connect(string connectionString)
    {
      if (_instance == null)
      {
        _instance = new RssFeedEaterDb(connectionString);

        _instance.database = await _instance.client.CreateDatabaseIfNotExistsAsync(_dbName);

      }

      return _instance;
    }
  }
}