namespace OnlineCourses.Core.Pricing;

public sealed record CoursePackagePrice(
    decimal BaseTotal,
    decimal DiscountRate,
    decimal Discount,
    decimal Payable);
