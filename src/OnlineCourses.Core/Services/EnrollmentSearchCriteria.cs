namespace OnlineCourses.Core.Services;

using OnlineCourses.Core.Domain;

/// <summary>Задає необов'язкові фільтри пошуку зарахувань.</summary>
/// <param name="MemberQuery">Фрагмент імені або email слухача.</param>
/// <param name="CourseCode">Фрагмент коду курсу.</param>
/// <param name="GroupId">Ідентифікатор групи для точного відбору.</param>
/// <param name="Status">Стан зарахування для точного відбору.</param>
public sealed record EnrollmentSearchCriteria(
    string? MemberQuery = null,
    string? CourseCode = null,
    int? GroupId = null,
    EnrollmentStatus? Status = null);
