using Newtonsoft.Json;

namespace RssFeedEater.Data
{
  public class Feed
  {
    [JsonProperty(PropertyName = "id")]
    public string Id { get; set; }
    public string Uri { get; set; }
    public string Name { get; set; }
  }
}
