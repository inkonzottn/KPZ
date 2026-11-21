using System.Text;
using OnlineCourses.App;
using OnlineCourses.Core.Infrastructure;
using OnlineCourses.Core.Pricing;
using OnlineCourses.Core.Reporting;
using OnlineCourses.Core.Services;

Console.OutputEncoding = Encoding.UTF8;

var repository = new JsonEnrollmentRepository(
    Path.Combine("data", "online-courses.json"),
    Path.Combine("data", "online-courses.seed.json"));
var clock = new SystemClock();
using var auditLog = new FileAuditLog(Path.Combine("logs", "application.log"), clock);
var service = new EnrollmentService(repository, clock, auditLog);
var application = new ConsoleApplication(
    repository,
    service,
    new CoursePackagePricingPolicy(),
    auditLog,
    new SystemConsoleAdapter());

application.Run();
