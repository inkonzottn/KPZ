using OnlineCourses.Core.Documents;
using OnlineCourses.Core.Domain;
using OnlineCourses.Core.Errors;
using Xunit;

namespace OnlineCourses.Tests;

public class RegistrationTests
{
    private static Registration NewWithLine()
    {
        var r = new Registration(1, 5, new DateOnly(2026, 3, 1));
        r.AddLine(new RegistrationItem("C-1", 1, 5m));
        return r;
    }

    [Fact]
    public void Confirm_WithLine_BecomesConfirmed()
    {
        var r = NewWithLine();
        r.Confirm();
        Assert.Equal(DocumentStatus.Confirmed, r.Status);
    }

    [Fact]
    public void Confirm_NoLines_ThrowsInvalidOperation()
    {
        var r = new Registration(2, 5, new DateOnly(2026, 3, 1));
        Assert.Throws<InvalidOperationException>(() => r.Confirm());
    }

    [Fact]
    public void Confirm_Twice_ThrowsInvalidOperation()
    {
        var r = NewWithLine();
        r.Confirm();
        Assert.Throws<InvalidOperationException>(() => r.Confirm());
    }

    [Fact]
    public void Pay_FromNew_ThrowsDomainRuleException()
    {
        var r = NewWithLine();
        Assert.Throws<DomainRuleException>(() => r.Pay());
    }

    [Fact]
    public void FullLifecycle_ConfirmPayActivateRefund_EndsRefunded()
    {
        var r = NewWithLine();
        r.Confirm();
        r.Pay();
        r.Activate();
        r.Refund();
        Assert.Equal(DocumentStatus.Refunded, r.Status);
    }

    [Fact]
    public void Activate_FromNew_ThrowsInvalidOperation()
    {
        var r = NewWithLine();
        Assert.Throws<InvalidOperationException>(() => r.Activate());
    }

    [Fact]
    public void Refund_FromNew_ThrowsInvalidOperation()
    {
        var r = NewWithLine();
        Assert.Throws<InvalidOperationException>(() => r.Refund());
    }

    [Fact]
    public void Cancel_NewRegistration_BecomesCancelled()
    {
        var r = NewWithLine();
        r.Cancel();
        Assert.Equal(DocumentStatus.Cancelled, r.Status);
    }

    [Fact]
    public void Cancel_PaidRegistration_ThrowsInvalidOperation()
    {
        var r = NewWithLine();
        r.Confirm();
        r.Pay();
        Assert.Throws<InvalidOperationException>(() => r.Cancel());
    }

    [Fact]
    public void AddLine_AfterConfirm_ThrowsInvalidOperation()
    {
        var r = NewWithLine();
        r.Confirm();
        Assert.Throws<InvalidOperationException>(
            () => r.AddLine(new RegistrationItem("C-2", 1, 5m)));
    }

    [Fact]
    public void Constructor_NonPositiveId_Throws()
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => new Registration(0, 5, new DateOnly(2026, 3, 1)));
    }
}
