namespace OnlineCourses.Core.Legacy;

/// <summary>Represents the RegistrationItem type.</summary>
public sealed class RegistrationItem
{
/// <summary>Gets the Sku value.</summary>
    public string Sku { get; set; } = string.Empty;

/// <summary>Gets the Quantity value.</summary>
    public int Quantity { get; set; }

/// <summary>Gets the UnitPrice value.</summary>
    public decimal UnitPrice { get; set; }
}
