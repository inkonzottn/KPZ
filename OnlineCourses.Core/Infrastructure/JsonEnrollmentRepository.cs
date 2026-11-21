namespace OnlineCourses.Core.Infrastructure;

using System.Text.Json;
using OnlineCourses.Core.Abstractions;
using OnlineCourses.Core.Domain;
using OnlineCourses.Core.Services;

/// <summary>
/// Зберігає навчальні дані у JSON-файлі та делегує пошук in-memory репозиторію.
/// </summary>
public sealed class JsonEnrollmentRepository : IEnrollmentRepository
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        WriteIndented = true
    };

    private readonly string _path;
    private readonly InMemoryEnrollmentRepository _memory = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="JsonEnrollmentRepository"/> class.
    /// Завантажує наявне сховище або копіює початковий набір даних.
    /// </summary>
    /// <param name="path">Шлях до робочого JSON-файлу.</param>
    /// <param name="seedPath">Шлях до початкового набору даних.</param>
    public JsonEnrollmentRepository(string path, string seedPath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        ArgumentException.ThrowIfNullOrWhiteSpace(seedPath);
        _path = Path.GetFullPath(path);
        var fullSeedPath = Path.GetFullPath(seedPath);
        Directory.CreateDirectory(Path.GetDirectoryName(_path)!);

        if (!File.Exists(_path))
        {
            if (!File.Exists(fullSeedPath))
            {
                throw new FileNotFoundException("Не знайдено початковий набір даних.", fullSeedPath);
            }

            File.Copy(fullSeedPath, _path);
        }

        Load(File.ReadAllText(_path));
    }

    /// <summary>Додає слухача та зберігає зміни.</summary>
    /// <param name="member">Слухач, якого потрібно додати.</param>
    public void AddMember(Member member)
    {
        _memory.AddMember(member);
        Persist();
    }

    /// <summary>Додає курс та зберігає зміни.</summary>
    /// <param name="course">Курс, який потрібно додати.</param>
    public void AddCourse(Course course)
    {
        _memory.AddCourse(course);
        Persist();
    }

    /// <summary>Додає групу та зберігає зміни.</summary>
    /// <param name="group">Група, яку потрібно додати.</param>
    public void AddGroup(CourseGroup group)
    {
        _memory.AddGroup(group);
        Persist();
    }

    /// <inheritdoc />
    public Member? GetMember(int memberId) => _memory.GetMember(memberId);

    /// <inheritdoc />
    public Course? GetCourse(int courseId) => _memory.GetCourse(courseId);

    /// <inheritdoc />
    public CourseGroup? GetGroup(int groupId) => _memory.GetGroup(groupId);

    /// <inheritdoc />
    public IReadOnlyList<Enrollment> GetAll() => _memory.GetAll();

    /// <inheritdoc />
    public IReadOnlyList<CourseGroup> GetGroups() => _memory.GetGroups();

    /// <inheritdoc />
    public Enrollment? GetById(int enrollmentId) => _memory.GetById(enrollmentId);

    /// <inheritdoc />
    public void Add(Enrollment enrollment)
    {
        _memory.Add(enrollment);
        Persist();
    }

    /// <inheritdoc />
    public void Save(Enrollment enrollment)
    {
        _memory.Save(enrollment);
        Persist();
    }

    /// <inheritdoc />
    public bool HasCompletedCourse(int memberId, int courseId) =>
        _memory.HasCompletedCourse(memberId, courseId);

    private static Snapshot DeserializeSnapshot(string json) =>
        JsonSerializer.Deserialize<Snapshot>(json, JsonOptions)
        ?? throw new InvalidDataException("JSON-файл не містить коректного набору даних.");

    private void Load(string json)
    {
        var snapshot = DeserializeSnapshot(json);
        LoadMembers(snapshot.Members);
        LoadCourses(snapshot.Courses);
        LoadGroups(snapshot.Groups);
        LoadEnrollments(snapshot.Enrollments);
    }

    private void LoadMembers(IEnumerable<MemberData> members)
    {
        foreach (var member in members)
        {
            _memory.AddMember(new Member(member.Id, member.Name, member.Email));
        }
    }

    private void LoadCourses(IEnumerable<CourseData> courses)
    {
        foreach (var course in courses)
        {
            _memory.AddCourse(new Course(
                course.Id,
                course.Code,
                course.Title,
                course.Price,
                course.PrerequisiteCourseIds));
        }
    }

    private void LoadGroups(IEnumerable<GroupData> groups)
    {
        foreach (var group in groups)
        {
            _memory.AddGroup(new CourseGroup(
                group.Id,
                group.CourseId,
                group.Name,
                group.Capacity));
        }
    }

    private void LoadEnrollments(IEnumerable<EnrollmentData> enrollments)
    {
        foreach (var item in enrollments)
        {
            _memory.Add(RestoreEnrollment(item));
        }
    }

    private Enrollment RestoreEnrollment(EnrollmentData item)
    {
        var enrollment = new Enrollment(
            item.Id,
            item.MemberId,
            item.CourseId,
            item.GroupId,
            item.EnrolledOn);

        switch (item.Status)
        {
            case nameof(EnrollmentStatus.Withdrawn):
                enrollment.Withdraw(item.WithdrawalReason ?? "Причину не вказано у файлі.");
                break;
            case nameof(EnrollmentStatus.Completed):
                enrollment.Complete();
                break;
            case nameof(EnrollmentStatus.Cancelled):
                enrollment.Cancel(item.CancellationReason ?? "Причину не вказано у файлі.");
                break;
        }

        return enrollment;
    }

    private void Persist()
    {
        var snapshot = new Snapshot
        {
            Members = _memory.GetMembersForPersistence().Select(x =>
                new MemberData { Id = x.Id, Name = x.Name, Email = x.Email }).ToList(),
            Courses = _memory.GetCoursesForPersistence().Select(x =>
                new CourseData
                {
                    Id = x.Id,
                    Code = x.Code,
                    Title = x.Title,
                    Price = x.Price,
                    PrerequisiteCourseIds = x.PrerequisiteCourseIds.ToList()
                }).ToList(),
            Groups = _memory.GetGroups().Select(x =>
                new GroupData
                {
                    Id = x.Id,
                    CourseId = x.CourseId,
                    Name = x.Name,
                    Capacity = x.Capacity
                }).ToList(),
            Enrollments = _memory.GetAll().Select(x =>
                new EnrollmentData
                {
                    Id = x.Id,
                    MemberId = x.MemberId,
                    CourseId = x.CourseId,
                    GroupId = x.GroupId,
                    EnrolledOn = x.EnrolledOn,
                    Status = x.Status.ToString(),
                    WithdrawalReason = x.WithdrawalReason,
                    CancellationReason = x.CancellationReason
                }).ToList()
        };

        var temporaryPath = _path + ".tmp";
        File.WriteAllText(temporaryPath, JsonSerializer.Serialize(snapshot, JsonOptions));
        File.Move(temporaryPath, _path, overwrite: true);
    }

    private sealed class Snapshot
    {
        public List<MemberData> Members { get; set; } = [];

        public List<CourseData> Courses { get; set; } = [];

        public List<GroupData> Groups { get; set; } = [];

        public List<EnrollmentData> Enrollments { get; set; } = [];
    }

    private sealed class MemberData
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;
    }

    private sealed class CourseData
    {
        public int Id { get; set; }

        public string Code { get; set; } = string.Empty;

        public string Title { get; set; } = string.Empty;

        public decimal Price { get; set; }

        public List<int> PrerequisiteCourseIds { get; set; } = [];
    }

    private sealed class GroupData
    {
        public int Id { get; set; }

        public int CourseId { get; set; }

        public string Name { get; set; } = string.Empty;

        public int Capacity { get; set; }
    }

    private sealed class EnrollmentData
    {
        public int Id { get; set; }

        public int MemberId { get; set; }

        public int CourseId { get; set; }

        public int GroupId { get; set; }

        public DateOnly EnrolledOn { get; set; }

        public string Status { get; set; } = nameof(EnrollmentStatus.Active);

        public string? WithdrawalReason { get; set; }

        public string? CancellationReason { get; set; }
    }
}
