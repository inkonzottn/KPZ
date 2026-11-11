namespace OnlineCourses.Core.Pricing;

public sealed class CourseDiscountPolicy
{
    // Знижка постійному студентові
    public const decimal RegularRate = 0.05m;
    public const decimal BigOrderRate = 0.10m;
    public const decimal BigOrderFrom = 1000m;

    public decimal DiscountFor(decimal amount, bool isRegular)
    {
        if (amount < 0m)
        {
            throw new ArgumentOutOfRangeException(nameof(amount), "Сума не може бути від’ємною.");
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

        return decimal.Round(amount * rate, 2);
    }
}
