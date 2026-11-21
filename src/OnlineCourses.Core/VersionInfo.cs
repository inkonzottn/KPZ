using System.Reflection;

namespace OnlineCourses.Core;

/// <summary>Provides version information embedded in the current assembly.</summary>
public static class VersionInfo
{
    /// <summary>Gets the assembly's informational version without build metadata.</summary>
    /// <returns>The semantic version string.</returns>
    public static string Current()
    {
        var assembly = typeof(VersionInfo).Assembly;
        var attribute = assembly.GetCustomAttribute<
            AssemblyInformationalVersionAttribute>();
        var raw = attribute?.InformationalVersion ?? "0.0.0";
        var plus = raw.IndexOf('+');
        return plus < 0 ? raw : raw[..plus];
    }
}
