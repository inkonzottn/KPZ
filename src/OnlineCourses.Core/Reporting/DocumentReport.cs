namespace OnlineCourses.Core.Reporting;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using OnlineCourses.Core.Documents;
using OnlineCourses.Core.Legacy;

/// <summary>Formats document totals and line items for human-readable output.</summary>
public static class DocumentReport
{
    /// <summary>Creates a readable summary from the request and calculated total.</summary>
    /// <param name="request">Document data and its line items.</param>
    /// <param name="result">Calculated totals and resulting document state.</param>
    /// <returns>Multi-line text representation of the document.</returns>
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
