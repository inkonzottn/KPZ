using Microsoft.Extensions.Logging.Abstractions;
using OnlineCourses.Core.Domain;
using OnlineCourses.Core.Errors;
using OnlineCourses.Core.Services;
using Xunit;

namespace OnlineCourses.Tests;

public class RegistrationServiceTests
{
    private static RegistrationService CreateService(
        FakeRegistrationRepository repository, decimal price) =>
        new(repository,
            new StubPricingPolicy(price),
            new StubNotifier(),
            NullLogger<RegistrationService>.Instance);

    [Fact]
    public void TotalOf_TwoLines_SumsPricesFromPolicy()
    {
        var repository = new FakeRegistrationRepository();
        var service = CreateService(repository, 25m);
        var registration = new Registration(7, 1, new DateOnly(2026, 3, 1));
        registration.AddLine(new RegistrationItem("C-1", 2, 10m));
        registration.AddLine(new RegistrationItem("C-2", 1, 30m));

        decimal actual = service.TotalOf(registration);

        Assert.Equal(50m, actual);
    }

    [Fact]
    public void AddLine_NewRegistration_StoresLineAndSavesOnce()
    {
        var repository = new FakeRegistrationRepository();
        var service = CreateService(repository, 1m);
        repository.Add(new Registration(1, 5, new DateOnly(2026, 3, 1)));

        service.AddLine(1, new RegistrationItem("C-1", 1, 5m));

        Assert.Single(repository.GetById(1)!.Lines);
        Assert.Equal(1, repository.SaveCalls);
    }

    [Fact]
    public void AddLine_UnknownId_ThrowsDomainRuleException()
    {
        var repository = new FakeRegistrationRepository();
        var service = CreateService(repository, 1m);
        var line = new RegistrationItem("C-1", 1, 5m);

        Assert.Throws<DomainRuleException>(() => service.AddLine(999, line));
    }
}