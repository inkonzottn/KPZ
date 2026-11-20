using System.Reflection;

namespace OnlineCourses.Core;

// Повертає версію збірки з Directory.Build.props.
/// <summary>Provides version information for the application.</summary>
public static class VersionInfo
{
    /// <summary>Gets the current application version.</summary>
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
