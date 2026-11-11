using OnlineCourses.Core.Pricing;
using Xunit;

namespace OnlineCourses.Tests;

public class CourseDiscountPolicyTests
{
    [Fact]
    public void DiscountFor_NegativeAmount_ThrowsArgumentOutOfRange()
    {
        // Arrange
        var policy = new CourseDiscountPolicy();

        // Act
        var ex = Record.Exception(() => policy.DiscountFor(-0.01m, true));

        // Assert
        Assert.IsType<ArgumentOutOfRangeException>(ex);
    }

    [Fact]
    public void DiscountFor_ZeroAmountNewCustomer_ReturnsZero()
    {
        // Arrange
        var policy = new CourseDiscountPolicy();

        // Act
        decimal actual = policy.DiscountFor(0m, false);

        // Assert
        Assert.Equal(0m, actual);
    }

    [Fact]
    public void DiscountFor_SmallOrderNewCustomer_ReturnsZero()
    {
        // Arrange
        var policy = new CourseDiscountPolicy();

        // Act
        decimal actual = policy.DiscountFor(400m, false);

        // Assert
        Assert.Equal(0m, actual);
    }

    [Fact]
    public void DiscountFor_SmallOrderRegularCustomer_ReturnsFivePercent()
    {
        // Arrange
        var policy = new CourseDiscountPolicy();

        // Act
        decimal actual = policy.DiscountFor(400m, true);

        // Assert
        Assert.Equal(20.00m, actual);
    }

    [Fact]
    public void DiscountFor_JustBelowBigOrderNewCustomer_ReturnsZero()
    {
        // Arrange
        var policy = new CourseDiscountPolicy();

        // Act
        decimal actual = policy.DiscountFor(999.99m, false);

        // Assert
        Assert.Equal(0m, actual);
    }

    [Fact]
    public void DiscountFor_ExactlyBigOrderNewCustomer_ReturnsTenPercent()
    {
        // Arrange
        var policy = new CourseDiscountPolicy();

        // Act
        decimal actual = policy.DiscountFor(1000m, false);

        // Assert
        Assert.Equal(100.00m, actual);
    }

    [Fact]
    public void DiscountFor_ExactlyBigOrderRegularCustomer_ReturnsFifteenPercent()
    {
        // Arrange
        var policy = new CourseDiscountPolicy();

        // Act
        decimal actual = policy.DiscountFor(1000m, true);

        // Assert
        Assert.Equal(150.00m, actual);
    }
}