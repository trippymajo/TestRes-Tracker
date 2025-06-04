namespace TrtUploadService
{
    public enum FileValidationError
    {
        None,
        Null,
        TooLarge,
        BadExtension
    }

    public class ValidationResult
    {
        public FileValidationError Error { get; }
        public string? Message { get; }

        public ValidationResult(FileValidationError error = FileValidationError.None, string? msg = null)
        {
            Error = error;
            Message = msg;
        }

        public static ValidationResult Success() => new ValidationResult();
        public static ValidationResult Fail(FileValidationError error, string? msg) => new ValidationResult(error, msg);

    }

    public class ValidateFile
    {
        private static readonly HashSet<string> AllowedExtensions = [".trx"];

        public ValidationResult Validate(IFormFile file)
        {
            if (file == null)
                return ValidationResult.Fail(FileValidationError.Null, "File is null");

            if (file.Length > 50 * 1024 * 1024)
                return ValidationResult.Fail(FileValidationError.TooLarge, "File is more than 50 MB");

            var fileExt = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (string.IsNullOrEmpty(fileExt) || !AllowedExtensions.Contains(fileExt))
                return ValidationResult.Fail(FileValidationError.BadExtension, "Unsuported file extension!");

            return ValidationResult.Success();
        }
    }
}
