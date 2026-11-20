namespace OnlineCourses.Core.Pricing;

using OnlineCourses.Core.Abstractions;
using OnlineCourses.Core.Domain;

/// <summary>Represents the VipPricingPolicy type.</summary>
public sealed class VipPricingPolicy : IPricingPolicy
{
    /// <summary>Performs the PriceOf operation.</summary>
    public decimal PriceOf(RegistrationItem item) =>
        Math.Round(item.Amount * 0.90m, 2);
}
