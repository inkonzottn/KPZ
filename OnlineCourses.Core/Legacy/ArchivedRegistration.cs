namespace OnlineCourses.Legacy;

using OnlineCourses.Core.Documents;
using OnlineCourses.Core.Domain;

public sealed class ArchivedRegistration
{
    private readonly Registration _registration;

    public ArchivedRegistration(Registration registration, DateOnly archivedAt)
    {
        ArgumentNullException.ThrowIfNull(registration);
        _registration = registration;
        ArchivedAt = archivedAt;
    }

    public DateOnly ArchivedAt { get; }

    public int Id => _registration.Id;

    public DocumentStatus Status => _registration.Status;

    public IReadOnlyList<RegistrationItem> Lines => _registration.Lines;
}
