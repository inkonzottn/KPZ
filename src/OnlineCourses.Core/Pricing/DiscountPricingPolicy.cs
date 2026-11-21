namespace OnlineCourses.Core.Pricing;

using OnlineCourses.Core.Abstractions;
using OnlineCourses.Core.Documents;
using OnlineCourses.Core.Domain;

public sealed class DiscountPricingPolicy : IPricingPolicy
{
    private readonly decimal _rate;

    public DiscountPricingPolicy(decimal rate)
    {
        if (rate < 0m || rate > 0.5m)
        {
            throw new ArgumentOutOfRangeException(nameof(rate));
        }

        _rate = rate;
    }

    public decimal PriceOf(RegistrationItem item) =>
        Math.Round(item.Amount * (1m - _rate), 2);
}
