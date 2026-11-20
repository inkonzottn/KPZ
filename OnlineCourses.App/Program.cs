using System.Text;
using OnlineCourses.Core.Domain;
using OnlineCourses.Core.Infrastructure;
using OnlineCourses.Core.Pricing;
using OnlineCourses.Core.Services;

Console.OutputEncoding = Encoding.UTF8;

var repository = new JsonEnrollmentRepository(
    Path.Combine("data", "online-courses.json"),
    Path.Combine("data", "online-courses.seed.json"));

var enrollmentService = new EnrollmentService(repository);
var pricingPolicy = new CoursePackagePricingPolicy();
var nextEnrollmentId = repository.GetAll().Select(x => x.Id).DefaultIfEmpty(0).Max() + 1;
var running = true;

Console.WriteLine("OnlineCourses — система обліку навчальних груп");

while (running)
{
    Console.WriteLine();
    Console.WriteLine("1 — Зарахувати слухача");
    Console.WriteLine("2 — Відрахувати слухача");
    Console.WriteLine("3 — Перевести до іншої групи");
    Console.WriteLine("4 — Звіт про наповнюваність груп");
    Console.WriteLine("5 — Розрахувати пакетну вартість");
    Console.WriteLine("6 — Пошук зарахувань");
    Console.WriteLine("7 — Позначити курс завершеним");
    Console.WriteLine("0 — Вихід");
    Console.Write("Оберіть дію: ");

    switch (Console.ReadLine())
    {
        case "1":
            var memberId = ReadInt("ID слухача: ");
            var groupId = ReadInt("ID групи: ");
            if (memberId.HasValue && groupId.HasValue)
            {
                try
                {
                    var enrollment = enrollmentService.Enroll(
                        nextEnrollmentId++,
                        memberId.Value,
                        groupId.Value,
                        DateOnly.FromDateTime(DateTime.Today));
                    Console.WriteLine($"Зарахування №{enrollment.Id} створено.");
                }
                catch (InvalidOperationException ex)
                {
                    Console.WriteLine($"Операцію відхилено: {ex.Message}");
                }
            }

            break;

        case "2":
            var withdrawId = ReadInt("ID зарахування: ");
            Console.Write("Причина відрахування: ");
            var reason = Console.ReadLine() ?? string.Empty;
            if (withdrawId.HasValue)
            {
                try
                {
                    enrollmentService.Withdraw(withdrawId.Value, reason);
                    Console.WriteLine("Слухача відраховано.");
                }
                catch (InvalidOperationException ex)
                {
                    Console.WriteLine($"Операцію відхилено: {ex.Message}");
                }
            }

            break;

        case "3":
            var transferId = ReadInt("ID зарахування: ");
            var targetGroupId = ReadInt("ID цільової групи: ");
            if (transferId.HasValue && targetGroupId.HasValue)
            {
                try
                {
                    enrollmentService.Transfer(transferId.Value, targetGroupId.Value);
                    Console.WriteLine("Слухача переведено до іншої групи.");
                }
                catch (InvalidOperationException ex)
                {
                    Console.WriteLine($"Операцію відхилено: {ex.Message}");
                }
            }

            break;

        case "4":
            Console.WriteLine();
            Console.WriteLine("Курс | Група | Зайнято | Місткість | Вільно | Наповнюваність");
            foreach (var item in enrollmentService.GetOccupancyReport())
            {
                Console.WriteLine(
                    $"{item.CourseCode} | {item.GroupName} | {item.Occupied} | " +
                    $"{item.Capacity} | {item.Free} | {item.OccupancyPercent:F2}%");
            }

            break;

        case "5":
            Console.Write("Введіть ID курсів через кому: ");
            var rawIds = Console.ReadLine() ?? string.Empty;
            var ids = rawIds.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            var courses = new List<Course>();
            var valid = true;
            foreach (var rawId in ids)
            {
                if (!int.TryParse(rawId, out var id) || repository.GetCourse(id) is not { } course)
                {
                    valid = false;
                    break;
                }

                courses.Add(course);
            }

            if (!valid)
            {
                Console.WriteLine("Перевірте ID курсів. Усі вказані курси мають існувати.");
                break;
            }

            try
            {
                var price = pricingPolicy.Calculate(courses);
                Console.WriteLine($"Базова вартість: {price.BaseTotal:F2} грн");
                Console.WriteLine($"Знижка: {price.DiscountRate:P0} ({price.Discount:F2} грн)");
                Console.WriteLine($"До сплати: {price.Payable:F2} грн");
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"Неможливо виконати розрахунок: {ex.Message}");
            }

            break;

        case "6":
            Console.Write("Ім'я або email слухача: ");
            var memberQuery = Console.ReadLine() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(memberQuery))
            {
                Console.WriteLine("Введіть хоча б один критерій пошуку.");
                break;
            }

            var found = enrollmentService.Search(
                new EnrollmentSearchCriteria(MemberQuery: memberQuery));
            if (found.Count == 0)
            {
                Console.WriteLine("За заданим критерієм нічого не знайдено.");
                break;
            }

            foreach (var item in found)
            {
                var member = repository.GetMember(item.MemberId);
                var course = repository.GetCourse(item.CourseId);
                var group = repository.GetGroup(item.GroupId);
                Console.WriteLine(
                    $"#{item.Id} | {member?.Name} | {course?.Code} | " +
                    $"{group?.Name} | {item.Status}");
            }

            break;

        case "7":
            var completedId = ReadInt("ID зарахування: ");
            if (completedId.HasValue)
            {
                try
                {
                    enrollmentService.Complete(completedId.Value);
                    Console.WriteLine("Навчання зарахування позначено завершеним.");
                }
                catch (InvalidOperationException ex)
                {
                    Console.WriteLine($"Операцію відхилено: {ex.Message}");
                }
            }

            break;

        case "0":
            running = false;
            break;

        default:
            Console.WriteLine("Невідома команда. Спробуйте ще раз.");
            break;
    }
}

static int? ReadInt(string prompt)
{
    Console.Write(prompt);
    return int.TryParse(Console.ReadLine(), out var value) ? value : null;
}
