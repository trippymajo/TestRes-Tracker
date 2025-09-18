namespace TrtShared.RetValType
{
    public enum ErrorType
    {
        BadRequest, // 400
        Forbidden, // 403
        Conflict, // 409
        NotFound, // 404
        ServerError, // 5xx
        Unexpected // 4xx - 5xx
    }

    /// <summary>
    /// A Helper class to connect workflow services with controllers via result object
    /// </summary>
    public class RetVal
    {
        public bool Success { get; }
        public ErrorType? ErrorType { get; }
        public string? ErrorText {  get; }

        protected RetVal(bool success, ErrorType? errorType, string? errorTxt)
        {
            Success = success;
            ErrorType = errorType;
            ErrorText = errorTxt;
        }

        /// <summary>
        /// Returns RetVal with successful result
        /// </summary>
        /// <returns>RetVal object</returns>
        public static RetVal Ok()
            => new RetVal(true, null, null);

        /// <summary>
        /// Returns RetVal with fail result, error type and error text
        /// </summary>
        /// <returns>RetVal object</returns>
        public static RetVal Fail(ErrorType errorType, string errorTxt)
            => new RetVal(false, errorType, errorTxt);
    }

    public class RetVal<T> : RetVal
    {
        public T? Value { get; }

        protected RetVal(bool success, T? value, ErrorType? errorType, string? errorTxt)
            : base(success, errorType, errorTxt)
        {
            Value = value;
        }

        /// <summary>
        /// Returns RetVal<T> with successful result
        /// </summary>
        /// <returns>RetVal<T> object</returns>
        public static RetVal<T> Ok(T value)
            => new RetVal<T>(true, value, null, null);

        /// <summary>
        /// Returns RetVal<T> with fail result, error type and error text
        /// </summary>
        /// <returns>RetVal<T> object</returns>
        public static new RetVal<T> Fail(ErrorType errorType, string errorTxt)
            => new RetVal<T>(false, default, errorType, errorTxt);
    }
}
