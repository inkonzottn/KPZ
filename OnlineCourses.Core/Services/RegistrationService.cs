namespace OnlineCourses.Core.Services;

using Microsoft.Extensions.Logging;
using OnlineCourses.Core.Abstractions;
using OnlineCourses.Core.Documents;
using OnlineCourses.Core.Domain;
using OnlineCourses.Core.Errors;

public sealed class RegistrationService
{
    private readonly IRegistrationRepository _repository;
    private readonly IPricingPolicy _pricing;
    private readonly INotifier _notifier;
    private readonly ILogger<RegistrationService> _logger;

    public RegistrationService(
        IRegistrationRepository repository,
        IPricingPolicy pricing,
        INotifier notifier,
        ILogger<RegistrationService> logger)
    {
        _repository = repository
            ?? throw new ArgumentNullException(nameof(repository));
        _pricing = pricing
            ?? throw new ArgumentNullException(nameof(pricing));
        _notifier = notifier
            ?? throw new ArgumentNullException(nameof(notifier));
        _logger = logger
            ?? throw new ArgumentNullException(nameof(logger));
    }

    // АНТИПАТЕРН 4: try/catch замість звичайної умови
    public static int CountValidQuantities(string[] rows)
    {
        var valid = 0;
        foreach (var row in rows)
        {
            if (int.TryParse(row, out var q) && q > 0)
            {
                valid++;
            }
        }

        return valid;
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

    public void AddLine(int registrationId, RegistrationItem line)
    {
        using var scope = _logger.BeginScope("Registration:{RegistrationId}", registrationId);
        _logger.LogDebug("Початок додавання позиції з SKU {Sku}", line.Sku);

        ArgumentNullException.ThrowIfNull(line);

        var registration = _repository.GetById(registrationId)
            ?? throw new DomainRuleException("registration.exists", $"Реєстрацію з ID {registrationId} не знайдено.");

        if (registration.Status != DocumentStatus.New)
        {
            throw new DomainRuleException("registration.editable", "Додавати позиції можна лише до нового документа.");
        }

        if (line.Quantity > 10)
        {
            _logger.LogWarning("Велика кількість позицій у замовленні: {Quantity}", line.Quantity);
        }

        registration.AddLine(line);
        _repository.Save(registration);

        _logger.LogInformation("Успішно додано позицію SKU {Sku} у реєстрацію {RegistrationId}", line.Sku, registrationId);
    }

    // АНТИПАТЕРН 1: порожній catch, дефект зникає
    public Registration? Load(int registrationId)
    {
        return _repository.GetById(registrationId);
    }

    // АНТИПАТЕРНИ 2 і 3: широке перехоплення, throw ex
    public void Cancel(int registrationId, string reason)
    {
        try
        {
            var registration = _repository.GetById(registrationId)
            ?? throw new DomainRuleException("registration.exists", $"Реєстрацію {registrationId} не знайдено");
            registration.Cancel();
            _repository.Save(registration);
        }
        catch (IOException)
        {
            throw;
        }
    }
}
