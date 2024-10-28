namespace KTRegistration.Core.Abstractions;
public class Result
{

    //internal use ctor
    protected Result(bool isSuccess, Error error)
    {
        if ((isSuccess && error != Error.None) || (!isSuccess && error == Error.None))
            throw new InvalidOperationException();

        this.IsSuccess = isSuccess;
        this.Error = error;
    }

    //prop
    public bool IsSuccess { get; }
    public bool IsFailure { get { return !IsSuccess; } }
    public Error Error { get; } = default!;


    // Methods 
    public static Result Success()
    {
        return new Result(true, Error.None);
    }

    public static Result Failure(Error error)
    {
        return new Result(false, error);
    }

    public static Result<T> Success<T>(T data)
    {
        return new Result<T>(data, true, Error.None);
    }

    public static Result<T> Failure<T>(Error error)
    {
        return new Result<T>(default, false, error);
    }
}

public class Result<T>(T? data, bool isSuccess, Error error) : Result(isSuccess, error)
{
    private readonly T? _data = data;

    public T Data
    {
        get
        {
            if (IsSuccess)
            {
                return _data!;
            }
            else
            {
                throw new InvalidOperationException("Failure Cannot have value");
            }
        }
    }
}
