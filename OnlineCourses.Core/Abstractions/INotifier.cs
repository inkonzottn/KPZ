namespace OnlineCourses.Core.Abstractions;

public interface INotifier
{
    void Notify(string recipient, string body);
}
