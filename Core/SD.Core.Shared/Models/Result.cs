namespace SD.Core.Shared.Models;
public record Result
{
    public bool IsSuccess { get; }
    public string Message { get; }
    public bool IsFailure => !IsSuccess;

    protected Result(bool isSuccess, string message)
    {
        if (isSuccess && message != string.Empty)
            throw new InvalidOperationException();
        if (!isSuccess && message == string.Empty)
            throw new InvalidOperationException();

        IsSuccess = isSuccess;
        Message = message;
    }

    public static Result Fail(string message)
    {
        return new Result(false, message);
    }

    public static Result<T> Fail<T>(string message)
    {
        return new Result<T>(default, false, message);
    }

    public static Result<T> Fail<T>(T value, string message)
    {
        return new Result<T>(value, false, message);
    }

    public static Result Ok()
    {
        return new Result(true, string.Empty);
    }

    public static Result Ok(string message)
    {
        return new Result(true, message);
    }

    public static Result<T> Ok<T>(T value)
    {
        return new Result<T>(value, true, string.Empty);
    }

    public static Result<T> Ok<T>(T value, string message)
    {
        return new Result<T>(value, true, message);
    }

    public static Result Combine(params Result[] results)
    {
        foreach (Result result in results)
        {
            if (result.IsFailure)
                return result;
        }

        return Ok();
    }
}

public record Result<T> : Result
{
    private readonly T? _value;

    public T? Value
    {
        get
        {
            if (!IsSuccess)
                throw new InvalidOperationException();

            return _value;
        }
    }

    protected internal Result(T? value, bool isSuccess, string error) : base(isSuccess, error)
    {
        _value = value;
    }
}