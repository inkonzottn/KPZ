// <copyright file="RegistrationItem.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

/// <summary>Позиція реєстрації: обраний курс, його код, тип, ціна.</summary>
public sealed class RegistrationItem
{
    public string Code { get; init; } = string.Empty;

    public decimal Price { get; init; }

    public decimal Amount => this.Price;
}
