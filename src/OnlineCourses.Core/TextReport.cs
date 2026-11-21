using System.Text;

namespace OnlineCourses.Core;

/// <summary>Represents a text report assembled from a title and prepared rows.</summary>
public sealed class TextReport
{
    /// <summary>Gets the heading displayed at the beginning of the report.</summary>
    public string Title { get; init; } = "Звіт";

    /// <summary>Builds the report text from the supplied rows.</summary>
    /// <param name="rows">Lines to append beneath the title.</param>
    /// <returns>Complete multi-line report text.</returns>
    public string Build(IReadOnlyList<string> rows)
    {
        ArgumentNullException.ThrowIfNull(rows);
        var text = new StringBuilder();
        text.AppendLine(Title.ToUpperInvariant());
        foreach (var row in rows)
        {
            text.AppendLine(row);
        }

        return text.ToString();
    }
}
