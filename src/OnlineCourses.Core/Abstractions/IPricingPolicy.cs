namespace OnlineCourses.Core.Abstractions;

using OnlineCourses.Core.Domain;

/// <summary>Визначає правило обчислення вартості окремої позиції реєстрації.</summary>
public interface IPricingPolicy
{
    /// <summary>Обчислює вартість позиції з урахуванням чинної цінової політики.</summary>
    /// <param name="item">Позиція з додатною кількістю та невід'ємною ціною.</param>
    /// <returns>Вартість позиції у гривнях.</returns>
    decimal PriceOf(RegistrationItem item);
}
