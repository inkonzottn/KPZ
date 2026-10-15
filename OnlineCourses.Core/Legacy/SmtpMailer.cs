namespace OnlineCourses.Core.Legacy;

using OnlineCourses.Core.Abstractions;

public class SmtpMailer : INotifier
{
    public static void Send(string to, string subject, string body)
    {
        // відправки пошти
    }

    public void Notify(string recipient, string body)
    {
        Send(recipient, "Notification", body);
    }
}
