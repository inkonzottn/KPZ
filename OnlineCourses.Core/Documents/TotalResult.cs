namespace OnlineCourses.Core.Documents;

using OnlineCourses.Core.Legacy;

/// <summary>Містить результат розрахунку документа.</summary>
/// <param name="Total">Підсумкова сума після знижок і доставки.</param>
/// <param name="Status">Стан документа після розрахунку.</param>
public sealed record TotalResult(
decimal Total,
DocumentStatus Status);
