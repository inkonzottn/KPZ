using OnlineCourses.Core.Infrastructure;
using OnlineCourses.Core.Pricing;
using OnlineCourses.Core.Reporting;
using OnlineCourses.Core.Services;

internal static class Program
{
    private static void Main()
    {
        var repository = new JsonEnrollmentRepository(
            Path.Combine("data", "online-courses.json"),
            Path.Combine("data", "online-courses.seed.json"));
        var clock = new SystemClock();
        using var auditLog = new FileAuditLog(
            Path.Combine("logs", "application.log"), clock);
        var service = new EnrollmentService(repository, clock, auditLog);
        var menu = new ConsoleMenu(new ConsoleMenuDependencies
        {
            Console = new SystemConsole(),
            Repository = repository,
            Service = service,
            Pricing = new CoursePackagePricingPolicy(),
            Logger = auditLog
        });

        menu.Run();
    }
}
