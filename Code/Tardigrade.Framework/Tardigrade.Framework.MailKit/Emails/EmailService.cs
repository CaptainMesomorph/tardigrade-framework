using AutoMapper;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using System;
using System.Threading.Tasks;
using Tardigrade.Framework.Emails;
using Tardigrade.Framework.Exceptions;
using Tardigrade.Framework.Extensions;
using Tardigrade.Framework.MailKit.Configurations;

namespace Tardigrade.Framework.MailKit.Emails
{
    /// <inheritdoc />
    public class EmailService : IEmailService
    {
        private readonly IMailKitConfiguration _config;
        private readonly IMapper _mapper;

        /// <summary>
        /// Create an instance of this class.
        /// </summary>
        /// <param name="config">MailKit specific application settings.</param>
        /// <param name="mapper">Object to object mapper.</param>
        public EmailService(IMailKitConfiguration config, IMapper mapper)
        {
            _config = config;
            _mapper = mapper;
        }

        /// <summary>
        /// Create the body of the email, including attachments if provided.
        /// </summary>
        /// <param name="htmlContent">Message content as HTML.</param>
        /// <param name="attachments">Email attachments.</param>
        /// <returns>Email body.</returns>
        private static MimeEntity CreateEmailBody(string htmlContent, EmailAttachment[] attachments = null)
        {
            var contentPart = new TextPart("html") { Text = htmlContent };

            if (attachments.IsAllEmpty())
            {
                return contentPart;
            }

            var emailBody = new Multipart("mixed") { contentPart };

            foreach (EmailAttachment attachment in attachments.OrEmptyIfNull())
            {
                var attachmentPart = new MimePart(attachment.MimeType)
                {
                    Content = new MimeContent(attachment.Content),
                    ContentDisposition = new ContentDisposition(ContentDisposition.Attachment),
                    ContentTransferEncoding = ContentEncoding.Base64,
                    FileName = attachment.Filename
                };

                emailBody.Add(attachmentPart);
            }

            return emailBody;
        }

        /// <inheritdoc />
        public async Task SendMessageAsync(
            EmailAddress fromSender,
            EmailAddress[] toRecipients,
            string subject,
            string body,
            EmailAddress[] ccRecipients = null,
            EmailAddress[] bccRecipients = null,
            EmailAttachment[] attachments = null)
        {
            if (fromSender == null) throw new ArgumentNullException(nameof(fromSender));
            if (toRecipients == null) throw new ArgumentNullException(nameof(toRecipients));
            if (body == null) throw new ArgumentNullException(nameof(body));

            var message = new MimeMessage
            {
                Subject = subject ?? throw new ArgumentNullException(nameof(subject)),
                Body = CreateEmailBody(body, attachments)
            };

            message.From.Add(_mapper.Map<MailboxAddress>(fromSender));

            foreach (EmailAddress recipient in toRecipients)
            {
                if (recipient == null) continue;
                message.To.Add(_mapper.Map<MailboxAddress>(recipient));
            }

            if (message.To.Count == 0) throw new ArgumentException("No recipients specified.", nameof(toRecipients));

            if (ccRecipients != null)
            {
                foreach (EmailAddress recipient in ccRecipients)
                {
                    if (recipient == null) continue;
                    message.Cc.Add(_mapper.Map<MailboxAddress>(recipient));
                }
            }

            if (bccRecipients != null)
            {
                foreach (EmailAddress recipient in bccRecipients)
                {
                    if (recipient == null) continue;
                    message.Bcc.Add(_mapper.Map<MailboxAddress>(recipient));
                }
            }

            try
            {
#if NET
                using var client = new SmtpClient();
                await client.ConnectAsync(_config.SmtpHost, _config.SmtpPort, SecureSocketOptions.Auto);
                await client.AuthenticateAsync(_config.SmtpUsername, _config.SmtpPassword);
                await client.SendAsync(message);
                await client.DisconnectAsync(true);
#else
                using (var client = new SmtpClient())
                {
                    await client.ConnectAsync(_config.SmtpHost, _config.SmtpPort, SecureSocketOptions.Auto);
                    await client.AuthenticateAsync(_config.SmtpUsername, _config.SmtpPassword);
                    await client.SendAsync(message);
                    await client.DisconnectAsync(true);
                }
#endif
            }
            catch (Exception e)
            {
                throw new EmailFailedException($"Error sending email message: {e.Message}", e);
            }
        }
    }
}