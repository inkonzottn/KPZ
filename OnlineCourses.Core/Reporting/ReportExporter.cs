using System.IO;
using OnlineCourses.Core.Domain;

namespace OnlineCourses.Core.Reporting;

public class ReportExporter
{
    // БУЛО: при винятку файл лишається відкритим
    public void ExportReport(string path, Registration registration)
    {
        var writer = new StreamWriter(path);
        writer.WriteLine($"Реєстрація №{registration.Id}");
        writer.WriteLine(registration.Total); // якщо тут кине виняток, Close() не виконається
        writer.Close();
    }
}