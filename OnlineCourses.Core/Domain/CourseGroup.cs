namespace OnlineCourses.Core.Domain;

/// <summary>Represents the CourseGroup type.</summary>
public sealed class CourseGroup
{
    /// <summary>Initializes a new instance of the <see cref="CourseGroup"/> class.</summary>
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

    /// <summary>Gets the Id value.</summary>
    public int Id { get; }

    /// <summary>Gets the CourseId value.</summary>
    public int CourseId { get; }

    /// <summary>Gets the Name value.</summary>
    public string Name { get; }

    /// <summary>Gets the Capacity value.</summary>
    public int Capacity { get; }
}
