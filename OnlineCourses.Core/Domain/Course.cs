/// <summary>Курс у каталозі курсів.</summary>
public sealed class Course 
{
    public string Code { get; init; } = string.Empty;
    public string Title { get; init; } = string.Empty;
    public string Type { get; init; } = "Basic";    
    public decimal Price { get; init; }
    public int AvailableSeats { get; init; }

    public DateOnly StartDate { get; init; }
    public DateOnly EndDate { get; init; }
    
    public int DurationInDays => EndDate.DayNumber - StartDate.DayNumber;
}