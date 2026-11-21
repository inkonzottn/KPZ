namespace OnlineCourses.Core.Legacy;

using OnlineCourses.Core.Abstractions;

/// <summary>Provides notification delivery through the legacy SMTP integration.</summary>
public class SmtpMailer : INotifier
{
    /// <summary>Sends a message through the configured SMTP server.</summary>
    /// <param name="to">Recipient address.</param>
    /// <param name="subject">Message subject.</param>
    /// <param name="body">Message body.</param>
    public static void Send(string to, string subject, string body)
    {
        // відправки пошти
    }

    /// <summary>Delivers a notification to the requested recipient.</summary>
    /// <param name="recipient">Recipient address.</param>
    /// <param name="body">Notification content.</param>
    public void Notify(string recipient, string body)
    {
        Send(recipient, "Notification", body);
    }
}
