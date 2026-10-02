// <copyright file="Registration.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

/// <summary>Заявка слухача на курси.</summary>
public sealed class Registration
{
    private readonly List<RegistrationItem> items = new();

    public int Id { get; init; }

    public int MemberId { get; init; }

    public RegistrationStatus Status { get; set; } = RegistrationStatus.New;

    public DateTimeOffset CreatedAt { get; init; }

    public IReadOnlyList<RegistrationItem> Items => this.items;

    /// <summary>Додає рядок до заявки.</summary>
    public void AddItem(RegistrationItem item) => this.items.Add(item);

    /// <summary>Обчислює суму заявки.</summary>
    /// <returns></returns>
    public decimal Total()
    {
        decimal sum = 0m;
        foreach (RegistrationItem item in this.items)
        {
            sum += item.Amount;
        }

        return sum;
    }
}
