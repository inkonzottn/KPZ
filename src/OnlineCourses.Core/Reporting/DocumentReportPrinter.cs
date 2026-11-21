namespace OnlineCourses.Core.Reporting;

/// <summary>Writes formatted document reports to a configured text output.</summary>
public sealed class DocumentReportPrinter
{
    private readonly TextWriter _output;

    /// <summary>Initializes a new instance of the <see cref="DocumentReportPrinter"/> class.</summary>
    /// <param name="output">Destination receiving the report text.</param>
    public DocumentReportPrinter(TextWriter output)
    {
        ArgumentNullException.ThrowIfNull(output);
        _output = output;
    }

    /// <summary>Writes a non-empty report to the configured destination.</summary>
    /// <param name="report">Formatted report content.</param>
    public void Print(string report)
    {
        ArgumentException
        .ThrowIfNullOrWhiteSpace(report);
        _output.Write(report);
    }
}
