namespace OnlineCourses.Core.Services;

using OnlineCourses.Core.Documents;
using OnlineCourses.Core.Domain;

/// <summary>Represents the RegistrationCsvReport type.</summary>
public sealed class RegistrationCsvReport
{
    /// <summary>Performs the BuildCsv operation.</summary>
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
