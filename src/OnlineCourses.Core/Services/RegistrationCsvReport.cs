namespace OnlineCourses.Core.Services;

using OnlineCourses.Core.Documents;
using OnlineCourses.Core.Domain;

public sealed class RegistrationCsvReport
{
    public static string BuildCsv(List<Registration> registrations)
    {
        var text = "id;total;status" + Environment.NewLine;
        foreach (var o in registrations)
        {
            text += o.Id + ";" + o.Total + ";" + o.Status + Environment.NewLine;
        }

        return text;
    }
}
