namespace OnlineCourses.Core.Domain;

using OnlineCourses.Core.Errors;

/// <summary>Відображає зарахування слухача на курс і керує його життєвим циклом.</summary>
public sealed class Enrollment
{
    /// <summary>Initializes a new instance of the <see cref="Enrollment"/> class.</summary>
    /// <param name="id">Додатний ідентифікатор зарахування.</param>
    /// <param name="memberId">Додатний ідентифікатор слухача.</param>
    /// <param name="courseId">Додатний ідентифікатор курсу.</param>
    /// <param name="groupId">Додатний ідентифікатор групи.</param>
    /// <param name="enrolledOn">Дата оформлення зарахування.</param>
    /// <exception cref="ArgumentOutOfRangeException">Один з ідентифікаторів недопустимий.</exception>
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

    /// <summary>Gets the enrollment identifier.</summary>
    public int Id { get; }

    /// <summary>Gets the learner identifier.</summary>
    public int MemberId { get; }

    /// <summary>Gets the course identifier.</summary>
    public int CourseId { get; }

    /// <summary>Gets the identifier of the learner's current group.</summary>
    public int GroupId { get; private set; }

    /// <summary>Gets the date when the learner enrolled.</summary>
    public DateOnly EnrolledOn { get; }

    /// <summary>Gets the current enrollment status.</summary>
    public EnrollmentStatus Status { get; private set; }

    /// <summary>Gets the withdrawal reason, or null if not withdrawn.</summary>
    public string? WithdrawalReason { get; private set; }

    /// <summary>Gets the administrative cancellation reason, if cancelled.</summary>
    public string? CancellationReason { get; private set; }

    /// <summary>Відраховує слухача та зберігає причину операції.</summary>
    /// <param name="reason">Непорожнє пояснення причини відрахування.</param>
    /// <exception cref="ArgumentException">Причина не вказана.</exception>
    /// <exception cref="DomainRuleException">Зарахування вже не є активним.</exception>
    public void Withdraw(string reason)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(reason);
        EnsureActive();
        WithdrawalReason = reason.Trim();
        Status = EnrollmentStatus.Withdrawn;
    }

    /// <summary>Анулює помилково оформлене активне зарахування.</summary>
    /// <param name="reason">Адміністративна причина анулювання.</param>
    /// <exception cref="ArgumentException">Причину не вказано.</exception>
    /// <exception cref="DomainRuleException">Зарахування вже не є активним.</exception>
    public void Cancel(string reason)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(reason);
        EnsureActive();
        CancellationReason = reason.Trim();
        Status = EnrollmentStatus.Cancelled;
    }

    /// <summary>Позначає активне зарахування як завершене.</summary>
    /// <exception cref="DomainRuleException">Зарахування вже не є активним.</exception>
    public void Complete()
    {
        EnsureActive();
        Status = EnrollmentStatus.Completed;
    }

    /// <summary>Переводить активне зарахування до іншої групи.</summary>
    /// <param name="groupId">Додатний ідентифікатор цільової групи.</param>
    /// <exception cref="ArgumentOutOfRangeException">Ідентифікатор групи недопустимий.</exception>
    /// <exception cref="DomainRuleException">Зарахування вже не є активним.</exception>
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
