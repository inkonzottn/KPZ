namespace OnlineCourses.Tests;

using OnlineCourses.Core.Abstractions;
using OnlineCourses.Core.Domain;
using OnlineCourses.Core.Errors;
using OnlineCourses.Core.Services;

public sealed class EnrollmentServiceBranchCoverageTests
{
    [Fact]
    public void Enroll_WithoutPrerequisites_CreatesEnrollment()
    {
        var repository = CreateRepository();
        var logger = new RecordingAuditLogger();
        var service = new EnrollmentService(repository, logger: logger);

        var enrollment = service.Enroll(1, 10, 11, new DateOnly(2026, 11, 20));

        Assert.Equal(EnrollmentStatus.Active, enrollment.Status);
        Assert.Contains(logger.Entries, entry =>
            entry.Level == AuditLevel.Information && entry.Message.Contains("успішно"));
    }

    [Fact]
    public void Enroll_RejectsDuplicateActiveEnrollment()
    {
        var repository = CreateRepository();
        repository.Add(new Enrollment(1, 10, 1, 11, new DateOnly(2026, 11, 1)));
        var logger = new RecordingAuditLogger();
        var service = new EnrollmentService(repository, logger: logger);

        var exception = Assert.Throws<DomainRuleException>(
            () => service.Enroll(2, 10, 11, new DateOnly(2026, 11, 20)));

        Assert.Equal("DUPLICATE_ENROLLMENT", exception.Rule);
        Assert.Contains(logger.Entries, entry =>
            entry.Level == AuditLevel.Warning &&
            entry.Scenario == "DUPLICATE_ENROLLMENT");
    }

    [Fact]
    public void Enroll_RejectsUnknownMember()
    {
        var service = new EnrollmentService(CreateRepository());

        var exception = Assert.Throws<DomainRuleException>(
            () => service.Enroll(1, 999, 11, new DateOnly(2026, 11, 20)));

        Assert.Equal("MEMBER_NOT_FOUND", exception.Rule);
    }

    [Fact]
    public void Enroll_RejectsUnknownGroup()
    {
        var service = new EnrollmentService(CreateRepository());

        var exception = Assert.Throws<DomainRuleException>(
            () => service.Enroll(1, 10, 999, new DateOnly(2026, 11, 20)));

        Assert.Equal("GROUP_NOT_FOUND", exception.Rule);
    }

    [Fact]
    public void Search_FiltersByCourseCodeAndGroup()
    {
        var repository = CreateRepository();
        repository.Add(new Enrollment(1, 10, 1, 11, new DateOnly(2026, 11, 1)));
        var service = new EnrollmentService(repository);

        var result = service.Search(new EnrollmentSearchCriteria(
            CourseCode: "cs-01",
            GroupId: 11));

        var enrollment = Assert.Single(result);
        Assert.Equal(1, enrollment.Id);
    }

    [Fact]
    public void Search_FiltersByStatusOnly()
    {
        var repository = CreateRepository();
        repository.Add(new Enrollment(1, 10, 1, 11, new DateOnly(2026, 11, 1)));
        var service = new EnrollmentService(repository);

        var result = service.Search(new EnrollmentSearchCriteria(Status: EnrollmentStatus.Active));

        Assert.Single(result);
    }

    [Fact]
    public void OccupancyReport_FiltersByCourseId()
    {
        var repository = CreateRepository();
        repository.AddCourse(new Course(2, "CS-02", "Інший курс", 200m));
        repository.AddGroup(new CourseGroup(20, 2, "Інша група", 5));
        var service = new EnrollmentService(repository);

        var result = service.GetOccupancyReport(2);

        var row = Assert.Single(result);
        Assert.Equal("CS-02", row.CourseCode);
    }

    [Fact]
    public void OccupancyReport_ReturnsEmptyWhenCourseHasNoGroups()
    {
        var repository = CreateRepository();
        repository.AddCourse(new Course(2, "CS-02", "Інший курс", 200m));
        var service = new EnrollmentService(repository);

        Assert.Empty(service.GetOccupancyReport(2));
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
