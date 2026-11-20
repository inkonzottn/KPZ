namespace OnlineCourses.Core.Reporting;

/// <summary>Represents the GroupOccupancy type.</summary>
public sealed record GroupOccupancy(

    string CourseCode,
    string GroupName,
    int Occupied,
    int Capacity)
{
    private const decimal PercentScale = 100m;

    /// <summary>Defines the Free member.</summary>
    public int Free => Capacity - Occupied;

    /// <summary>Defines the OccupancyPercent member.</summary>
    public decimal OccupancyPercent =>
        Capacity == 0 ? 0m : decimal.Round((decimal)Occupied / Capacity * PercentScale, 2);
}
