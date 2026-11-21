namespace OnlineCourses.Core.Abstractions;

/// <summary>Визначає канал надсилання повідомлень одержувачам.</summary>
public interface INotifier
{
    /// <summary>Передає текстове повідомлення вказаному одержувачу.</summary>
    /// <param name="recipient">Адреса або ідентифікатор одержувача.</param>
    /// <param name="body">Текст повідомлення.</param>
    void Notify(string recipient, string body);
}
