namespace OnlineCourses.Core.Services;

using OnlineCourses.Core.Abstractions;
using OnlineCourses.Core.Domain;

/// <summary>Represents the InMemoryEnrollmentRepository type.</summary>
public sealed class InMemoryEnrollmentRepository : IEnrollmentRepository
{
    private readonly Dictionary<int, Member> _members = new();
    private readonly Dictionary<int, Course> _courses = new();
    private readonly Dictionary<int, CourseGroup> _groups = new();
    private readonly Dictionary<int, Enrollment> _enrollments = new();

/// <summary>Performs the AddMember operation.</summary>
    public void AddMember(Member member)
    {
        ArgumentNullException.ThrowIfNull(member);
        _members.Add(member.Id, member);
    }

/// <summary>Performs the AddCourse operation.</summary>
    public void AddCourse(Course course)
    {
        ArgumentNullException.ThrowIfNull(course);
        _courses.Add(course.Id, course);
    }

/// <summary>Performs the AddGroup operation.</summary>
    public void AddGroup(CourseGroup group)
    {
        ArgumentNullException.ThrowIfNull(group);

        if (!_courses.ContainsKey(group.CourseId))
        {
            throw new InvalidOperationException("Неможливо додати групу для невідомого курсу.");
        }

        _groups.Add(group.Id, group);
    }

/// <summary>Performs the GetMember operation.</summary>
    public Member? GetMember(int memberId) =>
        _members.TryGetValue(memberId, out var member) ? member : null;

/// <summary>Performs the GetCourse operation.</summary>
    public Course? GetCourse(int courseId) =>
        _courses.TryGetValue(courseId, out var course) ? course : null;

/// <summary>Performs the GetGroup operation.</summary>
    public CourseGroup? GetGroup(int groupId) =>
        _groups.TryGetValue(groupId, out var group) ? group : null;

/// <summary>Performs the GetAll operation.</summary>
    public IReadOnlyList<Enrollment> GetAll() => _enrollments.Values.ToList();

/// <summary>Performs the GetGroups operation.</summary>
    public IReadOnlyList<CourseGroup> GetGroups() => _groups.Values.ToList();

/// <summary>Performs the GetMembersForPersistence operation.</summary>
    public IReadOnlyList<Member> GetMembersForPersistence() => _members.Values.ToList();

/// <summary>Performs the GetCoursesForPersistence operation.</summary>
    public IReadOnlyList<Course> GetCoursesForPersistence() => _courses.Values.ToList();

/// <summary>Performs the GetById operation.</summary>
    public Enrollment? GetById(int enrollmentId) =>
        _enrollments.TryGetValue(enrollmentId, out var enrollment) ? enrollment : null;

/// <summary>Performs the Add operation.</summary>
    public void Add(Enrollment enrollment)
    {
        ArgumentNullException.ThrowIfNull(enrollment);
        _enrollments.Add(enrollment.Id, enrollment);
    }

/// <summary>Performs the Save operation.</summary>
    public void Save(Enrollment enrollment)
    {
        ArgumentNullException.ThrowIfNull(enrollment);

        if (!_enrollments.ContainsKey(enrollment.Id))
        {
            throw new InvalidOperationException("Зарахування не знайдено.");
        }

        _enrollments[enrollment.Id] = enrollment;
    }

/// <summary>Performs the HasCompletedCourse operation.</summary>
    public bool HasCompletedCourse(int memberId, int courseId) =>
        _enrollments.Values.Any(x =>
            x.MemberId == memberId &&
            x.CourseId == courseId &&
            x.Status == EnrollmentStatus.Completed);
}
