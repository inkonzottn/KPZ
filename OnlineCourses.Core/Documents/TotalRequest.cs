namespace OnlineCourses.Core.Documents;

using OnlineCourses.Core.Legacy;

public sealed record TotalRequest(
int DocumentId,
ClientInfo Client,
IReadOnlyList<RegistrationItem> Lines,
DateOnly CreatedAt,
string? CouponCode,
DocumentStatus Status,
decimal DeliveryPrice);
