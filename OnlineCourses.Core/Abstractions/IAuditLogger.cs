namespace OnlineCourses.Core.Abstractions;

/// <summary>Визначає важливість події, яку записує аудиторський журнал.</summary>
public enum AuditLevel
{
    /// <summary>Фіксує початок або успішне завершення сценарію.</summary>
    Information,

    /// <summary>Фіксує відхилену операцію через порушення правила.</summary>
    Warning,

    /// <summary>Фіксує неочікувану помилку виконання.</summary>
    Error
}

/// <summary>Контракт журналювання подій прикладних сценаріїв.</summary>
public interface IAuditLogger
{
    /// <summary>Записує подію з рівнем важливості та контекстом сценарію.</summary>
    /// <param name="level">Рівень важливості події.</param>
    /// <param name="scenario">Ідентифікатор сценарію або назва операції.</param>
    /// <param name="message">Повідомлення без персональних даних.</param>
    void Write(AuditLevel level, string scenario, string message);
}
