namespace OnlineCourses.Core.Services;

using OnlineCourses.Core.Abstractions;
using OnlineCourses.Core.Domain;

/// <summary>Stores course catalog and enrollment data in process memory.</summary>
public sealed class InMemoryEnrollmentRepository : IEnrollmentRepository
{
    private readonly Dictionary<int, Member> _members = new();
    private readonly Dictionary<int, Course> _courses = new();
    private readonly Dictionary<int, CourseGroup> _groups = new();
    private readonly Dictionary<int, Enrollment> _enrollments = new();

    /// <summary>Adds a member to the in-memory catalog.</summary>
    /// <param name="member">Member to store.</param>
    public void AddMember(Member member)
    {
        ArgumentNullException.ThrowIfNull(member);
        _members.Add(member.Id, member);
    }

    /// <summary>Adds a course to the in-memory catalog.</summary>
    /// <param name="course">Course to store.</param>
    public void AddCourse(Course course)
    {
        ArgumentNullException.ThrowIfNull(course);
        _courses.Add(course.Id, course);
    }

    /// <summary>Adds a group whose course is already registered.</summary>
    /// <param name="group">Group to store.</param>
    public void AddGroup(CourseGroup group)
    {
        ArgumentNullException.ThrowIfNull(group);

        if (!_courses.ContainsKey(group.CourseId))
        {
            throw new InvalidOperationException("Неможливо додати групу для невідомого курсу.");
        }

        _groups.Add(group.Id, group);
    }

    /// <summary>Finds a member by identifier.</summary>
    /// <param name="memberId">Member identifier.</param>
    /// <returns>The member, or null when it is absent.</returns>
    public Member? GetMember(int memberId) =>
        _members.TryGetValue(memberId, out var member) ? member : null;

    /// <summary>Finds a course by identifier.</summary>
    /// <param name="courseId">Course identifier.</param>
    /// <returns>The course, or null when it is absent.</returns>
    public Course? GetCourse(int courseId) =>
        _courses.TryGetValue(courseId, out var course) ? course : null;

    /// <summary>Finds a group by identifier.</summary>
    /// <param name="groupId">Group identifier.</param>
    /// <returns>The group, or null when it is absent.</returns>
    public CourseGroup? GetGroup(int groupId) =>
        _groups.TryGetValue(groupId, out var group) ? group : null;

    /// <summary>Returns all stored enrollments.</summary>
    /// <returns>A snapshot of the current enrollments.</returns>
    public IReadOnlyList<Enrollment> GetAll() => _enrollments.Values.ToList();

    /// <summary>Returns all registered course groups.</summary>
    /// <returns>A snapshot of the course groups.</returns>
    public IReadOnlyList<CourseGroup> GetGroups() => _groups.Values.ToList();

    /// <summary>Returns members for persistence serialization.</summary>
    /// <returns>A snapshot of the member catalog.</returns>
    public IReadOnlyList<Member> GetMembersForPersistence() => _members.Values.ToList();

    /// <summary>Returns courses for persistence serialization.</summary>
    /// <returns>A snapshot of the course catalog.</returns>
    public IReadOnlyList<Course> GetCoursesForPersistence() => _courses.Values.ToList();

    /// <summary>Finds an enrollment by identifier.</summary>
    /// <param name="enrollmentId">Enrollment identifier.</param>
    /// <returns>The enrollment, or null when it is absent.</returns>
    public Enrollment? GetById(int enrollmentId) =>
        _enrollments.TryGetValue(enrollmentId, out var enrollment) ? enrollment : null;

    /// <summary>Adds a new enrollment to the repository.</summary>
    /// <param name="enrollment">Enrollment to store.</param>
    public void Add(Enrollment enrollment)
    {
        ArgumentNullException.ThrowIfNull(enrollment);
        _enrollments.Add(enrollment.Id, enrollment);
    }

    /// <summary>Replaces the stored enrollment with its updated state.</summary>
    /// <param name="enrollment">Updated enrollment.</param>
    public void Save(Enrollment enrollment)
    {
        ArgumentNullException.ThrowIfNull(enrollment);

        if (!_enrollments.ContainsKey(enrollment.Id))
        {
            throw new InvalidOperationException("Зарахування не знайдено.");
        }

        _enrollments[enrollment.Id] = enrollment;
    }

    /// <summary>Checks whether a member has completed the specified course.</summary>
    /// <param name="memberId">Member identifier.</param>
    /// <param name="courseId">Course identifier.</param>
    /// <returns>True when a completed enrollment exists.</returns>
    public bool HasCompletedCourse(int memberId, int courseId) =>
        _enrollments.Values.Any(x =>
            x.MemberId == memberId &&
            x.CourseId == courseId &&
            x.Status == EnrollmentStatus.Completed);
}
