namespace OnlineCourses.Legacy;

public class LegacyRegistration
{
    public int Id { get; set; }

    public int Status { get; set; }

    public decimal Total { get; set; }

    public string Email { get; set; } = string.Empty;

    public List<LegacyItem> Lines { get; set; } = new();

    public virtual void AddLine(LegacyItem line) =>
    Lines.Add(line);
}
