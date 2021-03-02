

using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace RssFeedEater.Data
{
  public class RssFeedEaterDb
  {
    private IConfiguration _configuration;
    private ILogger _log;
    private CosmosClient _client;
    private Database _database;

    private static readonly string _dbName = "RssFeedReaderDB";

    public RssFeedEaterDb(IConfiguration configuration, ILoggerFactory loggerFactory)
    {
      _configuration = configuration;
      _log = loggerFactory.CreateLogger<RssFeedEaterDb>();
      _client = new CosmosClient(
        _configuration["COSMOS_DB_CONNECTION_STRING"],
        new CosmosClientOptions()
        {
          AllowBulkExecution = true
        });
    }

    public async Task<Database> GetDatabase()
    {
      if (_database == null)
      {
        _database = await _client.CreateDatabaseIfNotExistsAsync(_dbName);
      }

      return _database;
    }
  }
}