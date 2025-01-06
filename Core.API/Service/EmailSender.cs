using Core.API.DTOs;
using Core.API.Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Core.API.Service
{
    public class EmailSender : IEmailSender
    {
        public async Task SendEmailWithGoogleAsync(SendEmailDTO sendEmailDTO)
        {
            var emailTemplatePath = Path.Combine(Directory.GetCurrentDirectory(), "EmailTemplates", "ConfirmEmail.html");
            var emailBody = await File.ReadAllTextAsync(emailTemplatePath);
            emailBody = emailBody.Replace("{{confirmationLink}}", sendEmailDTO.Body);

            var message = new MailMessage
            {
                From = new MailAddress("Accapt@accaptacounting.ir", "حسابداری ماهان"),
                Body = emailBody,
                Subject = sendEmailDTO.Subject,
                IsBodyHtml = true
            };
            message.To.Add(new MailAddress(sendEmailDTO.To));

            using (var smtpClient = new SmtpClient("webmail.accaptacounting.ir", 587))
            {
                smtpClient.Credentials = new NetworkCredential("Accapt@accaptacounting.ir", "mahan.z.road0908");
                smtpClient.EnableSsl = false;
                smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;

                await smtpClient.SendMailAsync(message);
            }
        }
    }
}
