namespace OnlineCourses.Core.Abstractions;

using OnlineCourses.Core.Domain;

/// <summary>Defines the contract for IEnrollmentRepository.</summary>
public interface IEnrollmentRepository
{
    Member? GetMember(int memberId);

    Course? GetCourse(int courseId);

    CourseGroup? GetGroup(int groupId);

    IReadOnlyList<Enrollment> GetAll();

    IReadOnlyList<CourseGroup> GetGroups();

    Enrollment? GetById(int enrollmentId);

    void Add(Enrollment enrollment);

    void Save(Enrollment enrollment);

    bool HasCompletedCourse(int memberId, int courseId);
}
