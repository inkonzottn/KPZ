namespace OnlineCourses.Core.Errors;

/// <summary>Represents the Result type.</summary>
public readonly struct Result<T>
{
    private Result(bool ok, T? value, string error)
    {
        IsSuccess = ok;
        Value = value;
        Error = error;
    }

    /// <summary>Gets the IsSuccess value.</summary>
    public bool IsSuccess { get; }

    /// <summary>Gets the Value value.</summary>
    public T? Value { get; }

    /// <summary>Gets the Error value.</summary>
    public string Error { get; }

    /// <summary>Performs the Ok operation.</summary>
    public static Result<T> Ok(T value)
        => new(true, value, string.Empty);

    /// <summary>Performs the Fail operation.</summary>
    public static Result<T> Fail(string error)
        => new(false, default, error);
}
