namespace OnlineCourses.Core.Domain;

/// <summary>Представляє навчальну групу конкретного курсу з обмеженою місткістю.</summary>
public sealed class CourseGroup
{
    /// <summary>Створює групу та перевіряє її курс, назву й місткість.</summary>
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

    /// <summary>Повертає ідентифікатор групи.</summary>
    public int Id { get; }

    /// <summary>Повертає ідентифікатор курсу, до якого належить група.</summary>
    public int CourseId { get; }

    /// <summary>Повертає назву групи.</summary>
    public string Name { get; }

    /// <summary>Повертає максимальну кількість слухачів у групі.</summary>
    public int Capacity { get; }
}
