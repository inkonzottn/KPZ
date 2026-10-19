namespace OnlineCourses.Core.Domain;

using OnlineCourses.Core.Documents;
using OnlineCourses.Core.Errors;

public sealed class Registration
{
    private readonly List<RegistrationItem> _lines = new();

    public Registration(int id, int memberId, DateOnly createdAt)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(id);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(memberId);

        Id = id;
        MemberId = memberId;
        CreatedAt = createdAt;
        Status = DocumentStatus.New;
    }

    public int Id { get; }

    public int MemberId { get; }

    public decimal Total { get; set; }

    public DateOnly CreatedAt { get; }

    public DocumentStatus Status { get; private set; }

    public IReadOnlyList<RegistrationItem> Lines => _lines;

    public void AddLine(RegistrationItem line)
    {
        ArgumentNullException.ThrowIfNull(line);

        if (Status != DocumentStatus.New)
        {
            throw new InvalidOperationException("Позиції можна додавати лише до нового документа.");
        }

        _lines.Add(line);
    }

    public void Confirm()
    {
        if (Status != DocumentStatus.New)
        {
            throw new InvalidOperationException("Перехід до стану Confirmed можливий лише з нового стану.");
        }

        if (_lines.Count == 0)
        {
            throw new InvalidOperationException("Документ не містить жодної позиції.");
        }

        Status = DocumentStatus.Confirmed;
    }

    public void Pay()
    {
        if (Status != DocumentStatus.Confirmed)
        {
            throw new DomainRuleException("document.status", "Перехід до стану Paid можливий лише з підтвердженого стану.");
        }

        Status = DocumentStatus.Paid;
    }

    public void Activate()
    {
        if (Status != DocumentStatus.Paid)
        {
            throw new InvalidOperationException("Перехід до стану Active можливий лише після оплати.");
        }

        Status = DocumentStatus.Active;
    }

    public void Refund()
    {
        if (Status != DocumentStatus.Active)
        {
            throw new InvalidOperationException("Повернення коштів можливе лише для активного курсу.");
        }

        Status = DocumentStatus.Refunded;
    }

    public void Cancel()
    {
        if (Status != DocumentStatus.New && Status != DocumentStatus.Confirmed)
        {
            throw new InvalidOperationException("Скасування документа у поточному стані неможливе.");
        }

        Status = DocumentStatus.Cancelled;
    }
}
