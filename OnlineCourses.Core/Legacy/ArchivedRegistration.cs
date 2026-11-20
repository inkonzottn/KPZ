namespace OnlineCourses.Legacy;

using OnlineCourses.Core.Documents;
using OnlineCourses.Core.Domain;

/// <summary>Represents the ArchivedRegistration type.</summary>
public sealed class ArchivedRegistration
{
    private readonly Registration _registration;

/// <summary>Initializes a new instance of the <see cref="ArchivedRegistration"/> class.</summary>
    public ArchivedRegistration(Registration registration, DateOnly archivedAt)
    {
        ArgumentNullException.ThrowIfNull(registration);
        _registration = registration;
        ArchivedAt = archivedAt;
    }

/// <summary>Gets the ArchivedAt value.</summary>
    public DateOnly ArchivedAt { get; }

/// <summary>Defines the Id member.</summary>
    public int Id => _registration.Id;

/// <summary>Defines the Status member.</summary>
    public DocumentStatus Status => _registration.Status;

/// <summary>Defines the Lines member.</summary>
    public IReadOnlyList<RegistrationItem> Lines => _registration.Lines;
}
