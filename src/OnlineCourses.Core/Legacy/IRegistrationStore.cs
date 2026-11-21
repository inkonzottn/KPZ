namespace OnlineCourses.Legacy;

/// <summary>Defines persistence operations for legacy registration records.</summary>
public interface IRegistrationStore
{
    /// <summary>Adds a registration to the legacy store.</summary>
    /// <param name="order">Record to store.</param>
    void Add(LegacyRegistration order);

    /// <summary>Finds a stored registration by identifier.</summary>
    /// <param name="id">Registration identifier.</param>
    /// <returns>The matching registration.</returns>
    LegacyRegistration GetById(int id);

    /// <summary>Writes a registration to a file.</summary>
    /// <param name="order">Record to persist.</param>
    /// <param name="path">Destination file path.</param>
    void SaveToFile(LegacyRegistration order, string path);

    /// <summary>Sends a notification message to a recipient.</summary>
    /// <param name="recipient">Destination email address.</param>
    /// <param name="body">Message content.</param>
    void SendEmail(string recipient, string body);

    /// <summary>Exports stored registrations as CSV text.</summary>
    /// <returns>CSV content for the current store.</returns>
    string ExportCsv();

    /// <summary>Creates a backup in the specified directory.</summary>
    /// <param name="folder">Directory receiving the backup.</param>
    void Backup(string folder);

    /// <summary>Prints an invoice for a stored registration.</summary>
    /// <param name="order">Registration represented by the invoice.</param>
    void PrintInvoice(LegacyRegistration order);
}
