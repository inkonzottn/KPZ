namespace OnlineCourses.Core.Pricing;

using OnlineCourses.Core.Abstractions;
using OnlineCourses.Core.Domain;

/// <summary>Represents the CoursePackagePricingPolicy type.</summary>
public sealed class CoursePackagePricingPolicy : ICoursePackagePricingPolicy
{
    private const int SingleCourseCount = 1;
    private const int TwoCourseCount = 2;
    private const int CurrencyDecimalPlaces = 2;
    private const decimal NoDiscountRate = 0m;
    private const decimal TwoCourseDiscountRate = 0.05m;
    private const decimal MaximumDiscountRate = 0.10m;

/// <summary>Performs the Calculate operation.</summary>
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
            SingleCourseCount => NoDiscountRate,
            TwoCourseCount => TwoCourseDiscountRate,
            _ => MaximumDiscountRate
        };

        var discount = decimal.Round(baseTotal * rate, CurrencyDecimalPlaces, MidpointRounding.AwayFromZero);
        return new CoursePackagePrice(baseTotal, rate, discount, baseTotal - discount);
    }
}
