using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace MoviesAPI.Validations
{
    public class FileSizeValidator : ValidationAttribute
    {
        private readonly int maxFileSizeInMb;

        public FileSizeValidator(int MaxFileSizeInMb)
        {
            maxFileSizeInMb = MaxFileSizeInMb;
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

           if (formFile.Length > maxFileSizeInMb * 1024 *1024)
           {
               return new ValidationResult($"File size must not be bigger than {maxFileSizeInMb} megabytes");
           }

           return ValidationResult.Success;
       }
    }
}