using System.Text;
using OnlineCourses.Core.Abstractions;

internal sealed class SystemConsole : IConsole
{
    public SystemConsole()
    {
        Console.OutputEncoding = Encoding.UTF8;
    }

    public string? ReadLine() => Console.ReadLine();

    public void Write(string value) => Console.Write(value);

    public void WriteLine(string value) => Console.WriteLine(value);
}
