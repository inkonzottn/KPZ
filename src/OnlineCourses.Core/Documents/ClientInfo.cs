namespace OnlineCourses.Core.Documents;

using OnlineCourses.Core.Legacy;

/// <summary>Містить контактні дані клієнта та ознаку постійного клієнта.</summary>
/// <param name="Name">Ім'я клієнта.</param>
/// <param name="Email">Адреса електронної пошти.</param>
/// <param name="IsRegular">Ознака постійного клієнта.</param>
public sealed record ClientInfo(
string Name,
string Email,
bool IsRegular);
