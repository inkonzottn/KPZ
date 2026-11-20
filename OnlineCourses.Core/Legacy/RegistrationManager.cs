namespace OnlineCourses.Legacy;

using OnlineCourses.Core.Domain;
using OnlineCourses.Core.Legacy;

/// <summary>Represents the RegistrationManager type.</summary>
public class RegistrationManager
{
/// <summary>Performs the BuildCsvReport operation.</summary>
    public static string BuildCsvReport(List<LegacyRegistration> orders)
    {
        var text = "id;total;status" + Environment.NewLine;
        foreach (var o in orders)
        {
            text += o.Id + ";" + o.Total + ";" + o.Status;
        }

        File.WriteAllText("report.csv", text);
        return text;
    }

/// <summary>Performs the Archive operation.</summary>
    public static void Archive(LegacyRegistration registration)
    {
        var coreRegistration = new Registration(registration.Id, 1, DateOnly.FromDateTime(DateTime.Today));
        var archived = new ArchivedRegistration(coreRegistration, DateOnly.FromDateTime(DateTime.Today));

        FileRegistrationStore.SaveToFile(registration, "archive.json");
    }

/// <summary>Performs the Place operation.</summary>
    public static decimal Place(LegacyRegistration registration, string customerType)
    {
        decimal total = 0m;
        foreach (var line in registration.Lines)
        {
            total += line.UnitPrice * line.Quantity;
        }

        if (customerType == "regular")
        {
            total = total * 0.95m;
        }
        else if (customerType == "vip")
        {
            total = total * 0.90m;
        }
        else if (customerType == "staff")
        {
            total = total * 0.70m;
        }

        if (total > 1000m)
        {
            total = total - 50m;
        }

        registration.Total = total;
        registration.Status = 1;
        FileRegistrationStore.SaveToFile(registration, "orders.json");
        SmtpMailer.Send(registration.Email, "Order " + registration.Id, string.Empty + total);
        return total;
    }
}
