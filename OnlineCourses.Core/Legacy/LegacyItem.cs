namespace OnlineCourses.Legacy;

/// <summary>Represents the LegacyItem type.</summary>
public class LegacyItem
{
/// <summary>Gets the Sku value.</summary>
    public string Sku { get; set; } = string.Empty;

/// <summary>Gets the Quantity value.</summary>
    public int Quantity { get; set; }

/// <summary>Gets the UnitPrice value.</summary>
    public decimal UnitPrice { get; set; }
}
