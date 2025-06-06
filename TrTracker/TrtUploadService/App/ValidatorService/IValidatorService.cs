namespace TrtUploadService.App.ValidatorService
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

        /// <summary>
        /// Successful Validation
        /// </summary>
        /// <returns>Successful ValidationResult item</returns>
        public static ValidationResult Success() => new ValidationResult();

        /// <summary>
        /// Failed Validation
        /// </summary>
        /// <param name="error"></param>
        /// <param name="msg"></param>
        /// <returns>Failed ValidationResult item</returns>
        public static ValidationResult Fail(FileValidationError error, string? msg) => new ValidationResult(error, msg);

    }

    /// <summary>
    /// Providing ability to validate
    /// </summary>
    public interface IValidatorService
    {
        /// <summary>
        /// Validate IForm file through checks related to file uploading
        /// </summary>
        /// <param name="file">File to check</param>
        /// <returns>Result in format of Error, Message</returns>
        public ValidationResult Validate(IFormFile file);
    }
}
