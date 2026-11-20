using System.Text;
using OnlineCourses.Core.Abstractions;
using OnlineCourses.Core.Domain;
using OnlineCourses.Core.Infrastructure;
using OnlineCourses.Core.Pricing;
using OnlineCourses.Core.Reporting;
using OnlineCourses.Core.Services;

internal static class Program
{
    private static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;
        var repository = new JsonEnrollmentRepository(
            Path.Combine("data", "online-courses.json"),
            Path.Combine("data", "online-courses.seed.json"));
        var clock = new SystemClock();
        using var auditLog = new FileAuditLog(
            Path.Combine("logs", "application.log"), clock);
        var service = new EnrollmentService(repository, clock, auditLog);
        var pricing = new CoursePackagePricingPolicy();
        var nextId = repository.GetAll().Select(x => x.Id).DefaultIfEmpty(0).Max() + 1;

        RunMenu(repository, service, pricing, auditLog, clock, ref nextId);
    }

    private static void RunMenu(
        JsonEnrollmentRepository repository,
        EnrollmentService service,
        CoursePackagePricingPolicy pricing,
        IAuditLogger logger,
        IClock clock,
        ref int nextId)
    {
        var running = true;
        while (running)
        {
            PrintMenu();
            switch (Console.ReadLine())
            {
                case "1": HandleEnroll(repository, service, logger, clock, ref nextId); break;
                case "2": HandleWithdraw(service, logger); break;
                case "3": HandleTransfer(service, logger); break;
                case "4": PrintOccupancy(service); break;
                case "5": HandlePackagePrice(repository, pricing); break;
                case "6": HandleSearch(repository, service); break;
                case "7": HandleComplete(service, logger); break;
                case "0": running = false; break;
                default: Console.WriteLine("Невідома команда. Спробуйте ще раз."); break;
            }
        }
    }

    private static void PrintMenu()
    {
        Console.WriteLine();
        Console.WriteLine("1 — Зарахувати слухача");
        Console.WriteLine("2 — Відрахувати слухача");
        Console.WriteLine("3 — Перевести до іншої групи");
        Console.WriteLine("4 — Звіт про наповнюваність груп");
        Console.WriteLine("5 — Розрахувати пакетну вартість");
        Console.WriteLine("6 — Пошук зарахувань");
        Console.WriteLine("7 — Позначити навчання завершеним");
        Console.WriteLine("0 — Вихід");
        Console.Write("Оберіть дію: ");
    }

    private static void HandleEnroll(
        IEnrollmentRepository repository,
        EnrollmentService service,
        IAuditLogger logger,
        IClock clock,
        ref int nextId)
    {
        var memberId = ReadInt("ID слухача: ");
        var groupId = ReadInt("ID групи: ");
        if (!memberId.HasValue || !groupId.HasValue)
        {
            return;
        }

        var enrollmentId = nextId;
        if (Execute("UC-01", logger, () =>
        {
            var enrollment = service.Enroll(enrollmentId, memberId.Value, groupId.Value, clock.Today);
            Console.WriteLine($"Зарахування №{enrollment.Id} створено.");
        }))
        {
            nextId++;
        }
    }

    private static void HandleWithdraw(EnrollmentService service, IAuditLogger logger)
    {
        var id = ReadInt("ID зарахування: ");
        var reason = ReadRequiredText("Причина відрахування: ");
        if (!id.HasValue || reason is null)
        {
            return;
        }

        Execute("UC-02", logger, () =>
        {
            service.Withdraw(id.Value, reason);
            Console.WriteLine("Слухача відраховано.");
        });
    }

    private static void HandleTransfer(EnrollmentService service, IAuditLogger logger)
    {
        var id = ReadInt("ID зарахування: ");
        var targetGroupId = ReadInt("ID цільової групи: ");
        if (!id.HasValue || !targetGroupId.HasValue)
        {
            return;
        }

        Execute("UC-03", logger, () =>
        {
            service.Transfer(id.Value, targetGroupId.Value);
            Console.WriteLine("Слухача переведено до іншої групи.");
        });
    }

    private static void PrintOccupancy(EnrollmentService service)
    {
        Console.WriteLine();
        Console.WriteLine("Курс | Група | Зайнято | Місткість | Вільно | Наповнюваність");
        foreach (var item in service.GetOccupancyReport())
        {
            Console.WriteLine(
                $"{item.CourseCode} | {item.GroupName} | {item.Occupied} | " +
                $"{item.Capacity} | {item.Free} | {item.OccupancyPercent:F2}%");
        }
    }

    private static void HandlePackagePrice(
        IEnrollmentRepository repository,
        CoursePackagePricingPolicy pricing)
    {
        Console.Write("Введіть ID курсів через кому: ");
        var rawIds = Console.ReadLine() ?? string.Empty;
        if (!TryParseCourses(rawIds, repository, out var courses))
        {
            Console.WriteLine("Перевірте ID курсів. Усі вказані курси мають існувати.");
            return;
        }

        try
        {
            var price = pricing.Calculate(courses);
            Console.WriteLine($"Базова вартість: {price.BaseTotal:F2} грн");
            Console.WriteLine($"Знижка: {price.DiscountRate:P0} ({price.Discount:F2} грн)");
            Console.WriteLine($"До сплати: {price.Payable:F2} грн");
        }
        catch (ArgumentException ex)
        {
            Console.WriteLine($"Неможливо виконати розрахунок: {ex.Message}");
        }
    }

    private static bool TryParseCourses(
        string rawIds,
        IEnrollmentRepository repository,
        out IReadOnlyList<Course> courses)
    {
        var result = new List<Course>();
        foreach (var rawId in rawIds.Split(
            ',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
        {
            if (!int.TryParse(rawId, out var id) || repository.GetCourse(id) is not { } course)
            {
                courses = [];
                return false;
            }

            result.Add(course);
        }

        courses = result;
        return true;
    }

    private static void HandleSearch(
        IEnrollmentRepository repository,
        EnrollmentService service)
    {
        Console.Write("Ім'я/email слухача (необов'язково): ");
        var memberQuery = Console.ReadLine();
        Console.Write("Код курсу (необов'язково): ");
        var courseCode = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(memberQuery) && string.IsNullOrWhiteSpace(courseCode))
        {
            Console.WriteLine("Введіть хоча б один критерій пошуку.");
            return;
        }

        var found = service.Search(new EnrollmentSearchCriteria(
            MemberQuery: memberQuery,
            CourseCode: courseCode));
        if (found.Count == 0)
        {
            Console.WriteLine("За заданим критерієм нічого не знайдено.");
            return;
        }

        foreach (var item in found)
        {
            var member = repository.GetMember(item.MemberId);
            var course = repository.GetCourse(item.CourseId);
            var group = repository.GetGroup(item.GroupId);
            Console.WriteLine(
                $"#{item.Id} | {member?.Name} | {course?.Code} | {group?.Name} | {item.Status}");
        }
    }

    private static void HandleComplete(EnrollmentService service, IAuditLogger logger)
    {
        var id = ReadInt("ID зарахування: ");
        if (!id.HasValue)
        {
            return;
        }

        Execute("UC-07", logger, () =>
        {
            service.Complete(id.Value);
            Console.WriteLine("Навчання зарахування позначено завершеним.");
        });
    }

    private static bool Execute(string scenario, IAuditLogger logger, Action operation)
    {
        try
        {
            operation();
            return true;
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine($"Операцію відхилено: {ex.Message}");
        }
        catch (ArgumentException ex)
        {
            Console.WriteLine($"Некоректні дані: {ex.Message}");
        }
        catch (Exception)
        {
            logger.Write(AuditLevel.Error, scenario, "Неочікувана помилка операції.");
            Console.WriteLine("Виникла внутрішня помилка. Подробиці записано до журналу.");
        }

        return false;
    }

    private static int? ReadInt(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            var input = Console.ReadLine();
            if (input is null)
            {
                return null;
            }

            if (int.TryParse(input, out var value))
            {
                return value;
            }

            Console.WriteLine("Некоректне значення. Введіть ціле число.");
        }
    }

    private static string? ReadRequiredText(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            var value = Console.ReadLine();
            if (value is null)
            {
                return null;
            }

            if (!string.IsNullOrWhiteSpace(value))
            {
                return value.Trim();
            }

            Console.WriteLine("Поле не може бути порожнім.");
        }
    }
}
