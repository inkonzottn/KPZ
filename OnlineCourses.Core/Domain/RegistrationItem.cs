/// <summary>Позиція реєстрації: обраний курс, його код, тип, ціна.</summary>
public sealed class RegistrationItem
{
    public string Code { get; init; } = string.Empty;
    public decimal Price { get; init; }
    public decimal Amount => Price;
}