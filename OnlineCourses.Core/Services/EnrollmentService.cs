namespace OnlineCourses.Core.Services;

using OnlineCourses.Core.Abstractions;
using OnlineCourses.Core.Domain;
using OnlineCourses.Core.Errors;
using OnlineCourses.Core.Reporting;

public sealed class EnrollmentService
{
    private readonly IEnrollmentRepository _repository;
    private readonly IClock? _clock;
    private readonly IAuditLogger? _logger;

    public EnrollmentService(
        IEnrollmentRepository repository,
        IClock? clock = null,
        IAuditLogger? logger = null)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _clock = clock;
        _logger = logger;
    }

    public Enrollment Enroll(int enrollmentId, int memberId, int groupId) =>
        Enroll(enrollmentId, memberId, groupId,
            _clock?.Today ?? DateOnly.FromDateTime(DateTime.Today));

    public Enrollment Enroll(int enrollmentId, int memberId, int groupId, DateOnly enrolledOn)
    {
        _logger?.Write(AuditLevel.Information, "UC-01", "Початок зарахування.");
        var member = _repository.GetMember(memberId)
            ?? throw Reject("MEMBER_NOT_FOUND", $"Слухача з ID {memberId} не знайдено.");
        var group = _repository.GetGroup(groupId)
            ?? throw Reject("GROUP_NOT_FOUND", $"Групу з ID {groupId} не знайдено.");
        var course = _repository.GetCourse(group.CourseId)
            ?? throw Reject("COURSE_NOT_FOUND", "Курс для групи не знайдено.");

        if (_repository.GetAll().Any(x =>
            x.MemberId == member.Id &&
            x.CourseId == course.Id &&
            x.Status == EnrollmentStatus.Active))
        {
            throw Reject("DUPLICATE_ENROLLMENT", "Слухач уже має активне зарахування на цей курс.");
        }

        var missingPrerequisites = course.PrerequisiteCourseIds
            .Where(id => !_repository.HasCompletedCourse(member.Id, id))
            .ToArray();

        if (missingPrerequisites.Length > 0)
        {
            throw Reject(
                "PREREQUISITES_NOT_MET",
                $"Не виконано передумови курсу: {string.Join(", ", missingPrerequisites)}.");
        }

        if (OccupiedSeats(group.Id) >= group.Capacity)
        {
            throw Reject("GROUP_CAPACITY_REACHED", "У групі немає вільних місць.");
        }

        var enrollment = new Enrollment(enrollmentId, member.Id, course.Id, group.Id, enrolledOn);
        _repository.Add(enrollment);
        _logger?.Write(AuditLevel.Information, "UC-01", "Зарахування успішно створено.");
        return enrollment;
    }

    public void Withdraw(int enrollmentId, string reason)
    {
        var enrollment = GetActiveEnrollment(enrollmentId);
        enrollment.Withdraw(reason);
        _repository.Save(enrollment);
        _logger?.Write(AuditLevel.Information, "UC-02", "Зарахування відраховано.");
    }

    public void Transfer(int enrollmentId, int targetGroupId)
    {
        var enrollment = GetActiveEnrollment(enrollmentId);
        var targetGroup = _repository.GetGroup(targetGroupId)
            ?? throw Reject("GROUP_NOT_FOUND", $"Групу з ID {targetGroupId} не знайдено.");

        if (targetGroup.CourseId != enrollment.CourseId)
        {
            throw Reject("TRANSFER_DIFFERENT_COURSE", "Переведення можливе лише до групи того самого курсу.");
        }

        if (targetGroup.Id == enrollment.GroupId)
        {
            throw Reject("TRANSFER_SAME_GROUP", "Слухач уже навчається в цій групі.");
        }

        if (OccupiedSeats(targetGroup.Id) >= targetGroup.Capacity)
        {
            throw Reject("TARGET_GROUP_FULL", "У цільовій групі немає вільних місць.");
        }

        enrollment.TransferTo(targetGroup.Id);
        _repository.Save(enrollment);
        _logger?.Write(AuditLevel.Information, "UC-03", "Слухача переведено до іншої групи.");
    }

    public void Complete(int enrollmentId)
    {
        var enrollment = GetActiveEnrollment(enrollmentId);
        enrollment.Complete();
        _repository.Save(enrollment);
        _logger?.Write(AuditLevel.Information, "UC-07", "Навчання позначено завершеним.");
    }

    public IReadOnlyList<Enrollment> Search(EnrollmentSearchCriteria criteria)
    {
        ArgumentNullException.ThrowIfNull(criteria);

        if (string.IsNullOrWhiteSpace(criteria.MemberQuery) &&
            string.IsNullOrWhiteSpace(criteria.CourseCode) &&
            !criteria.GroupId.HasValue &&
            !criteria.Status.HasValue)
        {
            throw new ArgumentException("Потрібно вказати хоча б один критерій пошуку.", nameof(criteria));
        }

        var query = _repository.GetAll().AsEnumerable();

        if (!string.IsNullOrWhiteSpace(criteria.MemberQuery))
        {
            var memberQuery = criteria.MemberQuery.Trim();
            query = query.Where(enrollment =>
            {
                var member = _repository.GetMember(enrollment.MemberId);
                return member is not null &&
                    (member.Name.Contains(memberQuery, StringComparison.OrdinalIgnoreCase) ||
                     member.Email.Contains(memberQuery, StringComparison.OrdinalIgnoreCase));
            });
        }

        if (!string.IsNullOrWhiteSpace(criteria.CourseCode))
        {
            var courseCode = criteria.CourseCode.Trim();
            query = query.Where(enrollment =>
            {
                var course = _repository.GetCourse(enrollment.CourseId);
                return course is not null &&
                    course.Code.Contains(courseCode, StringComparison.OrdinalIgnoreCase);
            });
        }

        if (criteria.GroupId.HasValue)
        {
            query = query.Where(x => x.GroupId == criteria.GroupId.Value);
        }

        if (criteria.Status.HasValue)
        {
            query = query.Where(x => x.Status == criteria.Status.Value);
        }

        return query
            .OrderBy(x => x.MemberId)
            .ThenBy(x => x.CourseId)
            .ThenBy(x => x.Id)
            .ToArray();
    }

    public IReadOnlyList<GroupOccupancy> GetOccupancyReport(int? courseId = null)
    {
        var groups = _repository.GetAll()
            .Where(x => x.Status == EnrollmentStatus.Active)
            .GroupBy(x => x.GroupId)
            .ToDictionary(x => x.Key, x => x.Count());

        var result = new List<GroupOccupancy>();
        foreach (var group in _repository.GetGroups())
        {
            if (courseId.HasValue && group.CourseId != courseId.Value)
            {
                continue;
            }

            var course = _repository.GetCourse(group.CourseId)
                ?? throw Reject("COURSE_NOT_FOUND", "Курс для групи не знайдено.");
            var occupied = groups.GetValueOrDefault(group.Id);
            result.Add(new GroupOccupancy(
                course.Code,
                group.Name,
                occupied,
                group.Capacity));
        }

        return result
            .OrderBy(x => x.CourseCode, StringComparer.Ordinal)
            .ThenBy(x => x.GroupName, StringComparer.Ordinal)
            .ToArray();
    }

    private DomainRuleException Reject(string rule, string message)
    {
        _logger?.Write(AuditLevel.Warning, rule, message);
        return new DomainRuleException(rule, message);
    }

    private int OccupiedSeats(int groupId) =>
        _repository.GetAll().Count(x =>
            x.GroupId == groupId && x.Status == EnrollmentStatus.Active);

    private Enrollment GetActiveEnrollment(int enrollmentId)
    {
        var enrollment = _repository.GetById(enrollmentId)
            ?? throw new DomainRuleException("ENROLLMENT_NOT_FOUND", $"Зарахування з ID {enrollmentId} не знайдено.");

        if (enrollment.Status != EnrollmentStatus.Active)
        {
            throw new DomainRuleException("ENROLLMENT_NOT_ACTIVE", "Операція доступна лише для активного зарахування.");
        }

        return enrollment;
    }
}
