using Azure.Communication.Email;
using Azure; // Necesario para WaitUntil

namespace API.Infrastructure.Services
{
    public class EmailService
    {
        private readonly EmailClient _client;

        public EmailService()
        {
            _client = new EmailClient(Environment.GetEnvironmentVariable("COMMUNICATION_SERVICES_CONNECTION_STRING") ?? "");
        }

        public async Task SendEmailAsync(List<string> toEmails, string subject, string body)
        {
            foreach(var profe in toEmails)
            {
                var emailMessage = new EmailMessage(
                    senderAddress: "DoNotReply@3a06e497-4933-4489-b3ea-1360c12e937a.azurecomm.net",
                    content: new EmailContent($"Horarios Generados {profe.Split("@")[0]}")
                    {
                        PlainText = @"Adjunto tendrá el horario generado.",
                        Html = @"
		                <html>
			                <body>
				                <h1>
					                Adjunto tendrá el horario generado.
				                </h1>
			                </body>
		                </html>"
                    },
                    recipients: new EmailRecipients(new List<EmailAddress>{
                        new EmailAddress(profe)
                    })
                );
                await _client.SendAsync(WaitUntil.Completed, emailMessage);

            }

        }
    }
}
