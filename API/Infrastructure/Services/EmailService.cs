using Azure.Communication.Email;
using Azure;
using Infrastructure.Interfaces;
using Infrastructure.Response.Email;
using Infrastructure.DTOs;

namespace API.Infrastructure.Services
{
    public class EmailService : IEmailService
    {
        private readonly EmailClient _client;

        public EmailService()
        {
            _client = new EmailClient(Environment.GetEnvironmentVariable("COMMUNICATION_SERVICES_CONNECTION_STRING") ?? "");
        }

        public async Task<EnviarEmailResponse?> SendEmail(EnviarEmailDto data)
        {
            var response = new EnviarEmailResponse();
            try
            {
                foreach (var profe in data.correos)
                {
                    string attachmentType = data.FileType == "pdf" ? "application/pdf" : "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

                    var attachment = data.base64file != null ? new EmailAttachment(
                        name: string.IsNullOrWhiteSpace(data.FileName) ? (attachmentType == "application/pdf" ? "horario.pdf" : "horario.xlsx") : data.FileName,
                        contentType: attachmentType,
                        content: BinaryData.FromString(data.base64file)
                    ) : null;

                    var emailMessage = new EmailMessage(
                        senderAddress: "DoNotReply@3a06e497-4933-4489-b3ea-1360c12e937a.azurecomm.net",
                        content: new EmailContent(data.subject)
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

                    if (attachment != null)
                    {
                        emailMessage.Attachments.Add(attachment);
                    }

                    await _client.SendAsync(WaitUntil.Completed, emailMessage);
                }
            }
            catch (Exception ex)
            {
                response.p_error = "Ocurrio un error en el envío del correo: " + ex.Message;
            }

            return response;
        }
    }
}
