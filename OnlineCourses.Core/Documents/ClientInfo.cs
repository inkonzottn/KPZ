namespace OnlineCourses.Core.Documents;

using OnlineCourses.Core.Legacy;

/// <summary>Represents the ClientInfo type.</summary>
public sealed record ClientInfo(
string Name,
string Email,
bool IsRegular);
