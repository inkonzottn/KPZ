namespace OnlineCourses.Core.Documents;

/// <summary>Represents the PricingRules type.</summary>
public static class PricingRules
{
    /// <summary>Defines the BulkQuantity member.</summary>
    public const int BulkQuantity = 10;
    /// <summary>Defines the BulkRate member.</summary>
    public const decimal BulkRate = 0.05m;
    /// <summary>Defines the RegularClientRate member.</summary>
    public const decimal RegularClientRate = 0.07m;
    /// <summary>Defines the LargeOrderFrom member.</summary>
    public const decimal LargeOrderFrom = 5000m;
    /// <summary>Defines the LargeOrderRate member.</summary>
    public const decimal LargeOrderRate = 0.10m;
    /// <summary>Defines the MediumOrderFrom member.</summary>
    public const decimal MediumOrderFrom = 2000m;
    /// <summary>Defines the MediumOrderRate member.</summary>
    public const decimal MediumOrderRate = 0.05m;
    /// <summary>Defines the WeekendRate member.</summary>
    public const decimal WeekendRate = 0.02m;
    /// <summary>Defines the CouponSaleRate member.</summary>
    public const decimal CouponSaleRate = 0.10m;
    /// <summary>Defines the CouponFixed member.</summary>
    public const decimal CouponFixed = 200m;
    /// <summary>Defines the CouponFixedFrom member.</summary>
    public const decimal CouponFixedFrom = 1000m;
    /// <summary>Defines the FreeDeliveryFrom member.</summary>
    public const decimal FreeDeliveryFrom = 1000m;
    /// <summary>Defines the SaleCoupon member.</summary>
    public const string SaleCoupon = "SALE10";
    /// <summary>Defines the FixedCoupon member.</summary>
    public const string FixedCoupon = "MINUS200";
}
