namespace OnlineCourses.Core.Domain;

public sealed class CourseGroup
{
    public CourseGroup(int id, int courseId, string name, int capacity)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(id);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(courseId);
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(capacity);

        Id = id;
        CourseId = courseId;
        Name = name.Trim();
        Capacity = capacity;
    }

    public int Id { get; }

    public int CourseId { get; }

    public string Name { get; }

    public int Capacity { get; }
}
