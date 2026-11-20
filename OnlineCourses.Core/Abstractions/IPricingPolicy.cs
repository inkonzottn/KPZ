namespace OnlineCourses.Core.Abstractions;

using OnlineCourses.Core.Domain;

/// <summary>Defines the contract for IPricingPolicy.</summary>
public interface IPricingPolicy
{
    decimal PriceOf(RegistrationItem item);
}
