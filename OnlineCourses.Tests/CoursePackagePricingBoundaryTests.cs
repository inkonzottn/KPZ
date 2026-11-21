namespace OnlineCourses.Tests;

using OnlineCourses.Core.Domain;
using OnlineCourses.Core.Pricing;

public sealed class CoursePackagePricingBoundaryTests
{
    [Fact]
    public void Calculate_RoundsHalfCentAwayFromZero()
    {
        var courses = new[]
        {
            new Course(1, "C-01", "Course one", 5.05m),
            new Course(2, "C-02", "Course two", 5.05m)
        };
        var policy = new CoursePackagePricingPolicy();

        var result = policy.Calculate(courses);

        Assert.Equal(10.10m, result.BaseTotal);
        Assert.Equal(0.51m, result.Discount);
        Assert.Equal(9.59m, result.Payable);
    }

    [Fact]
    public void Calculate_ReturnsZeroDiscountForZeroPricedPackage()
    {
        var courses = new[]
        {
            new Course(1, "C-01", "Course one", 0m),
            new Course(2, "C-02", "Course two", 0m)
        };
        var policy = new CoursePackagePricingPolicy();

        var result = policy.Calculate(courses);

        Assert.Equal(0m, result.BaseTotal);
        Assert.Equal(0.05m, result.DiscountRate);
        Assert.Equal(0m, result.Discount);
        Assert.Equal(0m, result.Payable);
    }

    [Fact]
    public void Course_RejectsNegativePrice()
    {
        var exception = Assert.Throws<ArgumentOutOfRangeException>(
            () => new Course(1, "C-01", "Course one", -0.01m));

        Assert.Equal("price", exception.ParamName);
    }
}
