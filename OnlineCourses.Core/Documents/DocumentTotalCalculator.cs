namespace OnlineCourses.Core.Documents;

using OnlineCourses.Core.Legacy;

public static class DocumentTotalCalculator
{
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

    public static decimal ClientDiscountOf(
    decimal amount, bool isRegularClient)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(amount);
        return isRegularClient
        ? amount * PricingRules.RegularClientRate
        : 0m;
    }

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

    public static decimal WeekendDiscountOf(
    decimal amount, DateOnly createdAt)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(amount);
        return createdAt.DayOfWeek == DayOfWeek.Sunday
        ? amount * PricingRules.WeekendRate
        : 0m;
    }

    public static decimal DeliveryFeeOf(
    decimal amount, decimal deliveryPrice)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(
        deliveryPrice);
        return amount < PricingRules.FreeDeliveryFrom
        ? deliveryPrice
        : 0m;
    }

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
