namespace OnlineCourses.Core.Domain;

/// <summary>Стани життєвого циклу зарахування.</summary>
public enum EnrollmentStatus
{
    /// <summary>Represents the Active value.</summary>
    Active,
    /// <summary>Represents the Suspended value.</summary>
    Suspended,
    /// <summary>Represents the Withdrawn value.</summary>
    Withdrawn,
    /// <summary>Represents the Completed value.</summary>
    Completed
}
