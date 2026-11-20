namespace OnlineCourses.Core.Abstractions;

using OnlineCourses.Core.Domain;

/// <summary>Defines the legacy registration pricing contract.</summary>
public interface IPricingPolicy
{
    /// <summary>Calculates the price of a registration item.</summary>
    decimal PriceOf(RegistrationItem item);
}
