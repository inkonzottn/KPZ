namespace OnlineCourses.Core.Legacy;

/// <summary>Represents a product line used by the legacy registration workflow.</summary>
public sealed class RegistrationItem
{
    /// <summary>Gets or sets the product stock keeping unit.</summary>
    public string Sku { get; set; } = string.Empty;

    /// <summary>Gets or sets the number of units.</summary>
    public int Quantity { get; set; }

    /// <summary>Gets or sets the price of one unit.</summary>
    public decimal UnitPrice { get; set; }
}
