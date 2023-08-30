using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TrackingChain.Core.Domain.Entities;
using TrackingChain.Core.Domain.Enums;
using TrackingChain.TransactionMonitorCore.Options;

namespace TrackingChain.TransactionMonitorCore.Services
{
    public class MailAlertService : IAlertService
    {
        // Fields.
        private readonly IReportGeneratorService reportGeneratorService;
        private readonly ILogger<MailAlertService> logger;
        private readonly MailSettingsOption mailSettings;

        // Constructors.
        public MailAlertService(
            IReportGeneratorService reportGeneratorService,
            ILogger<MailAlertService> logger,
            IOptions<MailSettingsOption> mailSettings)
        {
            ArgumentNullException.ThrowIfNull(mailSettings);

            this.reportGeneratorService = reportGeneratorService;
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
            var bodyBuilder = new BodyBuilder();
            bodyBuilder.HtmlBody = await reportGeneratorService.GenerateTxCancelReportAsync(reportData, reportItems);

            await SendEmailAsync(mailSettings.Mail, mailSettings.Mail, reportData.Description, bodyBuilder.ToMessageBody());
        }

        private async Task GenerateTransactionErrorAsync(
            ReportData reportData,
            IEnumerable<ReportItem> reportItems)
        {
            var bodyBuilder = new BodyBuilder();
            bodyBuilder.HtmlBody = await reportGeneratorService.GenerateTxFailedReportAsync(reportData, reportItems);

            await SendEmailAsync(mailSettings.Mail, mailSettings.Mail, reportData.Description, bodyBuilder.ToMessageBody());
        }

        private async Task SendEmailAsync(
            string to,
            string sender,
            string subject,
            MimeEntity body)
        {
            using var email = new MimeMessage();
            email.Sender = MailboxAddress.Parse(sender);
            email.To.Add(MailboxAddress.Parse(to));
            email.Subject = subject;
            email.Body = body;

            using var smtp = new SmtpClient();
            await smtp.ConnectAsync(mailSettings.Host, mailSettings.Port, SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync(mailSettings.Mail, mailSettings.Password);
            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);
        }

    }
}
