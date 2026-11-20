namespace OnlineCourses.Core.Abstractions;

using OnlineCourses.Core.Domain;

/// <summary>Defines storage operations for online course enrollments.</summary>
public interface IEnrollmentRepository
{
    /// <summary>Gets a learner by identifier.</summary>
    Member? GetMember(int memberId);

    /// <summary>Gets a course by identifier.</summary>
    Course? GetCourse(int courseId);

    /// <summary>Gets a course group by identifier.</summary>
    CourseGroup? GetGroup(int groupId);

    /// <summary>Gets all enrollments.</summary>
    IReadOnlyList<Enrollment> GetAll();

    /// <summary>Gets all course groups.</summary>
    IReadOnlyList<CourseGroup> GetGroups();

    /// <summary>Gets an enrollment by identifier.</summary>
    Enrollment? GetById(int enrollmentId);

    /// <summary>Adds a new enrollment.</summary>
    void Add(Enrollment enrollment);

    /// <summary>Persists the updated enrollment.</summary>
    void Save(Enrollment enrollment);

    /// <summary>Determines whether a learner completed a prerequisite course.</summary>
    bool HasCompletedCourse(int memberId, int courseId);
}
