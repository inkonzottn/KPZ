namespace OnlineCourses.Tests;

using OnlineCourses.Core.Abstractions;
using OnlineCourses.Core.Domain;
using OnlineCourses.Core.Errors;
using OnlineCourses.Core.Services;

public sealed class DomainAndRepositoryBoundaryTests
{
    [Fact]
    public void EnrollmentService_RejectsNullRepository()
    {
        Assert.Throws<ArgumentNullException>(() => new EnrollmentService(null!));
    }

    [Fact]
    public void Course_RejectsItselfAsPrerequisite()
    {
        var exception = Assert.Throws<ArgumentException>(
            () => new Course(1, new CourseDefinition("CS-01", "Основи C#", 100m, [1])));

        Assert.Equal("prerequisiteCourseIds", exception.ParamName);
    }


    [Fact]
    public void Course_RejectsNonPositivePrerequisiteId()
    {
        var exception = Assert.Throws<ArgumentException>(
            () => new Course(2, new CourseDefinition("CS-02", "Алгоритми", 200m, [0])));

        Assert.Equal("prerequisiteCourseIds", exception.ParamName);
    }

    [Fact]
    public void Repository_RejectsGroupForUnknownCourse()
    {
        var repository = new InMemoryEnrollmentRepository();

        Assert.Throws<InvalidOperationException>(
            () => repository.AddGroup(new CourseGroup(11, 999, "Невідома група", 4)));
    }

    [Fact]
    public void Repository_RejectsSavingUnknownEnrollment()
    {
        var repository = new InMemoryEnrollmentRepository();
        var enrollment = new Enrollment(1, 10, 1, 11, new DateOnly(2026, 11, 20));

        Assert.Throws<InvalidOperationException>(() => repository.Save(enrollment));
    }

    [Fact]
    public void Complete_WritesInformationLogOnSuccess()
    {
        var repository = CreateRepository();
        repository.Add(new Enrollment(1, 10, 1, 11, new DateOnly(2026, 11, 1)));
        var logger = new RecordingAuditLogger();
        var service = new EnrollmentService(repository, logger: logger);

        service.Complete(1);

        Assert.Contains(logger.Entries, entry =>
            entry.Level == AuditLevel.Information && entry.Scenario == "UC-07");
    }

    [Fact]
    public void Transfer_SucceedsWithoutOptionalLogger()
    {
        var repository = CreateRepository();
        repository.AddGroup(new CourseGroup(12, 1, "Ранкова", 4));
        repository.Add(new Enrollment(1, 10, 1, 11, new DateOnly(2026, 11, 1)));
        var service = new EnrollmentService(repository);

        service.Transfer(1, 12);

        Assert.Equal(12, repository.GetById(1)!.GroupId);
    }


    [Fact]
    public void Enroll_RejectsGroupWhoseCourseIsMissingFromRepository()
    {
        var service = new EnrollmentService(new MissingCourseRepository());

        var exception = Assert.Throws<DomainRuleException>(
            () => service.Enroll(1, 10, 11, new DateOnly(2026, 11, 20)));

        Assert.Equal("COURSE_NOT_FOUND", exception.Rule);
    }

    [Fact]
    public void OccupancyReport_RejectsGroupWhoseCourseIsMissingFromRepository()
    {
        var service = new EnrollmentService(new MissingCourseRepository());

        var exception = Assert.Throws<DomainRuleException>(() => service.GetOccupancyReport());

        Assert.Equal("COURSE_NOT_FOUND", exception.Rule);
    }

    private sealed class MissingCourseRepository : IEnrollmentRepository
    {
        private static readonly Member ExistingMember =
            new(10, "Олена", "olena@example.com");

        private static readonly CourseGroup OrphanGroup =
            new(11, 999, "Група без курсу", 4);

        public Member? GetMember(int memberId) => memberId == ExistingMember.Id
            ? ExistingMember
            : null;

        public Course? GetCourse(int courseId) => null;

        public CourseGroup? GetGroup(int groupId) => groupId == OrphanGroup.Id
            ? OrphanGroup
            : null;

        public IReadOnlyList<Enrollment> GetAll() => [];

        public IReadOnlyList<CourseGroup> GetGroups() => [OrphanGroup];

        public Enrollment? GetById(int enrollmentId) => null;

        public void Add(Enrollment enrollment) =>
            throw new NotSupportedException();

        public void Save(Enrollment enrollment) =>
            throw new NotSupportedException();

        public bool HasCompletedCourse(int memberId, int courseId) => false;
    }

    private static InMemoryEnrollmentRepository CreateRepository()
    {
        var repository = new InMemoryEnrollmentRepository();
        repository.AddMember(new Member(10, "Олена", "olena@example.com"));
        repository.AddCourse(new Course(1, "CS-01", "Основи C#", 100m));
        repository.AddGroup(new CourseGroup(11, 1, "Вечірня", 4));
        return repository;
    }
}
