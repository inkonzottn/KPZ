namespace OnlineCourses.Core.Domain;

public sealed class Course
{
    private readonly HashSet<int> _prerequisiteCourseIds;

    public Course(int id, string code, string title, decimal price, IEnumerable<int>? prerequisiteCourseIds = null)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(id);
        ArgumentException.ThrowIfNullOrWhiteSpace(code);
        ArgumentException.ThrowIfNullOrWhiteSpace(title);
        ArgumentOutOfRangeException.ThrowIfNegative(price);

        Id = id;
        Code = code.Trim();
        Title = title.Trim();
        Price = price;
        _prerequisiteCourseIds = prerequisiteCourseIds is null
            ? new HashSet<int>()
            : new HashSet<int>(prerequisiteCourseIds);

        if (_prerequisiteCourseIds.Contains(id))
        {
            throw new ArgumentException("Курс не може бути власною передумовою.", nameof(prerequisiteCourseIds));
        }

        if (_prerequisiteCourseIds.Any(x => x <= 0))
        {
            throw new ArgumentException("Ідентифікатори передумов мають бути додатними.", nameof(prerequisiteCourseIds));
        }
    }

    public int Id { get; }

    public string Code { get; }

    public string Title { get; }

    public decimal Price { get; }

    public IReadOnlySet<int> PrerequisiteCourseIds => _prerequisiteCourseIds;
}
