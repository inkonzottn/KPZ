namespace OnlineCourses.Core.Abstractions;

/// <summary>Надає поточний час для прикладних сценаріїв і журналювання.</summary>
public interface IClock
{
    /// <summary>Повертає поточний момент часу з часовим поясом.</summary>
    DateTimeOffset Now { get; }

    /// <summary>Повертає календарну дату поточного моменту.</summary>
    DateOnly Today { get; }
}
