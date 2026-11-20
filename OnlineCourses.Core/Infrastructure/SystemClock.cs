namespace OnlineCourses.Core.Infrastructure;

using OnlineCourses.Core.Abstractions;

/// <summary>Системна реалізація джерела календарної дати.</summary>
public sealed class SystemClock : IClock
{
    /// <inheritdoc />
    public DateOnly Today => DateOnly.FromDateTime(DateTime.Today);
}
