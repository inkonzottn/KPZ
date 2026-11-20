namespace OnlineCourses.Core.Infrastructure;

using System.Text.Json;
using OnlineCourses.Core.Abstractions;
using OnlineCourses.Core.Domain;
using OnlineCourses.Core.Services;

/// <summary>Persists online course data in a JSON file.</summary>
public sealed class JsonEnrollmentRepository : IEnrollmentRepository
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        WriteIndented = true
    };

    private readonly string _path;
    private readonly InMemoryEnrollmentRepository _memory = new();

    /// <summary>Initializes a new instance of the <see cref="JsonEnrollmentRepository"/> class.</summary>
    /// <param name="path">Path to the working JSON file.</param>
    /// <param name="seedPath">Path to the initial dataset.</param>
    public JsonEnrollmentRepository(string path, string seedPath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        ArgumentException.ThrowIfNullOrWhiteSpace(seedPath);
        _path = Path.GetFullPath(path);
        Directory.CreateDirectory(Path.GetDirectoryName(_path)!);
        EnsureWorkingFile(Path.GetFullPath(seedPath));
        Load(File.ReadAllText(_path));
    }

    /// <summary>Adds a learner and persists the change.</summary>
    /// <param name="member">Learner to add.</param>
    public void AddMember(Member member)
    {
        _memory.AddMember(member);
        Persist();
    }

    /// <summary>Adds a course and persists the change.</summary>
    /// <param name="course">Course to add.</param>
    public void AddCourse(Course course)
    {
        _memory.AddCourse(course);
        Persist();
    }

    /// <summary>Adds a group and persists the change.</summary>
    /// <param name="group">Group to add.</param>
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

    private void EnsureWorkingFile(string seedPath)
    {
        if (File.Exists(_path))
        {
            return;
        }

        if (!File.Exists(seedPath))
        {
            throw new FileNotFoundException("Не знайдено початковий набір даних.", seedPath);
        }

        File.Copy(seedPath, _path);
    }

    private void Load(string json)
    {
        var snapshot = JsonSerializer.Deserialize<Snapshot>(json, JsonOptions)
            ?? throw new InvalidDataException("JSON-файл не містить коректного набору даних.");
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
                course.Id, course.Code, course.Title, course.Price,
                course.PrerequisiteCourseIds));
        }
    }

    private void LoadGroups(IEnumerable<GroupData> groups)
    {
        foreach (var group in groups)
        {
            _memory.AddGroup(new CourseGroup(
                group.Id, group.CourseId, group.Name, group.Capacity));
        }
    }

    private void LoadEnrollments(IEnumerable<EnrollmentData> enrollments)
    {
        foreach (var item in enrollments)
        {
            var enrollment = new Enrollment(
                item.Id, item.MemberId, item.CourseId, item.GroupId, item.EnrolledOn);
            RestoreStatus(enrollment, item);
            _memory.Add(enrollment);
        }
    }

    private static void RestoreStatus(Enrollment enrollment, EnrollmentData item)
    {
        switch (item.Status)
        {
            case nameof(EnrollmentStatus.Withdrawn):
                enrollment.Withdraw(item.WithdrawalReason ?? "Причину не вказано у файлі.");
                break;
            case nameof(EnrollmentStatus.Suspended):
                enrollment.Suspend();
                break;
            case nameof(EnrollmentStatus.Completed):
                enrollment.Complete();
                break;
        }
    }

    private void Persist()
    {
        var json = JsonSerializer.Serialize(CreateSnapshot(), JsonOptions);
        WriteAtomically(json);
    }

    private Snapshot CreateSnapshot() => new()
    {
        Members = _memory.GetMembersForPersistence().Select(CreateMemberData).ToList(),
        Courses = _memory.GetCoursesForPersistence().Select(CreateCourseData).ToList(),
        Groups = _memory.GetGroups().Select(CreateGroupData).ToList(),
        Enrollments = _memory.GetAll().Select(CreateEnrollmentData).ToList()
    };

    private static MemberData CreateMemberData(Member member) => new()
    {
        Id = member.Id,
        Name = member.Name,
        Email = member.Email
    };

    private static CourseData CreateCourseData(Course course) => new()
    {
        Id = course.Id,
        Code = course.Code,
        Title = course.Title,
        Price = course.Price,
        PrerequisiteCourseIds = course.PrerequisiteCourseIds.ToList()
    };

    private static GroupData CreateGroupData(CourseGroup group) => new()
    {
        Id = group.Id,
        CourseId = group.CourseId,
        Name = group.Name,
        Capacity = group.Capacity
    };

    private static EnrollmentData CreateEnrollmentData(Enrollment enrollment) => new()
    {
        Id = enrollment.Id,
        MemberId = enrollment.MemberId,
        CourseId = enrollment.CourseId,
        GroupId = enrollment.GroupId,
        EnrolledOn = enrollment.EnrolledOn,
        Status = enrollment.Status.ToString(),
        WithdrawalReason = enrollment.WithdrawalReason
    };

    private void WriteAtomically(string json)
    {
        var temporaryPath = _path + ".tmp";
        File.WriteAllText(temporaryPath, json);
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
    }
}
