namespace OnlineCourses.Core.Domain;

/// <summary>Описує навчальний курс, його ціну та обов'язкові передумови.</summary>
public sealed class Course
{
    private readonly HashSet<int> _prerequisiteCourseIds;

    /// <summary>Initializes a new instance of the <see cref="Course"/> class.</summary>
    /// <param name="id">Додатний унікальний ідентифікатор курсу.</param>
    /// <param name="code">Непорожній код курсу.</param>
    /// <param name="title">Непорожня назва курсу.</param>
    /// <param name="price">Невід'ємна вартість курсу.</param>
    /// <param name="prerequisiteCourseIds">Необов'язкові ідентифікатори курсів, які треба завершити раніше.</param>
    /// <exception cref="ArgumentException">Код, назва або передумови некоректні.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Ідентифікатор або ціна поза допустимими межами.</exception>
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
}
