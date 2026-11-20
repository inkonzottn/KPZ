namespace OnlineCourses.Core.Abstractions;

using OnlineCourses.Core.Domain;

/// <summary>Defines the contract for IRegistrationRepository.</summary>
public interface IRegistrationRepository
{
    void Add(Registration document);

    void Save(Registration registration);

    Registration? GetById(int id);

    IReadOnlyList<Registration> GetAll();
}
