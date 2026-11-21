namespace OnlineCourses.Legacy;

using OnlineCourses.Core.Domain;

/// <summary>Persists legacy registration records as JSON files.</summary>
public class FileRegistrationStore
{
    /// <summary>Serializes a registration record to the specified file.</summary>
    /// <param name="order">Registration record to persist.</param>
    /// <param name="path">Destination file path.</param>
    public static void SaveToFile(LegacyRegistration order, string path)
    {
        // для демонстрації «класу-Бога»
    }
}
