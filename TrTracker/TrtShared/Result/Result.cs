namespace TrtShared.Result
{
    public enum ErrorType
    {
        BadRequest,
        Forbidden,
        Conflict,
        NotFound,
        Unexpected
    }

    public class Result
    {
        public bool Success { get; }
        public ErrorType? ErrorType { get; }
        public string? ErrorText {  get; }

        protected Result(bool success, ErrorType? errorType, string? errorTxt)
        {
            Success = success;
            ErrorType = errorType;
            ErrorText = errorTxt;
        }

        public static Result Ok()
            => new Result(true, null, null);
        public static Result Fail(ErrorType errorType, string errorTxt)
            => new Result(false, errorType, errorTxt);
    }

    public class Result<T> : Result
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
