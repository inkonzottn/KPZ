using OnlineCourses.Core.Documents;
using OnlineCourses.Core.Legacy;
using Xunit;

namespace OnlineCourses.Core.Legacy;

public class LegacyProcessorTests
{
    [Fact]
    public void Process_RegularClient_Returns1292()
    {
        List<RegistrationItem> lines = new()
        {
            new RegistrationItem { Sku = "A1", Quantity = 12, UnitPrice = 100m },
            new RegistrationItem { Sku = "B2", Quantity = 1, UnitPrice = 250m }
        };
        decimal total = LegacyProcessor.Process(
            1, "Іваненко", "i@ex.com", true, lines,
            new DateOnly(2026, 3, 10), null , DocumentStatus.New, false,
            60m, out DocumentStatus next);
        Assert.Equal(1292.70m, total);
        Assert.Equal(DocumentStatus.Paid, next);
    }

    [Fact]
    public void Process_SmallOrder_AddsDelivery()
    {
        List<RegistrationItem> lines = new()
        {
            new RegistrationItem { Sku = "A1", Quantity = 2, UnitPrice = 200m }
        };
        decimal total = LegacyProcessor.Process(
            2, "Петренко", "p@ex.com", false, lines,
            new DateOnly(2026, 3, 10), PricingRules.SaleCoupon, DocumentStatus.Paid,
            false, 60m, out DocumentStatus next);
        Assert.Equal(420m, total);
        Assert.Equal(DocumentStatus.Confirmed, next);
    }

    [Fact]
    public void Calculate_NoLines_ThrowsArgument()
    {
        Assert.Throws<ArgumentException>(() =>
        DocumentGuards.EnsureLinesValid(
        new List<RegistrationItem>()));
    }

    [Fact]
    public void Calculate_ZeroQuantity_ThrowsRange()
    {
        RegistrationItem line = new()
        {
            Sku = "A1",
            Quantity = 0,
            UnitPrice = 10m
        };
        Assert.Throws<ArgumentOutOfRangeException>(
        () => DocumentGuards.EnsureLineValid(line));
    }
}
