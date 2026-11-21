namespace OnlineCourses.Core.Abstractions;

using OnlineCourses.Core.Domain;

/// <summary>Визначає операції збереження та отримання реєстрацій.</summary>
public interface IRegistrationRepository
{
    /// <summary>Додає нову реєстрацію до сховища.</summary>
    /// <param name="document">Документ для додавання.</param>
    void Add(Registration document);

    /// <summary>Зберігає поточний стан реєстрації.</summary>
    /// <param name="registration">Оновлений документ.</param>
    void Save(Registration registration);

    /// <summary>Знаходить реєстрацію за її ідентифікатором.</summary>
    /// <param name="id">Ідентифікатор документа.</param>
    /// <returns>Документ або null, якщо його не знайдено.</returns>
    Registration? GetById(int id);

    /// <summary>Повертає всі збережені реєстрації.</summary>
    /// <returns>Список документів; порожній, якщо сховище не містить записів.</returns>
    IReadOnlyList<Registration> GetAll();
}
