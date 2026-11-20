namespace OnlineCourses.Core.Reporting;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using OnlineCourses.Core.Documents;
using OnlineCourses.Core.Legacy;

/// <summary>Represents the DocumentReport type.</summary>
public static class DocumentReport
{
/// <summary>Performs the TextOf operation.</summary>
    public static string TextOf(
    TotalRequest request, TotalResult result)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(result);
        CultureInfo culture = CultureInfo.InvariantCulture;
        StringBuilder text = new();
        text.AppendLine($"Документ No {request.DocumentId}");
        text.AppendLine($"Клієнт: {request.Client.Name}");
        foreach (RegistrationItem line in request.Lines)
        {
            text.AppendLine($"{line.Sku} x {line.Quantity}");
        }

        text.AppendLine(string.Format(
        culture, "Разом: {0:F2}", result.Total));
        text.AppendLine($"Стан: {result.Status}");
        return text.ToString();
    }
}
