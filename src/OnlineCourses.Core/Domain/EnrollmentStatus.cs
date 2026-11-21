namespace OnlineCourses.Core.Domain;

/// <summary>Визначає життєвий стан зарахування слухача на курс.</summary>
public enum EnrollmentStatus
{
    /// <summary>Слухач навчається та займає місце у групі.</summary>
    Active,

    /// <summary>Зарахування припинено за заявою або рішенням адміністратора.</summary>
    Withdrawn,

    /// <summary>Слухач завершив навчання за курсом.</summary>
    Completed,

    /// <summary>Адміністратор анулював помилково оформлене зарахування.</summary>
    Cancelled
}
