using Pitstop.Models;
using Pitstop.Models.PitstopData;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace Pitstop.Service
{
    public class EmailService
    {
        private readonly PitstopContext _PitstopContext;
        private readonly IOptions<EmailConfiguration> _emailConfiguration;
        private readonly CommonService _commonService;

        public EmailService(PitstopContext PitstopContext, IOptions<EmailConfiguration> emailConfiguration, CommonService commonService)
        {
            _PitstopContext = PitstopContext;
            _emailConfiguration = emailConfiguration;
            _commonService = commonService;
        }

        public string SendEmailRequestResetPassword(string userId, string emailSubmittedId)
        {
            using (var transaction = _PitstopContext.Database.BeginTransaction())
            {
                try
                {
                    var user = _PitstopContext.Users.Find(userId);
                    string emailTo = user.Email;
                    string link = $"Auth/ResetMyPassword/{emailSubmittedId}";
                    string subject = "Request for Reset Password at HaiLeck SSP";

                    StringBuilder template = new();
                    template.AppendLine($"Dear {user.UserName},");
                    template.AppendLine($"We have received a request to reset your password for the Hai Leck SSP.<br/><br/>");
                    template.AppendLine($"<p>To reset your password, please click on the following link:</p><br/>");
                    template.AppendLine($"<a href='{_emailConfiguration.Value.AppsUrl}{link}' target='_blank'>Reset My Password</a><br/><br/>");
                    template.AppendLine("<p>This is an automated message; please do not reply to this email.</p><br/>");
                    template.AppendLine("<p>Regards,</p>");
                    template.AppendLine("<p>IT@Hai Leck</p>");

                    MailMessage message = new MailMessage();
                    message.From = new MailAddress(_emailConfiguration.Value.FromUserNameEmail);
                    message.Subject = subject;
                    message.To.Add(new MailAddress(emailTo));
                    message.Body = template.ToString();
                    message.IsBodyHtml = true;

                    var smtpClient = new SmtpClient(_emailConfiguration.Value.SMTPServer)
                    {
                        Port = _emailConfiguration.Value.Port,
                        Credentials = new NetworkCredential(_emailConfiguration.Value.FromUserNameEmail, _emailConfiguration.Value.FromUserNamePassword),
                        EnableSsl = _emailConfiguration.Value.isEnableSSL
                    };

                    smtpClient.Send(message);

                    var emailType = _commonService.GetDataControlData(null, AppConstant.DataControl.EmailSubmitType, AppConstant.DataControl.EmailSubmitTypeData.RequestResetPassword);

                    var emailSubmitted = new EmailSubmitted
                    {
                        Id = emailSubmittedId,
                        IsValid = true,
                        EmailSubject = template.ToString(),
                        CreatedDate = DateTime.Now,
                        UserId = user.Id,
                        EmailSubmitTypeId = emailType.Id
                    };

                    _PitstopContext.EmailSubmitteds.Add(emailSubmitted);
                    _PitstopContext.SaveChanges();
                    transaction.Commit();

                    return "Success";
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return ex.Message;
                }
            }
        }

        public string SendNewGeneratedPassword(string userId, string newPassword, string emailSubmittedId, string emailType)
        {
            using (var transaction = _PitstopContext.Database.BeginTransaction())
            {
                try
                {
                    var user = _PitstopContext.Users.Find(userId);
                    string emailTo = user.Email;
                    string subject = "New Password for Login at HaiLeck SSP";

                    if (emailType == AppConstant.DataControl.EmailSubmitTypeData.CreateUserPassword)
                    {
                        subject = "Welcome to HaiLeck SSP";
                    }

                    StringBuilder template = new();
                    template.AppendLine($"Dear {user.UserName},");
                    template.AppendLine($"A new password has been generated for your Hai Leck SSP account. For security reasons, please ensure to change this password immediately after your first login.<br/><br/>");
                    template.AppendLine($"<p><strong>Password: {newPassword}</strong></p><br/>");
                    template.AppendLine($"<a href='{_emailConfiguration.Value.AppsUrl}' target='_blank'>Log in to Hai Leck SSP</a><br/><br/>");
                    template.AppendLine("<p>This is an automated message. Please do not reply to this email.</p><br/>");
                    template.AppendLine("<p>Regards,</p>");
                    template.AppendLine("<p>IT@Hai Leck</p>");

                    MailMessage message = new MailMessage();
                    message.From = new MailAddress(_emailConfiguration.Value.FromUserNameEmail);
                    message.Subject = subject;
                    message.To.Add(new MailAddress(emailTo));
                    message.Body = template.ToString();
                    message.IsBodyHtml = true;

                    var smtpClient = new SmtpClient(_emailConfiguration.Value.SMTPServer)
                    {
                        Port = _emailConfiguration.Value.Port,
                        Credentials = new NetworkCredential(_emailConfiguration.Value.FromUserNameEmail, _emailConfiguration.Value.FromUserNamePassword),
                        EnableSsl = _emailConfiguration.Value.isEnableSSL
                    };

                    smtpClient.Send(message);

                    var emailTypeData = _commonService.GetDataControlData(null, AppConstant.DataControl.EmailSubmitType, emailType);

                    var emailSubmitted = new EmailSubmitted
                    {
                        Id = emailSubmittedId,
                        IsValid = false,
                        EmailSubject = template.ToString(),
                        CreatedDate = DateTime.Now,
                        UserId = user.Id,
                        EmailSubmitTypeId = emailTypeData.Id
                    };

                    _PitstopContext.EmailSubmitteds.Add(emailSubmitted);
                    _PitstopContext.SaveChanges();
                    transaction.Commit();

                    return "Success";
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return ex.Message;
                }
            }
        }

        public string SendEmailToDeactiveUser(User user, string daysDeactive, int emailTypeDataId)
        {
            using (var transaction = _PitstopContext.Database.BeginTransaction())
            {
                try
                {
                    string emailTo = user.Email;
                    string subject = "Accout Deactived at HaiLeck SSP";

                    StringBuilder template = new();
                    template.AppendLine($"Dear {user.UserName},");
                    template.AppendLine($"We have noticed that your account has been inactive for {daysDeactive} days.<br/>");
                    template.AppendLine($"As a result, your account status has been changed to 'inactive'. To reactivate your account or if you believe this is an error, please contact the system administrator.<br/><br/>");
                    template.AppendLine($"<a href='{_emailConfiguration.Value.AppsUrl}' target='_blank'>Log in to Hai Leck SSP</a><br/><br/>");
                    template.AppendLine("<p>This is an automated message. Please do not reply to this email.</p><br/>");
                    template.AppendLine("<p>Regards,</p>");
                    template.AppendLine("<p>IT@Hai Leck</p>");

                    MailMessage message = new MailMessage();
                    message.From = new MailAddress(_emailConfiguration.Value.FromUserNameEmail);
                    message.Subject = subject;
                    message.To.Add(new MailAddress(emailTo));
                    message.Body = template.ToString();
                    message.IsBodyHtml = true;

                    var smtpClient = new SmtpClient(_emailConfiguration.Value.SMTPServer)
                    {
                        Port = _emailConfiguration.Value.Port,
                        Credentials = new NetworkCredential(_emailConfiguration.Value.FromUserNameEmail, _emailConfiguration.Value.FromUserNamePassword),
                        EnableSsl = _emailConfiguration.Value.isEnableSSL
                    };

                    smtpClient.Send(message);

                    var emailSubmitted = new EmailSubmitted
                    {
                        Id = Guid.NewGuid().ToString(),
                        IsValid = false,
                        EmailSubject = template.ToString(),
                        CreatedDate = DateTime.Now,
                        UserId = user.Id,
                        EmailSubmitTypeId = emailTypeDataId
                    };

                    _PitstopContext.EmailSubmitteds.Add(emailSubmitted);
                    _PitstopContext.SaveChanges();
                    transaction.Commit();

                    return "Success";
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return ex.Message;
                }
            }
        }
    }
}
