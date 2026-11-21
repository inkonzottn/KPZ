namespace OnlineCourses.Core.Abstractions;

using OnlineCourses.Core.Domain;
using OnlineCourses.Core.Pricing;

/// <summary>Визначає контракт розрахунку вартості набору навчальних курсів.</summary>
public interface ICoursePackagePricingPolicy
{
    /// <summary>Обчислює базову суму, знижку та підсумкову вартість пакета.</summary>
    /// <param name="courses">Непорожня послідовність різних курсів.</param>
    /// <returns>Розклад вартості пакета з використаною ставкою знижки.</returns>
    /// <exception cref="ArgumentNullException">Послідовність не передано.</exception>
    /// <exception cref="ArgumentException">Пакет порожній або містить повторні курси.</exception>
    CoursePackagePrice Calculate(IEnumerable<Course> courses);
}
