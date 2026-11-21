namespace OnlineCourses.Core.Reporting;

/// <summary>Показує кількість зайнятих і вільних місць у навчальній групі.</summary>
/// <param name="CourseCode">Код курсу, до якого належить група.</param>
/// <param name="GroupName">Назва навчальної групи.</param>
/// <param name="Occupied">Кількість активних зарахувань.</param>
/// <param name="Capacity">Максимальна місткість групи.</param>
public sealed record GroupOccupancy(
    string CourseCode,
    string GroupName,
    int Occupied,
    int Capacity)
{
    private const decimal PercentageScale = 100m;
    private const int PercentageDecimalPlaces = 2;

    /// <summary>Gets the number of available seats in the group.</summary>
    public int Free => Capacity - Occupied;

    /// <summary>Gets the percentage of occupied seats, rounded to two decimal places.</summary>
    public decimal OccupancyPercent =>
        Capacity == 0 ? 0m : decimal.Round((decimal)Occupied / Capacity * PercentageScale, PercentageDecimalPlaces);
}
