namespace OnlineCourses.Legacy;

/// <summary>Represents one product line in the legacy registration model.</summary>
public class LegacyItem
{
    /// <summary>Gets or sets the stock keeping unit.</summary>
    public string Sku { get; set; } = string.Empty;

    /// <summary>Gets or sets the number of units in the line.</summary>
    public int Quantity { get; set; }

    /// <summary>Gets or sets the price of one unit.</summary>
    public decimal UnitPrice { get; set; }
}
