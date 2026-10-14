namespace OnlineCourses.Core.Documents;

using OnlineCourses.Core.Legacy;

public sealed record TotalResult(
decimal Total,
DocumentStatus Status);
