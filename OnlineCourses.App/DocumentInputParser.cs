namespace OnlineCourses.App;

using OnlineCourses.Core.Documents;
using OnlineCourses.Core.Legacy;

public static class DocumentInputParser
{
    public static TotalRequest BuildRequestFromInput()
    {
        Console.Write("Введіть ID документа: ");
        int id = int.TryParse(Console.ReadLine(), out var parsedId) ? parsedId : 1;

        Console.Write("Ім'я клієнта: ");
        string name = Console.ReadLine() ?? "Іваненко";

        Console.Write("Email: ");
        string email = Console.ReadLine() ?? "i@ex.com";

        Console.Write("Чи є клієнт постійним (y/n): ");
        bool isRegular = Console.ReadLine()?.Trim().ToLowerInvariant() == "y";

        return new TotalRequest(
            DocumentId: id,
            Client: new ClientInfo(name, email, isRegular),
            Lines: new List<RegistrationItem>
            {
                new() { Sku = "A1", Quantity = 12, UnitPrice = 100m },
                new() { Sku = "B2", Quantity = 1, UnitPrice = 250m }
            },
            CreatedAt: DateOnly.FromDateTime(DateTime.Today),
            CouponCode: null,
            Status: DocumentStatus.New,
            DeliveryPrice: 60m);
    }
}
