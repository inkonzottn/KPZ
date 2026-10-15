namespace OnlineCourses.Core.Abstractions;

using OnlineCourses.Core.Domain;

public interface IPricingPolicy
{
    decimal PriceOf(RegistrationItem item);
}
