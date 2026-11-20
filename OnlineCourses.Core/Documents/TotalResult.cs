namespace OnlineCourses.Core.Documents;

using OnlineCourses.Core.Legacy;

/// <summary>Represents the TotalResult type.</summary>
public sealed record TotalResult(
decimal Total,
DocumentStatus Status);
