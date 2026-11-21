namespace OnlineCourses.Core.Services;

using Microsoft.Extensions.Logging;
using OnlineCourses.Core.Abstractions;
using OnlineCourses.Core.Documents;
using OnlineCourses.Core.Domain;
using OnlineCourses.Core.Errors;

/// <summary>Координує створення, зміну, розрахунок і експорт документів реєстрації.</summary>
public sealed class RegistrationService
{
    private readonly IRegistrationRepository _repository;
    private readonly IPricingPolicy _pricing;
    private readonly INotifier _notifier;
    private readonly ILogger<RegistrationService> _logger;

    /// <summary>Створює сервіс із репозиторієм, ціновою політикою, каналом повідомлень і журналом.</summary>
    /// <param name="repository">Сховище документів.</param>
    /// <param name="pricing">Політика розрахунку вартості позицій.</param>
    /// <param name="notifier">Канал надсилання повідомлень.</param>
    /// <param name="logger">Журнал подій сервісу.</param>
    /// <exception cref="ArgumentNullException">Одна із залежностей не передана.</exception>
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
    /// <summary>Підраховує рядки, що містять додатні цілі кількості.</summary>
    /// <param name="rows">Масив текстових значень для перевірки.</param>
    /// <returns>Кількість коректних додатних значень.</returns>
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

    /// <summary>Додає документ до сховища.</summary>
    /// <param name="registration">Документ для збереження.</param>
    /// <exception cref="ArgumentNullException">Документ не передано.</exception>
    public void Place(Registration registration)
    {
        ArgumentNullException.ThrowIfNull(registration);
        _repository.Add(registration);
    }

    /// <summary>Обчислює сумарну вартість усіх позицій документа.</summary>
    /// <param name="registration">Документ, для якого обчислюється сума.</param>
    /// <returns>Загальна вартість у гривнях.</returns>
    /// <exception cref="ArgumentNullException">Документ не передано.</exception>
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

    /// <summary>Додає позицію до нового документа та зберігає зміни.</summary>
    /// <param name="registrationId">Ідентифікатор документа.</param>
    /// <param name="line">Позиція, яку потрібно додати.</param>
    /// <exception cref="ArgumentNullException">Позицію не передано.</exception>
    /// <exception cref="DomainRuleException">Документ не знайдено або його стан не дозволяє редагування.</exception>
    public void AddLine(int registrationId, RegistrationItem line)
    {
        ArgumentNullException.ThrowIfNull(line);

        using var scope = _logger.BeginScope("Registration:{RegistrationId}", registrationId);
        _logger.LogDebug("Початок додавання позиції з SKU {Sku}", line.Sku);

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
    /// <summary>Завантажує документ за ідентифікатором.</summary>
    /// <param name="registrationId">Ідентифікатор документа.</param>
    /// <returns>Документ або null, якщо його не знайдено.</returns>
    public Registration? Load(int registrationId)
    {
        return _repository.GetById(registrationId);
    }

    // АНТИПАТЕРНИ 2 і 3: широке перехоплення, throw ex
    /// <summary>Скасовує документ, якщо його поточний стан дозволяє скасування.</summary>
    /// <param name="registrationId">Ідентифікатор документа.</param>
    /// <param name="reason">Причина скасування, яку надає викликач.</param>
    /// <exception cref="DomainRuleException">Документ не знайдено.</exception>
    /// <exception cref="IOException">Сховище не змогло зберегти зміни.</exception>
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
    /// <summary>Експортує текстовий звіт із номером документа та його сумою.</summary>
    /// <param name="path">Шлях до файлу звіту.</param>
    /// <param name="registration">Документ, який потрібно включити до звіту.</param>
    /// <exception cref="ArgumentNullException">Документ не передано.</exception>
    /// <exception cref="IOException">Не вдалося записати файл.</exception>
    /// <exception cref="UnauthorizedAccessException">Немає дозволу на запис за вказаним шляхом.</exception>
    public void ExportReport(string path, Registration registration)
    {
        using var writer = new StreamWriter(path);
        writer.WriteLine($"Замовлення {registration.Id}");
        writer.WriteLine(TotalOf(registration));
    }
}
