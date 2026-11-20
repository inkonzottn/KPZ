namespace OnlineCourses.Tests;

using OnlineCourses.Core.Abstractions;
using OnlineCourses.Core.Domain;
using OnlineCourses.Core.Pricing;
using OnlineCourses.Core.Services;

public sealed class ConsoleMenuTests
{
    [Fact]
    public void Run_EnrollsLearner_WhenInputIsValid()
    {
        var repository = CreateRepository();
        var console = new FakeConsole(["1", "10", "11", "0"]);
        var logger = new RecordingAuditLogger();
        var menu = CreateMenu(repository, console, logger);

        menu.Run();

        var enrollment = Assert.Single(repository.GetAll());
        Assert.Equal(10, enrollment.MemberId);
        Assert.Equal(11, enrollment.GroupId);
        Assert.Contains("створено", console.Output.ToString());
        Assert.Contains(logger.Entries, entry =>
            entry.Level == AuditLevel.Information && entry.Scenario == "UC-01");
    }

    [Fact]
    public void Run_RepeatsIntegerPrompt_WhenInputIsInvalid()
    {
        var repository = CreateRepository();
        var console = new FakeConsole(["1", "not-an-id", "10", "11", "0"]);
        var menu = CreateMenu(repository, console, new RecordingAuditLogger());

        menu.Run();

        Assert.Single(repository.GetAll());
        Assert.Contains("Некоректне значення", console.Output.ToString());
    }

    [Fact]
    public void Run_ReportsDomainRejection_AndKeepsMenuAvailable()
    {
        var repository = CreateRepository();
        repository.Add(new Enrollment(1, 11, 1, 11, new DateOnly(2026, 1, 1)));
        var console = new FakeConsole(["1", "10", "11", "0"]);
        var logger = new RecordingAuditLogger();
        var menu = CreateMenu(repository, console, logger);

        menu.Run();

        Assert.Single(repository.GetAll());
        Assert.Contains("Операцію відхилено", console.Output.ToString());
        Assert.Contains(logger.Entries, entry => entry.Level == AuditLevel.Warning);
        Assert.DoesNotContain(logger.Entries, entry => entry.Level == AuditLevel.Error);
    }

    private static FakeEnrollmentRepository CreateRepository()
    {
        var repository = new FakeEnrollmentRepository();
        repository.AddMember(new Member(10, "Олена", "olena@example.com"));
        repository.AddMember(new Member(11, "Іван", "ivan@example.com"));
        repository.AddCourse(new Course(1, "CS-01", "Основи C#", 100m));
        repository.AddGroup(new CourseGroup(11, 1, "Вечірня", 1));
        return repository;
    }

    private static ConsoleMenu CreateMenu(
        FakeEnrollmentRepository repository,
        FakeConsole console,
        RecordingAuditLogger logger)
    {
        var clock = new FixedClock(
            new DateTimeOffset(2026, 11, 20, 9, 30, 0, TimeSpan.FromHours(2)));
        var service = new EnrollmentService(repository, clock, logger);
        return new ConsoleMenu(new ConsoleMenuDependencies
        {
            Console = console,
            Repository = repository,
            Service = service,
            Pricing = new CoursePackagePricingPolicy(),
            Logger = logger
        });
    }
}
