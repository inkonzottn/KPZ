namespace OnlineCourses.Core.Documents;

using OnlineCourses.Core.Legacy;

public static class DocumentGuards
{
    public static void EnsureRequestValid(TotalRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(request.Client);
        EnsureHeaderValid(
        request.DocumentId,
        request.Client.Name,
        request.Client.Email);
        EnsureLinesValid(request.Lines);
        ArgumentOutOfRangeException.ThrowIfNegative(
        request.DeliveryPrice);
    }

    public static void EnsureHeaderValid(
    int documentId,
    string clientName,
    string clientEmail)
    {
        ArgumentOutOfRangeException
            .ThrowIfNegativeOrZero(documentId);
        ArgumentException
            .ThrowIfNullOrWhiteSpace(clientName);
        ArgumentException
            .ThrowIfNullOrWhiteSpace(clientEmail);
        if (!clientEmail.Contains('@'))
        {
            throw new ArgumentException(
            "Пошта має містити символ @.",
            nameof(clientEmail));
        }
    }

    public static void EnsureLineValid(RegistrationItem line)
    {
        ArgumentNullException.ThrowIfNull(line);
        ArgumentException
        .ThrowIfNullOrWhiteSpace(line.Sku);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(
        line.Quantity, nameof(line.Quantity));
        ArgumentOutOfRangeException.ThrowIfNegative(
        line.UnitPrice, nameof(line.UnitPrice));
    }

    public static void EnsureLinesValid(
    IReadOnlyList<RegistrationItem> lines)
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
