namespace OnlineCourses.Tests;

using OnlineCourses.Core.Domain;

public sealed class DomainValidationTests
{
    [Fact]
    public void Member_EmptyName_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() =>
            new Member(1, " ", "learner@example.com"));
    }

    [Fact]
    public void Course_NullCode_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() =>
            new Course(1, null!, "Course", 100m));
    }

    [Fact]
    public void CourseGroup_ZeroCapacity_ThrowsOutOfRange()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            new CourseGroup(1, 1, "Group A", 0));
    }

    [Fact]
    public void Enrollment_NonPositiveMemberId_ThrowsOutOfRange()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            new Enrollment(1, 0, 1, 1, new DateOnly(2026, 1, 1)));
    }

    [Fact]
    public void Course_CannotRequireItselfAsPrerequisite()
    {
        Assert.Throws<ArgumentException>(() =>
            new Course(1, "CS-1", "Course", 100m, [1]));
    }
}
