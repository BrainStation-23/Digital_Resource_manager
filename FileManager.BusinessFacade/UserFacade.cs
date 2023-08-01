using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Configuration;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace FileManager.BusinessFacade
{
    public partial class Facade
    {
        public bool Login(string userName, string password)
        {
            bool isUser = false;

            if (WebSecurity.Login(userName, password))
            {
                HttpContext.Current.Session["emailAddress"] = userName;
                isUser = true;
            }

            return isUser;
        }
        public bool RecoverPassword(string email)
        {
            bool isSend = false;

            if (WebSecurity.GetUser(email) != null && WebSecurity.GetUser(email).IsApproved)
            {
                User user = this.GetUserByUserName(email);//db.Users.Where(x => x.Username == email).FirstOrDefault();
                // TODO ASP.NET membership should be replaced with ASP.NET Core identity. For more details see https://docs.microsoft.com/aspnet/core/migration/proper-to-2x/membership-to-core-identity.
                string newPassword = Membership.GeneratePassword(6, 2);
                string newHashPassword = WebSecurity.GetHash(newPassword);

                user.Password = newHashPassword;
                user.LastPasswordChangedDate = DateTime.Now;
                var isUpdated = this.UpdateUser(user);


                if (isUpdated && !string.IsNullOrEmpty(user.Email))
                {
                    if (newPassword.Length > 0)
                        isSend = this.SendMail(user.Email, newPassword);
                }
            }

            return isSend;
        }
        private bool SendMail(string email, string newPassword)
        {
            try
            {
                var config = WebConfigurationManager.OpenWebConfiguration(HttpContext.Current.Request.ApplicationPath);
                var settings = (MailSettingsSectionGroup)config.GetSectionGroup("system.net/mailSettings");

                if (settings == null || settings.Smtp.From == null || settings.Smtp.From == string.Empty)
                    throw new ConfigurationErrorsException(
                        "No system.net/mailSettings section found or settings/mailSettings/smtp from address is empty");


                var body = GetBodyAsString(email, newPassword);
                var mail = new MailMessage(
                     string.Format("{0} <{1}>", "BS FileManager", settings.Smtp.Network.UserName),
                     email,
                     "BS FileManager",
                     body);
                var smtpclient = new SmtpClient(settings.Smtp.Network.Host, settings.Smtp.Network.Port);
                smtpclient.EnableSsl = settings.Smtp.Network.EnableSsl;
                smtpclient.UseDefaultCredentials = false;
                smtpclient.Credentials = new NetworkCredential(settings.Smtp.Network.UserName, settings.Smtp.Network.Password);
                smtpclient.Timeout = 100000;
                smtpclient.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtpclient.Send(mail);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        private string GetBodyAsString(string email, string newPassword)
        {
            var strings = new StringBuilder();

            strings.AppendLine("Email : " + email);
            strings.AppendLine("Password : " + newPassword);


            return strings.ToString();
        }
    }
}
