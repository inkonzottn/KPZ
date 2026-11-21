namespace OnlineCourses.Core.Abstractions;

using OnlineCourses.Core.Domain;

/// <summary>Визначає операції читання та збереження даних про зарахування.</summary>
public interface IEnrollmentRepository
{
    /// <summary>Знаходить слухача за ідентифікатором.</summary>
    /// <param name="memberId">Ідентифікатор слухача.</param>
    /// <returns>Слухач або null, якщо запис відсутній.</returns>
    Member? GetMember(int memberId);

    /// <summary>Знаходить курс за ідентифікатором.</summary>
    /// <param name="courseId">Ідентифікатор курсу.</param>
    /// <returns>Курс або null, якщо запис відсутній.</returns>
    Course? GetCourse(int courseId);

    /// <summary>Знаходить групу за ідентифікатором.</summary>
    /// <param name="groupId">Ідентифікатор групи.</param>
    /// <returns>Група або null, якщо запис відсутній.</returns>
    CourseGroup? GetGroup(int groupId);

    /// <summary>Повертає всі зарахування, доступні у сховищі.</summary>
    /// <returns>Список зарахувань; порожній, якщо записів немає.</returns>
    IReadOnlyList<Enrollment> GetAll();

    /// <summary>Повертає всі навчальні групи.</summary>
    /// <returns>Список груп; порожній, якщо груп немає.</returns>
    IReadOnlyList<CourseGroup> GetGroups();

    /// <summary>Знаходить зарахування за його ідентифікатором.</summary>
    /// <param name="enrollmentId">Ідентифікатор зарахування.</param>
    /// <returns>Зарахування або null, якщо запис відсутній.</returns>
    Enrollment? GetById(int enrollmentId);

    /// <summary>Додає нове зарахування до сховища.</summary>
    /// <param name="enrollment">Зарахування для збереження.</param>
    void Add(Enrollment enrollment);

    /// <summary>Зберігає поточний стан уже наявного зарахування.</summary>
    /// <param name="enrollment">Оновлене зарахування.</param>
    void Save(Enrollment enrollment);

    /// <summary>Перевіряє, чи завершив слухач зазначений курс.</summary>
    /// <param name="memberId">Ідентифікатор слухача.</param>
    /// <param name="courseId">Ідентифікатор курсу.</param>
    /// <returns>true, якщо є завершене зарахування на курс.</returns>
    bool HasCompletedCourse(int memberId, int courseId);
}
