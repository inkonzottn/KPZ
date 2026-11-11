using Microsoft.Extensions.Logging.Abstractions;
using OnlineCourses.Core.Documents;
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

    [Fact]
    public void AddLine_ConfirmedRegistration_ThrowsDomainRuleException()
    {
        var repository = new FakeRegistrationRepository();
        var service = CreateService(repository, 1m);
        var registration = new Registration(2, 5, new DateOnly(2026, 3, 1));
        registration.AddLine(new RegistrationItem("C-1", 1, 5m));
        registration.Confirm();
        repository.Add(registration);

        Assert.Throws<DomainRuleException>(
            () => service.AddLine(2, new RegistrationItem("C-2", 1, 5m)));
    }

    [Fact]
    public void AddLine_BigQuantity_StillSaves()
    {
        var repository = new FakeRegistrationRepository();
        var service = CreateService(repository, 1m);
        repository.Add(new Registration(1, 5, new DateOnly(2026, 3, 1)));

        service.AddLine(1, new RegistrationItem("C-1", 11, 5m));

        Assert.Equal(1, repository.SaveCalls);
    }

    [Fact]
    public void Cancel_NewRegistration_SetsCancelledAndSaves()
    {
        var repository = new FakeRegistrationRepository();
        var service = CreateService(repository, 1m);
        repository.Add(new Registration(1, 5, new DateOnly(2026, 3, 1)));

        service.Cancel(1, "передумав");

        Assert.Equal(DocumentStatus.Cancelled, repository.GetById(1)!.Status);
        Assert.Equal(1, repository.SaveCalls);
    }

    [Fact]
    public void Cancel_UnknownId_ThrowsDomainRuleException()
    {
        var service = CreateService(new FakeRegistrationRepository(), 1m);

        Assert.Throws<DomainRuleException>(() => service.Cancel(999, "x"));
    }

    [Fact]
    public void Place_Registration_StoresInRepository()
    {
        var repository = new FakeRegistrationRepository();
        var service = CreateService(repository, 1m);

        service.Place(new Registration(3, 5, new DateOnly(2026, 3, 1)));

        Assert.NotNull(repository.GetById(3));
    }

    [Fact]
    public void Load_ExistingId_ReturnsRegistration()
    {
        var repository = new FakeRegistrationRepository();
        var service = CreateService(repository, 1m);
        repository.Add(new Registration(4, 5, new DateOnly(2026, 3, 1)));

        Assert.NotNull(service.Load(4));
    }

    [Fact]
    public void CountValidQuantities_MixedRows_CountsOnlyPositiveIntegers()
    {
        int actual = RegistrationService.CountValidQuantities(
            new[] { "3", "0", "-1", "abc", "7" });

        Assert.Equal(2, actual);
    }
}
