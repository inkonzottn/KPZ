namespace OnlineCourses.Core.Domain;

/// <summary>Represents the RegistrationItem type.</summary>
public sealed record RegistrationItem
{
/// <summary>Initializes a new instance of the <see cref="RegistrationItem"/> class.</summary>
    public RegistrationItem(string sku, int quantity, decimal unitPrice)
    {
        if (string.IsNullOrWhiteSpace(sku))
        {
            throw new ArgumentException("Порожній артикул", nameof(sku));
        }

        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(quantity, nameof(quantity));
        ArgumentOutOfRangeException.ThrowIfNegative(unitPrice, nameof(unitPrice));

        Sku = sku;
        Quantity = quantity;
        UnitPrice = unitPrice;
    }

/// <summary>Gets the Sku value.</summary>
    public string Sku { get; }

/// <summary>Gets the Quantity value.</summary>
    public int Quantity { get; }

/// <summary>Gets the UnitPrice value.</summary>
    public decimal UnitPrice { get; }

/// <summary>Defines the Amount member.</summary>
    public decimal Amount => UnitPrice * Quantity;
}
