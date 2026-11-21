namespace OnlineCourses.Core.Documents;

using OnlineCourses.Core.Legacy;

/// <summary>Обчислює підсумок документа з урахуванням знижок і доставки.</summary>
public static class DocumentTotalCalculator
{
    private const int MoneyDecimalPlaces = 2;
    /// <summary>Перевіряє запит і повертає підсумкову суму та наступний стан документа.</summary>
    /// <param name="request">Дані документа, клієнта, позицій і знижок.</param>
    /// <returns>Розрахована сума та стан документа після розрахунку.</returns>
    /// <exception cref="ArgumentException">Дані запиту некоректні.</exception>
    public static TotalResult Calculate(TotalRequest request)
    {
        DocumentGuards.EnsureRequestValid(request);
        decimal total = SubtotalOf(request.Lines);
        total -= ClientDiscountOf(total, request.Client.IsRegular);
        total -= VolumeDiscountOf(total);
        total -= CouponDiscountOf(total, request.CouponCode);
        total -= WeekendDiscountOf(total, request.CreatedAt);
        total += DeliveryFeeOf(total, request.DeliveryPrice);

        total = request.Status == DocumentStatus.Cancelled
            ? 0m
            : Math.Round(Math.Max(total, 0m), MoneyDecimalPlaces);

        return new TotalResult(
            total,
            NextStatusOf(request.Status, total));
    }

    /// <summary>Підсумовує вартість позицій із кількісною знижкою.</summary>
    /// <param name="lines">Непорожній список позицій документа.</param>
    /// <returns>Сума позицій до знижок рівня документа.</returns>
    /// <exception cref="ArgumentException">Список позицій некоректний.</exception>
    public static decimal SubtotalOf(
    IReadOnlyList<RegistrationItem> lines)
    {
        DocumentGuards.EnsureLinesValid((IReadOnlyList<Legacy.RegistrationItem>)lines);
        decimal subtotal = 0m;
        foreach (RegistrationItem line in lines)
        {
            decimal sum = line.Quantity * line.UnitPrice;
            if (line.Quantity >= PricingRules.BulkQuantity)
            {
                sum -= sum * PricingRules.BulkRate;
            }

            subtotal += sum;
        }

        return subtotal;
    }

    /// <summary>Обчислює знижку постійного клієнта.</summary>
    /// <param name="amount">База для розрахунку знижки.</param>
    /// <param name="isRegularClient">Чи має клієнт статус постійного.</param>
    /// <returns>Сума знижки.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Сума від'ємна.</exception>
    public static decimal ClientDiscountOf(
    decimal amount, bool isRegularClient)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(amount);
        return isRegularClient
        ? amount * PricingRules.RegularClientRate
        : 0m;
    }

    /// <summary>Обчислює знижку залежно від суми замовлення.</summary>
    /// <param name="amount">Сума до застосування об'ємної знижки.</param>
    /// <returns>Сума об'ємної знижки.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Сума від'ємна.</exception>
    public static decimal VolumeDiscountOf(decimal amount)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(amount);
        if (amount > PricingRules.LargeOrderFrom)
        {
            return amount * PricingRules.LargeOrderRate;
        }

        if (amount > PricingRules.MediumOrderFrom)
        {
            return amount * PricingRules.MediumOrderRate;
        }

        return 0m;
    }

    /// <summary>Обчислює знижку за відомим промокодом.</summary>
    /// <param name="amount">Сума до застосування промокоду.</param>
    /// <param name="couponCode">Код купона або null.</param>
    /// <returns>Сума купонної знижки.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Сума від'ємна.</exception>
    public static decimal CouponDiscountOf(
    decimal amount, string? couponCode)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(amount);
        return couponCode switch
        {
            PricingRules.SaleCoupon
            => amount * PricingRules.CouponSaleRate,
            PricingRules.FixedCoupon
            when amount > PricingRules.CouponFixedFrom
            => PricingRules.CouponFixed,
            _ => 0m
        };
    }

    /// <summary>Обчислює знижку, що діє для документа, створеного в неділю.</summary>
    /// <param name="amount">Сума до застосування знижки.</param>
    /// <param name="createdAt">Дата створення документа.</param>
    /// <returns>Сума знижки вихідного дня.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Сума від'ємна.</exception>
    public static decimal WeekendDiscountOf(
    decimal amount, DateOnly createdAt)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(amount);
        return createdAt.DayOfWeek == DayOfWeek.Sunday
        ? amount * PricingRules.WeekendRate
        : 0m;
    }

    /// <summary>Визначає вартість доставки з урахуванням порогу безкоштовної доставки.</summary>
    /// <param name="amount">Сума замовлення до доставки.</param>
    /// <param name="deliveryPrice">Базова вартість доставки.</param>
    /// <returns>Вартість доставки для цього замовлення.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Вартість доставки від'ємна.</exception>
    public static decimal DeliveryFeeOf(
    decimal amount, decimal deliveryPrice)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(
        deliveryPrice);
        return amount < PricingRules.FreeDeliveryFrom
        ? deliveryPrice
        : 0m;
    }

    /// <summary>Визначає наступний стан документа за поточним станом і сумою.</summary>
    /// <param name="current">Поточний стан документа.</param>
    /// <param name="total">Розрахована сума.</param>
    /// <returns>Наступний стан відповідно до правил життєвого циклу.</returns>
    public static DocumentStatus NextStatusOf(
    DocumentStatus current, decimal total)
    => current switch
    {
        DocumentStatus.New when total > 0m
    => DocumentStatus.Paid,
        DocumentStatus.Paid => DocumentStatus.Confirmed,
        _ => current
    };
}
