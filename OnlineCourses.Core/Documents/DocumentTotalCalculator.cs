namespace OnlineCourses.Core.Documents;

using OnlineCourses.Core.Legacy;

/// <summary>Represents the DocumentTotalCalculator type.</summary>
public static class DocumentTotalCalculator
{
/// <summary>Performs the Calculate operation.</summary>
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
            : Math.Round(Math.Max(total, 0m), 2);

        return new TotalResult(
            total,
            NextStatusOf(request.Status, total));
    }

/// <summary>Performs the SubtotalOf operation.</summary>
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

/// <summary>Performs the ClientDiscountOf operation.</summary>
    public static decimal ClientDiscountOf(
    decimal amount, bool isRegularClient)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(amount);
        return isRegularClient
        ? amount * PricingRules.RegularClientRate
        : 0m;
    }

/// <summary>Performs the VolumeDiscountOf operation.</summary>
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

/// <summary>Performs the CouponDiscountOf operation.</summary>
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

/// <summary>Performs the WeekendDiscountOf operation.</summary>
    public static decimal WeekendDiscountOf(
    decimal amount, DateOnly createdAt)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(amount);
        return createdAt.DayOfWeek == DayOfWeek.Sunday
        ? amount * PricingRules.WeekendRate
        : 0m;
    }

/// <summary>Performs the DeliveryFeeOf operation.</summary>
    public static decimal DeliveryFeeOf(
    decimal amount, decimal deliveryPrice)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(
        deliveryPrice);
        return amount < PricingRules.FreeDeliveryFrom
        ? deliveryPrice
        : 0m;
    }

/// <summary>Performs the NextStatusOf operation.</summary>
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
