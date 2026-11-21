namespace OnlineCourses.Core.Abstractions;

/// <summary>Надає поточний час для прикладних сценаріїв і журналювання.</summary>
public interface IClock
{
    /// <summary>Gets the current date and time with its time zone.</summary>
    DateTimeOffset Now { get; }

    /// <summary>Gets the calendar date of the current moment.</summary>
    DateOnly Today { get; }
}
