namespace OnlineCourses.Core.Domain;

/// <summary>Описує навчальний курс, його ціну та обов'язкові передумови.</summary>
public sealed class Course
{
    private readonly HashSet<int> _prerequisiteCourseIds;

    /// <summary>Initializes a new instance of the <see cref="Course"/> class without prerequisites.</summary>
    /// <param name="id">Додатний унікальний ідентифікатор курсу.</param>
    /// <param name="code">Непорожній код курсу.</param>
    /// <param name="title">Непорожня назва курсу.</param>
    /// <param name="price">Невід'ємна вартість курсу.</param>
    public Course(int id, string code, string title, decimal price)
        : this(id, new CourseDefinition(code, title, price))
    {
    }

    /// <summary>Initializes a new instance of the <see cref="Course"/> class with prerequisites.</summary>
    /// <param name="id">Додатний унікальний ідентифікатор курсу.</param>
    /// <param name="definition">Опис курсу, включно з передумовами.</param>
    /// <exception cref="ArgumentException">Опис курсу або передумови некоректні.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Ідентифікатор або ціна поза допустимими межами.</exception>
    public Course(int id, CourseDefinition definition)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(id);
        ArgumentNullException.ThrowIfNull(definition);
        ArgumentException.ThrowIfNullOrWhiteSpace(definition.Code);
        ArgumentException.ThrowIfNullOrWhiteSpace(definition.Title);
        ArgumentOutOfRangeException.ThrowIfNegative(definition.Price);

        Id = id;
        Code = definition.Code.Trim();
        Title = definition.Title.Trim();
        Price = definition.Price;
        _prerequisiteCourseIds = definition.PrerequisiteCourseIds is null
            ? new HashSet<int>()
            : new HashSet<int>(definition.PrerequisiteCourseIds);

        ValidatePrerequisites(id, _prerequisiteCourseIds);
    }

    /// <summary>Gets the unique course identifier.</summary>
    public int Id { get; }

    /// <summary>Gets the short course code.</summary>
    public string Code { get; }

    /// <summary>Gets the course title.</summary>
    public string Title { get; }

    /// <summary>Gets the course price in UAH.</summary>
    public decimal Price { get; }

    /// <summary>Gets the identifiers of courses that must be completed first.</summary>
    public IReadOnlySet<int> PrerequisiteCourseIds => _prerequisiteCourseIds;

    private static void ValidatePrerequisites(int courseId, IReadOnlySet<int> prerequisiteIds)
    {
        if (prerequisiteIds.Contains(courseId))
        {
            throw new ArgumentException("Курс не може бути власною передумовою.", nameof(prerequisiteIds));
        }

        if (prerequisiteIds.Any(id => id <= 0))
        {
            throw new ArgumentException("Ідентифікатори передумов мають бути додатними.", nameof(prerequisiteIds));
        }
    }
}
