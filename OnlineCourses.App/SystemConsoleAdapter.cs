namespace OnlineCourses.App;

public sealed class SystemConsoleAdapter : IConsole
{
    public string? ReadLine() => Console.ReadLine();

    public void Write(string value) => Console.Write(value);

    public void WriteLine(string? value = null) => Console.WriteLine(value);
}
