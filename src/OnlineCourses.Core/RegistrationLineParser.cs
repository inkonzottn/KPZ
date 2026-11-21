using System.Globalization;
using OnlineCourses.Core.Domain;

namespace OnlineCourses.Core;

/// <summary>Converts delimited text into validated registration line data.</summary>
public static class RegistrationLineParser
{
    private const int ExpectedFieldCount = 3;
    private const int MaximumSkuLength = 20;
    private const int MinimumQuantity = 1;
    private const int MaximumQuantity = 1000;

    /// <summary>Attempts to parse a registration line without throwing for invalid input.</summary>
    /// <param name="raw">Text containing SKU, quantity, and unit price separated by semicolons.</param>
    /// <param name="line">Parsed line when conversion succeeds; otherwise null.</param>
    /// <returns>True when all fields satisfy the input rules.</returns>
    public static bool TryParse(string? raw, out RegistrationItem? line)
    {
        line = null;
        if (string.IsNullOrWhiteSpace(raw))
        {
            return false;
        }

        var p = raw.Split(';');
        if (p.Length != ExpectedFieldCount || p[0].Trim().Length is 0 or > MaximumSkuLength)
        {
            return false;
        }

        var ci = CultureInfo.InvariantCulture;
        if (!int.TryParse(p[1], NumberStyles.Integer, ci, out var qty)
            || qty is < MinimumQuantity or > MaximumQuantity)
        {
            return false;
        }

        if (!decimal.TryParse(p[2], NumberStyles.Number, ci, out var price)
            || price < 0m)
        {
            return false;
        }

        line = new RegistrationItem(p[0].Trim(), qty, price);
        return true;
    }
}
