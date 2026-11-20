namespace OnlineCourses.Core.Domain;

/// <summary>Represents the Course type.</summary>
public sealed class Course
{
    private readonly HashSet<int> _prerequisiteCourseIds;

    /// <summary>Initializes a new instance of the <see cref="Course"/> class.</summary>
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

    /// <summary>Gets the Id value.</summary>
    public int Id { get; }

    /// <summary>Gets the Code value.</summary>
    public string Code { get; }

    /// <summary>Gets the Title value.</summary>
    public string Title { get; }

    /// <summary>Gets the Price value.</summary>
    public decimal Price { get; }

    /// <summary>Defines the PrerequisiteCourseIds member.</summary>
    public IReadOnlySet<int> PrerequisiteCourseIds => _prerequisiteCourseIds;
}
