namespace OnlineCourses.Core.Domain;

using OnlineCourses.Core.Documents;
using OnlineCourses.Core.Errors;

/// <summary>Представляє документ реєстрації з позиціями та контрольованими переходами станів.</summary>
public sealed class Registration
{
    private readonly List<RegistrationItem> _lines = new();

    /// <summary>Initializes a new instance of the <see cref="Registration"/> class.</summary>
    /// <param name="id">Додатний ідентифікатор документа.</param>
    /// <param name="memberId">Додатний ідентифікатор слухача.</param>
    /// <param name="createdAt">Календарна дата створення.</param>
    /// <exception cref="ArgumentOutOfRangeException">Один з ідентифікаторів недодатний.</exception>
    public Registration(int id, int memberId, DateOnly createdAt)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(id);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(memberId);

        Id = id;
        MemberId = memberId;
        CreatedAt = createdAt;
        Status = DocumentStatus.New;
    }

    /// <summary>Gets the unique document identifier.</summary>
    public int Id { get; }

    /// <summary>Gets the identifier of the learner associated with this document.</summary>
    public int MemberId { get; }

    /// <summary>Gets or sets the calculated document total in UAH.</summary>
    public decimal Total { get; set; }

    /// <summary>Gets the date when the document was created.</summary>
    public DateOnly CreatedAt { get; }

    /// <summary>Gets the current lifecycle state of the document.</summary>
    public DocumentStatus Status { get; private set; }

    /// <summary>Gets the document positions in their insertion order.</summary>
    public IReadOnlyList<RegistrationItem> Lines => _lines;

    /// <summary>Adds a position while the document is new.</summary>
    /// <param name="line">Non-null position to append.</param>
    /// <exception cref="ArgumentNullException">The position is null.</exception>
    /// <exception cref="InvalidOperationException">The document is not new.</exception>
    public void AddLine(RegistrationItem line)
    {
        ArgumentNullException.ThrowIfNull(line);

        if (Status != DocumentStatus.New)
        {
            throw new InvalidOperationException("Позиції можна додавати лише до нового документа.");
        }

        _lines.Add(line);
    }

    /// <summary>Confirms a non-empty new document.</summary>
    /// <exception cref="InvalidOperationException">The document is not new or has no positions.</exception>
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

    /// <summary>Marks a confirmed document as paid.</summary>
    /// <exception cref="DomainRuleException">The document is not confirmed.</exception>
    public void Pay()
    {
        if (Status != DocumentStatus.Confirmed)
        {
            throw new DomainRuleException("document.status", "Перехід до стану Paid можливий лише з підтвердженого стану.");
        }

        Status = DocumentStatus.Paid;
    }

    /// <summary>Activates a paid document.</summary>
    /// <exception cref="InvalidOperationException">The document is not paid.</exception>
    public void Activate()
    {
        if (Status != DocumentStatus.Paid)
        {
            throw new InvalidOperationException("Перехід до стану Active можливий лише після оплати.");
        }

        Status = DocumentStatus.Active;
    }

    /// <summary>Marks an active document as refunded.</summary>
    /// <exception cref="InvalidOperationException">The document is not active.</exception>
    public void Refund()
    {
        if (Status != DocumentStatus.Active)
        {
            throw new InvalidOperationException("Повернення коштів можливе лише для активного курсу.");
        }

        Status = DocumentStatus.Refunded;
    }

    /// <summary>Cancels a new or confirmed document.</summary>
    /// <exception cref="InvalidOperationException">The document is in a state that cannot be cancelled.</exception>
    public void Cancel()
    {
        if (Status != DocumentStatus.New && Status != DocumentStatus.Confirmed)
        {
            throw new InvalidOperationException("Скасування документа у поточному стані неможливе.");
        }

        Status = DocumentStatus.Cancelled;
    }
}
