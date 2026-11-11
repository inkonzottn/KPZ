namespace OnlineCourses.src.Legacy;

public interface IDiscountRule
{
    decimal? RateFor(Member c);
}

public sealed class VipDiscountRule : IDiscountRule
{
    public decimal? RateFor(Member c) =>
        c.Kind == ClientKind.Vip ? PricingRules.VipRate : null;
}

public sealed class StaffDiscountRule : IDiscountRule
{
    public decimal? RateFor(Member c) =>
        c.Kind == ClientKind.Staff ? PricingRules.StaffRate : null;
}

public sealed class LoyalDiscountRule : IDiscountRule
{
    public decimal? RateFor(Member c) =>
        c.IsLoyal ? PricingRules.LoyalRate : null;
}