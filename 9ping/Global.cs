using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ClassINI;
//using System.Windows.Forms;

namespace ninePing
{
    class GlobalParam
    {
        public static bool AppVersionCurrentCheckAuto;
        public static string AppVersion = Functions.GetVersion();
        public static string AppVersionRevision = Functions.GetVersionRevision();
        public static string AppLatestVersion="";
        public static int ReCheck = 0;
    }
    class GlobalAction
    {
        public static string LastAction;
        public static string LastActionDid; // This var do not delete his value
    }
    class GlobalPingStatus
    {
        public static Boolean[] PingIsRunning = new Boolean[9];
        public static Boolean PingStopClicked =true;
        public static int[] PingPL = new int[9];
        public static int[] PingOK = new int[9];

        public static int[] PingContinuesPL = new int[9];
        public static Boolean[] PingContinuesPLMailSent = new Boolean[9];
    }
    class GlobalMailStatus
    {
        public static int[] MailSentPerMinute = new int[9];
        public static int[] MailSentPerHour = new int[9];
        public static DateTime[] MailSentMinute = new DateTime[9];
        public static DateTime[] MailSentHour = new DateTime[9];
    }

    class GlobalBookmarks
    {
        public static List<string> Bookmarks = new List<string>();
        //public static string[] Bookmarks = new string[0];
    }

    class GlobalConfig
    {
        public static string CurrentProfileName;
        public static bool AutoCheckForUpdate;

        public static class Path
        {
            public static string AppPath = AppDomain.CurrentDomain.BaseDirectory;
            public static string ProfilesPath=AppPath + @"profiles\";
            public static IniFile iniSystemFile=new IniFile(AppPath + @"9ping.ini");

            public static string CurrentProfilePath;
            public static IniFile iniConfigFile;
        }
        public static void InitPath()
        {
            Path.CurrentProfilePath = Path.ProfilesPath + CurrentProfileName + @"\";
            Path.iniConfigFile = new IniFile(Path.CurrentProfilePath + @"config.ini");
        }
        
        public static class Hosts
        {
            public static string[] IP = new string[9];
            public static string[] Name = new string[9];
            public static bool[] Enabled = new bool[9];
        }
     
        public static class Ping
        {
            public static int PingTimeDelta;
            public static int PingTimeout;
        }
        public static class General
        {
            public static bool PingAutoStart;
            public static bool AutoScroll;
            public static bool PlaySounds;
            public static bool ShowTime;
            public static bool LogToFiles;
            public static bool PingResultsViewMinimal;
            public static bool PingResultsViewCompact; 
            public static bool PingResultsViewBasic;
            public static bool PingResultsViewDetailed;
        }
        public static class Mail
        {
            public static string MailFrom;
            public static string MailTo;
            public static string MailSubject;
            public static string MailBody;
            public static string SMTPServer;
            public static int SMTPServerPort;
            public static string SMTPUser;
            public static string SMTPPass;
            public static bool SMTPSSL;
            public static bool MailAlerts;
            public static int SendAfter;
            public static int MaxPerMinute;
            public static int MaxPerHour;
        }

        public static class Default
        {
            public static class Ping
            {
                public static int PingTimeDelta=1000;
                public static int PingTimeout=3000;
            }
            public static class General
            {
                public static bool PingAutoStart = false;
                public static bool AutoScroll= true;
                public static bool PlaySounds= true;
                public static bool ShowTime=   true;
                public static bool LogToFiles= true;
                public static bool PingResultsViewMinimal = true;
                public static bool PingResultsViewCompact = false;
                public static bool PingResultsViewBasic=false;
                public static bool PingResultsViewDetailed=false;
            }
            public static class Mail
            {
                // Special words:
                // %HOST_ID%  %HOST_IP% %HOST_NAME%
                // %HOST_STATUS% %DATE% %PL_COUNT%

                public static string MailFrom = "";
                public static string MailTo = "";
                public static string MailSubject = "Host  %HOST_NAME% is %HOST_STATUS%";
                public static string MailBody = "Host %HOST_ID%-%HOST_NAME%(%HOST_IP%) is %HOST_STATUS%\n" +
                                                "At %DATE%, lost - %PL_COUNT% packets\n" +
                                                "\n" +
                                                "Message sent from 9Ping application\n" +
                                                "http://www.sharontools.com\n";
                public static string SMTPServer = "smtp.gmail.com";
                public static int SMTPServerPort = 587;
                public static string SMTPUser = "";
                public static string SMTPPass = "";
                public static bool SMTPSSL = true;
                public static bool MailAlerts = false;
                public static int SendAfter = 5;
                public static int MaxPerMinute = 2;
                public static int MaxPerHour = 20;

            }
        }
        public static void InitSystem()
        {
            string str;
            str = Path.iniSystemFile.IniReadValue("Profile", "Name");
            if (str.Length > 0)
                CurrentProfileName = str;
            else
                CurrentProfileName = "Default";

            str = Path.iniSystemFile.IniReadValue("Program", "AutoCheckForUpdate");
            if (str.Length > 0)
                AutoCheckForUpdate =Convert.ToBoolean(str);
            else
                AutoCheckForUpdate = true;
        }
        public static void Init(){
            string str;
            InitPath();

            for (int i = 0; i < 9; i++)
            {
                Hosts.IP[i] = Path.iniConfigFile.IniReadValue("Hosts", "IP-" + Convert.ToString(i));
                Hosts.Name[i] = Path.iniConfigFile.IniReadValue("Hosts", "Name-" + Convert.ToString(i));

                str = Path.iniConfigFile.IniReadValue("Hosts", "Enabled-" + Convert.ToString(i));
                if (str.Length > 0)
                    Hosts.Enabled[i] = Convert.ToBoolean(str);
                else
                    Hosts.Enabled[i] = true;
             }

            str = Path.iniConfigFile.IniReadValue("Ping", "PingTimeDelta");
            if (str.Length > 0)
                Ping.PingTimeDelta = Convert.ToInt32(str);
            else
                Ping.PingTimeDelta = Default.Ping.PingTimeDelta;

            str = Path.iniConfigFile.IniReadValue("Ping", "PingTimeout");
            if (str.Length > 0)
                Ping.PingTimeout = Convert.ToInt32(str);
            else
                Ping.PingTimeout = Default.Ping.PingTimeout;


            str = Path.iniConfigFile.IniReadValue("General", "PingAutoStart");
            if (str.Length > 0)
                General.PingAutoStart = Convert.ToBoolean(str);
            else
                General.PingAutoStart = Default.General.PingAutoStart;

            str = Path.iniConfigFile.IniReadValue("General", "AutoScroll");
            if (str.Length > 0)
                General.AutoScroll = Convert.ToBoolean(str);
            else
                General.AutoScroll = Default.General.AutoScroll;

            str = Path.iniConfigFile.IniReadValue("General", "PlaySounds");
            if (str.Length > 0)
                General.PlaySounds = Convert.ToBoolean(str);
            else
                General.PlaySounds = Default.General.PlaySounds;

            str = Path.iniConfigFile.IniReadValue("General", "ShowTime");
            if (str.Length > 0)
                General.ShowTime = Convert.ToBoolean(str);
            else
                General.ShowTime = Default.General.ShowTime;

            str = Path.iniConfigFile.IniReadValue("General", "LogToFiles");
            if (str.Length > 0)
                General.LogToFiles = Convert.ToBoolean(str);
            else
                General.LogToFiles = Default.General.LogToFiles;

            str = Path.iniConfigFile.IniReadValue("General", "PingResultsViewMinimal");
            if (str.Length > 0)
                General.PingResultsViewMinimal = Convert.ToBoolean(str);
            else
                General.PingResultsViewMinimal = Default.General.PingResultsViewMinimal;

            str = Path.iniConfigFile.IniReadValue("General", "PingResultsViewCompact");
            if (str.Length > 0)
                General.PingResultsViewCompact = Convert.ToBoolean(str);
            else
                General.PingResultsViewCompact = Default.General.PingResultsViewCompact;

            str = Path.iniConfigFile.IniReadValue("General", "PingResultsViewDetailed");
            if (str.Length > 0)
                General.PingResultsViewDetailed = Convert.ToBoolean(str);
            else
                General.PingResultsViewDetailed = Default.General.PingResultsViewDetailed;

            str = Path.iniConfigFile.IniReadValue("General", "PingResultsViewBasic");
            if (str.Length > 0)
                General.PingResultsViewBasic = Convert.ToBoolean(str);
            else
                General.PingResultsViewBasic = Default.General.PingResultsViewBasic;

            // ######################### Mail ######################################

            str = Path.iniConfigFile.IniReadValue("Mail", "MailFrom");
            if (str.Length > 0)
                Mail.MailFrom = str;
            else
                Mail.MailFrom = Default.Mail.MailFrom;

            str = Path.iniConfigFile.IniReadValue("Mail", "MailTo");
            if (str.Length > 0)
                Mail.MailTo = str;
            else
                Mail.MailTo = Default.Mail.MailTo;

            str = Path.iniConfigFile.IniReadValue("Mail", "MailSubject");
            if (str.Length > 0)
                Mail.MailSubject = str;
            else
                Mail.MailSubject = Default.Mail.MailSubject;

            str = Path.iniConfigFile.IniReadValue("Mail", "MailBody");
            if (str.Length > 0)
                Mail.MailBody = str.Replace('|','\n');
            else
                Mail.MailBody = Default.Mail.MailBody;

            str = Path.iniConfigFile.IniReadValue("Mail", "SMTPServer");
            if (str.Length > 0)
                Mail.SMTPServer = str;
            else
                Mail.SMTPServer = Default.Mail.SMTPServer;

            str = Path.iniConfigFile.IniReadValue("Mail", "SMTPServerPort");
            if (str.Length > 0)
                Mail.SMTPServerPort = Convert.ToInt16(str);
            else
                Mail.SMTPServerPort = Default.Mail.SMTPServerPort;

            str = Path.iniConfigFile.IniReadValue("Mail", "SMTPUser");
            if (str.Length > 0)
                Mail.SMTPUser = str;
            else
                Mail.SMTPUser = Default.Mail.SMTPUser;

            str = Path.iniConfigFile.IniReadValue("Mail", "SMTPPass");
            if (str.Length > 0)
                Mail.SMTPPass = System.Text.Encoding.Default.GetString(Convert.FromBase64String(str));
            else
                Mail.SMTPPass = Default.Mail.SMTPPass;

            str = Path.iniConfigFile.IniReadValue("Mail", "SMTPSSL");
            if (str.Length > 0)
                Mail.SMTPSSL = Convert.ToBoolean(str);
            else
                Mail.SMTPSSL = Default.Mail.SMTPSSL;

            str = Path.iniConfigFile.IniReadValue("Mail", "MailAlerts");
            if (str.Length > 0)
                Mail.MailAlerts = Convert.ToBoolean(str);
            else
                Mail.MailAlerts = Default.Mail.MailAlerts;

            str = Path.iniConfigFile.IniReadValue("Mail", "SendAfter");
            if (str.Length > 0)
                Mail.SendAfter = Convert.ToInt32(str);
            else
                Mail.SendAfter = Default.Mail.SendAfter;

            str = Path.iniConfigFile.IniReadValue("Mail", "MaxPerMinute");
            if (str.Length > 0)
                Mail.MaxPerMinute = Convert.ToInt32(str);
            else
                Mail.MaxPerMinute = Default.Mail.MaxPerMinute;

            str = Path.iniConfigFile.IniReadValue("Mail", "MaxPerHour");
            if (str.Length > 0)
                Mail.MaxPerHour = Convert.ToInt32(str);
            else
                Mail.MaxPerHour = Default.Mail.MaxPerHour;
        }
    }
}
