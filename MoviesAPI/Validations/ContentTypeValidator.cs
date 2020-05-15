using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.AspNetCore.Http;

namespace MoviesAPI.Validations
{
    public class ContentTypeValidator : ValidationAttribute
    {
        private readonly string[] validContentType;
        private readonly string[] imageContentType = new string[] { "image/jpeg", "image/png", "image/gif" };

        public ContentTypeValidator(string[] ValidContentType)
        {
            validContentType = ValidContentType;
        }

        public ContentTypeValidator(ContentTypeGroup contentTypeGroup)
        {
            switch (contentTypeGroup)
            {
               case ContentTypeGroup.Image:
                validContentType = imageContentType;
                break;
            }
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
           {
               return ValidationResult.Success;
           }

           IFormFile formFile = value as IFormFile;
           if (formFile == null)
           {
               return ValidationResult.Success;
           }

           if (!validContentType.Contains(formFile.ContentType))
           {
               return new ValidationResult($"Content-Type should one of the following: {string.Join(", ", validContentType)}");
           }

           return ValidationResult.Success;
        }
    }

    public enum ContentTypeGroup
    {
        Image
    }
}