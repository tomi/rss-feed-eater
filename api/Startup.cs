using Microsoft.Azure.Functions.Extensions.DependencyInjection;
// using Microsoft.Azure.KeyVault;
// using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RssFeedEater.Data;
using RssFeedEater.Services;
using System;

[assembly: FunctionsStartup(typeof(RssFeedEater.Startup))]
namespace RssFeedEater
{
  public class Startup : FunctionsStartup
  {
    public override void Configure(IFunctionsHostBuilder builder)
    {
      builder.Services.AddScoped<AzureADJwtBearerValidation>();
      builder.Services.AddScoped<RssFeedEaterDb>();
      builder.Services.AddScoped<FeedRepository>();
    }

    public override void ConfigureAppConfiguration(IFunctionsConfigurationBuilder builder)
    {
      var builtConfig = builder.ConfigurationBuilder.Build();

      // local dev no Key Vault
      builder.ConfigurationBuilder
         .SetBasePath(Environment.CurrentDirectory)
         .AddJsonFile("local.settings.json", true)
         .AddEnvironmentVariables()
         .Build();
    }
  }
}