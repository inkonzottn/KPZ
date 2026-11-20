namespace OnlineCourses.Core.Abstractions;

using OnlineCourses.Core.Domain;
using OnlineCourses.Core.Pricing;

/// <summary>Defines the contract for ICoursePackagePricingPolicy.</summary>
public interface ICoursePackagePricingPolicy
{
    CoursePackagePrice Calculate(IEnumerable<Course> courses);
}
