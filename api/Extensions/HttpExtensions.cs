
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Newtonsoft.Json;

namespace RssFeedEater
{
  public class ValidatableRequest<T>
  {
    /// <summary>
    /// The deserialized value of the request.
    /// </summary>
    public T Value { get; set; }

    /// <summary>
    /// Whether or not the deserialized value was found to be valid.
    /// </summary>
    public bool IsValid { get; set; }

    /// <summary>
    /// The collection of validation errors.
    /// </summary>
    public IList<ValidationFailure> Errors { get; set; }
  }


  public static class HttpRequestExtensions
  {
    public static async Task<ValidatableRequest<T>> GetJsonBodyAsync<T, V>(this HttpRequest request) where V : AbstractValidator<T>, new()
    {
      var requestObject = await request.GetJsonBody<T>();
      var validator = new V();
      if (requestObject == null)
      {
        return new ValidatableRequest<T>
        {
          Value = requestObject,
          IsValid = false,
          Errors = new List<ValidationFailure>()
          {
            new ValidationFailure("body", "Body can't be empty")
          }
        };
      }

      var validationResult = validator.Validate(requestObject);
      if (!validationResult.IsValid)
      {
        return new ValidatableRequest<T>
        {
          Value = requestObject,
          IsValid = false,
          Errors = validationResult.Errors
        };
      }

      return new ValidatableRequest<T>
      {
        Value = requestObject,
        IsValid = true
      };
    }


    /// <summary>
    /// Returns the deserialized request body.
    /// </summary>

    /// <typeparam name="T">Type used for deserialization of the request body.</typeparam>
    /// <param name="request"></param>
    /// <returns></returns>
    public static async Task<T> GetJsonBody<T>(this HttpRequest request)
    {
      var requestBody = await request.ReadAsStringAsync();

      return JsonConvert.DeserializeObject<T>(requestBody);
    }
  }
}