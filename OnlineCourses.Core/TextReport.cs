using System.Text;

namespace OnlineCourses.Core;

// Формує текст простого звіту з готових рядків.
/// <summary>Builds a plain text report.</summary>
public sealed class TextReport
{
    /// <summary>Gets the report title.</summary>
    public string Title { get; init; } = "Звіт";

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
