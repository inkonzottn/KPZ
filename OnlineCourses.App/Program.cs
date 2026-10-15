using System.Text;
using OnlineCourses.App;
using OnlineCourses.Core.Abstractions;
using OnlineCourses.Core.Documents;
using OnlineCourses.Core.Domain;
using OnlineCourses.Core.Legacy;
using OnlineCourses.Core.Services;

Console.OutputEncoding = Encoding.UTF8;

var repository = new InMemoryRegistrationRepository();
IPricingPolicy pricing = new DiscountPricingPolicy(0.05m);
var notifier = new SmtpMailer();
var service = new RegistrationService(repository, pricing, notifier);
var registration = new OnlineCourses.Core.Domain.Registration(1, 100, DateOnly.FromDateTime(DateTime.Today));
registration.AddLine(new OnlineCourses.Core.Domain.RegistrationItem("SKU-1", 2, 250m));
service.Place(registration);
Console.WriteLine($"Сума: {service.TotalOf(registration):0.00}");
