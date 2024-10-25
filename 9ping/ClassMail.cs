using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Mail;
using System.Windows.Forms;
using System.Timers;

namespace ninePing
{
    class ClassMail
    {
        public static MailMessage mail;
        public static SmtpClient SmtpClient1;
        public static string CreateMail(int HostID, bool HostIsDown)
        {
            string HostStatus;
            if (HostIsDown)
                HostStatus = "down";
            else
                HostStatus = "up";

            // Check Mail per X time limit
            if (HostIsDown){
                TimeSpan ts =DateTime.Now - GlobalMailStatus.MailSentMinute[HostID];
                if (ts.Minutes > 0)
                {
                    // It's a new Minute, cleanning vars
                    GlobalMailStatus.MailSentMinute[HostID] = DateTime.Now;
                    GlobalMailStatus.MailSentPerMinute[HostID] = 1;
                }
                else
                {
                    if (++GlobalMailStatus.MailSentPerMinute[HostID]>GlobalConfig.Mail.MaxPerMinute)
                        return "Host is down, but mail did not sent: Mails per minute limit reached(" + GlobalConfig.Mail.MaxPerMinute.ToString() + ")";
                }

                if (ts.Hours > 0)
                {
                    // It's a new hour, cleanning vars
                    GlobalMailStatus.MailSentHour[HostID] = DateTime.Now;
                    GlobalMailStatus.MailSentPerHour[HostID] = 1;
                }
                else
                {
                    if (++GlobalMailStatus.MailSentPerHour[HostID] > GlobalConfig.Mail.MaxPerHour)
                    return "Host is down, but mail did not sent: Mails per Hour limit reached(" + GlobalConfig.Mail.MaxPerHour.ToString() + ")";
                }
            }


            string subject = ConvertSpecialWords(GlobalConfig.Mail.MailSubject, HostID, HostStatus);
            string body = ConvertSpecialWords(GlobalConfig.Mail.MailBody, HostID, HostStatus);

            bool sentResult = send(
                GlobalConfig.Mail.MailFrom,
                GlobalConfig.Mail.MailTo,
                subject,
                body,
                GlobalConfig.Mail.SMTPServer,
                GlobalConfig.Mail.SMTPServerPort,
                GlobalConfig.Mail.SMTPUser,
                GlobalConfig.Mail.SMTPPass,
                GlobalConfig.Mail.SMTPSSL,
                false, HostID, HostIsDown);
            if (sentResult)
            {
                return "Host is " + HostStatus + ", mail sent";
            }
            else
            {
                return "Host is " + HostStatus + ", !!!!! mail sent failed, Wrong Mail config or no connection to SMTP server !!!!!";
            }
        }
        public static string ConvertSpecialWords(string str, int HostID, string HostStatus)
        {
            // Special words:
            // %HOST_ID%  %HOST_IP% %HOST_NAME%
            // %HOST_STATUS% %DATE% %PL_COUNT%

            str = str.Replace("%HOST_ID%", HostID.ToString());
            str = str.Replace("%HOST_IP%", GlobalConfig.Hosts.IP[HostID]);
            str = str.Replace("%HOST_NAME%", GlobalConfig.Hosts.Name[HostID]);
            str = str.Replace("%HOST_STATUS%", HostStatus);
            str = str.Replace("%DATE%", Convert.ToString(DateTime.Now));
            str = str.Replace("%PL_COUNT%", Convert.ToString(GlobalPingStatus.PingContinuesPL[HostID]));
            return str;
        }

        public static bool send(string MailFrom,string MailTo,string MailSubject,string MailBody,
            string SMTPServer,int SMTPServerPort, string SMTPUser,string SMTPPass,bool SMTPSSL, 
            bool test, int HostID=0, bool HostIsDown=false)
        {
            if (
                String.IsNullOrEmpty(MailFrom)  ||
                String.IsNullOrEmpty(MailTo)    ||
                String.IsNullOrEmpty(MailSubject) ||
                String.IsNullOrEmpty(MailBody)  ||
                String.IsNullOrEmpty(SMTPServer) ||
                SMTPServerPort == 0             ||
                String.IsNullOrEmpty(SMTPUser)  ||
                String.IsNullOrEmpty(SMTPPass)
                )
            {
                if (test)
                {
                    MessageBox.Show("Error: You must fill all Email & SMTP files");
                    return false;
                }
                else
                {
                    return false;
                }
            }
                
            try
            {
                mail = new MailMessage();
                SmtpClient1 = new SmtpClient(SMTPServer);

                mail.From = new MailAddress(MailFrom);
                string[] MailToArr = MailTo.Split(';');
                foreach (string MailToaddress in MailToArr){
                    mail.To.Add(MailToaddress);
                }
                mail.Subject = MailSubject;
                mail.Body = MailBody;

                SmtpClient1.Port = SMTPServerPort;
                SmtpClient1.Credentials = new System.Net.NetworkCredential(SMTPUser, SMTPPass);
                SmtpClient1.EnableSsl = SMTPSSL;

                SmtpClient1.Send(mail);
                
                if (test)
                    MessageBox.Show("Email Sent");
            }
            
            catch (Exception ex)
            {
                if (test)
                    MessageBox.Show(ex.ToString());
            }
            return true;
        }
    }
}
