namespace OnlineCourses.Tests;

using System.Text;
using OnlineCourses.App;
using OnlineCourses.Core.Abstractions;
using OnlineCourses.Core.Domain;
using OnlineCourses.Core.Pricing;
using OnlineCourses.Core.Services;

public sealed class ConsoleApplicationTests
{

    [Fact]
    public void Run_ExitsWhenInputStreamEnds()
    {
        var console = new TestConsole();
        var application = CreateApplication(CreateRepository(), console);

        application.Run();

        Assert.Contains("OnlineCourses", console.Output);
    }

    [Fact]
    public void Run_EnrollsMemberUsingConsoleInput()
    {
        var repository = CreateRepository();
        var console = new TestConsole("1", "10", "11", "0");
        var application = CreateApplication(repository, console);

        application.Run();

        var enrollment = Assert.Single(repository.GetAll());
        Assert.Equal(10, enrollment.MemberId);
        Assert.Equal(11, enrollment.GroupId);
        Assert.Contains("Зарахування №1 створено.", console.Output);
    }

    [Fact]
    public void Run_ReportsInvalidIntegerInput()
    {
        var repository = CreateRepository();
        var console = new TestConsole("1", "not-a-number", "10", "11", "0");
        var application = CreateApplication(repository, console);

        application.Run();

        var enrollment = Assert.Single(repository.GetAll());
        Assert.Equal(10, enrollment.MemberId);
        Assert.Contains("Потрібно ввести ціле число.", console.Output);
    }


    [Fact]
    public void Run_SearchesUsingMultipleCriteria()
    {
        var repository = CreateRepository();
        repository.Add(new Enrollment(1, 10, 1, 11, new DateOnly(2026, 11, 20)));
        var console = new TestConsole("6", "olena", "CS-01", "11", "Active", "0");
        var application = CreateApplication(repository, console);

        application.Run();

        Assert.Contains("#1 | Олена | CS-01 | Вечірня | Active", console.Output);
    }

    [Fact]
    public void Run_ShowsDomainRejectionWithoutLoggingItAsUnexpectedError()
    {
        var repository = CreateRepository();
        var logger = new RecordingAuditLogger();
        var console = new TestConsole("2", "999", "Заява", "0");
        var application = CreateApplication(repository, console, logger);

        application.Run();

        Assert.Contains("Операцію відхилено:", console.Output);
        Assert.DoesNotContain(logger.Entries, entry => entry.Level == AuditLevel.Error);
        Assert.Contains(logger.Entries, entry => entry.Level == AuditLevel.Warning);
    }

    private static ConsoleApplication CreateApplication(
        InMemoryEnrollmentRepository repository,
        TestConsole console,
        RecordingAuditLogger? logger = null)
    {
        var auditLogger = logger ?? new RecordingAuditLogger();
        return new ConsoleApplication(
            repository,
            new EnrollmentService(repository, logger: auditLogger),
            new CoursePackagePricingPolicy(),
            auditLogger,
            console);
    }

    private static InMemoryEnrollmentRepository CreateRepository()
    {
        var repository = new InMemoryEnrollmentRepository();
        repository.AddMember(new Member(10, "Олена", "olena@example.com"));
        repository.AddCourse(new Course(1, "CS-01", "Основи C#", 100m));
        repository.AddGroup(new CourseGroup(11, 1, "Вечірня", 4));
        return repository;
    }

    private sealed class TestConsole(params string[] input) : IConsole
    {
        private readonly Queue<string> _input = new(input);
        private readonly StringBuilder _output = new();

        public string Output => _output.ToString();

        public string? ReadLine() => _input.Count > 0 ? _input.Dequeue() : null;

        public void Write(string value) => _output.Append(value);

        public void WriteLine(string? value = null) => _output.AppendLine(value);
    }
}
