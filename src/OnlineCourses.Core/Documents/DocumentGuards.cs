namespace OnlineCourses.Core.Documents;

using OnlineCourses.Core.Legacy;

/// <summary>Перевіряє коректність вхідних даних документа до розрахунку.</summary>
public static class DocumentGuards
{
    /// <summary>Перевіряє заголовок, клієнта, позиції та вартість доставки.</summary>
    /// <param name="request">Запит на розрахунок документа.</param>
    /// <exception cref="ArgumentNullException">Запит або його обов'язкові об'єкти не передані.</exception>
    /// <exception cref="ArgumentException">Дані клієнта або позиції некоректні.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Ідентифікатор або ціна доставки некоректні.</exception>
    public static void EnsureRequestValid(TotalRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(request.Client);
        EnsureHeaderValid(
            request.DocumentId,
            request.Client.Name,
            request.Client.Email);
        EnsureLinesValid(request.Lines);
        ArgumentOutOfRangeException.ThrowIfNegative(request.DeliveryPrice);
    }

    /// <summary>Перевіряє ідентифікатор документа та контактні дані клієнта.</summary>
    /// <param name="documentId">Додатний ідентифікатор документа.</param>
    /// <param name="clientName">Непорожнє ім'я клієнта.</param>
    /// <param name="clientEmail">Адреса електронної пошти, що містить символ @.</param>
    /// <exception cref="ArgumentOutOfRangeException">Ідентифікатор недодатний.</exception>
    /// <exception cref="ArgumentException">Ім'я або адреса порожні чи адреса не містить @.</exception>
    public static void EnsureHeaderValid(
        int documentId,
        string clientName,
        string clientEmail)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(documentId);
        ArgumentException.ThrowIfNullOrWhiteSpace(clientName);
        ArgumentException.ThrowIfNullOrWhiteSpace(clientEmail);
        if (!clientEmail.Contains('@'))
        {
            throw new ArgumentException(
                "Пошта має містити символ @.",
                nameof(clientEmail));
        }
    }

    /// <summary>Перевіряє артикул, кількість і ціну однієї позиції.</summary>
    /// <param name="line">Позиція документа для перевірки.</param>
    /// <exception cref="ArgumentNullException">Позицію не передано.</exception>
    /// <exception cref="ArgumentException">Артикул порожній.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Кількість недодатна або ціна від'ємна.</exception>
    public static void EnsureLineValid(RegistrationItem line)
    {
        ArgumentNullException.ThrowIfNull(line);
        ArgumentException.ThrowIfNullOrWhiteSpace(line.Sku);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(
            line.Quantity,
            nameof(line.Quantity));
        ArgumentOutOfRangeException.ThrowIfNegative(
            line.UnitPrice,
            nameof(line.UnitPrice));
    }

    /// <summary>Перевіряє, що список позицій непорожній і кожна позиція коректна.</summary>
    /// <param name="lines">Список позицій документа.</param>
    /// <exception cref="ArgumentNullException">Список не передано.</exception>
    /// <exception cref="ArgumentException">Список порожній або містить некоректну позицію.</exception>
    public static void EnsureLinesValid(IReadOnlyList<RegistrationItem> lines)
    {
        ArgumentNullException.ThrowIfNull(lines);
        if (lines.Count == 0)
        {
            throw new ArgumentException(
                "Документ не містить жодної позиції.",
                nameof(lines));
        }

        foreach (RegistrationItem line in lines)
        {
            EnsureLineValid(line);
        }
    }
}
