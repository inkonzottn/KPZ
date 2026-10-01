/// <summary>Учасник курсу.</summary>
public sealed class Member 
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public bool IsRegular { get; init; }
}