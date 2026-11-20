namespace OnlineCourses.Tests;

using OnlineCourses.Core.Abstractions;
using OnlineCourses.Core.Domain;
using OnlineCourses.Core.Errors;
using OnlineCourses.Core.Services;

public sealed class EnrollmentCompletionServiceTests
{
    [Fact]
    public void Complete_ChangesStatusAndReleasesOccupiedSeat()
    {
        var repository = CreateRepository();
        repository.Add(new Enrollment(1, 10, 1, 11, new DateOnly(2026, 11, 1)));
        var service = new EnrollmentService(repository);

        service.Complete(1);

        Assert.Equal(EnrollmentStatus.Completed, repository.GetById(1)!.Status);
        var occupancy = Assert.Single(service.GetOccupancyReport());
        Assert.Equal(0, occupancy.Occupied);
        Assert.Equal(3, occupancy.Free);
    }

    [Fact]
    public void Complete_RejectsMissingEnrollmentAndLogsWarning()
    {
        var logger = new RecordingAuditLogger();
        var service = new EnrollmentService(CreateRepository(), logger: logger);

        var exception = Assert.Throws<DomainRuleException>(() => service.Complete(999));

        Assert.Equal("ENROLLMENT_NOT_FOUND", exception.Rule);
        Assert.Contains(logger.Entries, entry =>
            entry.Level == AuditLevel.Warning &&
            entry.Scenario == "ENROLLMENT_NOT_FOUND");
    }

    [Fact]
    public void Complete_RejectsAlreadyCompletedEnrollment()
    {
        var repository = CreateRepository();
        var enrollment = new Enrollment(1, 10, 1, 11, new DateOnly(2026, 11, 1));
        enrollment.Complete();
        repository.Add(enrollment);
        var service = new EnrollmentService(repository);

        var exception = Assert.Throws<DomainRuleException>(() => service.Complete(1));

        Assert.Equal("ENROLLMENT_NOT_ACTIVE", exception.Rule);
        Assert.Equal(EnrollmentStatus.Completed, repository.GetById(1)!.Status);
    }

    private static InMemoryEnrollmentRepository CreateRepository()
    {
        var repository = new InMemoryEnrollmentRepository();
        repository.AddMember(new Member(10, "Олена", "olena@example.com"));
        repository.AddCourse(new Course(1, "CS-01", "Основи C#", 100m));
        repository.AddGroup(new CourseGroup(11, 1, "Вечірня", 3));
        return repository;
    }
}
