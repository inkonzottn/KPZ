namespace OnlineCourses.Core.Abstractions;

/// <summary>Defines replaceable text console input and output operations.</summary>
public interface IConsole
{
    /// <summary>Reads one line of input or returns null at end of input.</summary>
    string? ReadLine();

    /// <summary>Writes text without a trailing newline.</summary>
    void Write(string value);

    /// <summary>Writes text followed by a newline.</summary>
    void WriteLine(string value);
}
