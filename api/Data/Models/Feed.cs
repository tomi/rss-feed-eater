using System;
using Newtonsoft.Json;

namespace RssFeedEater.Data
{
  public class Feed
  {
    [JsonProperty(PropertyName = "id")]
    public string Id { get; set; }
    public string FeedId { get; set; }
    public string Type { get; set; } = "Feed";
    public string RssUrl { get; set; }
    public string Link { get; set; }
    public string Language { get; set; }
    public string ImageUrl { get; set; }
    public string Title { get; set; }
    public DateTime? LastUpdated { get; set; }
  }
}
