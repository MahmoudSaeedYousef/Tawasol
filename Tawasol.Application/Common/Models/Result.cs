namespace Tawasol.Application.Common.Models;

// Non-generic base Result class for failures without data
public class Result
{
    public bool IsSuccess { get; protected set; }
    public string Message { get; protected set; } = string.Empty;
    public List<string>? Errors { get; protected set; }

    protected Result(bool isSuccess, string message, List<string>? errors)
    {
        IsSuccess = isSuccess;
        Message = message;
        Errors = errors;
    }

    public static Result Failure(List<string> errors, string message = "فشلت العملية")
    {
        return new Result(false, message, errors);
    }

    public static Result Failure(string error, string message = "فشلت العملية")
    {
        return new Result(false, message, new List<string> { error });
    }
}

public class Result<T> : Result
{
    public T? Data { get; private set; }

    private protected Result(bool isSuccess, string message, T? data, List<string>? errors)
        : base(isSuccess, message, errors)
    {
        Data = data;
    }

    public static Result<T> Success(T data, string message = "تمت العملية بنجاح")
    {
        return new Result<T>(true, message, data, null);
    }

    // Overload Failure to return generic Result<T> for consistency, even if Data is default
    public new static Result<T> Failure(List<string> errors, string message = "فشلت العملية")
    {
        return new Result<T>(false, message, default, errors);
    }

    public new static Result<T> Failure(string error, string message = "فشلت العملية")
    {
        return new Result<T>(false, message, default, new List<string> { error });
    }
}
