using System.Text;
using Microsoft.Extensions.Logging;
using OnlineCourses.App;
using OnlineCourses.Core.Abstractions;
using OnlineCourses.Core.Documents;
using OnlineCourses.Core.Domain;
using OnlineCourses.Core.Legacy;
using OnlineCourses.Core.Services;

Console.OutputEncoding = Encoding.UTF8;

using var factory = LoggerFactory.Create(builder =>
{
    builder.AddSimpleConsole(options =>
    {
        options.TimestampFormat = "HH:mm:ss ";
        options.IncludeScopes = true;
    });
    builder.SetMinimumLevel(LogLevel.Debug);
});

var logger = factory.CreateLogger<RegistrationService>();

var repository = new InMemoryRegistrationRepository();
IPricingPolicy pricing = new DiscountPricingPolicy(0.05m);
var notifier = new SmtpMailer();

var service = new RegistrationService(repository, pricing, notifier, logger);

int registrationId = 1;
var registration = new Registration(registrationId, 100, DateOnly.FromDateTime(DateTime.Today));
repository.Add(registration);

while (true)
{
    Console.Write("SKU: ");
    string sku = Console.ReadLine() ?? string.Empty;
    Console.Write("Кількість: ");
    string qty = Console.ReadLine() ?? string.Empty;
    Console.Write("Ціна: ");
    string price = Console.ReadLine() ?? string.Empty;

    var parsed = RegistrationItemParser.Parse(sku, qty, price);
    if (!parsed.IsSuccess)
    {
        Console.WriteLine($"Помилка вводу: {parsed.Error}");
        continue;
    }

    try
    {
        service.AddLine(registrationId, parsed.Value!);
        Console.WriteLine("Позицію успішно додано!");
        break;
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Помилка домену: {ex.Message}");
    }
}
