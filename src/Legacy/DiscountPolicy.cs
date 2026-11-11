using System;

namespace OnlineCourses.src.Legacy;

public sealed class DiscountPolicy
{
    public decimal For(decimal subtotal, Member c)
    {
        var rate = RateFor(c);
        return Math.Min(subtotal * rate,
            PricingRules.MaxDiscount);
    }

    private static decimal RateFor(Member c) => c.Kind
        switch
    {
        ClientKind.Vip => PricingRules.VipRate,
        ClientKind.Staff => PricingRules.StaffRate,
        _ => c.IsLoyal ? PricingRules.LoyalRate : 0m
    };
}
