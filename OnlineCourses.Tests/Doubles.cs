using System.Text;
using OnlineCourses.Core.Abstractions;
using OnlineCourses.Core.Domain;
using OnlineCourses.Core.Services;

namespace OnlineCourses.Tests;

internal sealed class FakeRegistrationRepository : IRegistrationRepository
{
    private readonly Dictionary<int, Registration> _items = new();

    public int SaveCalls { get; private set; }

    public void Add(Registration document) => _items[document.Id] = document;

    public void Save(Registration registration) => SaveCalls++;

    public Registration? GetById(int id) =>
        _items.TryGetValue(id, out Registration? found)
            ? found
            : null;

    public IReadOnlyList<Registration> GetAll() => _items.Values.ToList();
}

internal sealed class StubPricingPolicy : IPricingPolicy
{
    private readonly decimal _price;

    public StubPricingPolicy(decimal price) => _price = price;

    public decimal PriceOf(RegistrationItem item) => _price;
}

internal sealed class StubNotifier : INotifier
{
    public void Notify(string recipient, string body)
    {
    }
}

internal sealed class FixedClock : IClock
{
    public FixedClock(DateTimeOffset now) => Now = now;

    public DateTimeOffset Now { get; }

    public DateOnly Today => DateOnly.FromDateTime(Now.LocalDateTime);
}

internal sealed class RecordingAuditLogger : IAuditLogger
{
    public List<(AuditLevel Level, string Scenario, string Message)> Entries { get; } = [];

    public void Write(AuditLevel level, string scenario, string message) =>
        Entries.Add((level, scenario, message));
}

internal sealed class FakeEnrollmentRepository : IEnrollmentRepository
{
    private readonly InMemoryEnrollmentRepository _store = new();

    public void AddMember(Member member) => _store.AddMember(member);

    public void AddCourse(Course course) => _store.AddCourse(course);

    public void AddGroup(CourseGroup group) => _store.AddGroup(group);

    public Member? GetMember(int memberId) => _store.GetMember(memberId);

    public Course? GetCourse(int courseId) => _store.GetCourse(courseId);

    public CourseGroup? GetGroup(int groupId) => _store.GetGroup(groupId);

    public IReadOnlyList<Enrollment> GetAll() => _store.GetAll();

    public IReadOnlyList<CourseGroup> GetGroups() => _store.GetGroups();

    public Enrollment? GetById(int enrollmentId) => _store.GetById(enrollmentId);

    public void Add(Enrollment enrollment) => _store.Add(enrollment);

    public void Save(Enrollment enrollment) => _store.Save(enrollment);

    public bool HasCompletedCourse(int memberId, int courseId) =>
        _store.HasCompletedCourse(memberId, courseId);
}

internal sealed class FakeConsole : IConsole
{
    private readonly Queue<string?> _input;

    public FakeConsole(IEnumerable<string?> input) => _input = new Queue<string?>(input);

    public StringBuilder Output { get; } = new();

    public string? ReadLine() => _input.Count > 0 ? _input.Dequeue() : null;

    public void Write(string value) => Output.Append(value);

    public void WriteLine(string value) => Output.AppendLine(value);
}
