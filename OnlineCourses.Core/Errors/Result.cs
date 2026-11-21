namespace OnlineCourses.Core.Errors;

/// <summary>Представляє успішний результат операції або опис її невдачі.</summary>
/// <typeparam name="T">Тип значення успішного результату.</typeparam>
public readonly struct Result<T>
{
    private Result(bool ok, T? value, string error)
    {
        IsSuccess = ok;
        Value = value;
        Error = error;
    }

    /// <summary>Gets a value indicating whether the operation succeeded.</summary>
    public bool IsSuccess { get; }

    /// <summary>Gets the value returned by a successful operation.</summary>
    public T? Value { get; }

    /// <summary>Gets the error message returned by a failed operation.</summary>
    public string Error { get; }

    /// <summary>Creates a successful result containing the specified value.</summary>
    /// <param name="value">The value returned by the operation.</param>
    /// <returns>A successful result.</returns>
    public static Result<T> Ok(T value)
        => new(true, value, string.Empty);

    /// <summary>Creates a failed result containing the specified error message.</summary>
    /// <param name="error">A description of the failure.</param>
    /// <returns>A failed result.</returns>
    public static Result<T> Fail(string error)
        => new(false, default, error);
}
