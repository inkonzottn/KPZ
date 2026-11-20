namespace OnlineCourses.Core.Abstractions;

/// <summary>Надає поточний час для прикладних сценаріїв і журналювання.</summary>
public interface IClock
{
    /// <summary>Gets the current point in time with its time zone.</summary>
    DateTimeOffset Now { get; }

    /// <summary>Gets the calendar date of the current point in time.</summary>
    DateOnly Today { get; }
}
