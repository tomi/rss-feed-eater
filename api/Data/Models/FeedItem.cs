using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace RssFeedEater.Data
{
  public class FeedItem
  {
    [JsonProperty(PropertyName = "id")]
    public string Id { get; set; }
    public string FeedId { get; set; }
    public string ItemId { get; set; }
    public string Type { get; set; } = "FeedItem";
    public string Link { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public DateTime? PublishingDate { get; set; }
    public string Author { get; set; }
    public ICollection<string> Categories { get; set; }
    public string Content { get; set; }
  }
}
