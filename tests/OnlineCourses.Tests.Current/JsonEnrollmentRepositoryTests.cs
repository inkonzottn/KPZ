namespace OnlineCourses.Tests;

using OnlineCourses.Core.Domain;
using OnlineCourses.Core.Infrastructure;

public sealed class JsonEnrollmentRepositoryTests
{
    [Fact]
    public void Add_Enrollment_PersistsDataAcrossRepositoryInstances()
    {
        var (directory, seedPath, dataPath) = CreateStorage();

        try
        {
            var repository = new JsonEnrollmentRepository(dataPath, seedPath);
            var enrollment = new Enrollment(
                2,
                1,
                1,
                1,
                new DateOnly(2026, 11, 20));

            repository.Add(enrollment);

            var reopenedRepository = new JsonEnrollmentRepository(dataPath, seedPath);
            var restored = reopenedRepository.GetById(enrollment.Id);

            Assert.NotNull(restored);
            Assert.Equal(enrollment.MemberId, restored.MemberId);
            Assert.Equal(enrollment.CourseId, restored.CourseId);
            Assert.Equal(enrollment.GroupId, restored.GroupId);
            Assert.Equal(EnrollmentStatus.Active, restored.Status);
        }
        finally
        {
            Directory.Delete(directory, recursive: true);
        }
    }

    [Fact]
    public void Save_WithdrawnEnrollment_PersistsStatusAndReason()
    {
        var (directory, seedPath, dataPath) = CreateStorage();

        try
        {
            var repository = new JsonEnrollmentRepository(dataPath, seedPath);
            var enrollment = new Enrollment(
                2,
                1,
                1,
                1,
                new DateOnly(2026, 11, 20));

            repository.Add(enrollment);
            enrollment.Withdraw("Заява слухача");
            repository.Save(enrollment);

            var reopenedRepository = new JsonEnrollmentRepository(dataPath, seedPath);
            var restored = reopenedRepository.GetById(enrollment.Id);

            Assert.NotNull(restored);
            Assert.Equal(EnrollmentStatus.Withdrawn, restored.Status);
            Assert.Equal("Заява слухача", restored.WithdrawalReason);
        }
        finally
        {
            Directory.Delete(directory, recursive: true);
        }
    }

    [Fact]
    public void Save_CompletedEnrollment_PersistsStatusAcrossRepositoryInstances()
    {
        var (directory, seedPath, dataPath) = CreateStorage();

        try
        {
            var repository = new JsonEnrollmentRepository(dataPath, seedPath);
            var enrollment = new Enrollment(2, 1, 1, 1, new DateOnly(2026, 11, 20));
            repository.Add(enrollment);
            enrollment.Complete();
            repository.Save(enrollment);

            var reopened = new JsonEnrollmentRepository(dataPath, seedPath);
            var restored = reopened.GetById(enrollment.Id);

            Assert.NotNull(restored);
            Assert.Equal(EnrollmentStatus.Completed, restored.Status);
        }
        finally
        {
            Directory.Delete(directory, recursive: true);
        }
    }

    [Fact]
    public void Save_CancelledEnrollment_PersistsStatusAndReason()
    {
        var (directory, seedPath, dataPath) = CreateStorage();

        try
        {
            var repository = new JsonEnrollmentRepository(dataPath, seedPath);
            var enrollment = new Enrollment(2, 1, 1, 1, new DateOnly(2026, 11, 20));
            repository.Add(enrollment);
            enrollment.Cancel("Помилкове оформлення");
            repository.Save(enrollment);

            var reopened = new JsonEnrollmentRepository(dataPath, seedPath);
            var restored = reopened.GetById(2);

            Assert.NotNull(restored);
            Assert.Equal(EnrollmentStatus.Cancelled, restored.Status);
            Assert.Equal("Помилкове оформлення", restored.CancellationReason);
        }
        finally
        {
            Directory.Delete(directory, recursive: true);
        }
    }

    [Fact]
    public void Constructor_MissingDataFile_CopiesSeedData()
    {
        var (directory, seedPath, dataPath) = CreateStorage();

        try
        {
            Assert.False(File.Exists(dataPath));

            var repository = new JsonEnrollmentRepository(dataPath, seedPath);

            Assert.True(File.Exists(dataPath));
            Assert.NotNull(repository.GetMember(1));
            Assert.NotNull(repository.GetCourse(1));
            Assert.NotNull(repository.GetGroup(1));
        }
        finally
        {
            Directory.Delete(directory, recursive: true);
        }
    }

    private static (string Directory, string SeedPath, string DataPath) CreateStorage()
    {
        var directory = Path.Combine(
            Path.GetTempPath(),
            Guid.NewGuid().ToString("N"));

        Directory.CreateDirectory(directory);

        var seedPath = Path.Combine(directory, "seed.json");
        var dataPath = Path.Combine(directory, "data.json");

        File.WriteAllText(seedPath, """
        {
          "members": [
            {
              "id": 1,
              "name": "Тестовий слухач",
              "email": "test@example.com"
            }
          ],
          "courses": [
            {
              "id": 1,
              "code": "CS-101",
              "title": "Основи програмування",
              "price": 1200,
              "prerequisiteCourseIds": []
            }
          ],
          "groups": [
            {
              "id": 1,
              "courseId": 1,
              "name": "CS-101-A",
              "capacity": 10
            }
          ],
          "enrollments": []
        }
        """);

        return (directory, seedPath, dataPath);
    }
}
