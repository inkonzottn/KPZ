namespace OnlineCourses.Core.Pricing;

/// <summary>Обчислює персональну знижку з урахуванням статусу слухача та суми замовлення.</summary>
public sealed class CourseDiscountPolicy
{
    /// <summary>Gets the discount rate for a regular learner.</summary>
    public const decimal RegularRate = 0.05m;

    /// <summary>Gets the additional discount rate for a large order.</summary>
    public const decimal BigOrderRate = 0.10m;

    /// <summary>Gets the minimum amount that qualifies as a large order, in UAH.</summary>
    public const decimal BigOrderFrom = 1000m;

    /// <summary>Gets the maximum discount amount, in UAH.</summary>
    public const decimal MaxDiscount = 150m;

    /// <summary>Обчислює знижку та обмежує її максимально допустимою сумою.</summary>
    /// <param name="amount">Невід'ємна сума замовлення у гривнях.</param>
    /// <param name="isRegular">Чи має слухач статус постійного клієнта.</param>
    /// <returns>Сума знижки, округлена до копійок.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Сума замовлення від'ємна.</exception>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Performance", "CA1822:Mark members as static", Justification = "Метод викликається як екземплярний, як у тестах")]
    public decimal DiscountFor(decimal amount, bool isRegular)
    {
        EnsureAmountIsValid(amount);
        decimal raw = amount * RateFor(amount, isRegular);
        return Math.Min(decimal.Round(raw, 2), MaxDiscount);
    }

    private static void EnsureAmountIsValid(decimal amount)
    {
        if (amount < 0m)
        {
            throw new ArgumentOutOfRangeException(
                nameof(amount), "Сума не може бути від’ємною.");
        }
    }

    private static decimal RateFor(
        decimal amount, bool isRegular)
    {
        decimal rate = isRegular ? RegularRate : 0m;
        if (amount >= BigOrderFrom)
        {
            rate += BigOrderRate;
        }

        return rate;
    }
}
