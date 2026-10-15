namespace OnlineCourses.Core.Services;

using OnlineCourses.Core.Abstractions;
using OnlineCourses.Core.Domain;

public sealed class InMemoryRegistrationRepository : IRegistrationRepository
{
    private readonly Dictionary<int, Registration> _storage = new();

    public void Add(Registration document)
    {
        ArgumentNullException.ThrowIfNull(document);

        if (_storage.ContainsKey(document.Id))
        {
            throw new InvalidOperationException($"Реєстрація з ID {document.Id} вже існує.");
        }

        _storage[document.Id] = document;
    }

    public Registration? GetById(int id)
    {
        return _storage.TryGetValue(id, out var reg) ? reg : null;
    }

    public IReadOnlyList<Registration> GetAll()
    {
        return _storage.Values.ToList();
    }
}
