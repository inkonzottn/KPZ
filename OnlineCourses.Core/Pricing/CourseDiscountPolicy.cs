namespace OnlineCourses.Core.Pricing;

public sealed class CourseDiscountPolicy
{
    // Знижка постійному студентові
    public const decimal RegularRate = 0.05m;

    // Додаткова знижка за велике замовлення
    public const decimal BigOrderRate = 0.10m;

    // Межа великого замовлення, грн
    public const decimal BigOrderFrom = 1000m;

    // Максимальна сума знижки, грн
    public const decimal MaxDiscount = 150m;

    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Performance", "CA1822:Mark members as static", Justification = "Метод викликається як екземплярний, як у тестах")]
    public decimal DiscountFor(decimal amount, bool isRegular)
    {
        if (amount < 0m)
        {
            throw new ArgumentOutOfRangeException(
                nameof(amount), "Сума не може бути від’ємною.");
        }

        decimal rate = 0m;
        if (isRegular)
        {
            rate += RegularRate;
        }

        if (amount >= BigOrderFrom)
        {
            rate += BigOrderRate;
        }

        decimal discount = decimal.Round(amount * rate, 2);
        if (discount > MaxDiscount)
        {
            discount = MaxDiscount;
        }

        return discount;
    }
}
