namespace OnlineCourses.Core.Reporting;

public sealed record GroupOccupancy(
    string CourseCode,
    string GroupName,
    int Occupied,
    int Capacity)
{
    public int Free => Capacity - Occupied;

    public decimal OccupancyPercent =>
        Capacity == 0 ? 0m : decimal.Round((decimal)Occupied / Capacity * 100m, 2);
}
