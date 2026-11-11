using OnlineCourses.Core.Abstractions;
using OnlineCourses.Core.Domain;

namespace OnlineCourses.Tests;

internal sealed class FakeRegistrationRepository : IRegistrationRepository
{
    private readonly Dictionary<int, Registration> _items = new();

    public int SaveCalls { get; private set; }

    public void Add(Registration document) => _items[document.Id] = document;

    public void Save(Registration registration) => SaveCalls++;

    public Registration? GetById(int id) =>
        _items.TryGetValue(id, out Registration? found)
            ? found
            : null;

    public IReadOnlyList<Registration> GetAll() => _items.Values.ToList();
}

internal sealed class StubPricingPolicy : IPricingPolicy
{
    private readonly decimal _price;

    public StubPricingPolicy(decimal price) => _price = price;

    public decimal PriceOf(RegistrationItem item) => _price;
}

internal sealed class StubNotifier : INotifier
{
    public void Notify(string recipient, string body)
    {
    }
}
