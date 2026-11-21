namespace OnlineCourses.App;

/// <summary>Реалізує консольний інтерфейс засобами стандартної консолі .NET.</summary>
public sealed class SystemConsoleAdapter : IConsole
{

    /// <inheritdoc />
    public string? ReadLine() => Console.ReadLine();

    /// <inheritdoc />
    public void Write(string value) => Console.Write(value);

    /// <inheritdoc />
    public void WriteLine(string? value = null) => Console.WriteLine(value);
}
