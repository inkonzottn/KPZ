namespace OnlineCourses.App;

using OnlineCourses.Core.Abstractions;
using OnlineCourses.Core.Domain;
using OnlineCourses.Core.Errors;
using OnlineCourses.Core.Pricing;
using OnlineCourses.Core.Services;

/// <summary>Керує консольними сценаріями системи OnlineCourses.</summary>
public sealed class ConsoleApplication
{
    private readonly IEnrollmentRepository _repository;
    private readonly EnrollmentService _enrollmentService;
    private readonly ICoursePackagePricingPolicy _pricingPolicy;
    private readonly IAuditLogger _auditLogger;
    private readonly IConsole _console;
    private int _nextEnrollmentId;

    /// <summary>Initializes a new instance of the <see cref="ConsoleApplication"/> class.</summary>
    /// <param name="repository">Сховище навчальних даних.</param>
    /// <param name="enrollmentService">Сервіс сценаріїв зарахування.</param>
    /// <param name="pricingPolicy">Політика пакетної ціни.</param>
    /// <param name="auditLogger">Журнал аудиту.</param>
    /// <param name="console">Абстракція консольного вводу й виводу.</param>
    public ConsoleApplication(
        IEnrollmentRepository repository,
        EnrollmentService enrollmentService,
        ICoursePackagePricingPolicy pricingPolicy,
        IAuditLogger auditLogger,
        IConsole console)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _enrollmentService = enrollmentService ?? throw new ArgumentNullException(nameof(enrollmentService));
        _pricingPolicy = pricingPolicy ?? throw new ArgumentNullException(nameof(pricingPolicy));
        _auditLogger = auditLogger ?? throw new ArgumentNullException(nameof(auditLogger));
        _console = console ?? throw new ArgumentNullException(nameof(console));
        _nextEnrollmentId = repository.GetAll().Select(item => item.Id).DefaultIfEmpty(0).Max() + 1;
    }

    /// <summary>Запускає цикл обробки команд до завершення вводу або виходу.</summary>
    public void Run()
    {
        _console.WriteLine("OnlineCourses — система обліку навчальних груп");

        while (true)
        {
            DisplayMenu();
            if (!HandleCommand(_console.ReadLine()))
            {
                return;
            }
        }
    }

    private void DisplayMenu()
    {
        _console.WriteLine();
        _console.WriteLine("1 — Зарахувати слухача");
        _console.WriteLine("2 — Відрахувати слухача");
        _console.WriteLine("3 — Перевести до іншої групи");
        _console.WriteLine("4 — Звіт про наповнюваність груп");
        _console.WriteLine("5 — Розрахувати пакетну вартість");
        _console.WriteLine("6 — Пошук зарахувань");
        _console.WriteLine("7 — Позначити курс завершеним");
        _console.WriteLine("8 — Анулювати помилкове зарахування");
        _console.WriteLine("0 — Вихід");
        _console.Write("Оберіть дію: ");
    }

    private bool HandleCommand(string? command)
    {
        if (command is null || command == "0")
        {
            return false;
        }

        switch (command)
        {
            case "1": Enroll(); break;
            case "2": Withdraw(); break;
            case "3": Transfer(); break;
            case "4": ShowOccupancy(); break;
            case "5": CalculatePackagePrice(); break;
            case "6": SearchEnrollments(); break;
            case "7": CompleteEnrollment(); break;
            case "8": CancelEnrollment(); break;
            default: _console.WriteLine("Невідома команда. Спробуйте ще раз."); break;
        }

        return true;
    }

    private void Enroll()
    {
        var memberId = ReadInt("ID слухача: ");
        var groupId = ReadInt("ID групи: ");
        if (!memberId.HasValue || !groupId.HasValue)
        {
            return;
        }

        Execute("UC-01", () =>
        {
            var enrollment = _enrollmentService.Enroll(
                _nextEnrollmentId++,
                memberId.Value,
                groupId.Value);
            _console.WriteLine($"Зарахування №{enrollment.Id} створено.");
        });
    }

    private void Withdraw()
    {
        var enrollmentId = ReadInt("ID зарахування: ");
        if (!enrollmentId.HasValue)
        {
            return;
        }

        _console.Write("Причина відрахування: ");
        var reason = _console.ReadLine() ?? string.Empty;
        Execute("UC-02", () =>
        {
            _enrollmentService.Withdraw(enrollmentId.Value, reason);
            _console.WriteLine("Слухача відраховано.");
        });
    }

    private void CancelEnrollment()
    {
        var enrollmentId = ReadInt("ID зарахування: ");
        if (!enrollmentId.HasValue)
        {
            return;
        }

        _console.Write("Причина анулювання: ");
        var reason = _console.ReadLine() ?? string.Empty;
        Execute("UC-02", () =>
        {
            _enrollmentService.Cancel(enrollmentId.Value, reason);
            _console.WriteLine("Помилкове зарахування анульовано.");
        });
    }

    private void Transfer()
    {
        var enrollmentId = ReadInt("ID зарахування: ");
        var targetGroupId = ReadInt("ID цільової групи: ");
        if (!enrollmentId.HasValue || !targetGroupId.HasValue)
        {
            return;
        }

        Execute("UC-03", () =>
        {
            _enrollmentService.Transfer(enrollmentId.Value, targetGroupId.Value);
            _console.WriteLine("Слухача переведено до іншої групи.");
        });
    }

    private void ShowOccupancy()
    {
        _console.WriteLine();
        _console.WriteLine("Курс | Група | Зайнято | Місткість | Вільно | Наповнюваність");
        foreach (var item in _enrollmentService.GetOccupancyReport())
        {
            _console.WriteLine(
                $"{item.CourseCode} | {item.GroupName} | {item.Occupied} | " +
                $"{item.Capacity} | {item.Free} | {item.OccupancyPercent:F2}%");
        }
    }

    private void CalculatePackagePrice()
    {
        _console.Write("Введіть ID курсів через кому: ");
        var rawIds = _console.ReadLine() ?? string.Empty;
        var courses = ParseCourses(rawIds);
        if (courses is null)
        {
            return;
        }

        Execute("UC-04", () =>
        {
            var price = _pricingPolicy.Calculate(courses);
            _console.WriteLine($"Базова вартість: {price.BaseTotal:F2} грн");
            _console.WriteLine($"Знижка: {price.DiscountRate:P0} ({price.Discount:F2} грн)");
            _console.WriteLine($"До сплати: {price.Payable:F2} грн");
        });
    }

    private List<Course>? ParseCourses(string rawIds)
    {
        var ids = rawIds.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        var courses = new List<Course>();
        foreach (var rawId in ids)
        {
            if (!int.TryParse(rawId, out var id) || _repository.GetCourse(id) is not { } course)
            {
                _console.WriteLine("Перевірте ID курсів. Усі вказані курси мають існувати.");
                return null;
            }

            courses.Add(course);
        }

        return courses;
    }

    private void SearchEnrollments()
    {
        var criteria = ReadSearchCriteria();
        if (criteria is null)
        {
            return;
        }

        var found = _enrollmentService.Search(criteria);
        DisplaySearchResults(found);
    }

    private EnrollmentSearchCriteria? ReadSearchCriteria()
    {
        _console.Write("Ім'я або email (необов'язково): ");
        var memberQuery = _console.ReadLine();
        _console.Write("Код курсу (необов'язково): ");
        var courseCode = _console.ReadLine();
        var groupId = ReadOptionalInt("ID групи (необов'язково): ");
        var status = ReadOptionalStatus();

        if (!string.IsNullOrWhiteSpace(memberQuery) ||
            !string.IsNullOrWhiteSpace(courseCode) ||
            groupId.HasValue ||
            status.HasValue)
        {
            return new EnrollmentSearchCriteria(memberQuery, courseCode, groupId, status);
        }

        _console.WriteLine("Введіть хоча б один критерій пошуку.");
        return null;
    }

    private void DisplaySearchResults(IReadOnlyList<Enrollment> enrollments)
    {
        if (enrollments.Count == 0)
        {
            _console.WriteLine("За заданим критерієм нічого не знайдено.");
            return;
        }

        foreach (var enrollment in enrollments)
        {
            PrintEnrollment(enrollment);
        }
    }

    private int? ReadOptionalInt(string prompt)
    {
        while (true)
        {
            _console.Write(prompt);
            var input = _console.ReadLine();
            if (input is null || string.IsNullOrWhiteSpace(input))
            {
                return null;
            }

            if (int.TryParse(input, out var value) && value > 0)
            {
                return value;
            }

            _console.WriteLine("ID групи має бути додатним цілим числом або порожнім.");
        }
    }

    private EnrollmentStatus? ReadOptionalStatus()
    {
        while (true)
        {
            _console.Write("Стан Active/Withdrawn/Completed/Cancelled (необов'язково): ");
            var input = _console.ReadLine();
            if (input is null || string.IsNullOrWhiteSpace(input))
            {
                return null;
            }

            if (Enum.TryParse<EnrollmentStatus>(input, true, out var status))
            {
                return status;
            }

            _console.WriteLine("Введіть Active, Withdrawn, Completed, Cancelled або залиште поле порожнім.");
        }
    }

    private void PrintEnrollment(Enrollment enrollment)
    {
        var member = _repository.GetMember(enrollment.MemberId);
        var course = _repository.GetCourse(enrollment.CourseId);
        var group = _repository.GetGroup(enrollment.GroupId);
        _console.WriteLine(
            $"#{enrollment.Id} | {member?.Name} | {course?.Code} | " +
            $"{group?.Name} | {enrollment.Status}");
    }

    private void CompleteEnrollment()
    {
        var enrollmentId = ReadInt("ID зарахування: ");
        if (!enrollmentId.HasValue)
        {
            return;
        }

        Execute("UC-07", () =>
        {
            _enrollmentService.Complete(enrollmentId.Value);
            _console.WriteLine("Навчання зарахування позначено завершеним.");
        });
    }

    private int? ReadInt(string prompt)
    {
        while (true)
        {
            _console.Write(prompt);
            var input = _console.ReadLine();
            if (input is null)
            {
                return null;
            }

            if (int.TryParse(input, out var value))
            {
                return value;
            }

            _console.WriteLine("Потрібно ввести ціле число.");
        }
    }

    private void Execute(string scenario, Action operation)
    {
        try
        {
            operation();
        }
        catch (DomainRuleException exception)
        {
            _console.WriteLine($"Операцію відхилено: {exception.Message}");
        }
        catch (ArgumentException exception)
        {
            _console.WriteLine($"Перевірте введені дані: {exception.Message}");
        }
        catch (Exception exception)
        {
            _auditLogger.Write(
                AuditLevel.Error,
                scenario,
                $"Неочікувана помилка: {exception.GetType().Name}.");
            _console.WriteLine("Виникла внутрішня помилка. Подробиці записано до журналу.");
        }
    }
}
