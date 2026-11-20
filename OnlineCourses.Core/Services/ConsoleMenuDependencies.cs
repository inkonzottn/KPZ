namespace OnlineCourses.Core.Services;

using OnlineCourses.Core.Abstractions;
using OnlineCourses.Core.Pricing;

/// <summary>Collects dependencies for the replaceable console workflow.</summary>
public sealed class ConsoleMenuDependencies
{
    /// <summary>Gets or sets the console adapter.</summary>
    public required IConsole Console { get; init; }

    /// <summary>Gets or sets the enrollment repository.</summary>
    public required IEnrollmentRepository Repository { get; init; }

    /// <summary>Gets or sets the application service.</summary>
    public required EnrollmentService Service { get; init; }

    /// <summary>Gets or sets the package pricing policy.</summary>
    public required ICoursePackagePricingPolicy Pricing { get; init; }

    /// <summary>Gets or sets the audit logger.</summary>
    public required IAuditLogger Logger { get; init; }
}
