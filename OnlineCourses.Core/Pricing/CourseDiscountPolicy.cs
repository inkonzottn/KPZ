namespace OnlineCourses.Core.Pricing;

/// <summary>Represents the CourseDiscountPolicy type.</summary>
public sealed class CourseDiscountPolicy
{
    // Знижка постійному студентові
/// <summary>Defines the RegularRate member.</summary>
    public const decimal RegularRate = 0.05m;

    // Додаткова знижка за велике замовлення
/// <summary>Defines the BigOrderRate member.</summary>
    public const decimal BigOrderRate = 0.10m;

    // Межа великого замовлення, грн
/// <summary>Defines the BigOrderFrom member.</summary>
    public const decimal BigOrderFrom = 1000m;

    // Максимальна сума знижки, грн
/// <summary>Defines the MaxDiscount member.</summary>
    public const decimal MaxDiscount = 150m;

    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Performance", "CA1822:Mark members as static", Justification = "Метод викликається як екземплярний, як у тестах")]
/// <summary>Performs the DiscountFor operation.</summary>
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
