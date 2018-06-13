using SharedClass.Enums;
using SharedClass.Resources;
using System;
using System.Web;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;


namespace SharedClass
{
    public static class CommonFunctions
    {
        /// <summary>
        /// Sends all the mails from here
        /// </summary>
        /// <param name="mailType"></param>
        /// <param name="subject"></param>
        /// <param name="toAddress"></param>
        /// <param name="toName"></param>
        /// <param name="fromAddress"></param>
        /// <param name="fromName"></param>
        /// <param name="mailBody"></param>
        /// <param name="newGuid"></param>
        /// <param name="userName"></param>
        /// <param name="pswd"></param>
        /// <returns></returns>
        public static bool SendMail(int mailType, string subject, string toAddress, string toName, string fromAddress, string fromName, string mailBody, string newGuid = null, string userName = null, string pswd = null, Attachment file = null)
        {
            AlternateView plainView = null;
            string link = null;

            //Create link for password reset                     
            Uri uri = HttpContext.Current.Request.Url;

            if (uri.ToString().Contains("localhost"))
            {
                if (mailType == (int)MailType.mail_pswdReset)
                {
                    link = uri.Scheme + "://" + uri.Host + ":" + uri.Port + "//Account//Login//Index?id=" + newGuid;                  
                }
                if (mailType == (int)MailType.mail_login)
                {
                    link = uri.Scheme + "://" + uri.Host + ":" + uri.Port;
                }
            }
            else
            {
                if (mailType == (int)MailType.mail_pswdReset)
                {
                    link = uri.Scheme + "://" + uri.Host + "//Account//Login//ChangePassword?id=" + newGuid;
                }
                if (mailType == (int)MailType.mail_login)
                {                   
                    link = uri.Scheme + "://" + uri.Host;
                }
            }

            //Send email here.
            #region Send Email
            //Set subject
            MailMessage msg = new MailMessage { Subject = subject };
            //To address
            msg.To.Add(new MailAddress(toAddress, toName));
            //From noreply@forumresearch.com
            msg.From = new MailAddress(fromAddress, fromName);

            if (mailType == (int)MailType.mail_pswdReset)
            {
                plainView = AlternateView.CreateAlternateViewFromString(String.Format(mailBody, link), null, "text/plain");
            }

            if (mailType == (int)MailType.mail_login)
            {
                plainView = AlternateView.CreateAlternateViewFromString(String.Format(mailBody, link, userName, pswd), null, "text/plain");
            }

            if (mailType == (int)MailType.mail_help)
            {
                plainView = AlternateView.CreateAlternateViewFromString(mailBody, null, "text/plain");

                if (file != null)
                {
                    msg.Attachments.Add(file);
                }
            }

            msg.AlternateViews.Add(plainView);

            #endregion

            try
            {
                //Settings in web.config
                SmtpClient smtp = new SmtpClient();
                smtp.Send(msg);
                return true; //Success
            }
            catch (Exception ex)
            {
                //CommonFunctions.LogErrors("TarionForum.SendMail", "Error Sending Email", ErrorTypes.General, ex);
                return false; //Critical error!                               
            }
            finally
            {
                msg.Dispose();
            }
        }

        /// <summary>
        /// Checks the password meets all password requirements
        /// </summary>
        /// <param name="password"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static bool IsPasswordValid(string password, out string message)
        {
            message = "";

            if (string.IsNullOrEmpty(password))
            {
                message = AccountResource.Error_NoPasswordSpecified;
                return false;
            }

            var isValid = (!String.IsNullOrEmpty(password) && password.Length >= 6 && password.Length <= 12);

            if (!isValid)
            {
                message = AccountResource.Error_PasswordLengthNotMet;
                return false;
            }

            int numbers = (Regex.Matches(password, @"[0-9]")).Count;
            int uppers = (Regex.Matches(password, @"[A-Z]")).Count;
            int lowers = (Regex.Matches(password, @"[a-z]")).Count;

            bool complexityMet =
                numbers > 0 &&
                uppers > 0 &&
                lowers > 0;

            if (!complexityMet)
            {
                message = AccountResource.Error_PasswordNotComplex;
                return false;
            }

            return isValid;
        }


        /// <summary>
        /// Generating temporay password
        /// </summary>
        /// <returns></returns>
        public static string CreatePassword()
        {
            try
            {
                Random Random = new Random();
                int seed = Random.Next(1, int.MaxValue);
                const string allowedChars = "abcdefghijkmnopqrstuvwxyzABCDEFGHJKLMNOPQRSTUVWXYZ0123456789";

                var chars = new char[6];
                var rd = new Random(seed);

                for (var i = 0; i < 6; i++)
                {
                    chars[i] = allowedChars[rd.Next(0, allowedChars.Length)];
                }
                return new string(chars);
            }
            catch (Exception ex)
            {
                //CommonFunctions.LogErrors("TarionForum.AddUser", HttpContext.GetLocalResourceObject("~/Users/AddUser.aspx", "err_PswdCreation").ToString(), ErrorTypes.General, ex);
                return null;
            }

        }


    }
}
