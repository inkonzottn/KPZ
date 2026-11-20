namespace OnlineCourses.Core.Abstractions;

using OnlineCourses.Core.Domain;
using OnlineCourses.Core.Pricing;

public interface ICoursePackagePricingPolicy
{
    CoursePackagePrice Calculate(IEnumerable<Course> courses);
}
