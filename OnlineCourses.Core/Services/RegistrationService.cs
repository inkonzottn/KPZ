namespace OnlineCourses.Core.Services;

using OnlineCourses.Core.Abstractions;
using OnlineCourses.Core.Domain;

public sealed class RegistrationService
{
    private readonly IRegistrationRepository _repository;
    private readonly IPricingPolicy _pricing;
    private readonly INotifier _notifier;

    public RegistrationService(
        IRegistrationRepository repository,
        IPricingPolicy pricing,
        INotifier notifier)
    {
        _repository = repository
            ?? throw new ArgumentNullException(nameof(repository));
        _pricing = pricing
            ?? throw new ArgumentNullException(nameof(pricing));
        _notifier = notifier
            ?? throw new ArgumentNullException(nameof(notifier));
    }

    public void Place(Registration registration)
    {
        ArgumentNullException.ThrowIfNull(registration);
        _repository.Add(registration);
    }

    public decimal TotalOf(Registration registration)
    {
        ArgumentNullException.ThrowIfNull(registration);
        decimal total = 0m;
        foreach (var item in registration.Lines)
        {
            total += _pricing.PriceOf(item);
        }

        return total;
    }

    // ПОГАНИЙ ВАРІАНТ (Антипатерн Anemic Domain Model + потік даних без валідації)
    public void AddLineUnsafe(int registrationId, string courseName, string quantityStr, string priceStr)
    {
        var registration = _repository.GetById(registrationId);
        
        var quantity = int.Parse(quantityStr);
        var price = decimal.Parse(priceStr);
        
        registration.Items.Add(new RegistrationItem(courseName, quantity, price));
        
        _repository.Save(registration);
    }
}
