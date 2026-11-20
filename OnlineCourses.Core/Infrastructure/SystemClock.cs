namespace OnlineCourses.Core.Infrastructure;

using OnlineCourses.Core.Abstractions;

/// <summary>Системна реалізація джерела часу.</summary>
public sealed class SystemClock : IClock
{
    /// <inheritdoc />
    public DateTimeOffset Now => DateTimeOffset.Now;

    /// <inheritdoc />
    public DateOnly Today => DateOnly.FromDateTime(Now.LocalDateTime);
}
