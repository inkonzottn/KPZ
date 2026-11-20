namespace OnlineCourses.Core.Domain;

public sealed class Enrollment
{
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

    public int Id { get; }

    public int MemberId { get; }

    public int CourseId { get; }

    public int GroupId { get; private set; }

    public DateOnly EnrolledOn { get; }

    public EnrollmentStatus Status { get; private set; }

    public string? WithdrawalReason { get; private set; }

    public void Withdraw(string reason)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(reason);
        EnsureActive();
        WithdrawalReason = reason.Trim();
        Status = EnrollmentStatus.Withdrawn;
    }

    public void Complete()
    {
        EnsureActive();
        Status = EnrollmentStatus.Completed;
    }

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
            throw new InvalidOperationException("Операція доступна лише для активного зарахування.");
        }
    }
}
