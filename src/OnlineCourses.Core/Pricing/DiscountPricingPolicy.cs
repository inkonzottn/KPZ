namespace OnlineCourses.Core.Pricing;

using OnlineCourses.Core.Abstractions;
using OnlineCourses.Core.Documents;
using OnlineCourses.Core.Domain;

/// <summary>Applies a configurable reduction to registration line amounts.</summary>
public sealed class DiscountPricingPolicy : IPricingPolicy
{
    private readonly decimal _rate;

    /// <summary>Initializes the policy with a validated discount rate.</summary>
    /// <param name="rate">Discount fraction from zero through one half.</param>
    public DiscountPricingPolicy(decimal rate)
    {
        if (rate < 0m || rate > 0.5m)
        {
            throw new ArgumentOutOfRangeException(nameof(rate));
        }

        _rate = rate;
    }

    /// <summary>Calculates the discounted amount for one registration line.</summary>
    /// <param name="item">Registration line whose amount is discounted.</param>
    /// <returns>The rounded amount payable.</returns>
    public decimal PriceOf(RegistrationItem item) =>
        Math.Round(item.Amount * (1m - _rate), 2);
}
