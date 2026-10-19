using System;
using System.Collections.Generic;

namespace OnlineCourses.Domain;

/// <summary>
/// Замовлення в системі курсів.
/// </summary>
public class Order
{
    /// <summary>Ставка податку на додану вартість.</summary>
    private const decimal VatRate = 0.2m;

    /// <summary>Поріг суми для знижки постійному покупцю.</summary>
    private const decimal RegularDiscountThreshold = 1000m;

    /// <summary>Частка знижки постійному покупцю.</summary>
    private const decimal RegularDiscountRate = 0.10m;

    /// <summary>Поріг суми для знижки на велике замовлення.</summary>
    private const decimal LargeOrderThreshold = 5000m;

    /// <summary>Частка знижки на велике замовлення.</summary>
    private const decimal LargeOrderDiscountRate = 0.15m;

    /// <summary>Кількість рядків, з якої діє гуртова знижка.</summary>
    private const int BulkLineCount = 10;

    /// <summary>Сума гуртової знижки, грн.</summary>
    private const decimal BulkDiscountAmount = 100m;

    /// <summary>Межа кількості рядків у замовленні.</summary>
    private const int MaxLineCount = 100;

    /// <summary>Найкоротше ім’я покупця.</summary>
    private const int MinCustomerNameLength = 2;

    /// <summary>Стан замовлення - Нове.</summary>
    private const int StatusNew = 0;

    /// <summary>Стан замовлення - Оплачено.</summary>
    private const int StatusPayed = 1;

    /// <summary>Стан замовлення - Відправлено.</summary>
    private const int StatusShipped = 2;

    /// <summary>Стан замовлення - Скасовано.</summary>
    private const int StatusCancelled = 3;

    private readonly List<RegistrationItem> _lines;

    /// <summary>
    /// Initializes a new instance of the <see cref="Order"/> class.
    /// Ініціалізує новий екземпляр класу <see cref="Order"/>.
    /// </summary>
    /// <param name="id">Ідентифікатор замовлення.</param>
    /// <param name="customerName">Ім'я покупця.</param>
    public Order(string id, string customerName)
    {
        Id = id;
        CustomerName = customerName;
        CreatedAt = DateTime.Now;
    }

    /// <summary>
    /// Gets отримує ідентифікатор замовлення.
    /// </summary>
    public string Id { get; private set; }

    /// <summary>
    /// Gets отримує ім'я покупця.
    /// </summary>
    public string CustomerName { get; private set; }

    /// <summary>
    /// Gets отримує поточний стан замовлення (0 - новий, 1 - оплачений, 2 - відправлений, 3 - скасований).
    /// </summary>
    public int Status { get; private set; }

    /// <summary>
    /// Gets отримує дату та час створення замовлення.
    /// </summary>
    public DateTime CreatedAt { get; private set; }

    /// <summary>
    /// Знаходить замовлення за його ідентифікатором у списку.
    /// </summary>
    /// <param name="orders">Список замовлень для пошуку.</param>
    /// <param name="orderId">Шуканий ідентифікатор замовлення.</param>
    /// <returns>Знайдене замовлення або null, якщо нічого не знайдено.</returns>
    public static Order? FindById(List<Order> orders, string orderId)
    {
        for (int i = 0; i < orders.Count; i++)
        {
            if (orders[i].Id == orderId)
            {
                return orders[i];
            }
        }

        return null;
    }

    /// <summary>
    /// Додає до замовлення рядок із товаром.
    /// </summary>
    /// <param name="sku">Артикул товару.</param>
    /// <param name="quantity">Кількість одиниць.</param>
    /// <param name="unitPrice">Ціна за одиницю, грн.</param>
    public void AddLine(string sku, int quantity, decimal unitPrice)
    {
        string[] lineParts = new string[3];
        lineParts[0] = sku;
        lineParts[1] = quantity.ToString();
        lineParts[2] = unitPrice.ToString();
        _lines.Add(lineParts);
    }

    /// <summary>
    /// Обчислює загальну суму замовлення з урахуванням знижок та податків.
    /// </summary>
    /// <param name="isRegularCustomer">Прапорець, чи є клієнт постійним.</param>
    /// <returns>Остаточна сума замовлення після всіх розрахунків.</returns>
    public decimal CalculateTotal(bool isRegularCustomer)
    {
        decimal total = 0;
        int lineCount = 0;
        for (int i = 0; i < _lines.Count; i++)
        {
            int quantity = _lines[i].Quantity;
            decimal unitPrice = _lines[i].UnitPrice;
            total = total + (quantity * unitPrice);
            lineCount = lineCount + 1;
        }

        // нарахування знижок до ПДВ
        if (isRegularCustomer && total > RegularDiscountThreshold)
        {
            total *= 1m - RegularDiscountRate;
        }
        else if (total > LargeOrderThreshold)
        {
            total *= 1m - LargeOrderDiscountRate;
        }

        if (lineCount > BulkLineCount)
        {
            total -= BulkDiscountAmount;
        }

        // нарахування ПДВ після всіх знижок
        total += total * VatRate;

        return Math.Round(total, 2);
    }

    /// <summary>
    /// Змінює стан замовлення, якщо перехід дозволений
    /// правилами предметної області.
    /// </summary>
    /// <param name="newStatus">Цільовий стан замовлення.</param>
    /// <returns>
    /// true, якщо перехід виконано; false, якщо він заборонений.
    /// </returns>
    public bool TryChangeStatus(int newStatus)
    {
        if (Status == 0 && newStatus == 1)
        {
            Status = 1;
            return true;
        }

        if (Status == 1 && newStatus == 2)
        {
            Status = 2;
            return true;
        }

        if (Status == 0 && newStatus == 3)
        {
            Status = 3;
            return true;
        }

        return false;
    }

    /// <summary>
    /// Перевіряє замовлення на відповідність вимогам валідності.
    /// </summary>
    /// <returns>true, якщо замовлення валідне; інакше false.</returns>
    public bool IsValid()
    {
        if (Id != null && Id != string.Empty && CustomerName != null && CustomerName.Length > 2 &&
         _lines.Count > 0 && _lines.Count < 100 && Status >= 0 && Status <= 3)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// Формує детальний текстовий звіт за замовленням.
    /// </summary>
    /// <returns>Рядок із переліком товарів та загальною сумою.</returns>
    public string BuildReport()
    {
        string reportString = string.Empty;
        for (int i = 0; i < _lines.Count; i++)
        {
            reportString = reportString + "Товар: " + _lines[i][0] + "; кількість: " +
            _lines[i][1] + "; ціна: " + _lines[i][2] + "; сума: " +
            _lines[i].Quantity * _lines[i].UnitPrice + "\n";
        }

        reportString = reportString + "Разом: " + CalculateTotal(false) + "\n";
        return reportString;
    }
}
