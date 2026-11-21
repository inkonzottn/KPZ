namespace OnlineCourses.Core.Documents;

using OnlineCourses.Core.Legacy;

/// <summary>Містить вхідні дані для розрахунку підсумкової вартості документа.</summary>
/// <param name="DocumentId">Ідентифікатор документа.</param>
/// <param name="Client">Контактні дані клієнта.</param>
/// <param name="Lines">Позиції документа.</param>
/// <param name="CreatedAt">Дата створення документа.</param>
/// <param name="CouponCode">Необов'язковий код купона.</param>
/// <param name="Status">Поточний стан документа.</param>
/// <param name="DeliveryPrice">Базова вартість доставки.</param>
public sealed record TotalRequest(
int DocumentId,
ClientInfo Client,
IReadOnlyList<RegistrationItem> Lines,
DateOnly CreatedAt,
string? CouponCode,
DocumentStatus Status,
decimal DeliveryPrice);
