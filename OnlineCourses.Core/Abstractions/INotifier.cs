namespace OnlineCourses.Core.Abstractions;

/// <summary>Defines a notification delivery contract.</summary>
public interface INotifier
{
    /// <summary>Sends a notification to a recipient.</summary>
    void Notify(string recipient, string message);
}
