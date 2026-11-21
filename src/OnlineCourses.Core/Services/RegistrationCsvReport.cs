namespace OnlineCourses.Core.Services;

using OnlineCourses.Core.Documents;
using OnlineCourses.Core.Domain;

/// <summary>Exports registration records as comma-separated-value text.</summary>
public sealed class RegistrationCsvReport
{
    /// <summary>Builds a delimited report containing identifiers, totals, and states.</summary>
    /// <param name="registrations">Documents included in the report.</param>
    /// <returns>CSV-formatted text with a header row.</returns>
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
