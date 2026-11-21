namespace OnlineCourses.Core.Domain;

/// <summary>Містить основні дані слухача та ознаку постійного клієнта.</summary>
public sealed record Member
{
    /// <summary>Initializes a new instance of the <see cref="Member"/> record.</summary>
    /// <param name="id">Додатний ідентифікатор слухача.</param>
    /// <param name="name">Ім'я слухача.</param>
    /// <param name="email">Адреса електронної пошти слухача.</param>
    /// <exception cref="ArgumentOutOfRangeException">Ідентифікатор не є додатним.</exception>
    /// <exception cref="ArgumentException">Ім'я або email некоректні.</exception>
    public Member(int id, string name, string email)
    {
        if (id <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(id), "ID клієнта має бути більшим за 0");
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Ім'я клієнта не може бути порожнім", nameof(name));
        }

        if (string.IsNullOrWhiteSpace(email) || !email.Contains('@'))
        {
            throw new ArgumentException("Некоректний формат email (повинна бути '@')", nameof(email));
        }

        Id = id;
        Name = name;
        Email = email;
    }

    /// <summary>Gets the learner identifier.</summary>
    public int Id { get; }

    /// <summary>Gets the learner's name.</summary>
    public string Name { get; }

    /// <summary>Gets the learner's email address.</summary>
    public string Email { get; }

    /// <summary>Gets a value indicating whether the learner qualifies for the regular-customer discount.</summary>
    public bool IsRegular { get; init; }
}
