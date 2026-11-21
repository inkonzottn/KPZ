using OnlineCourses.Core;
using Xunit;

namespace OnlineCourses.Tests;

public class RegistrationLineParserTests
{
    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("SKU-1;2")]
    [InlineData("SKU-1;0;10.50")]
    [InlineData("SKU-1;-2;10.50")]
    [InlineData("SKU-1;2;-10.50")]
    public void TryParse_InvalidInput_ReturnsFalse(string raw)
    {
        var ok = RegistrationLineParser.TryParse(raw, out var line);

        Assert.False(ok);
        Assert.Null(line);
    }

    [Fact]
    public void TryParse_ValidInput_ReturnsItem()
    {
        var ok = RegistrationLineParser.TryParse("SKU-1;2;10.50", out var line);

        Assert.True(ok);
        Assert.Equal(2, line!.Quantity);
        Assert.Equal(10.50m, line.UnitPrice);
    }
}
