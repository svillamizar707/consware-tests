using TravelRequests.Application.Interfaces;

namespace TravelRequests.Infrastructure.Services
{
    public class ConsoleEmailService : IEmailService
    {
        public Task SendAsync(string to, string subject, string body)
        {
            // Simulate sending email by logging to console - in real world replace with SMTP or provider
            Console.WriteLine($"Sending email to: {to}\nSubject: {subject}\nBody: {body}");
            return Task.CompletedTask;
        }
    }
}
