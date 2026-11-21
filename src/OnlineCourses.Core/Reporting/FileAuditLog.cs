namespace OnlineCourses.Core.Reporting;

using OnlineCourses.Core.Abstractions;

/// <summary>Додає записи аудиту до текстового файлу.</summary>
public sealed class FileAuditLog : IAuditLogger, IDisposable
{
    private readonly StreamWriter _writer;
    private readonly IClock _clock;
    private bool _disposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="FileAuditLog"/> class.
    /// Відкриває файл журналу для додавання записів.
    /// </summary>
    /// <param name="path">Шлях до файлу журналу.</param>
    /// <param name="clock">Джерело часу; у тестах може бути фіксованим.</param>
    public FileAuditLog(string path, IClock clock)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        ArgumentNullException.ThrowIfNull(clock);
        Directory.CreateDirectory(Path.GetDirectoryName(Path.GetFullPath(path))!);
        _writer = new StreamWriter(path, append: true);
        _clock = clock;
    }

    /// <inheritdoc />
    public void Write(AuditLevel level, string scenario, string message)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        ArgumentException.ThrowIfNullOrWhiteSpace(scenario);
        ArgumentException.ThrowIfNullOrWhiteSpace(message);
        _writer.WriteLine(
            $"{_clock.Now:yyyy-MM-dd};{_clock.Now:HH:mm:ss zzz};{level};{scenario};{message}");
        _writer.Flush();
    }

    /// <inheritdoc />
    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _writer.Dispose();
        _disposed = true;
    }
}
