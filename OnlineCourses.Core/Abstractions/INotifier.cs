namespace OnlineCourses.Core.Abstractions;

/// <summary>Defines the contract for INotifier.</summary>
public interface INotifier
{
    void Notify(string recipient, string body);
}
