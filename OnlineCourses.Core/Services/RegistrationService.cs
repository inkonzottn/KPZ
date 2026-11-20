namespace OnlineCourses.Core.Services;

using Microsoft.Extensions.Logging;
using OnlineCourses.Core.Abstractions;
using OnlineCourses.Core.Documents;
using OnlineCourses.Core.Domain;
using OnlineCourses.Core.Errors;

/// <summary>Represents the RegistrationService type.</summary>
public sealed class RegistrationService
{
    private readonly IRegistrationRepository _repository;
    private readonly IPricingPolicy _pricing;
    private readonly INotifier _notifier;
    private readonly ILogger<RegistrationService> _logger;

    /// <summary>Initializes a new instance of the <see cref="RegistrationService"/> class.</summary>
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
    /// <summary>Performs the CountValidQuantities operation.</summary>
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

    /// <summary>Performs the Place operation.</summary>
    public void Place(Registration registration)
    {
        ArgumentNullException.ThrowIfNull(registration);
        _repository.Add(registration);
    }

    /// <summary>Performs the TotalOf operation.</summary>
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

    /// <summary>Performs the AddLine operation.</summary>
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
    /// <summary>Performs the Load operation.</summary>
    public Registration? Load(int registrationId)
    {
        return _repository.GetById(registrationId);
    }

    // АНТИПАТЕРНИ 2 і 3: широке перехоплення, throw ex
    /// <summary>Performs the Cancel operation.</summary>
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

    // СТАЛО: using звільняє потік навіть при винятку
    /// <summary>Performs the ExportReport operation.</summary>
    public void ExportReport(string path, Registration registration)
    {
        using var writer = new StreamWriter(path);
        writer.WriteLine($"Замовлення {registration.Id}");
        writer.WriteLine(TotalOf(registration));
    }
}
