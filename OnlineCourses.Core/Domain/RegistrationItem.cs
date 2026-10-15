namespace OnlineCourses.Core.Domain;

public sealed record RegistrationItem
{
    public RegistrationItem(string sku, int quantity, decimal unitPrice)
    {
        if (string.IsNullOrWhiteSpace(sku))
        {
            throw new ArgumentException("Порожній артикул", nameof(sku));
        }

        if (quantity <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(quantity));
        }

        if (unitPrice < 0m)
        {
            throw new ArgumentOutOfRangeException(nameof(unitPrice));
        }

        Sku = sku;
        Quantity = quantity;
        UnitPrice = unitPrice;
    }

    public string Sku { get; }
    public int Quantity { get; }
    public decimal UnitPrice { get; }
    public decimal Amount => UnitPrice * Quantity;
}