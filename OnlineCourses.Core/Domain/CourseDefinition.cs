namespace OnlineCourses.Core.Domain;

/// <summary>Містить змінні характеристики курсу для його створення.</summary>
/// <param name="Code">Короткий унікальний код курсу.</param>
/// <param name="Title">Назва курсу.</param>
/// <param name="Price">Невід'ємна ціна курсу.</param>
/// <param name="PrerequisiteCourseIds">Ідентифікатори курсів-передумов.</param>
public sealed record CourseDefinition(
    string Code,
    string Title,
    decimal Price,
    IEnumerable<int>? PrerequisiteCourseIds = null);
