namespace OnlineCourses.Core.Abstractions;

/// <summary>Надає поточну календарну дату для прикладних сценаріїв.</summary>
public interface IClock
{
    /// <summary>Повертає сьогоднішню дату.</summary>
    DateOnly Today { get; }
}
