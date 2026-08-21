namespace Claims.Application.Common
{
    /// <summary>
    /// Result class that represents the outcome of an operation, indicating success or failure and providing an error message.
    /// </summary>
    public class Result
    {
        public bool IsSuccess { get; private protected set; }
        public string Error { get; private protected set; } = string.Empty;

        private protected Result() { }

        public static Result Success() => new Result { IsSuccess = true };

        public static Result Failure(string error) => new Result { IsSuccess = false, Error = error };
    }

    /// <summary>
    /// Result class that inherits from Result and includes a value for successful outcomes.
    /// </summary>
    public class Result<T> : Result
    {
        public T? Value { get; private set; }

        private Result() { }

        public static Result<T> Success(T value) => new Result<T> { IsSuccess = true, Value = value };

        public static new Result<T> Failure(string error) => new Result<T> { IsSuccess = false, Error = error };
    }
}
