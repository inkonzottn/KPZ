namespace OnlineCourses.Core.Pricing;

using OnlineCourses.Core.Abstractions;
using OnlineCourses.Core.Domain;

public sealed class CoursePackagePricingPolicy : ICoursePackagePricingPolicy
{
    public CoursePackagePrice Calculate(IEnumerable<Course> courses)
    {
        ArgumentNullException.ThrowIfNull(courses);
        var package = courses.ToArray();

        if (package.Length == 0)
        {
            throw new ArgumentException("Пакет має містити хоча б один курс.", nameof(courses));
        }

        if (package.Select(x => x.Id).Distinct().Count() != package.Length)
        {
            throw new ArgumentException("Пакет не може містити повтори курсів.", nameof(courses));
        }

        var baseTotal = package.Sum(x => x.Price);
        var rate = package.Length switch
        {
            1 => 0m,
            2 => 0.06m,
            _ => 0.10m
        };

        var discount = decimal.Round(baseTotal * rate, 2, MidpointRounding.AwayFromZero);
        return new CoursePackagePrice(baseTotal, rate, discount, baseTotal - discount);
    }
}
