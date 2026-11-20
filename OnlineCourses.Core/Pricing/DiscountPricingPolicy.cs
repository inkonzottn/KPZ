namespace OnlineCourses.Core.Pricing;

using OnlineCourses.Core.Abstractions;
using OnlineCourses.Core.Documents;
using OnlineCourses.Core.Domain;

/// <summary>Represents the DiscountPricingPolicy type.</summary>
public sealed class DiscountPricingPolicy : IPricingPolicy
{
    private readonly decimal _rate;

    /// <summary>Initializes a new instance of the <see cref="DiscountPricingPolicy"/> class.</summary>
    public DiscountPricingPolicy(decimal rate)
    {
        if (rate < 0m || rate > 0.5m)
        {
            throw new ArgumentOutOfRangeException(nameof(rate));
        }

        _rate = rate;
    }

    /// <summary>Performs the PriceOf operation.</summary>
    public decimal PriceOf(RegistrationItem item) =>
        Math.Round(item.Amount * (1m - _rate), 2);
}
