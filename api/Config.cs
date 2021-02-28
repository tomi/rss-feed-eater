
using System;

namespace RssFeedEater
{
  public class Config
  {
    public string CosmosDbConnString { get; set; }
  }

  public static class ConfigParser
  {
    public static Config Parse()
    {
      var cosmosDbConnString = System.Environment.GetEnvironmentVariable("COSMOS_DB_CONNECTION_STRING", EnvironmentVariableTarget.Process);
      if (String.IsNullOrEmpty(cosmosDbConnString))
      {
        throw new Exception("Missing env variable COSMOS_DB_CONNECTION_STRING");
      }

      return new Config()
      {
        CosmosDbConnString = cosmosDbConnString
      };
    }
  }
}