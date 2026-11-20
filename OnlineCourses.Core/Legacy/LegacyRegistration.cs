namespace OnlineCourses.Legacy;

/// <summary>Represents the LegacyRegistration type.</summary>
public class LegacyRegistration
{
    /// <summary>Gets the Id value.</summary>
    public int Id { get; set; }

    /// <summary>Gets the Status value.</summary>
    public int Status { get; set; }

    /// <summary>Gets the Total value.</summary>
    public decimal Total { get; set; }

    /// <summary>Gets the Email value.</summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>Gets the Lines value.</summary>
    public List<LegacyItem> Lines { get; set; } = new();

    /// <summary>Performs the AddLine operation.</summary>
    public virtual void AddLine(LegacyItem line) =>
    Lines.Add(line);
}
