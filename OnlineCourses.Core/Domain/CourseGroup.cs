namespace OnlineCourses.Core.Domain;

/// <summary>Представляє навчальну групу конкретного курсу з обмеженою місткістю.</summary>
public sealed class CourseGroup
{
    /// <summary>Initializes a new instance of the <see cref="CourseGroup"/> class.</summary>
    /// <param name="id">Додатний ідентифікатор групи.</param>
    /// <param name="courseId">Додатний ідентифікатор курсу.</param>
    /// <param name="name">Непорожня назва групи.</param>
    /// <param name="capacity">Додатна максимальна кількість слухачів.</param>
    /// <exception cref="ArgumentException">Назва групи порожня.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Один з ідентифікаторів або місткість недопустимі.</exception>
    public CourseGroup(int id, int courseId, string name, int capacity)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(id);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(courseId);
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(capacity);

        Id = id;
        CourseId = courseId;
        Name = name.Trim();
        Capacity = capacity;
    }

    /// <summary>Gets the group identifier.</summary>
    public int Id { get; }

    /// <summary>Gets the identifier of the course assigned to this group.</summary>
    public int CourseId { get; }

    /// <summary>Gets the group name.</summary>
    public string Name { get; }

    /// <summary>Gets the maximum number of learners in the group.</summary>
    public int Capacity { get; }
}
