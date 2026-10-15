namespace OnlineCourses.Legacy;

public interface IRegistrationStore
{
    void Add(LegacyRegistration order);

    LegacyRegistration GetById(int id);

    void SaveToFile(LegacyRegistration order, string path);

    void SendEmail(string recipient, string body);

    string ExportCsv();

    void Backup(string folder);

    void PrintInvoice(LegacyRegistration order);
}
