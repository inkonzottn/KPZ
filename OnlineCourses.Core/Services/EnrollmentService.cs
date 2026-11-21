namespace OnlineCourses.Core.Services;

using OnlineCourses.Core.Abstractions;
using OnlineCourses.Core.Domain;
using OnlineCourses.Core.Errors;
using OnlineCourses.Core.Reporting;

/// <summary>Виконує сценарії зарахування, відрахування, переведення, пошуку та звітності.</summary>
public sealed class EnrollmentService
{
    private readonly IEnrollmentRepository _repository;
    private readonly IClock? _clock;
    private readonly IAuditLogger? _logger;

    /// <summary>Initializes a new instance of the <see cref="EnrollmentService"/> class.</summary>
    /// <param name="repository">Сховище даних навчальних курсів і зарахувань.</param>
    /// <param name="clock">Джерело поточної дати; за відсутності використовується системна дата.</param>
    /// <param name="logger">Журнал подій; може бути відсутнім.</param>
    /// <exception cref="ArgumentNullException">Репозиторій не передано.</exception>
    public EnrollmentService(
        IEnrollmentRepository repository,
        IClock? clock = null,
        IAuditLogger? logger = null)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _clock = clock;
        _logger = logger;
    }

    /// <summary>Зараховує слухача, використовуючи дату з переданого годинника.</summary>
    /// <param name="enrollmentId">Ідентифікатор нового зарахування.</param>
    /// <param name="memberId">Ідентифікатор слухача.</param>
    /// <param name="groupId">Ідентифікатор навчальної групи.</param>
    /// <returns>Створене активне зарахування.</returns>
    /// <exception cref="DomainRuleException">Слухача, групу або курс не знайдено чи порушено правило зарахування.</exception>
    public Enrollment Enroll(int enrollmentId, int memberId, int groupId) =>
        Enroll(
            enrollmentId,
            memberId,
            groupId,
            _clock?.Today ?? DateOnly.FromDateTime(DateTime.Today));

    /// <summary>Перевіряє умови та створює зарахування на задану дату.</summary>
    /// <param name="enrollmentId">Ідентифікатор нового зарахування.</param>
    /// <param name="memberId">Ідентифікатор слухача.</param>
    /// <param name="groupId">Ідентифікатор навчальної групи.</param>
    /// <param name="enrolledOn">Дата оформлення зарахування.</param>
    /// <returns>Створене активне зарахування.</returns>
    /// <exception cref="DomainRuleException">Порушено передумови, місткість або правило повторного зарахування.</exception>
    public Enrollment Enroll(int enrollmentId, int memberId, int groupId, DateOnly enrolledOn)
    {
        _logger?.Write(AuditLevel.Information, "UC-01", "Початок зарахування.");
        var member = FindMember(memberId);
        var group = FindGroup(groupId);
        var course = FindCourse(group.CourseId);
        EnsureNotEnrolled(member.Id, course.Id);
        EnsurePrerequisites(member.Id, course);
        EnsureCapacity(group, "GROUP_CAPACITY_REACHED", "У групі немає вільних місць.");
        var enrollment = new Enrollment(enrollmentId, member.Id, course.Id, group.Id, enrolledOn);
        _repository.Add(enrollment);
        _logger?.Write(AuditLevel.Information, "UC-01", "Зарахування успішно створено.");
        return enrollment;
    }

    /// <summary>Відраховує слухача з активного зарахування та зберігає причину.</summary>
    /// <param name="enrollmentId">Ідентифікатор зарахування.</param>
    /// <param name="reason">Причина відрахування.</param>
    /// <exception cref="DomainRuleException">Зарахування відсутнє або вже неактивне.</exception>
    public void Withdraw(int enrollmentId, string reason)
    {
        var enrollment = GetActiveEnrollment(enrollmentId);
        enrollment.Withdraw(reason);
        _repository.Save(enrollment);
        _logger?.Write(AuditLevel.Information, "UC-02", "Зарахування відраховано.");
    }

    /// <summary>Переводить слухача до іншої групи того самого курсу.</summary>
    /// <param name="enrollmentId">Ідентифікатор активного зарахування.</param>
    /// <param name="targetGroupId">Ідентифікатор цільової групи.</param>
    /// <exception cref="DomainRuleException">Група відсутня, належить іншому курсу, переповнена або збігається з поточною.</exception>
    public void Transfer(int enrollmentId, int targetGroupId)
    {
        var enrollment = GetActiveEnrollment(enrollmentId);
        var targetGroup = FindGroup(targetGroupId);
        EnsureSameCourse(enrollment, targetGroup);
        EnsureDifferentGroup(enrollment, targetGroup);
        EnsureCapacity(targetGroup, "TARGET_GROUP_FULL", "У цільовій групі немає вільних місць.");
        enrollment.TransferTo(targetGroup.Id);
        _repository.Save(enrollment);
        _logger?.Write(AuditLevel.Information, "UC-03", "Слухача переведено до іншої групи.");
    }

    /// <summary>Позначає активне зарахування як завершене.</summary>
    /// <param name="enrollmentId">Ідентифікатор зарахування.</param>
    /// <exception cref="DomainRuleException">Зарахування відсутнє або вже неактивне.</exception>
    public void Complete(int enrollmentId)
    {
        var enrollment = GetActiveEnrollment(enrollmentId);
        enrollment.Complete();
        _repository.Save(enrollment);
        _logger?.Write(AuditLevel.Information, "UC-07", "Навчання позначено завершеним.");
    }

    /// <summary>Шукає зарахування за одним або кількома заданими критеріями.</summary>
    /// <param name="criteria">Фільтри за слухачем, кодом курсу, групою або станом.</param>
    /// <returns>Знайдені зарахування у стабільному порядку за слухачем, курсом та ID.</returns>
    /// <exception cref="ArgumentNullException">Критерії не передано.</exception>
    /// <exception cref="ArgumentException">Не задано жодного критерію пошуку.</exception>
    public IReadOnlyList<Enrollment> Search(EnrollmentSearchCriteria criteria)
    {
        ArgumentNullException.ThrowIfNull(criteria);
        EnsureSearchCriteria(criteria);
        IEnumerable<Enrollment> query = _repository.GetAll();
        query = FilterByMember(query, criteria.MemberQuery);
        query = FilterByCourse(query, criteria.CourseCode);
        query = FilterByGroup(query, criteria.GroupId);
        query = FilterByStatus(query, criteria.Status);
        return query.OrderBy(x => x.MemberId)
            .ThenBy(x => x.CourseId)
            .ThenBy(x => x.Id)
            .ToArray();
    }

    /// <summary>Формує наповнюваність груп за всіма курсами або одним курсом.</summary>
    /// <param name="courseId">Необов'язковий ідентифікатор курсу для фільтрації.</param>
    /// <returns>Кількість активних зарахувань і місткість кожної групи.</returns>
    /// <exception cref="DomainRuleException">Для групи не знайдено пов'язаний курс.</exception>
    public IReadOnlyList<GroupOccupancy> GetOccupancyReport(int? courseId = null)
    {
        var occupiedByGroup = _repository.GetAll()
            .Where(x => x.Status == EnrollmentStatus.Active)
            .GroupBy(x => x.GroupId)
            .ToDictionary(x => x.Key, x => x.Count());
        return _repository.GetGroups()
            .Where(group => !courseId.HasValue || group.CourseId == courseId.Value)
            .Select(group => CreateOccupancy(group, occupiedByGroup))
            .OrderBy(x => x.CourseCode, StringComparer.Ordinal)
            .ThenBy(x => x.GroupName, StringComparer.Ordinal)
            .ToArray();
    }

    private Member FindMember(int memberId) =>
        _repository.GetMember(memberId)
        ?? throw Reject("MEMBER_NOT_FOUND", $"Слухача з ID {memberId} не знайдено.");

    private CourseGroup FindGroup(int groupId) =>
        _repository.GetGroup(groupId)
        ?? throw Reject("GROUP_NOT_FOUND", $"Групу з ID {groupId} не знайдено.");

    private Course FindCourse(int courseId) =>
        _repository.GetCourse(courseId)
        ?? throw Reject("COURSE_NOT_FOUND", "Курс для групи не знайдено.");

    private void EnsureNotEnrolled(int memberId, int courseId)
    {
        bool alreadyEnrolled = _repository.GetAll().Any(x =>
            x.MemberId == memberId &&
            x.CourseId == courseId &&
            x.Status == EnrollmentStatus.Active);
        if (alreadyEnrolled)
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
            throw Reject("PREREQUISITES_NOT_MET", $"Не виконано передумови курсу: {string.Join(", ", missing)}.");
        }
    }

    private void EnsureCapacity(CourseGroup group, string rule, string message)
    {
        if (OccupiedSeats(group.Id) >= group.Capacity)
        {
            throw Reject(rule, message);
        }
    }

    private void EnsureSameCourse(Enrollment enrollment, CourseGroup targetGroup)
    {
        if (targetGroup.CourseId != enrollment.CourseId)
        {
            throw Reject("TRANSFER_DIFFERENT_COURSE", "Переведення можливе лише до групи того самого курсу.");
        }
    }

    private void EnsureDifferentGroup(Enrollment enrollment, CourseGroup targetGroup)
    {
        if (targetGroup.Id == enrollment.GroupId)
        {
            throw Reject("TRANSFER_SAME_GROUP", "Слухач уже навчається в цій групі.");
        }
    }

    private void EnsureSearchCriteria(EnrollmentSearchCriteria criteria)
    {
        bool hasCriteria = !string.IsNullOrWhiteSpace(criteria.MemberQuery) ||
            !string.IsNullOrWhiteSpace(criteria.CourseCode) ||
            criteria.GroupId.HasValue ||
            criteria.Status.HasValue;
        if (!hasCriteria)
        {
            throw new ArgumentException("Потрібно вказати хоча б один критерій пошуку.", nameof(criteria));
        }
    }

    private IEnumerable<Enrollment> FilterByMember(IEnumerable<Enrollment> query, string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return query;
        }

        string search = value.Trim();
        return query.Where(x =>
        {
            var member = _repository.GetMember(x.MemberId);
            return member is not null &&
                (member.Name.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                 member.Email.Contains(search, StringComparison.OrdinalIgnoreCase));
        });
    }

    private IEnumerable<Enrollment> FilterByCourse(IEnumerable<Enrollment> query, string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return query;
        }

        string search = value.Trim();
        return query.Where(x =>
        {
            var course = _repository.GetCourse(x.CourseId);
            return course is not null && course.Code.Contains(search, StringComparison.OrdinalIgnoreCase);
        });
    }

    private IEnumerable<Enrollment> FilterByGroup(IEnumerable<Enrollment> query, int? groupId) =>
        groupId.HasValue ? query.Where(x => x.GroupId == groupId.Value) : query;

    private IEnumerable<Enrollment> FilterByStatus(IEnumerable<Enrollment> query, EnrollmentStatus? status) =>
        status.HasValue ? query.Where(x => x.Status == status.Value) : query;

    private GroupOccupancy CreateOccupancy(
        CourseGroup group,
        IReadOnlyDictionary<int, int> occupiedByGroup)
    {
        var course = FindCourse(group.CourseId);
        int occupied = occupiedByGroup.GetValueOrDefault(group.Id);
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
