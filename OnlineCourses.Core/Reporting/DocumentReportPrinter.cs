namespace OnlineCourses.Core.Reporting;

/// <summary>Represents the DocumentReportPrinter type.</summary>
public sealed class DocumentReportPrinter
{
    private readonly TextWriter _output;

/// <summary>Initializes a new instance of the <see cref="DocumentReportPrinter"/> class.</summary>
    public DocumentReportPrinter(TextWriter output)
    {
        ArgumentNullException.ThrowIfNull(output);
        _output = output;
    }

/// <summary>Performs the Print operation.</summary>
    public void Print(string report)
    {
        ArgumentException
        .ThrowIfNullOrWhiteSpace(report);
        _output.Write(report);
    }
}
