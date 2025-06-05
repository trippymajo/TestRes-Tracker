namespace TrtUploadService
{
    public enum FileValidationError
    {
        None,
        Null,
        TooLarge,
        BadExtension
    }

    /// <summary>
    /// Helping class for validation result instance
    /// </summary>
    public class ValidationResult
    {
        public FileValidationError Error { get; }
        public string? Message { get; }

        /// <summary>
        /// Basic constructor
        /// </summary>
        /// <param name="error">
        /// Error from FileValidationError enum 
        /// [default] FileValidationError.None
        /// </param>
        /// <param name="msg">
        /// Message related to error 
        /// [default] null
        /// </param>
        public ValidationResult(FileValidationError error = FileValidationError.None, string? msg = null)
        {
            Error = error;
            Message = msg;
        }

        public static ValidationResult Success() => new ValidationResult();
        public static ValidationResult Fail(FileValidationError error, string? msg) => new ValidationResult(error, msg);

    }

    /// <summary>
    /// class providing ability to validate IFormFile
    /// </summary>
    public class ValidateFileService
    {
        private static readonly HashSet<string> AllowedExtensions = [".trx"];
        
        /// <summary>
        /// Validate IForm file through checks related to file uploading
        /// </summary>
        /// <param name="file">File to check</param>
        /// <returns>Result in format of Error, Message</returns>
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
