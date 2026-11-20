namespace OnlineCourses.Core.Services;

using OnlineCourses.Core.Abstractions;
using OnlineCourses.Core.Domain;
using OnlineCourses.Core.Errors;
using OnlineCourses.Core.Reporting;

/// <summary>Coordinates learner enrollment and group workflows.</summary>
public sealed class EnrollmentService
{
    private readonly IEnrollmentRepository _repository;
    private readonly IClock? _clock;
    private readonly IAuditLogger? _logger;

    /// <summary>Initializes a new instance of the <see cref="EnrollmentService"/> class.</summary>
    public EnrollmentService(
        IEnrollmentRepository repository,
        IClock? clock = null,
        IAuditLogger? logger = null)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _clock = clock;
        _logger = logger;
    }

    /// <summary>Enrolls a learner using the injected clock date.</summary>
    public Enrollment Enroll(int enrollmentId, int memberId, int groupId) =>
        Enroll(
            enrollmentId,
            memberId,
            groupId,
            _clock?.Today ?? DateOnly.FromDateTime(DateTime.Today));

    /// <summary>Enrolls a learner after checking prerequisites and group capacity.</summary>
    public Enrollment Enroll(int enrollmentId, int memberId, int groupId, DateOnly enrolledOn)
    {
        _logger?.Write(AuditLevel.Information, "UC-01", "Початок зарахування.");
        var member = RequireMember(memberId);
        var group = RequireGroup(groupId);
        var course = RequireCourse(group.CourseId);
        EnsureNoActiveDuplicate(member.Id, course.Id);
        EnsurePrerequisites(member.Id, course);
        EnsureCapacity(group.Id, group.Capacity, "GROUP_CAPACITY_REACHED");

        var enrollment = new Enrollment(enrollmentId, member.Id, course.Id, group.Id, enrolledOn);
        _repository.Add(enrollment);
        _logger?.Write(AuditLevel.Information, "UC-01", "Зарахування успішно створено.");
        return enrollment;
    }

    /// <summary>Withdraws an active enrollment and releases its seat.</summary>
    public void Withdraw(int enrollmentId, string reason)
    {
        _logger?.Write(AuditLevel.Information, "UC-02", "Початок відрахування.");
        if (string.IsNullOrWhiteSpace(reason))
        {
            throw Reject("WITHDRAW_REASON_REQUIRED", "Причину відрахування потрібно вказати.");
        }

        var enrollment = GetActiveEnrollment(enrollmentId);
        enrollment.Withdraw(reason);
        _repository.Save(enrollment);
        _logger?.Write(AuditLevel.Information, "UC-02", "Зарахування відраховано.");
    }

    /// <summary>Transfers an active enrollment to another group of the same course.</summary>
    public void Transfer(int enrollmentId, int targetGroupId)
    {
        _logger?.Write(AuditLevel.Information, "UC-03", "Початок переведення.");
        var enrollment = GetActiveEnrollment(enrollmentId);
        var targetGroup = RequireGroup(targetGroupId);
        EnsureSameCourse(enrollment.CourseId, targetGroup.CourseId);
        EnsureDifferentGroup(enrollment.GroupId, targetGroup.Id);
        EnsureCapacity(targetGroup.Id, targetGroup.Capacity, "TARGET_GROUP_FULL");

        enrollment.TransferTo(targetGroup.Id);
        _repository.Save(enrollment);
        _logger?.Write(AuditLevel.Information, "UC-03", "Слухача переведено до іншої групи.");
    }

    /// <summary>Marks an active enrollment as completed.</summary>
    public void Complete(int enrollmentId)
    {
        _logger?.Write(AuditLevel.Information, "UC-07", "Початок завершення навчання.");
        var enrollment = GetActiveEnrollment(enrollmentId);
        enrollment.Complete();
        _repository.Save(enrollment);
        _logger?.Write(AuditLevel.Information, "UC-07", "Навчання позначено завершеним.");
    }

    /// <summary>Searches enrollments by the supplied non-empty criteria.</summary>
    public IReadOnlyList<Enrollment> Search(EnrollmentSearchCriteria criteria)
    {
        ArgumentNullException.ThrowIfNull(criteria);
        EnsureSearchCriteria(criteria);

        _logger?.Write(AuditLevel.Information, "UC-05", "Початок пошуку зарахувань.");
        IEnumerable<Enrollment> result = _repository.GetAll();
        result = FilterByMember(result, criteria.MemberQuery);
        result = FilterByCourse(result, criteria.CourseCode);
        if (criteria.GroupId.HasValue)
        {
            result = result.Where(x => x.GroupId == criteria.GroupId.Value);
        }

        if (criteria.Status.HasValue)
        {
            result = result.Where(x => x.Status == criteria.Status.Value);
        }

        var ordered = result
            .OrderBy(x => x.MemberId)
            .ThenBy(x => x.CourseId)
            .ThenBy(x => x.Id)
            .ToArray();
        _logger?.Write(AuditLevel.Information, "UC-05", "Пошук зарахувань завершено.");
        return ordered;
    }

    /// <summary>Builds occupancy rows for all groups or one selected course.</summary>
    public IReadOnlyList<GroupOccupancy> GetOccupancyReport(int? courseId = null)
    {
        _logger?.Write(AuditLevel.Information, "UC-06", "Початок формування звіту.");
        var activeCounts = _repository.GetAll()
            .Where(x => x.Status == EnrollmentStatus.Active)
            .GroupBy(x => x.GroupId)
            .ToDictionary(x => x.Key, x => x.Count());

        var report = _repository.GetGroups()
            .Where(group => !courseId.HasValue || group.CourseId == courseId.Value)
            .Select(group => CreateOccupancy(group, activeCounts))
            .OrderBy(x => x.CourseCode, StringComparer.Ordinal)
            .ThenBy(x => x.GroupName, StringComparer.Ordinal)
            .ToArray();
        _logger?.Write(AuditLevel.Information, "UC-06", "Звіт наповнюваності сформовано.");
        return report;
    }

    private Member RequireMember(int memberId) =>
        _repository.GetMember(memberId)
        ?? throw Reject("MEMBER_NOT_FOUND", $"Слухача з ID {memberId} не знайдено.");

    private CourseGroup RequireGroup(int groupId) =>
        _repository.GetGroup(groupId)
        ?? throw Reject("GROUP_NOT_FOUND", $"Групу з ID {groupId} не знайдено.");

    private Course RequireCourse(int courseId) =>
        _repository.GetCourse(courseId)
        ?? throw Reject("COURSE_NOT_FOUND", "Курс для групи не знайдено.");

    private void EnsureNoActiveDuplicate(int memberId, int courseId)
    {
        var exists = _repository.GetAll().Any(x =>
            x.MemberId == memberId &&
            x.CourseId == courseId &&
            x.Status == EnrollmentStatus.Active);
        if (exists)
        {
            throw Reject("DUPLICATE_ENROLLMENT", "Слухач уже має активне зарахування на цей курс.");
        }
    }

    private void EnsurePrerequisites(int memberId, Course course)
    {
        var missing = course.PrerequisiteCourseIds
            .Where(id => !_repository.HasCompletedCourse(memberId, id))
            .ToArray();
        if (missing.Length > 0)
        {
            throw Reject(
                "PREREQUISITES_NOT_MET",
                $"Не виконано передумови курсу: {string.Join(", ", missing)}.");
        }
    }

    private void EnsureCapacity(int groupId, int capacity, string rule)
    {
        if (OccupiedSeats(groupId) >= capacity)
        {
            throw Reject(rule, "У групі немає вільних місць.");
        }
    }

    private void EnsureSameCourse(int enrollmentCourseId, int targetCourseId)
    {
        if (targetCourseId != enrollmentCourseId)
        {
            throw Reject(
                "TRANSFER_DIFFERENT_COURSE",
                "Переведення можливе лише до групи того самого курсу.");
        }
    }

    private void EnsureDifferentGroup(int currentGroupId, int targetGroupId)
    {
        if (currentGroupId == targetGroupId)
        {
            throw Reject("TRANSFER_SAME_GROUP", "Слухач уже навчається в цій групі.");
        }
    }

    private void EnsureSearchCriteria(EnrollmentSearchCriteria criteria)
    {
        if (string.IsNullOrWhiteSpace(criteria.MemberQuery) &&
            string.IsNullOrWhiteSpace(criteria.CourseCode) &&
            !criteria.GroupId.HasValue &&
            !criteria.Status.HasValue)
        {
            throw new ArgumentException(
                "Потрібно вказати хоча б один критерій пошуку.", nameof(criteria));
        }
    }

    private IEnumerable<Enrollment> FilterByMember(
        IEnumerable<Enrollment> source,
        string? memberQuery)
    {
        if (string.IsNullOrWhiteSpace(memberQuery))
        {
            return source;
        }

        var query = memberQuery.Trim();
        return source.Where(enrollment =>
        {
            var member = _repository.GetMember(enrollment.MemberId);
            return member is not null &&
                (member.Name.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                 member.Email.Contains(query, StringComparison.OrdinalIgnoreCase));
        });
    }

    private IEnumerable<Enrollment> FilterByCourse(
        IEnumerable<Enrollment> source,
        string? courseCode)
    {
        if (string.IsNullOrWhiteSpace(courseCode))
        {
            return source;
        }

        var query = courseCode.Trim();
        return source.Where(enrollment =>
        {
            var course = _repository.GetCourse(enrollment.CourseId);
            return course is not null &&
                course.Code.Contains(query, StringComparison.OrdinalIgnoreCase);
        });
    }

    private GroupOccupancy CreateOccupancy(
        CourseGroup group,
        IReadOnlyDictionary<int, int> activeCounts)
    {
        var course = RequireCourse(group.CourseId);
        var occupied = activeCounts.GetValueOrDefault(group.Id);
        return new GroupOccupancy(course.Code, group.Name, occupied, group.Capacity);
    }

    private DomainRuleException Reject(string rule, string message)
    {
        _logger?.Write(AuditLevel.Warning, rule, message);
        return new DomainRuleException(rule, message);
    }

    private int OccupiedSeats(int groupId) =>
        _repository.GetAll().Count(x =>
            x.GroupId == groupId && x.Status == EnrollmentStatus.Active);

    private Enrollment GetSuspendedEnrollment(int enrollmentId)
    {
        var enrollment = _repository.GetById(enrollmentId)
            ?? throw Reject("ENROLLMENT_NOT_FOUND", $"Зарахування з ID {enrollmentId} не знайдено.");
        if (enrollment.Status != EnrollmentStatus.Suspended)
        {
            throw Reject("ENROLLMENT_NOT_SUSPENDED", "Поновити можна лише призупинене зарахування.");
        }

        return enrollment;
    }

    private Enrollment GetActiveEnrollment(int enrollmentId)
    {
        var enrollment = _repository.GetById(enrollmentId)
            ?? throw Reject("ENROLLMENT_NOT_FOUND", $"Зарахування з ID {enrollmentId} не знайдено.");
        if (enrollment.Status != EnrollmentStatus.Active)
        {
            throw Reject("ENROLLMENT_NOT_ACTIVE", "Операція доступна лише для активного зарахування.");
        }

        return enrollment;
    }
}
