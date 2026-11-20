namespace OnlineCourses.Core.Services;

using OnlineCourses.Core.Abstractions;
using OnlineCourses.Core.Domain;
using OnlineCourses.Core.Errors;
using OnlineCourses.Core.Pricing;
using OnlineCourses.Core.Reporting;

/// <summary>Runs the text-based online course workflows through an injected console.</summary>
public sealed class ConsoleMenu
{
    private readonly IConsole _console;
    private readonly IEnrollmentRepository _repository;
    private readonly EnrollmentService _service;
    private readonly ICoursePackagePricingPolicy _pricing;
    private readonly IAuditLogger _logger;
    private int _nextEnrollmentId;

    /// <summary>Initializes a new instance of the <see cref="ConsoleMenu"/> class.</summary>
    /// <param name="dependencies">The console workflow dependencies.</param>
    public ConsoleMenu(ConsoleMenuDependencies dependencies)
    {
        ArgumentNullException.ThrowIfNull(dependencies);
        _console = dependencies.Console;
        _repository = dependencies.Repository;
        _service = dependencies.Service;
        _pricing = dependencies.Pricing;
        _logger = dependencies.Logger;
        _nextEnrollmentId = _repository.GetAll().Select(x => x.Id).DefaultIfEmpty(0).Max() + 1;
    }

    /// <summary>Runs the menu until the user selects the exit command.</summary>
    public void Run()
    {
        var running = true;
        while (running)
        {
            PrintMenu();
            switch (_console.ReadLine())
            {
                case "1": HandleEnroll(); break;
                case "2": HandleWithdraw(); break;
                case "3": HandleTransfer(); break;
                case "4": PrintOccupancy(); break;
                case "5": HandlePackagePrice(); break;
                case "6": HandleSearch(); break;
                case "7": HandleComplete(); break;
                case "0": running = false; break;
                default: _console.WriteLine("Невідома команда. Спробуйте ще раз."); break;
            }
        }
    }

    private void PrintMenu()
    {
        _console.WriteLine(string.Empty);
        _console.WriteLine("1 — Зарахувати слухача");
        _console.WriteLine("2 — Відрахувати слухача");
        _console.WriteLine("3 — Перевести до іншої групи");
        _console.WriteLine("4 — Звіт про наповнюваність груп");
        _console.WriteLine("5 — Розрахувати пакетну вартість");
        _console.WriteLine("6 — Пошук зарахувань");
        _console.WriteLine("7 — Позначити навчання завершеним");
        _console.WriteLine("0 — Вихід");
        _console.Write("Оберіть дію: ");
    }

    private void HandleEnroll()
    {
        var memberId = ReadInt("ID слухача: ");
        var groupId = ReadInt("ID групи: ");
        if (!memberId.HasValue || !groupId.HasValue)
        {
            return;
        }

        var enrollmentId = _nextEnrollmentId;
        if (Execute("UC-01", () =>
        {
            var enrollment = _service.Enroll(enrollmentId, memberId.Value, groupId.Value);
            _console.WriteLine($"Зарахування №{enrollment.Id} створено.");
        }))
        {
            _nextEnrollmentId++;
        }
    }

    private void HandleWithdraw()
    {
        var id = ReadInt("ID зарахування: ");
        var reason = ReadRequiredText("Причина відрахування: ");
        if (!id.HasValue || reason is null)
        {
            return;
        }

        Execute("UC-02", () =>
        {
            _service.Withdraw(id.Value, reason);
            _console.WriteLine("Слухача відраховано.");
        });
    }

    private void HandleTransfer()
    {
        var id = ReadInt("ID зарахування: ");
        var targetGroupId = ReadInt("ID цільової групи: ");
        if (!id.HasValue || !targetGroupId.HasValue)
        {
            return;
        }

        Execute("UC-03", () =>
        {
            _service.Transfer(id.Value, targetGroupId.Value);
            _console.WriteLine("Слухача переведено до іншої групи.");
        });
    }

    private void PrintOccupancy()
    {
        _console.WriteLine("");
        _console.WriteLine("Курс | Група | Зайнято | Місткість | Вільно | Наповнюваність");
        foreach (var item in _service.GetOccupancyReport())
        {
            _console.WriteLine(
                $"{item.CourseCode} | {item.GroupName} | {item.Occupied} | " +
                $"{item.Capacity} | {item.Free} | {item.OccupancyPercent:F2}%");
        }
    }

    private void HandlePackagePrice()
    {
        _console.Write("Введіть ID курсів через кому: ");
        var rawIds = _console.ReadLine() ?? string.Empty;
        if (!TryParseCourses(rawIds, out var courses))
        {
            _console.WriteLine("Перевірте ID курсів. Усі вказані курси мають існувати.");
            return;
        }

        try
        {
            var price = _pricing.Calculate(courses);
            _console.WriteLine($"Базова вартість: {price.BaseTotal:F2} грн");
            _console.WriteLine($"Знижка: {price.DiscountRate:P0} ({price.Discount:F2} грн)");
            _console.WriteLine($"До сплати: {price.Payable:F2} грн");
        }
        catch (ArgumentException ex)
        {
            _console.WriteLine($"Неможливо виконати розрахунок: {ex.Message}");
        }
    }

    private bool TryParseCourses(string rawIds, out IReadOnlyList<Course> courses)
    {
        var result = new List<Course>();
        foreach (var rawId in rawIds.Split(
            ',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
        {
            if (!int.TryParse(rawId, out var id) || _repository.GetCourse(id) is not { } course)
            {
                courses = [];
                return false;
            }

            result.Add(course);
        }

        courses = result;
        return true;
    }

    private void HandleSearch()
    {
        _console.Write("Ім'я/email слухача (необов'язково): ");
        var memberQuery = _console.ReadLine();
        _console.Write("Код курсу (необов'язково): ");
        var courseCode = _console.ReadLine();
        if (string.IsNullOrWhiteSpace(memberQuery) && string.IsNullOrWhiteSpace(courseCode))
        {
            _console.WriteLine("Введіть хоча б один критерій пошуку.");
            return;
        }

        var found = _service.Search(new EnrollmentSearchCriteria(
            MemberQuery: memberQuery,
            CourseCode: courseCode));
        if (found.Count == 0)
        {
            _console.WriteLine("За заданим критерієм нічого не знайдено.");
            return;
        }

        foreach (var item in found)
        {
            var member = _repository.GetMember(item.MemberId);
            var course = _repository.GetCourse(item.CourseId);
            var group = _repository.GetGroup(item.GroupId);
            _console.WriteLine(
                $"#{item.Id} | {member?.Name} | {course?.Code} | {group?.Name} | {item.Status}");
        }
    }

    private void HandleComplete()
    {
        var id = ReadInt("ID зарахування: ");
        if (!id.HasValue)
        {
            return;
        }

        Execute("UC-07", () =>
        {
            _service.Complete(id.Value);
            _console.WriteLine("Навчання зарахування позначено завершеним.");
        });
    }

    private bool Execute(string scenario, Action operation)
    {
        try
        {
            operation();
            return true;
        }
        catch (DomainRuleException ex)
        {
            _console.WriteLine($"Операцію відхилено: {ex.Message}");
        }
        catch (ArgumentException ex)
        {
            _console.WriteLine($"Некоректні дані: {ex.Message}");
        }
        catch (Exception)
        {
            _logger.Write(AuditLevel.Error, scenario, "Неочікувана помилка операції.");
            throw;
        }

        return false;
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

            _console.WriteLine("Некоректне значення. Введіть ціле число.");
        }
    }

    private string? ReadRequiredText(string prompt)
    {
        while (true)
        {
            _console.Write(prompt);
            var value = _console.ReadLine();
            if (value is null)
            {
                return null;
            }

            if (!string.IsNullOrWhiteSpace(value))
            {
                return value.Trim();
            }

            _console.WriteLine("Поле не може бути порожнім.");
        }
    }
}
