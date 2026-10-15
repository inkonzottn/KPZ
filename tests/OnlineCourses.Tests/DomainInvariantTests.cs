using OnlineCourses.Core.Domain;
using OnlineCourses.Core.Pricing;
using OnlineCourses.Core.Services;
using OnlineCourses.Core.Legacy;
using Xunit;

public class DomainInvariantTests
{
    [Fact]
    public void RegistrationItem_WithZeroQuantity_Throws()
    {
        Assert.Throws<ArgumentOutOfRangeException>(
        () => new OnlineCourses.Core.Domain.RegistrationItem("SKU-1", 0, 10m));
    }
    [Fact]
    public void Registration_AfterPay_RejectsNewLines()
    {
        var registration = NewRegistration(1);
        registration.AddLine(new OnlineCourses.Core.Domain.RegistrationItem("SKU-1", 1, 10m));
        registration.Confirm();
        registration.Pay();
        Assert.Throws<InvalidOperationException>(
        () => registration.AddLine(new OnlineCourses.Core.Domain.RegistrationItem("SKU-2", 1, 5m)));
    }

    [Fact]
    public void TotalOf_WithTenPercentDiscount_Returns180()
    {
        var service = new RegistrationService(
        new InMemoryRegistrationRepository(),
        new OnlineCourses.Core.Pricing.DiscountPricingPolicy(0.10m),
        new SmtpMailer());

        var registration = NewRegistration(2);
        registration.AddLine(new OnlineCourses.Core.Domain.RegistrationItem("SKU-1", 2, 100m));
        Assert.Equal(180m, service.TotalOf(registration));
    }
    private static Registration NewRegistration(int id) =>
    new Registration(id, 100, DateOnly.FromDateTime(DateTime.Today));
}
