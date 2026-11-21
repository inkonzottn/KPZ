namespace OnlineCourses.Core.Pricing;

using OnlineCourses.Core.Abstractions;
using OnlineCourses.Core.Domain;

/// <summary>Applies the fixed VIP rate to registration line amounts.</summary>
public sealed class VipPricingPolicy : IPricingPolicy
{
    /// <summary>Calculates the VIP amount for one registration line.</summary>
    /// <param name="item">Registration line to price.</param>
    /// <returns>The rounded amount payable.</returns>
    public decimal PriceOf(RegistrationItem item) =>
        Math.Round(item.Amount * 0.90m, 2);
}
