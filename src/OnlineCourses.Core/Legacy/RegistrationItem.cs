namespace OnlineCourses.Core.Legacy;

public sealed class RegistrationItem
{
    public string Sku { get; set; } = string.Empty;

    public int Quantity { get; set; }

    public decimal UnitPrice { get; set; }
}
