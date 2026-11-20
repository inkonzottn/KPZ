namespace OnlineCourses.Core.Services;

using OnlineCourses.Core.Abstractions;
using OnlineCourses.Core.Domain;

public sealed class InMemoryEnrollmentRepository : IEnrollmentRepository
{
    private readonly Dictionary<int, Member> _members = new();
    private readonly Dictionary<int, Course> _courses = new();
    private readonly Dictionary<int, CourseGroup> _groups = new();
    private readonly Dictionary<int, Enrollment> _enrollments = new();

    public void AddMember(Member member)
    {
        ArgumentNullException.ThrowIfNull(member);
        _members.Add(member.Id, member);
    }

    public void AddCourse(Course course)
    {
        ArgumentNullException.ThrowIfNull(course);
        _courses.Add(course.Id, course);
    }

    public void AddGroup(CourseGroup group)
    {
        ArgumentNullException.ThrowIfNull(group);

        if (!_courses.ContainsKey(group.CourseId))
        {
            throw new InvalidOperationException("Неможливо додати групу для невідомого курсу.");
        }

        _groups.Add(group.Id, group);
    }

    public Member? GetMember(int memberId) =>
        _members.TryGetValue(memberId, out var member) ? member : null;

    public Course? GetCourse(int courseId) =>
        _courses.TryGetValue(courseId, out var course) ? course : null;

    public CourseGroup? GetGroup(int groupId) =>
        _groups.TryGetValue(groupId, out var group) ? group : null;

    public IReadOnlyList<Enrollment> GetAll() => _enrollments.Values.ToList();

    public IReadOnlyList<CourseGroup> GetGroups() => _groups.Values.ToList();

    public Enrollment? GetById(int enrollmentId) =>
        _enrollments.TryGetValue(enrollmentId, out var enrollment) ? enrollment : null;

    public void Add(Enrollment enrollment)
    {
        ArgumentNullException.ThrowIfNull(enrollment);
        _enrollments.Add(enrollment.Id, enrollment);
    }

    public void Save(Enrollment enrollment)
    {
        ArgumentNullException.ThrowIfNull(enrollment);

        if (!_enrollments.ContainsKey(enrollment.Id))
        {
            throw new InvalidOperationException("Зарахування не знайдено.");
        }

        _enrollments[enrollment.Id] = enrollment;
    }

    public bool HasCompletedCourse(int memberId, int courseId) =>
        _enrollments.Values.Any(x =>
            x.MemberId == memberId &&
            x.CourseId == courseId &&
            x.Status == EnrollmentStatus.Completed);
}
