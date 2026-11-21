namespace OnlineCourses.Core.Pricing;

using OnlineCourses.Core.Abstractions;
using OnlineCourses.Core.Domain;

/// <summary>Застосовує пакетну знижку залежно від кількості вибраних курсів.</summary>
public sealed class CoursePackagePricingPolicy : ICoursePackagePricingPolicy
{
    /// <summary>Розраховує суму пакета із знижкою 5 % за два курси та 10 % за три й більше.</summary>
    /// <param name="courses">Непорожня послідовність різних курсів із невід'ємними цінами.</param>
    /// <returns>Базова сума, ставка, сума знижки та сума до сплати.</returns>
    /// <exception cref="ArgumentNullException">Послідовність не передано.</exception>
    /// <exception cref="ArgumentException">Пакет порожній або містить повторні курси.</exception>
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
            2 => 0.05m,
            _ => 0.10m
        };

        var discount = decimal.Round(baseTotal * rate, 2, MidpointRounding.AwayFromZero);
        return new CoursePackagePrice(baseTotal, rate, discount, baseTotal - discount);
    }
}
