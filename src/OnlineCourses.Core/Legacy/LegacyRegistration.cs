namespace OnlineCourses.Legacy;

/// <summary>Represents a registration record from the earlier coursework module.</summary>
public class LegacyRegistration
{
    /// <summary>Gets or sets the registration identifier.</summary>
    public int Id { get; set; }

    /// <summary>Gets or sets the numeric registration state used by the legacy module.</summary>
    public int Status { get; set; }

    /// <summary>Gets or sets the calculated registration total.</summary>
    public decimal Total { get; set; }

    /// <summary>Gets or sets the contact email address.</summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>Gets or sets the product lines included in the registration.</summary>
    public List<LegacyItem> Lines { get; set; } = new();

    /// <summary>Adds a product line to the registration.</summary>
    /// <param name="line">Line to append.</param>
    public virtual void AddLine(LegacyItem line) =>
    Lines.Add(line);
}
