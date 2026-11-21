namespace OnlineCourses.Core.Documents;

/// <summary>Містить числові межі та ставки цінової політики навчального документа.</summary>
public static class PricingRules
{
    /// <summary>Мінімальна кількість одиниць для кількісної знижки.</summary>
    public const int BulkQuantity = 10;

    /// <summary>Ставка кількісної знижки.</summary>
    public const decimal BulkRate = 0.05m;

    /// <summary>Ставка знижки постійного клієнта.</summary>
    public const decimal RegularClientRate = 0.07m;

    /// <summary>Поріг великого замовлення.</summary>
    public const decimal LargeOrderFrom = 5000m;

    /// <summary>Ставка знижки для великого замовлення.</summary>
    public const decimal LargeOrderRate = 0.10m;

    /// <summary>Поріг середнього замовлення.</summary>
    public const decimal MediumOrderFrom = 2000m;

    /// <summary>Ставка знижки для середнього замовлення.</summary>
    public const decimal MediumOrderRate = 0.05m;

    /// <summary>Ставка знижки для замовлення, створеного в неділю.</summary>
    public const decimal WeekendRate = 0.02m;

    /// <summary>Відсоткова ставка купона SALE10.</summary>
    public const decimal CouponSaleRate = 0.10m;

    /// <summary>Фіксована сума знижки купона MINUS200.</summary>
    public const decimal CouponFixed = 200m;

    /// <summary>Мінімальна сума для застосування купона MINUS200.</summary>
    public const decimal CouponFixedFrom = 1000m;

    /// <summary>Поріг безкоштовної доставки.</summary>
    public const decimal FreeDeliveryFrom = 1000m;

    /// <summary>Код купона для відсоткової знижки.</summary>
    public const string SaleCoupon = "SALE10";

    /// <summary>Код купона для фіксованої знижки.</summary>
    public const string FixedCoupon = "MINUS200";
}
