namespace OnlineCourses.Core.Documents;

using OnlineCourses.Core.Legacy;

public sealed record ClientInfo(
string Name,
string Email,
bool IsRegular);
