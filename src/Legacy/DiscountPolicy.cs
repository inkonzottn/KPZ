namespace OnlineCourses.src.Legacy;

public sealed class DiscountPolicy
{
    private readonly IReadOnlyList<IDiscountRule> _rules = new IDiscountRule[]
    {
        new VipDiscountRule(),
        new StaffDiscountRule(),
        new LoyalDiscountRule()
    };

    public decimal For(decimal subtotal, Member c)
    {
        var rate = RateFor(c);
        return Math.Min(subtotal * rate,
            PricingRules.MaxDiscount);
    }

    private decimal RateFor(Member c)
    {
        foreach (var rule in _rules)
        {
            var rate = rule.RateFor(c);
            if (rate.HasValue) return rate.Value;
        }
        return 0m;
    }
}