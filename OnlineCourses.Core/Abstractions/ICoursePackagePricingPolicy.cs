namespace OnlineCourses.Core.Abstractions;

using OnlineCourses.Core.Domain;

/// <summary>Defines the contract for package price calculation.</summary>
public interface ICoursePackagePricingPolicy
{
    /// <summary>Calculates the package total, discount, and payable amount.</summary>
    CoursePackagePrice Calculate(IEnumerable<Course> courses);
}
