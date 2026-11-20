namespace OnlineCourses.Core.Services;

using OnlineCourses.Core.Domain;

/// <summary>Represents the EnrollmentSearchCriteria type.</summary>
public sealed record EnrollmentSearchCriteria(
    string? MemberQuery = null,
    string? CourseCode = null,
    int? GroupId = null,
    EnrollmentStatus? Status = null);
