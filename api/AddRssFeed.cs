using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using CodeHollow.FeedReader;
using System.Linq;
using RssFeedEater.Data;

namespace RssFeedEater.AddRssFeed
{
  public static class AddRssFeed
  {
    [FunctionName("AddRssFeed")]
    public static async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
        ILogger log)
    {
      log.LogInformation("C# HTTP trigger function processed a request.");

      var config = ConfigParser.Parse();

      var bodyParsingResult = await req.GetJsonBodyAsync<AddRssFeedBody, AddRssFeedBodyValidator>();
      if (!bodyParsingResult.IsValid)
      {
        bodyParsingResult.LogErrors(log);
        return bodyParsingResult.ToBadRequest();
      }

      var db = await RssFeedEaterDb.Connect(config.CosmosDbConnString);
      var feedRepo = new FeedRepository(db);
      var maybeFeed = await feedRepo.GetFeedByUri(bodyParsingResult.Value.Uri);
      if (maybeFeed != null)
      {
        return new OkObjectResult(maybeFeed);
      }

      var rssFeed = await FeedReader.ReadAsync(bodyParsingResult.Value.Uri);
      log.LogInformation("Feed Title: " + rssFeed.Title);
      log.LogInformation("Feed Description: " + rssFeed.Description);
      log.LogInformation("Feed Image: " + rssFeed.ImageUrl);

      var newFeed = await feedRepo.CreateFeed(new Data.Feed()
      {
        Id = Guid.NewGuid().ToString(),
        Name = rssFeed.Title,
        Uri = bodyParsingResult.Value.Uri
      });

      return new OkObjectResult(newFeed);
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
