namespace OnlineCourses.Core.Pricing;

/// <summary>Represents the CoursePackagePrice type.</summary>
public sealed record CoursePackagePrice(
    decimal BaseTotal,
    decimal DiscountRate,
    decimal Discount,
    decimal Payable);
