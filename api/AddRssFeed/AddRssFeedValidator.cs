
using System;
using FluentValidation;

namespace RssFeedEater.AddRssFeed
{
  public class AddRssFeedBodyValidator : AbstractValidator<AddRssFeedBody>
  {
    public AddRssFeedBodyValidator()
    {
      RuleFor(x => x.Uri).NotEmpty().Must(BeAValidUri).WithMessage("Please give a valid uri");
    }

    public bool BeAValidUri(string uri)
    {
      if (string.IsNullOrEmpty(uri))
      {
        return true;
      }

      return Uri.TryCreate(uri, UriKind.Absolute, out _);
    }
  }
}