namespace OnlineCourses.Core.Domain;

/// <summary>Містить артикул, кількість і ціну одиниці в позиції документа.</summary>
public sealed record RegistrationItem
{
    /// <summary>Створює позицію документа з перевіреними значеннями.</summary>
    /// <param name="sku">Непорожній артикул товару або послуги.</param>
    /// <param name="quantity">Додатна кількість одиниць.</param>
    /// <param name="unitPrice">Невід'ємна ціна однієї одиниці у гривнях.</param>
    /// <exception cref="ArgumentException">Артикул порожній.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Кількість недодатна або ціна від'ємна.</exception>
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

    /// <summary>Gets the stock-keeping unit code.</summary>
    public string Sku { get; }

    /// <summary>Gets the number of units in this position.</summary>
    public int Quantity { get; }

    /// <summary>Gets the price of one unit in UAH.</summary>
    public decimal UnitPrice { get; }

    /// <summary>Gets the total amount for this position in UAH.</summary>
    public decimal Amount => UnitPrice * Quantity;
}
