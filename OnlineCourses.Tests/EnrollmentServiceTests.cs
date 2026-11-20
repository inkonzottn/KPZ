namespace OnlineCourses.Tests;

using OnlineCourses.Core.Abstractions;
using OnlineCourses.Core.Domain;
using OnlineCourses.Core.Errors;
using OnlineCourses.Core.Pricing;
using OnlineCourses.Core.Services;

public sealed class EnrollmentServiceTests
{
    [Fact]
    public void Enroll_AddsActiveEnrollment_WhenGroupHasSpaceAndPrerequisitesAreCompleted()
    {
        var repository = CreateRepository();
        repository.AddCourse(new Course(2, "CS-02", "Advanced C#", 200m, [1]));
        repository.AddGroup(new CourseGroup(20, 2, "Advanced evening", 2));
        repository.AddGroup(new CourseGroup(10, 1, "C# prerequisite", 3));
        repository.Add(new Enrollment(1, 10, 1, 10, new DateOnly(2026, 1, 1)));
        repository.GetById(1)!.Complete();

        var service = new EnrollmentService(repository);
        var enrollment = service.Enroll(2, 10, 20, new DateOnly(2026, 2, 1));

        Assert.Equal(EnrollmentStatus.Active, enrollment.Status);
        Assert.Equal(2, repository.GetAll().Count);
    }

    [Fact]
    public void Enroll_RejectsMissingPrerequisite()
    {
        var repository = CreateRepository();
        repository.AddCourse(new Course(2, "CS-02", "Advanced C#", 200m, [1]));
        repository.AddGroup(new CourseGroup(20, 2, "Advanced evening", 2));

        var service = new EnrollmentService(repository);

        Assert.Throws<InvalidOperationException>(() =>
            service.Enroll(2, 10, 20, new DateOnly(2026, 2, 1)));
        Assert.Empty(repository.GetAll());
    }

    [Fact]
    public void Enroll_RejectsFullGroup()
    {
        var repository = CreateRepository();
        repository.AddGroup(new CourseGroup(13, 1, "C# full", 1));
        repository.Add(new Enrollment(1, 10, 1, 13, new DateOnly(2026, 1, 1)));

        var service = new EnrollmentService(repository);

        Assert.Throws<InvalidOperationException>(() =>
            service.Enroll(2, 11, 13, new DateOnly(2026, 2, 1)));
    }

    [Fact]
    public void Withdraw_ReleasesSeat()
    {
        var repository = CreateRepository();
        repository.AddGroup(new CourseGroup(13, 1, "C# limited", 1));
        repository.Add(new Enrollment(1, 10, 1, 13, new DateOnly(2026, 1, 1)));
        var service = new EnrollmentService(repository);

        service.Withdraw(1, "Заява слухача");
        var next = service.Enroll(2, 11, 13, new DateOnly(2026, 2, 1));

        Assert.Equal(EnrollmentStatus.Withdrawn, repository.GetById(1)!.Status);
        Assert.Equal(EnrollmentStatus.Active, next.Status);
    }

    [Fact]
    public void Transfer_RejectsFullTargetGroup_WithoutChangingOriginalGroup()
    {
        var repository = CreateRepository();
        repository.AddGroup(new CourseGroup(12, 1, "C# morning", 1));
        repository.Add(new Enrollment(1, 10, 1, 11, new DateOnly(2026, 1, 1)));
        repository.Add(new Enrollment(2, 11, 1, 12, new DateOnly(2026, 1, 1)));
        var service = new EnrollmentService(repository);

        Assert.Throws<InvalidOperationException>(() => service.Transfer(1, 12));
        Assert.Equal(11, repository.GetById(1)!.GroupId);
    }

    [Fact]
    public void OccupancyReport_IncludesEmptyGroupsAndCalculatesPercent()
    {
        var repository = CreateRepository();
        repository.AddGroup(new CourseGroup(12, 1, "C# morning", 4));
        repository.Add(new Enrollment(1, 10, 1, 11, new DateOnly(2026, 1, 1)));
        var service = new EnrollmentService(repository);

        var report = service.GetOccupancyReport();

        Assert.Equal(2, report.Count);
        Assert.Equal(33.33m, report.Single(x => x.GroupName == "C# evening").OccupancyPercent);
        Assert.Equal(0, report.Single(x => x.GroupName == "C# morning").Occupied);
        Assert.Equal(4, report.Single(x => x.GroupName == "C# morning").Free);
    }

    [Fact]
    public void Search_FiltersByMemberAndStatus()
    {
        var repository = CreateRepository();
        repository.Add(new Enrollment(1, 10, 1, 11, new DateOnly(2026, 1, 1)));
        repository.Add(new Enrollment(2, 11, 1, 11, new DateOnly(2026, 1, 2)));
        repository.GetById(2)!.Withdraw("Заява");
        var service = new EnrollmentService(repository);

        var result = service.Search(new EnrollmentSearchCriteria(
            MemberQuery: "olena",
            Status: EnrollmentStatus.Active));

        var enrollment = Assert.Single(result);
        Assert.Equal(1, enrollment.Id);
    }

    [Fact]
    public void Search_RejectsEmptyCriteria()
    {
        var service = new EnrollmentService(CreateRepository());

        Assert.Throws<ArgumentException>(() =>
            service.Search(new EnrollmentSearchCriteria()));
    }

    [Fact]
    public void Enroll_UsesFakeRepositoryAndFixedClock()
    {
        var repository = new FakeEnrollmentRepository();
        repository.AddMember(new Member(1, "Test", "test@example.com"));
        repository.AddCourse(new Course(1, "CS-1", "Basics", 100m));
        repository.AddGroup(new CourseGroup(1, 1, "Group A", 2));
        var clock = new FixedClock(
            new DateTimeOffset(2026, 11, 20, 9, 30, 0, TimeSpan.FromHours(2)));
        var service = new EnrollmentService(repository, clock);

        var result = service.Enroll(1, 1, 1);

        Assert.Equal(new DateOnly(2026, 11, 20), result.EnrolledOn);
        Assert.Equal(EnrollmentStatus.Active, result.Status);
    }

    [Fact]
    public void Enroll_UsesInjectedClockDate()
    {
        var repository = CreateRepository();
        var clock = new FixedClock(
            new DateTimeOffset(2026, 11, 20, 9, 30, 0, TimeSpan.FromHours(2)));
        var service = new EnrollmentService(repository, clock);

        var enrollment = service.Enroll(1, 10, 11);

        Assert.Equal(new DateOnly(2026, 11, 20), enrollment.EnrolledOn);
    }

    [Fact]
    public void Enroll_LogsWarning_WhenGroupIsFull()
    {
        var repository = CreateRepository();
        repository.Add(new Enrollment(1, 10, 1, 11, new DateOnly(2026, 1, 1)));
        repository.Add(new Enrollment(2, 11, 1, 11, new DateOnly(2026, 1, 1)));
        repository.Add(new Enrollment(3, 12, 1, 11, new DateOnly(2026, 1, 1)));
        repository.AddMember(new Member(13, "Марія", "maria@example.com"));
        var logger = new RecordingAuditLogger();
        var service = new EnrollmentService(repository, logger: logger);

        Assert.Throws<DomainRuleException>(() => service.Enroll(4, 13, 11));

        Assert.Contains(logger.Entries, x => x.Level == AuditLevel.Warning);
    }

    private static InMemoryEnrollmentRepository CreateRepository()
    {
        var repository = new InMemoryEnrollmentRepository();
        repository.AddMember(new Member(10, "Олена", "olena@example.com"));
        repository.AddMember(new Member(11, "Іван", "ivan@example.com"));
        repository.AddCourse(new Course(1, "CS-01", "Основи C#", 100m));
        repository.AddGroup(new CourseGroup(11, 1, "C# evening", 3));
        return repository;
    }
}

public sealed class CoursePackagePricingPolicyTests
{
    [Theory]
    [InlineData(1, 0)]
    [InlineData(2, 5)]
    [InlineData(3, 10)]
    [InlineData(4, 10)]
    public void Calculate_UsesDiscountRateByNumberOfCourses(int count, int expectedPercent)
    {
        var courses = Enumerable.Range(1, count)
            .Select(i => new Course(i, $"C-{i}", $"Course {i}", 100m));
        var policy = new CoursePackagePricingPolicy();

        var result = policy.Calculate(courses);

        Assert.Equal(expectedPercent / 100m, result.DiscountRate);
        Assert.Equal(100m * count, result.BaseTotal);
        Assert.Equal(result.BaseTotal - result.Discount, result.Payable);
    }

    [Fact]
    public void Calculate_RejectsEmptyPackage()
    {
        var policy = new CoursePackagePricingPolicy();

        Assert.Throws<ArgumentException>(() => policy.Calculate([]));
    }

    [Fact]
    public void Calculate_RejectsDuplicateCourses()
    {
        var course = new Course(1, "C-1", "Course 1", 100m);
        var policy = new CoursePackagePricingPolicy();

        Assert.Throws<ArgumentException>(() => policy.Calculate([course, course]));
    }
}
