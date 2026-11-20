namespace OnlineCourses.Core.Abstractions;

using OnlineCourses.Core.Domain;

/// <summary>Defines persistence operations for legacy registrations.</summary>
public interface IRegistrationRepository
{
    /// <summary>Adds a registration.</summary>
    void Add(Registration registration);

    /// <summary>Persists a registration update.</summary>
    void Save(Registration registration);

    /// <summary>Gets a registration by identifier.</summary>
    Registration? GetById(int id);

    /// <summary>Gets all registrations.</summary>
    IReadOnlyList<Registration> GetAll();
}
