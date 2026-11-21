namespace OnlineCourses.Legacy;

using OnlineCourses.Core.Documents;
using OnlineCourses.Core.Domain;

/// <summary>Provides a read-only view of a registration retained in the archive.</summary>
public sealed class ArchivedRegistration
{
    private readonly Registration _registration;

    public ArchivedRegistration(Registration registration, DateOnly archivedAt)
    {
        ArgumentNullException.ThrowIfNull(registration);
        _registration = registration;
        ArchivedAt = archivedAt;
    }

    /// <summary>Gets the date on which the registration was archived.</summary>
    public DateOnly ArchivedAt { get; }

    /// <summary>Gets the identifier of the archived registration.</summary>
    public int Id => _registration.Id;

    /// <summary>Gets the archived document state.</summary>
    public DocumentStatus Status => _registration.Status;

    /// <summary>Gets the immutable list of archived registration lines.</summary>
    public IReadOnlyList<RegistrationItem> Lines => _registration.Lines;
}
