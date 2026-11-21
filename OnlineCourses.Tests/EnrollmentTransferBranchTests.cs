namespace OnlineCourses.Tests;

using OnlineCourses.Core.Abstractions;
using OnlineCourses.Core.Domain;
using OnlineCourses.Core.Services;

public sealed class EnrollmentTransferBranchTests
{
    [Fact]
    public void Transfer_ToAvailableGroup_SavesEnrollmentAndWritesInformationLog()
    {
        var repository = CreateRepository();
        repository.AddGroup(new CourseGroup(12, 1, "Ранкова", 4));
        repository.Add(new Enrollment(1, 10, 1, 11, new DateOnly(2026, 11, 1)));
        var logger = new RecordingAuditLogger();
        var service = new EnrollmentService(repository, logger: logger);

        service.Transfer(1, 12);

        Assert.Equal(12, repository.GetById(1)!.GroupId);
        Assert.Contains(logger.Entries, entry =>
            entry.Level == AuditLevel.Information &&
            entry.Scenario == "UC-03");
    }

    [Fact]
    public void Transfer_ToSameGroup_IsRejectedAndLogged()
    {
        var repository = CreateRepository();
        repository.Add(new Enrollment(1, 10, 1, 11, new DateOnly(2026, 11, 1)));
        var logger = new RecordingAuditLogger();
        var service = new EnrollmentService(repository, logger: logger);

        var exception = Assert.Throws<OnlineCourses.Core.Errors.DomainRuleException>(
            () => service.Transfer(1, 11));

        Assert.Equal("TRANSFER_SAME_GROUP", exception.Rule);
        Assert.Contains(logger.Entries, entry =>
            entry.Level == AuditLevel.Warning &&
            entry.Scenario == "TRANSFER_SAME_GROUP");
    }

    [Fact]
    public void Transfer_ToDifferentCourse_IsRejected()
    {
        var repository = CreateRepository();
        repository.AddCourse(new Course(2, "CS-02", "Інший курс", 200m));
        repository.AddGroup(new CourseGroup(20, 2, "Інша група", 4));
        repository.Add(new Enrollment(1, 10, 1, 11, new DateOnly(2026, 11, 1)));
        var service = new EnrollmentService(repository);

        var exception = Assert.Throws<OnlineCourses.Core.Errors.DomainRuleException>(
            () => service.Transfer(1, 20));

        Assert.Equal("TRANSFER_DIFFERENT_COURSE", exception.Rule);
        Assert.Equal(11, repository.GetById(1)!.GroupId);
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
