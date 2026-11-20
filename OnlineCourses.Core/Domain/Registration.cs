namespace OnlineCourses.Core.Domain;

using OnlineCourses.Core.Documents;
using OnlineCourses.Core.Errors;

/// <summary>Represents the Registration type.</summary>
public sealed class Registration
{
    private readonly List<RegistrationItem> _lines = new();

/// <summary>Initializes a new instance of the <see cref="Registration"/> class.</summary>
    public Registration(int id, int memberId, DateOnly createdAt)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(id);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(memberId);

        Id = id;
        MemberId = memberId;
        CreatedAt = createdAt;
        Status = DocumentStatus.New;
    }

/// <summary>Gets the Id value.</summary>
    public int Id { get; }

/// <summary>Gets the MemberId value.</summary>
    public int MemberId { get; }

/// <summary>Gets the Total value.</summary>
    public decimal Total { get; set; }

/// <summary>Gets the CreatedAt value.</summary>
    public DateOnly CreatedAt { get; }

/// <summary>Gets the Status value.</summary>
    public DocumentStatus Status { get; private set; }

/// <summary>Defines the Lines member.</summary>
    public IReadOnlyList<RegistrationItem> Lines => _lines;

/// <summary>Performs the AddLine operation.</summary>
    public void AddLine(RegistrationItem line)
    {
        ArgumentNullException.ThrowIfNull(line);

        if (Status != DocumentStatus.New)
        {
            throw new InvalidOperationException("Позиції можна додавати лише до нового документа.");
        }

        _lines.Add(line);
    }

/// <summary>Performs the Confirm operation.</summary>
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

/// <summary>Performs the Pay operation.</summary>
    public void Pay()
    {
        if (Status != DocumentStatus.Confirmed)
        {
            throw new DomainRuleException("document.status", "Перехід до стану Paid можливий лише з підтвердженого стану.");
        }

        Status = DocumentStatus.Paid;
    }

/// <summary>Performs the Activate operation.</summary>
    public void Activate()
    {
        if (Status != DocumentStatus.Paid)
        {
            throw new InvalidOperationException("Перехід до стану Active можливий лише після оплати.");
        }

        Status = DocumentStatus.Active;
    }

/// <summary>Performs the Refund operation.</summary>
    public void Refund()
    {
        if (Status != DocumentStatus.Active)
        {
            throw new InvalidOperationException("Повернення коштів можливе лише для активного курсу.");
        }

        Status = DocumentStatus.Refunded;
    }

/// <summary>Performs the Cancel operation.</summary>
    public void Cancel()
    {
        if (Status != DocumentStatus.New && Status != DocumentStatus.Confirmed)
        {
            throw new InvalidOperationException("Скасування документа у поточному стані неможливе.");
        }

        Status = DocumentStatus.Cancelled;
    }
}
