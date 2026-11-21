namespace OnlineCourses.Core.Services;

using OnlineCourses.Core.Abstractions;
using OnlineCourses.Core.Domain;

/// <summary>Stores registration documents in process memory.</summary>
public sealed class InMemoryRegistrationRepository : IRegistrationRepository
{
    private readonly Dictionary<int, Registration> _storage = new();

    /// <summary>Adds a registration document when its identifier is unique.</summary>
    /// <param name="document">Document to store.</param>
    public void Add(Registration document)
    {
        ArgumentNullException.ThrowIfNull(document);

        if (_storage.ContainsKey(document.Id))
        {
            throw new InvalidOperationException($"Реєстрація з ID {document.Id} вже існує.");
        }

        _storage[document.Id] = document;
    }

    /// <summary>Finds a document by its identifier.</summary>
    /// <param name="id">Registration identifier.</param>
    /// <returns>The document, or null when it is absent.</returns>
    public Registration? GetById(int id)
    {
        return _storage.TryGetValue(id, out var reg) ? reg : null;
    }

    /// <summary>Returns all stored registration documents.</summary>
    /// <returns>A snapshot of the stored documents.</returns>
    public IReadOnlyList<Registration> GetAll()
    {
        return _storage.Values.ToList();
    }

    /// <summary>Stores the latest state of a registration document.</summary>
    /// <param name="registration">Updated document.</param>
    public void Save(Registration registration)
    {
        _storage[registration.Id] = registration;
    }
}
