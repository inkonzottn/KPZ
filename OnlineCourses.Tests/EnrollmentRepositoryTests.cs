namespace OnlineCourses.Tests;

using OnlineCourses.Core.Abstractions;
using OnlineCourses.Core.Domain;
using OnlineCourses.Core.Infrastructure;
using OnlineCourses.Core.Reporting;

public sealed class EnrollmentRepositoryTests
{
    [Fact]
    public void JsonRepository_PersistsEnrollmentAcrossInstances()
    {
        var directory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(directory);
        var seedPath = Path.Combine(directory, "seed.json");
        var dataPath = Path.Combine(directory, "data.json");
        File.WriteAllText(seedPath, """
            {
              "members": [{"id": 1, "name": "Test User", "email": "test@example.com"}],
              "courses": [{"id": 1, "code": "CS1", "title": "C#", "price": 100, "prerequisiteCourseIds": []}],
              "groups": [{"id": 1, "courseId": 1, "name": "Group A", "capacity": 2}],
              "enrollments": []
            }
            """);

        var first = new JsonEnrollmentRepository(dataPath, seedPath);
        first.Add(new Enrollment(1, 1, 1, 1, new DateOnly(2026, 11, 20)));
        first.GetById(1)!.Withdraw("Test withdrawal");
        var second = new JsonEnrollmentRepository(dataPath, seedPath);

        var restored = second.GetById(1);
        Assert.NotNull(restored);
        Assert.Equal(EnrollmentStatus.Withdrawn, restored.Status);
        Assert.Equal("Test withdrawal", restored.WithdrawalReason);

        Directory.Delete(directory, recursive: true);
    }

    [Fact]
    public void FileAuditLog_WritesInformationWarningAndErrorWithFixedTime()
    {
        var path = Path.Combine(
            Path.GetTempPath(), Guid.NewGuid().ToString("N"), "audit.log");
        var clock = new FixedClock(
            new DateTimeOffset(2026, 11, 20, 9, 30, 0, TimeSpan.FromHours(2)));

        using (var log = new FileAuditLog(path, clock))
        {
            log.Write(AuditLevel.Information, "UC-01", "Started");
            log.Write(AuditLevel.Warning, "UC-01", "Rejected");
            log.Write(AuditLevel.Error, "APP", "Unexpected failure");
        }

        var content = File.ReadAllText(path);
        Assert.Contains("Information", content);
        Assert.Contains("Warning", content);
        Assert.Contains("Error", content);
        Assert.Contains("2026-11-20", content);

        Directory.Delete(Path.GetDirectoryName(path)!, recursive: true);
    }
}
