namespace OnlineCourses.Core.Legacy;

using OnlineCourses.Core.Abstractions;

/// <summary>Represents the SmtpMailer type.</summary>
public class SmtpMailer : INotifier
{
/// <summary>Performs the Send operation.</summary>
    public static void Send(string to, string subject, string body)
    {
        // відправки пошти
    }

/// <summary>Performs the Notify operation.</summary>
    public void Notify(string recipient, string body)
    {
        Send(recipient, "Notification", body);
    }
}
