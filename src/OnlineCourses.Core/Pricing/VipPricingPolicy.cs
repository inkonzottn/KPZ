namespace OnlineCourses.Core.Pricing;

using OnlineCourses.Core.Abstractions;
using OnlineCourses.Core.Domain;

public sealed class VipPricingPolicy : IPricingPolicy
{
    public decimal PriceOf(RegistrationItem item) =>
        Math.Round(item.Amount * 0.90m, 2);
}
