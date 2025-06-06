using TrtUploadService.App.ValidatorService;

namespace TrtUploadService.Implementation.ValidatorService
{
    public class ValidatorService : IValidatorService
    {
        public ValidationResult Validate(IFormFile file)
        {
            if (file == null)
                return ValidationResult.Fail(FileValidationError.Null, "File is null");

            if (file.Length > 50 * 1024 * 1024)
                return ValidationResult.Fail(FileValidationError.TooLarge, "File is more than 50 MB");

            var fileExt = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (string.IsNullOrEmpty(fileExt) || !FileExtensionsDefaults.AllowedExtensions.Contains(fileExt))
                return ValidationResult.Fail(FileValidationError.BadExtension, "Unsuported file extension!");

            return ValidationResult.Success();
        }
    }
}
