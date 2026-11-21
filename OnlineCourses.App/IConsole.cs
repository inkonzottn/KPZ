namespace OnlineCourses.App;

/// <summary>Визначає контракт консольного введення та виведення.</summary>
public interface IConsole
{

    /// <summary>Reads the next line of input or returns null at end of input.</summary>
    /// <returns>The entered line, or null when input ends.</returns>
    string? ReadLine();

    /// <summary>Writes text without appending a newline.</summary>
    /// <param name="value">The text to write.</param>
    void Write(string value);

    /// <summary>Writes text followed by a newline.</summary>
    /// <param name="value">The optional text to write.</param>
    void WriteLine(string? value = null);
}
