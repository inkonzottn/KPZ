namespace OnlineCourses.Core.Abstractions;

/// <summary>Рівні аудиторського журналу.</summary>
public enum AuditLevel
{
    Information,
    Warning,
    Error
}

/// <summary>Контракт журналювання подій прикладних сценаріїв.</summary>
public interface IAuditLogger
{
    /// <summary>Записує подію з контекстом сценарію.</summary>
    /// <param name="level">Рівень важливості події.</param>
    /// <param name="scenario">Ідентифікатор сценарію або назва операції.</param>
    /// <param name="message">Повідомлення без персональних даних.</param>
    void Write(AuditLevel level, string scenario, string message);
}
