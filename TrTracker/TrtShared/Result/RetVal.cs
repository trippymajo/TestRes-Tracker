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

        public static RetVal Ok()
            => new RetVal(true, null, null);
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

        public static RetVal<T> Ok(T value)
            => new RetVal<T>(true, value, null, null);

        public static RetVal<T> Fail(ErrorType errorType, string errorTxt)
            => new RetVal<T>(false, default, errorType, errorTxt);
    }
}
