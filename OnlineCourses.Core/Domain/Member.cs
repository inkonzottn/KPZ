namespace OnlineCourses.Core.Domain;

/// <summary>Represents the Member type.</summary>
public sealed record Member
{
/// <summary>Initializes a new instance of the <see cref="Member"/> class.</summary>
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

/// <summary>Gets the Id value.</summary>
    public int Id { get; }

/// <summary>Gets the Name value.</summary>
    public string Name { get; }

/// <summary>Gets the Email value.</summary>
    public string Email { get; }

/// <summary>Gets the IsRegular value.</summary>
    public bool IsRegular { get; init; }
}
