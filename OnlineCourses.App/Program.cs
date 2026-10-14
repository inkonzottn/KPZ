using System.Text;
using OnlineCourses.Domain;

Console.OutputEncoding = Encoding.UTF8;

TotalRequest request = BuildRequestFromInput();
TotalResult result =
DocumentTotalCalculator.Calculate(request);
string report = DocumentReport.TextOf(request, result);
new DocumentReportPrinter(Console.Out).Print(report);
