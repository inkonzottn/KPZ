namespace OnlineCourses.Core.Services;

using OnlineCourses.Core.Abstractions;
using OnlineCourses.Core.Documents;
using OnlineCourses.Core.Domain;
using OnlineCourses.Core.Errors;

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

    public void AddLine(int registrationId, RegistrationItem line)
    {
        ArgumentNullException.ThrowIfNull(line);

        var registration = _repository.GetById(registrationId)
            ?? throw new DomainRuleException("registration.exists", $"Реєстрацію з ID {registrationId} не знайдено.");

        if (registration.Status != DocumentStatus.New)
        {
            throw new DomainRuleException("registration.editable", "Додавати позиції можна лише до нового документа.");
        }

        registration.AddLine(line);

        _repository.Save(registration);
    }

    // АНТИПАТЕРН 1: порожній catch, дефект зникає
    public Registration? Load(int registrationId)
    {
        try
        {
            return _repo.GetById(registrationId);
        }
        catch (Exception)
        {
        }
        return null;
    }

    // АНТИПАТЕРНИ 2 і 3: широке перехоплення, throw ex
    public void Cancel(int registrationId, string reason)
    {
        try
        {
            var registration = _repo.GetById(registrationId);
            registration.Status = DocumentStatus.Cancelled;
            _repo.Save(registration);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Щось пішло не так");
            throw ex;
        }
    }

    // АНТИПАТЕРН 4: try/catch замість звичайної умови
    public int CountValidQuantities(string[] rows)
    {
        var valid = 0;
        foreach (var row in rows)
        {
            try
            {
                var q = int.Parse(row);
                if (q > 0)
                {
                    valid++;
                }
            }
            catch (FormatException)
            {
                // не число — пропускаємо
            }
        }
        return valid;
    }
}
