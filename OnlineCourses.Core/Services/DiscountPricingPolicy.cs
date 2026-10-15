namespace OnlineCourses.Core.Services;

using OnlineCourses.Core.Abstractions;
using OnlineCourses.Core.Documents;
using OnlineCourses.Core.Domain;

public sealed class DiscountPricingPolicy : IPricingPolicy
{
    private readonly decimal _discountRate;

    public DiscountPricingPolicy(decimal discountRate)
    {
        _discountRate = discountRate;
    }

    public decimal PriceOf(RegistrationItem item)
    {
        ArgumentNullException.ThrowIfNull(item);

        return item.Amount * (1m - _discountRate);
    }
}
