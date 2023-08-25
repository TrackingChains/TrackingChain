using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using TrackingChain.Core.Domain.Entities;
using TrackingChain.Core.Domain.Enums;
using TrackingChain.TransactionMonitorCore.Options;

namespace TrackingChain.TransactionMonitorCore.Services
{
    public class MailAlertService : IAlertService
    {
        // Fields.
        private readonly ILogger<MailAlertService> logger;
        private readonly MailSettingsOption mailSettings;

        // Constructors.
        public MailAlertService(
            ILogger<MailAlertService> logger,
            IOptions<MailSettingsOption> mailSettings)
        {
            ArgumentNullException.ThrowIfNull(mailSettings);

            this.logger = logger;
            this.mailSettings = mailSettings.Value;
        }

        // Methods.
        public async Task<bool> SendReportAsync(
            ReportData reportData, 
            IEnumerable<ReportItem> reportItems)
        {
            ArgumentNullException.ThrowIfNull(reportData);

            switch (reportData.Type)
            {
                case ReportDataType.TxCancel:
                    await GenerateTransactionCancelAsync(reportData, reportItems);
                        break;
                case ReportDataType.TxError:
                    await GenerateTransactionErrorAsync(reportData, reportItems);
                    break;
                default:
                    var ex = new NotImplementedException("SendReportAsync not implemented Type");
                    ex.Data.Add("Type", reportData.Type);
                    throw ex;
            }

            return true;
        }

        // Helpers.
        private async Task GenerateTransactionCancelAsync(
            ReportData reportData, 
            IEnumerable<ReportItem> reportItems)
        {
            string FilePath = Directory.GetCurrentDirectory() + "\\Templates\\WelcomeTemplate.html";
            StreamReader str = new StreamReader(FilePath);
            string MailText = str.ReadToEnd();
            str.Close();
            MailText = MailText.Replace("[username]", request.UserName).Replace("[email]", request.ToEmail);
        }

        private async Task GenerateTransactionErrorAsync(
            ReportData reportData,
            IEnumerable<ReportItem> reportItems)
        {
            var builder = new BodyBuilder();


            string FilePath = "\\MailTemplates\\WelcomeTemplate.html";
            StreamReader str = new StreamReader(FilePath);
            string MailText = await str.ReadToEndAsync();
            str.Close();
            MailText = MailText.Replace("[username]", request.UserName).Replace("[email]", request.ToEmail);

            await SendEmailAsync("", "", "", builder.ToMessageBody());
        }

        private async Task SendEmailAsync(
            string to, 
            string sender, 
            string subject,
            string body)
        {
            using var email = new MimeMessage();
            email.Sender = MailboxAddress.Parse(sender);
            email.To.Add(MailboxAddress.Parse(to));
            email.Subject = subject;

            using var smtp = new SmtpClient();
            await smtp.ConnectAsync(mailSettings.Host, mailSettings.Port, SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync(mailSettings.Mail, mailSettings.Password);
            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);
        }
    }
}
