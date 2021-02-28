using System.Threading.Tasks;
using CodeHollow.FeedReader;

namespace RssFeedEater.Services
{
  static class Feed
  {
    static async public Task<CodeHollow.FeedReader.Feed> ReadAsync(string url)
    {
      var feed = await FeedReader.ReadAsync("https://azurecomcdn.azureedge.net/en-us/blog/feed/");

      return feed;
    }
  }
}

