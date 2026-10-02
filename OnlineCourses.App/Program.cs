// <copyright file="Program.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System.Globalization;

Registration registration = new()
{
    Id = 1,
    MemberId = 10,
    CreatedAt = DateTimeOffset.Now,
};

registration.AddItem(new RegistrationItem
{
    Code = "C-SHARP-1",
    Price = 4500.50m,
});

registration.AddItem(new RegistrationItem
{
    Code = "SQL-BASICS",
    Price = 2300.00m,
});

string total = registration.Total()
    .ToString("F2", CultureInfo.InvariantCulture);
Console.WriteLine($"Заявка #{registration.Id}");
Console.WriteLine($"Стан: {registration.Status}");
Console.WriteLine($"Позицій (курсів): {registration.Items.Count}");
Console.WriteLine($"Сума: {total}");
