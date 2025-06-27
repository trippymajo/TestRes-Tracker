namespace TrtShared.RetVal
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

    public class Result<T> : RetVal
    {
        public T? Value { get; }

        protected Result(bool success, T? value, ErrorType? errorType, string? errorTxt)
            : base(success, errorType, errorTxt)
        {
            Value = value;
        }

        public static Result<T> Ok(T value)
            => new Result<T>(true, value, null, null);

        public static Result<T> Fail(ErrorType errorType, string errorTxt)
            => new Result<T>(false, default, errorType, errorTxt);
    }
}
