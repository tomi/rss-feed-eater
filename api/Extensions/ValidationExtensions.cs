using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace RssFeedEater
{
  public static class ValidationExtensions
  {
    /// <summary>
    /// Creates a <see cref="BadRequestObjectResult"/> containing a collection
    /// of minimal validation error details.
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    public static BadRequestObjectResult ToBadRequest<T>(this ValidatableRequest<T> request) =>
      new BadRequestObjectResult(request.Errors.Select(e => new
      {
        Field = e.PropertyName,
        Error = e.ErrorMessage
      }));

    public static void LogErrors<T>(this ValidatableRequest<T> request, ILogger log)
    {
      var errors = String.Join(
        ",",
        request.Errors.Select(e =>
          $"{{ Field = {e.PropertyName}, Error = {e.ErrorMessage} }}")
      );

      log.LogWarning($"Bad request: {errors}");
    }
  }
}
