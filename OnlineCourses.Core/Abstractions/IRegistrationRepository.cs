namespace OnlineCourses.Core.Abstractions;

using OnlineCourses.Core.Domain;

public interface IRegistrationRepository
{
    void Add(Registration document);

    void Save(Registration registration);

    Registration? GetById(int id);

    IReadOnlyList<Registration> GetAll();
}
