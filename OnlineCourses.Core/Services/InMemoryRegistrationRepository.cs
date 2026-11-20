namespace OnlineCourses.Core.Services;

using OnlineCourses.Core.Abstractions;
using OnlineCourses.Core.Domain;

/// <summary>Represents the InMemoryRegistrationRepository type.</summary>
public sealed class InMemoryRegistrationRepository : IRegistrationRepository
{
    private readonly Dictionary<int, Registration> _storage = new();

/// <summary>Performs the Add operation.</summary>
    public void Add(Registration document)
    {
        ArgumentNullException.ThrowIfNull(document);

        if (_storage.ContainsKey(document.Id))
        {
            throw new InvalidOperationException($"Реєстрація з ID {document.Id} вже існує.");
        }

        _storage[document.Id] = document;
    }

/// <summary>Performs the GetById operation.</summary>
    public Registration? GetById(int id)
    {
        return _storage.TryGetValue(id, out var reg) ? reg : null;
    }

/// <summary>Performs the GetAll operation.</summary>
    public IReadOnlyList<Registration> GetAll()
    {
        return _storage.Values.ToList();
    }

/// <summary>Performs the Save operation.</summary>
    public void Save(Registration registration)
    {
        _storage[registration.Id] = registration;
    }
}
