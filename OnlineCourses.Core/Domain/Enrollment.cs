namespace OnlineCourses.Core.Domain;

using OnlineCourses.Core.Errors;

/// <summary>Represents the Enrollment type.</summary>
public sealed class Enrollment
{
    /// <summary>Initializes a new instance of the <see cref="Enrollment"/> class.</summary>
    public Enrollment(int id, int memberId, int courseId, int groupId, DateOnly enrolledOn)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(id);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(memberId);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(courseId);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(groupId);

        Id = id;
        MemberId = memberId;
        CourseId = courseId;
        GroupId = groupId;
        EnrolledOn = enrolledOn;
        Status = EnrollmentStatus.Active;
    }

    /// <summary>Gets the Id value.</summary>
    public int Id { get; }

    /// <summary>Gets the MemberId value.</summary>
    public int MemberId { get; }

    /// <summary>Gets the CourseId value.</summary>
    public int CourseId { get; }

    /// <summary>Gets the GroupId value.</summary>
    public int GroupId { get; private set; }

    /// <summary>Gets the EnrolledOn value.</summary>
    public DateOnly EnrolledOn { get; }

    /// <summary>Gets the Status value.</summary>
    public EnrollmentStatus Status { get; private set; }

    /// <summary>Gets the WithdrawalReason value.</summary>
    public string? WithdrawalReason { get; private set; }

    /// <summary>Performs the Withdraw operation.</summary>
    public void Withdraw(string reason)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(reason);
        EnsureActive();
        WithdrawalReason = reason.Trim();
        Status = EnrollmentStatus.Withdrawn;
    }

    /// <summary>Performs the Suspend operation.</summary>
    public void Suspend()
    {
        EnsureActive();
        Status = EnrollmentStatus.Suspended;
    }

    /// <summary>Performs the Reactivate operation.</summary>
    public void Reactivate(int occupiedSeats, int capacity)
    {
        if (Status != EnrollmentStatus.Suspended)
        {
            throw new DomainRuleException("ENROLLMENT_NOT_SUSPENDED", "Повторно активувати можна лише призупинене зарахування.");
        }

        ArgumentOutOfRangeException.ThrowIfNegative(occupiedSeats);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(capacity);
        if (occupiedSeats >= capacity)
        {
            throw new DomainRuleException("GROUP_CAPACITY_REACHED", "У групі немає вільних місць.");
        }

        Status = EnrollmentStatus.Active;
    }

    /// <summary>Performs the Complete operation.</summary>
    public void Complete()
    {
        EnsureActive();
        Status = EnrollmentStatus.Completed;
    }

    /// <summary>Performs the TransferTo operation.</summary>
    public void TransferTo(int groupId)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(groupId);
        EnsureActive();
        GroupId = groupId;
    }

    private void EnsureActive()
    {
        if (Status != EnrollmentStatus.Active)
        {
            throw new DomainRuleException("ENROLLMENT_NOT_ACTIVE", "Операція доступна лише для активного зарахування.");
        }
    }
}
