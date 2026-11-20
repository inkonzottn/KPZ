namespace OnlineCourses.Tests;

using OnlineCourses.Core.Domain;
using OnlineCourses.Core.Errors;

public sealed class EnrollmentTransitionTests
{
    [Fact]
    public void Withdraw_ChangesStatusAndTrimsReason()
    {
        var enrollment = CreateEnrollment();

        enrollment.Withdraw("  Особиста заява  ");

        Assert.Equal(EnrollmentStatus.Withdrawn, enrollment.Status);
        Assert.Equal("Особиста заява", enrollment.WithdrawalReason);
    }

    [Fact]
    public void Withdraw_RejectsBlankReason()
    {
        var enrollment = CreateEnrollment();

        Assert.Throws<ArgumentException>(() => enrollment.Withdraw("  "));
        Assert.Equal(EnrollmentStatus.Active, enrollment.Status);
    }

    [Fact]
    public void Withdraw_RejectsSecondTransition()
    {
        var enrollment = CreateEnrollment();
        enrollment.Withdraw("Особиста заява");

        var exception = Assert.Throws<DomainRuleException>(
            () => enrollment.Withdraw("Повторна заява"));

        Assert.Equal("ENROLLMENT_NOT_ACTIVE", exception.Rule);
    }

    [Fact]
    public void Complete_ChangesStatus()
    {
        var enrollment = CreateEnrollment();

        enrollment.Complete();

        Assert.Equal(EnrollmentStatus.Completed, enrollment.Status);
    }

    [Fact]
    public void Complete_RejectsWithdrawnEnrollment()
    {
        var enrollment = CreateEnrollment();
        enrollment.Withdraw("Особиста заява");

        var exception = Assert.Throws<DomainRuleException>(() => enrollment.Complete());

        Assert.Equal("ENROLLMENT_NOT_ACTIVE", exception.Rule);
        Assert.Equal(EnrollmentStatus.Withdrawn, enrollment.Status);
    }

    [Fact]
    public void TransferTo_ChangesGroupForActiveEnrollment()
    {
        var enrollment = CreateEnrollment();

        enrollment.TransferTo(12);

        Assert.Equal(12, enrollment.GroupId);
        Assert.Equal(EnrollmentStatus.Active, enrollment.Status);
    }

    [Fact]
    public void TransferTo_RejectsNonPositiveGroupId()
    {
        var enrollment = CreateEnrollment();

        Assert.Throws<ArgumentOutOfRangeException>(() => enrollment.TransferTo(0));
        Assert.Equal(11, enrollment.GroupId);
    }

    [Fact]
    public void TransferTo_RejectsWithdrawnEnrollment()
    {
        var enrollment = CreateEnrollment();
        enrollment.Withdraw("Особиста заява");

        var exception = Assert.Throws<DomainRuleException>(() => enrollment.TransferTo(12));

        Assert.Equal("ENROLLMENT_NOT_ACTIVE", exception.Rule);
        Assert.Equal(11, enrollment.GroupId);
    }

    private static Enrollment CreateEnrollment() =>
        new(1, 10, 1, 11, new DateOnly(2026, 11, 20));
}
