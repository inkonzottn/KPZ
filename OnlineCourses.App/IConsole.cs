namespace OnlineCourses.App;

public interface IConsole
{
    string? ReadLine();

    void Write(string value);

    void WriteLine(string? value = null);
}
