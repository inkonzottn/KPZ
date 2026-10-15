namespace OnlineCourses.Core.Domain;

public sealed record Member
{
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

    public int Id { get; }
    public string Name { get; }
    public string Email { get; }
    public bool IsRegular { get; init; }
}