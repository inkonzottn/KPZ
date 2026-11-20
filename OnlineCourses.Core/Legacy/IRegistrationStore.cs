namespace OnlineCourses.Core.Legacy;

/// <summary>Defines the legacy registration storage and output contract.</summary>
public interface IRegistrationStore
{
    /// <summary>Adds a legacy registration.</summary>
    void Add(LegacyRegistration registration);

    /// <summary>Gets a legacy registration by identifier.</summary>
    LegacyRegistration? GetById(int id);

    /// <summary>Saves a registration to a file.</summary>
    void SaveToFile(LegacyRegistration order, string path);

    /// <summary>Sends an email message.</summary>
    void SendEmail(string recipient, string body);

    /// <summary>Exports registrations as CSV text.</summary>
    string ExportCsv();

    /// <summary>Creates a backup at the specified path.</summary>
    void Backup(string path);

    /// <summary>Prints a registration invoice.</summary>
    void PrintInvoice(LegacyRegistration registration);
}
