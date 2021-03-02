using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using CodeHollow.FeedReader;
using RssFeedEater.Data;
using RssFeedEater.Services;
using System.Linq;

namespace RssFeedEater.AddRssFeed
{
  public class AddRssFeedFunction
  {
    private readonly AzureADJwtBearerValidation _azureADJwtBearerValidation;
    private readonly FeedRepository _repository;

    public AddRssFeedFunction(
      AzureADJwtBearerValidation azureADJwtBearerValidation,
      FeedRepository repository
    )
    {
      _azureADJwtBearerValidation = azureADJwtBearerValidation;
      _repository = repository;
    }

    [FunctionName("AddRssFeed")]
    public async Task<IActionResult> AddRssFeed(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
        ILogger log)
    {
      log.LogInformation("C# HTTP trigger function processed a request.");

      var bodyParsingResult = await req.GetJsonBodyAsync<AddRssFeedBody, AddRssFeedBodyValidator>();
      if (!bodyParsingResult.IsValid)
      {
        bodyParsingResult.LogErrors(log);
        return bodyParsingResult.ToBadRequest();
      }

      var maybeFeed = await this._repository.GetFeedByUri(bodyParsingResult.Value.Uri);
      if (maybeFeed != null)
      {
        return new OkObjectResult(maybeFeed);
      }

      var rssFeed = await FeedReader.ReadAsync(bodyParsingResult.Value.Uri);
      log.LogInformation("Feed Title: " + rssFeed.Title);
      log.LogInformation("Feed Description: " + rssFeed.Description);
      log.LogInformation("Feed Image: " + rssFeed.ImageUrl);

      var feedId = Guid.NewGuid().ToString();
      log.LogInformation($"Creating feed with id {feedId}");
      var newFeed = await this._repository.CreateFeed(new Data.Feed()
      {
        Id = feedId,
        FeedId = feedId,
        Title = rssFeed.Title,
        RssUrl = bodyParsingResult.Value.Uri,
        Link = rssFeed.Link,
        ImageUrl = rssFeed.ImageUrl,
        Language = rssFeed.Language,
        LastUpdated = rssFeed.LastUpdatedDate
      });

      log.LogInformation("Creating feed items");
      var feedItems = await this._repository.CreateFeedItems(
        rssFeed.Items.Select(item => new Data.FeedItem()
        {
          Id = Guid.NewGuid().ToString(),
          FeedId = newFeed.Id,
          ItemId = item.Id,
          Link = item.Link,
          Title = item.Title,
          Description = item.Description,
          PublishingDate = item.PublishingDate,
          Author = item.Author,
          Categories = item.Categories,
          Content = item.Content,
        })
      );

      return new OkObjectResult(rssFeed);
      // return new OkObjectResult(new List<object>() { newFeed }.Concat(feedItems));
      // string name = req.Query["name"];
      // string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
      // dynamic data = JsonConvert.DeserializeObject(requestBody);
      // name = name ?? data?.name;

      // string responseMessage = string.IsNullOrEmpty(name)
      //     ? "This HTTP triggered function executed successfully. Pass a name in the query string or in the request body for a personalized response."
      //     : $"Hello, {name}. This HTTP triggered function executed successfully.";


      // log.LogInformation("Feed Title: " + feed.Title);
      // log.LogInformation("Feed Description: " + feed.Description);
      // log.LogInformation("Feed Image: " + feed.ImageUrl);
      // // ...
      // foreach (var item in feed.Items)
      // {
      //   log.LogInformation(item.Title + " - " + item.Link + " - " + item.Id);
      // }

      // return new OkObjectResult(responseMessage);
    }
  }
}
