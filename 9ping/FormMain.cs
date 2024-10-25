using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Timers;
using System.IO;


namespace ninePing
{

    public partial class FormMain : Form
    {

        private static System.Timers.Timer[] TimerArr = new System.Timers.Timer[9];
        private static ToolStrip[] toolStripArr;
        private static ToolStripLabel[] toolStriplblArr;
        private static RichTextBox[] richTextBoxArr;
        private static Control[] richTextBoxOriginalParent = new Control[9];

        private static ToolStripLabel[] toolStriplblPLArr;
        private static ToolStripLabel[] toolStriplblOKArr;
        private static ToolStripLabel[] toolStriplblPrArr;
        private static ToolStripButton[] toolStripbtnResizeArr;

        private static string[] datafilesPath = new string[9];
        private static string[] datafilesPathTemp = new string[9];

        private static StreamReader[] datafilesReader = new StreamReader[9];
        private static StreamWriter[] datafilesWriter = new StreamWriter[9];
        private static bool CurrentProfileInUse = false;
       
        public FormMain()
        {
            InitializeComponent();
        }
        private void FormMain_Load(object sender, EventArgs e)
        {
            toolStripArr= new ToolStrip[] { toolStrip1, toolStrip2, toolStrip3, toolStrip4, toolStrip5, toolStrip6, toolStrip7, toolStrip8, toolStrip9 };
            toolStriplblArr = new ToolStripLabel[] { TSlbl1, TSlbl2, TSlbl3, TSlbl4, TSlbl5, TSlbl6, TSlbl7, TSlbl8, TSlbl9 };
            toolStriplblPLArr = new ToolStripLabel[] { TSlblPL1, TSlblPL2, TSlblPL3, TSlblPL4, TSlblPL5, TSlblPL6, TSlblPL7, TSlblPL8, TSlblPL9 };
            toolStriplblOKArr = new ToolStripLabel[] { TSlblOK1, TSlblOK2, TSlblOK3, TSlblOK4, TSlblOK5, TSlblOK6, TSlblOK7, TSlblOK8, TSlblOK9 };
            toolStriplblPrArr = new ToolStripLabel[] { TSlblPr1, TSlblPr2, TSlblPr3, TSlblPr4, TSlblPr5, TSlblPr6, TSlblPr7, TSlblPr8, TSlblPr9 };
            toolStripbtnResizeArr = new ToolStripButton[] { TSBtnResize1, TSBtnResize2, TSBtnResize3, TSBtnResize4, TSBtnResize5, TSBtnResize6, TSBtnResize7, TSBtnResize8, TSBtnResize9 };
            
            richTextBoxArr = new RichTextBox[] { richTextBox1, richTextBox2, richTextBox3, richTextBox4, richTextBox5, richTextBox6, richTextBox7, richTextBox8, richTextBox9 };

            System.Timers.Timer Timer1;
            
            //Disable JavaScript error messagebox (if occurs)
            webBrowserVersionCheck.ScriptErrorsSuppressed = true;
            
            // Set system env (Load last used Profile name)
            GlobalConfig.InitSystem();

            // verify that folders exist, if not, create them
            Functions.CheckDirectory(@"Profiles");
            Functions.CheckDirectory(@"Profiles\Default");
            GlobalConfig.Init();

            GlobalAction.LastAction = "FormIsLoading";
            UpdateStatusButtons();

            for (int i = 0; i < 9; i++)
            {
                richTextBoxOriginalParent[i] = richTextBoxArr[i].Parent;
                if (GlobalConfig.General.PingResultsViewMinimal | GlobalConfig.General.PingResultsViewCompact)
                    richTextBoxArr[i].WordWrap = true;
                else
                    richTextBoxArr[i].WordWrap = false;
            }

            Timer1 = new System.Timers.Timer(10000);
            Timer1.AutoReset = false;
            Timer1.Elapsed += new ElapsedEventHandler(AftherFormLoadConpleted);
            Timer1.Interval = 1000;
            Timer1.Enabled = true;

            toolStripMain.Enabled = false;
        }
        private void AftherFormLoadConpleted(object sender, EventArgs e)
        {
            // Writing data initial setup
            initDataFilesPath();
            if (!LoadDataFilesToWindowsCheck())
                ProfileInUse();
            if (!OpenDataFilesForWriting())
                ProfileInUse();

            
            this.Invoke((MethodInvoker)delegate
            {
                toolStripMain.Enabled = true;
            });

            // Sceduale version check 
            System.Timers.Timer Timer1;
            Timer1 = new System.Timers.Timer(100000000);
            Timer1.AutoReset = true;
            Timer1.Elapsed += new ElapsedEventHandler(AutoWebBrowserVersionCheck);
            Timer1.Interval = 86400000;
            Timer1.Enabled = true;
            
            // run now version check 
            AutoWebBrowserVersionCheck(sender, e);

        }
        private void AutoWebBrowserVersionCheck(object sender, EventArgs e)
        {
            GlobalParam.AppVersionCurrentCheckAuto = true;
            webBrowserVersionCheck.Navigate("http://version-check.sharontools.com/9ping/?type=auto&recheck=" + (GlobalParam.ReCheck++) + "&v=" + Functions.GetVersion());
            
        }

        private void webBrowserVersionCheck_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            VersionCheck();
        }
        private void VersionCheck()
        {
            if (!GlobalConfig.AutoCheckForUpdate)
                return;
            if (!(webBrowserVersionCheck.DocumentTitle.Contains("Tools")))
            {// Page can't open for some reason
                if (GlobalParam.AppVersionCurrentCheckAuto == false)
                {
                    MessageBox.Show("Error getting latest version," + Environment.NewLine + " Please check your internet connection or try again later", "Check for update", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                    return;
                }
                else
                {
                    return;
                }
            }
            this.Invoke((MethodInvoker)delegate
            {
                try { GlobalParam.AppLatestVersion = webBrowserVersionCheck.Document.Body.InnerText; }
                catch (Exception) { }
            });
            if (GlobalParam.AppVersionCurrentCheckAuto == false)
            { //  Check version clicked
                FormVersionCheck formVersionCheck = new FormVersionCheck();
                formVersionCheck.ShowDialog();
            }
            else
            { //  Auto check at program start
                if (Functions.CompareVersions(GlobalParam.AppLatestVersion)=="upgrade")
                {
                    FormVersionCheck formVersionCheck = new FormVersionCheck();
                    formVersionCheck.ShowDialog();
                }
            }
        }
        private void ProfileInUse()
        {
            MessageBox.Show("Can't open data files" + Environment.NewLine + "Writing / Reading to files disabled"  + Environment.NewLine + "You can use other profiles" , "Profile " + GlobalConfig.CurrentProfileName, MessageBoxButtons.OK, MessageBoxIcon.Information);
            CurrentProfileInUse = true;
        }
        private bool LoadDataFilesToWindowsCheck()
        {
            DialogResult dialogResult = DialogResult.Cancel;
            bool questionAsked = false;
            FileInfo fileInfo;

            for (int i = 0; i < 9; i++) {
                if (GlobalConfig.General.PingAutoStart == true && GlobalAction.LastActionDid=="FormIsLoading")
                {
                    UpdateWindowsWithOuput("message", i, "", "", "", "## Pings auto starting, Old data file did not checked, (Ping-Auto-Start enabled) ## ");
                }
                else
                {

                    if (File.Exists(datafilesPath[i]))
                    {
                        fileInfo = new FileInfo(datafilesPath[i]);
                        if (fileInfo.Length > 30)
                        {
                            if (questionAsked == false)
                            {
                                questionAsked = true;
                                dialogResult = FormBetterDialog.ShowDialog("Profile " + GlobalConfig.CurrentProfileName,
                                    "Load old data",
                                    "You can load ping results from last running," + Environment.NewLine + "Do you want to Load ping result ?",
                                    "Yes", "No", "Delete old Data", imageList1.Images[0]);

                            }
                            //MessageBox.Show(dialogResult.ToString());
                            if (dialogResult == DialogResult.No || dialogResult == DialogResult.Cancel || dialogResult == DialogResult.Ignore) // Do not load old data
                            {
                                UpdateWindowsWithOuput("message", i, "", "", "", "## Data file exist but did not loaded (" + (fileInfo.Length / 1024) + " KB) ##");
                            }
                            else if (dialogResult == DialogResult.Abort) // Delete old data file
                            {
                                File.Delete(datafilesPath[i]);
                                UpdateWindowsWithOuput("message", i, "", "", "", "## Data file deleted ##");
                            }
                            else // Load old data from files
                            {
                                if (!LoadDataFileToWindow(i))
                                    return false;
                            }
                        }
                    }
                }
                
            }
            if (GlobalConfig.General.PingAutoStart == true && GlobalAction.LastActionDid=="FormIsLoading")
            {
                object sender = null;
                EventArgs e = null;
                this.Invoke((MethodInvoker)delegate
                {
                    startAllToolStripMenuItem_Click(sender, e);
                });
            }
            return true;
        }
        private bool LoadDataFileToWindow(int i){
            DialogResult dialogResult = DialogResult.Cancel;
            string line, action, HostInFile;
            string[] arr;

            if (CurrentProfileInUse)
            {
                return false;
            }
            try
            {
                datafilesReader[i] = new StreamReader(datafilesPath[i]);
            }
            catch (IOException)
            {
                return false;
            }
            this.Invoke((MethodInvoker)delegate
            {
                richTextBoxArr[i].Visible = false;
            });
            HostInFile = datafilesReader[i].ReadLine();
            if (HostInFile != GlobalConfig.Hosts.IP[i])// Data file is of another IP
            {
                dialogResult = FormBetterDialog.ShowDialog("Profile " + GlobalConfig.CurrentProfileName,
                                "Load old data at Window " + i,
                                "Data file is of Host - " + HostInFile + Environment.NewLine + "At profile " + GlobalConfig.CurrentProfileName + " is for host- " + GlobalConfig.Hosts.IP[i] + Environment.NewLine +
                                "Load anyway ?",
                                "Yes", "No", "Delete This file", imageList1.Images[0]);

                if (dialogResult == DialogResult.No || dialogResult == DialogResult.Cancel || dialogResult == DialogResult.Ignore)// Ignore data file
                {
                    datafilesReader[i].Close();
                    this.Invoke((MethodInvoker)delegate { richTextBoxArr[i].Visible = true; });
                    UpdateWindowsWithOuput("message", i, "", "", "", "## Data file exist but did not loaded (Data file is not of - " + GlobalConfig.CurrentProfileName + ") ##");
                }
                else if (dialogResult == DialogResult.Abort) // Delete data file
                {
                    this.Invoke((MethodInvoker)delegate { richTextBoxArr[i].Visible = true; });
                    datafilesReader[i].Close();
                    File.Delete(datafilesPath[i]);
                    OpenDataFilesForWriting(i);
                    UpdateWindowsWithOuput("message", i, "", "", "", "## Data file deleted (Data file is not of - " + GlobalConfig.CurrentProfileName + ") ##");
                }
            }
            int line_num = 0;
            while ((line = datafilesReader[i].ReadLine()) != null)
            {
                line_num++;
                arr = line.Split(',');
                if (arr.Length == 4)
                {
                    if (arr[1] == "P")
                        action = "load";
                    else if (arr[1] == "B")
                        action = "loadBookmark";
                    else
                        action = "";
                    UpdateWindowsWithOuput(action, i, arr[0], "", arr[2], arr[3]);
                }
                else
                {
                    UpdateWindowsWithOuput("message", i, "", "", "", "## Data file is corrupted (File - " + datafilesPath[i] + " at line - " + line_num + ") ##", 1);
                    break;
                }

            }
            datafilesReader[i].Close();
            this.Invoke((MethodInvoker)delegate // Update windows
            {
                richTextBoxArr[i].Visible = true;
                // update ping summery
                toolStriplblOKArr[i].Text = Convert.ToString(GlobalPingStatus.PingOK[i]);
                toolStriplblPLArr[i].Text = Convert.ToString(GlobalPingStatus.PingPL[i]);

                if (GlobalPingStatus.PingPL[i] > 0)
                {
                    toolStriplblPLArr[i].ForeColor = Color.Red;
                    toolStriplblPrArr[i].Text = Convert.ToString((GlobalPingStatus.PingPL[i] * 100) / (GlobalPingStatus.PingOK[i] + GlobalPingStatus.PingPL[i]));

                }
            });
            UpdateWindowsWithOuput("message", i, "", "", "", "## Old Data Reloaded ##");
            return true;
        }      
        private void initDataFilesPath()
        {
            for (int i = 0; i < 9; i++)
            {
                datafilesPath[i] = GlobalConfig.Path.CurrentProfilePath + "Data"+Convert.ToString(i)+".csv";
                datafilesPathTemp[i] = GlobalConfig.Path.CurrentProfilePath + "_Data" + Convert.ToString(i) + ".tmp";
            }
        }

        private bool OpenDataFilesForWriting(int j=-1)
        {
            if (CurrentProfileInUse)
                return false;
            int first,last;
            if (j >= 0)
                first =last= j;
            else
            {
                first = 0;
                last = 8;
            }
            for (int i = first; i <= last; i++)
            {
                if (datafilesWriter[i] != null)
                    datafilesWriter[i].Close();

                if (File.Exists(datafilesPath[i]))
                {
                    try
                    {
                        datafilesWriter[i] = File.AppendText(datafilesPath[i]);
                    }
                    catch (IOException)
                    {
                        return false;
                    }
                }
                else
                {
                    datafilesWriter[i] = new StreamWriter(datafilesPath[i]);
                    datafilesWriter[i].WriteLine(GlobalConfig.Hosts.IP[i]);
                }
                
            }
            return true;
        }
        private void UpdateStatusButtons()
        {
            this.Text = "9Ping - " + GlobalConfig.CurrentProfileName;
            autoScrollToolStripMenuItem.Checked = TSbtnAutoScroll.Checked = GlobalConfig.General.AutoScroll;
            soundsToolStripMenuItem.Checked = TSbtnSounds.Checked = GlobalConfig.General.PlaySounds;
            showTimeToolStripMenuItem.Checked = TSbtnShowTime.Checked = GlobalConfig.General.ShowTime;
            MailsToolStripMenuItem.Checked = TSbtnMails.Checked = GlobalConfig.Mail.MailAlerts;
            logToFilesToolStripMenuItem.Checked = GlobalConfig.General.LogToFiles;
            basicViewToolStripMenuItem.Checked = basicToolStripMenuItem.Checked = GlobalConfig.General.PingResultsViewBasic;
            minimalViewToolStripMenuItem.Checked = minimalToolStripMenuItem.Checked = GlobalConfig.General.PingResultsViewMinimal;
            detailedViewToolStripMenuItem.Checked = detailedToolStripMenuItem.Checked = GlobalConfig.General.PingResultsViewDetailed;
            compactViewToolStripMenuItem.Checked = compactToolStripMenuItem.Checked = GlobalConfig.General.PingResultsViewCompact;

            if (GlobalConfig.General.PingResultsViewMinimal)
            {
                toolStripDDBViewType.Image = minimalToolStripMenuItem.Image;
                TSbtnShowTime.Enabled = showTimeToolStripMenuItem.Enabled = false;
            }
            else if (GlobalConfig.General.PingResultsViewCompact)
            {
                toolStripDDBViewType.Image = compactToolStripMenuItem.Image;
                TSbtnShowTime.Enabled = showTimeToolStripMenuItem.Enabled = false;
            }
            else if (GlobalConfig.General.PingResultsViewDetailed)
            {
                toolStripDDBViewType.Image = detailedToolStripMenuItem.Image;
                TSbtnShowTime.Enabled = showTimeToolStripMenuItem.Enabled = true;
            }
            else if (GlobalConfig.General.PingResultsViewBasic)
            {
                toolStripDDBViewType.Image = basicToolStripMenuItem.Image;
                TSbtnShowTime.Enabled = showTimeToolStripMenuItem.Enabled = true;
            }
            

            if (GlobalAction.LastAction == "Options" || GlobalAction.LastAction == "AutoScroll" || GlobalAction.LastAction=="FormIsLoading")
            {
                for (int i = 0; i < 9; i++)
                {
                    richTextBoxArr[i].HideSelection = !GlobalConfig.General.AutoScroll;
                    if (GlobalAction.LastAction != "AutoScroll")
                    {
                        if (GlobalConfig.Hosts.Enabled[i] && GlobalConfig.Hosts.IP[i].Length > 0)
                        {
                            if (GlobalConfig.Hosts.Name[i].Length > 0)
                            {
                                toolStriplblArr[i].Text = GlobalConfig.Hosts.Name[i] + " - ";
                                toolStriplblArr[i].ToolTipText = GlobalConfig.Hosts.IP[i];
                            }
                            else
                            {
                                toolStriplblArr[i].Text = GlobalConfig.Hosts.IP[i];
                            }
                        }
                    }
                    
                }
            }
            GlobalAction.LastActionDid = GlobalAction.LastAction;
            GlobalAction.LastAction="";
            
        }
        private void UpdateStatusConfig()
        {
            GlobalConfig.General.AutoScroll = autoScrollToolStripMenuItem.Checked;
            GlobalConfig.General.PlaySounds = soundsToolStripMenuItem.Checked;
            GlobalConfig.General.ShowTime = showTimeToolStripMenuItem.Checked;
            GlobalConfig.General.LogToFiles = logToFilesToolStripMenuItem.Checked;
            GlobalConfig.Mail.MailAlerts = MailsToolStripMenuItem.Checked;
            GlobalConfig.General.PingResultsViewMinimal = minimalViewToolStripMenuItem.Checked;
            GlobalConfig.General.PingResultsViewCompact = compactViewToolStripMenuItem.Checked;
            GlobalConfig.General.PingResultsViewBasic = basicViewToolStripMenuItem.Checked;
            GlobalConfig.General.PingResultsViewDetailed = detailedViewToolStripMenuItem.Checked;

            GlobalConfig.Path.iniConfigFile.IniWriteValue("General", "AutoScroll", Convert.ToString(GlobalConfig.General.AutoScroll));
            GlobalConfig.Path.iniConfigFile.IniWriteValue("General", "PlaySounds", Convert.ToString(GlobalConfig.General.PlaySounds));
            GlobalConfig.Path.iniConfigFile.IniWriteValue("General", "ShowTime", Convert.ToString(GlobalConfig.General.ShowTime));
            GlobalConfig.Path.iniConfigFile.IniWriteValue("General", "LogToFiles", Convert.ToString(GlobalConfig.General.LogToFiles));
            GlobalConfig.Path.iniConfigFile.IniWriteValue("General", "PingResultsViewMinimal", Convert.ToString(GlobalConfig.General.PingResultsViewMinimal));
            GlobalConfig.Path.iniConfigFile.IniWriteValue("General", "PingResultsViewCompact", Convert.ToString(GlobalConfig.General.PingResultsViewCompact));
            GlobalConfig.Path.iniConfigFile.IniWriteValue("General", "PingResultsViewBasic", Convert.ToString(GlobalConfig.General.PingResultsViewBasic));
            GlobalConfig.Path.iniConfigFile.IniWriteValue("General", "PingResultsViewDetailed", Convert.ToString(GlobalConfig.General.PingResultsViewDetailed));
            GlobalConfig.Path.iniConfigFile.IniWriteValue("Mail", "MailAlerts", Convert.ToString(GlobalConfig.Mail.MailAlerts));


            for (int i = 0; i < 9; i++)
            {
                richTextBoxArr[i].HideSelection = !GlobalConfig.General.AutoScroll;
                if (!GlobalConfig.General.AutoScroll)
                    richTextBoxArr[i].ScrollToCaret();

            }
        }

        private void OnTimedEvent1(object sender, EventArgs e){OnTimedEvent(0);}
        private void OnTimedEvent2(object sender, EventArgs e){OnTimedEvent(1);}
        private void OnTimedEvent3(object sender, EventArgs e){OnTimedEvent(2);}
        private void OnTimedEvent4(object sender, EventArgs e){OnTimedEvent(3);}
        private void OnTimedEvent5(object sender, EventArgs e){OnTimedEvent(4);}
        private void OnTimedEvent6(object sender, EventArgs e){OnTimedEvent(5);}
        private void OnTimedEvent7(object sender, EventArgs e){OnTimedEvent(6);}
        private void OnTimedEvent8(object sender, EventArgs e){OnTimedEvent(7);}
        private void OnTimedEvent9(object sender, EventArgs e){OnTimedEvent(8);}
        
        private void OnTimedEvent(int i)

        {
            int PingResult=0;
            string PingResultSTR="";
            string MSG="";
            //string action;
            // if ping reply is not yet returned from last ping
            if (GlobalPingStatus.PingIsRunning[i])
                return;

            string HostIP = GlobalConfig.Hosts.IP[i];
            GlobalPingStatus.PingIsRunning[i] = true;
            string Result = PingClass.pingHost(HostIP);
            GlobalPingStatus.PingIsRunning[i] = false;

            // If stop button clicked before ping reply
            if (GlobalPingStatus.PingStopClicked)
                return;

            if (!int.TryParse(Result, out PingResult))
            {
                // Ping Error
                MSG = Result;
            }
            else
            {
                // Ping OK
                PingResultSTR = Convert.ToString(PingResult);
            }

            UpdateWindowsWithOuput("run", i, "", HostIP, PingResultSTR, MSG);
        }
        public void UpdateWindowsWithOuput(string action, int i, string CurrentTime, string HostIP, string PingResultSTR, string MSG, int severity=0)
        {
            // ## Messages severity ##
            // severity 0 - No severity
            // severity 1 - Error
            // severity 2 - Minor Error
            // severity 3 - Importent info
            
            string type="",str,timeSeperator="";
            int numRetries;
            //RichTextBox RichTextBoxToUpdate;
            //if (action == "Load")
            //    RichTextBoxTemp.AppendText

            if (action == "run")
                type = "P";
            else if (action == "AddBookmark")
                type = "B";

            
            if (CurrentTime.Length==0)
                CurrentTime = Convert.ToString(DateTime.Now);

            if (!CurrentProfileInUse)
            {
                if (action == "run" || action == "AddBookmark")
                {
                    if (GlobalConfig.General.LogToFiles)
                    {
                        numRetries = 3;
                        do
                        {
                            try
                            {
                                datafilesWriter[i].WriteLine(CurrentTime + "," + type + "," + PingResultSTR + "," + MSG);
                                numRetries = -1;
                            }
                            catch (Exception err)
                            {
                                if (numRetries <= 0)
                                {
                                    MessageBox.Show("Can't write 1 ping Result to data file: " + err.Message + Environment.NewLine + "It's can happend due to refreshing data", "Window " + i, MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    //throw;  // improved to avoid silent failure
                                }
                                else
                                {
                                    numRetries--;
                                    //MessageBox.Show("File close, Waiting..  " + err.Message + Environment.NewLine , "Window " + i, MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    System.Threading.Thread.Sleep(300);
                                }
                            }
                        } while (numRetries > 0);


                        //try { datafilesWriter[i].WriteLine(CurrentTime + "," + type + "," + PingResultSTR + "," + MSG); }
                        //catch (Exception err) { MessageBox.Show("Can't write 1 ping Result to data file: " + err.Message + Environment.NewLine + "It's can happend due to refreshing data","Window " + i,MessageBoxButtons.OK,MessageBoxIcon.Information ); }
                        //finally { }
                    }
                }
            }
            if (!GlobalConfig.General.ShowTime)
            {
                CurrentTime = "";
                timeSeperator = "";
            }
            else
            {
                timeSeperator = ": ";
            }
            this.Invoke((MethodInvoker)delegate
            {
                if (PingResultSTR.Length>0) // ping OK
                {
                    if (HostIP.Length == 0)// If loading HostIP can be blank
                        HostIP = GlobalConfig.Hosts.IP[i];
                    if (GlobalConfig.General.PingResultsViewMinimal)
                        str = "!";
                    else if (GlobalConfig.General.PingResultsViewCompact)
                        str = PingResultSTR.PadRight(5, ' ');
                    else if (GlobalConfig.General.PingResultsViewBasic)
                        str = CurrentTime + timeSeperator + PingResultSTR + "ms" + Environment.NewLine;
                    else if (GlobalConfig.General.PingResultsViewDetailed)
                        str = CurrentTime + timeSeperator + "Reply from " + HostIP + ": time=" + PingResultSTR + "ms" + Environment.NewLine;
                    else if (GlobalConfig.General.PingResultsViewCompact)
                        str = CurrentTime + timeSeperator + "Reply from " + HostIP + ": time=" + PingResultSTR + "ms" + Environment.NewLine;
                    else
                        str = "Error in PingResultsView " + Environment.NewLine;
                    
                    richTextBoxArr[i].AppendText(str);
                    GlobalPingStatus.PingOK[i]++;
                    if (action == "run")// if "load" i will update oolStriplblOKArr[i].Text at the end
                    {
                        toolStriplblOKArr[i].Text = Convert.ToString(GlobalPingStatus.PingOK[i]);
                        if (GlobalConfig.Mail.MailAlerts)
                        {
                            if (GlobalPingStatus.PingContinuesPLMailSent[i] == true)// Need to send mail Recover alert
                            {
                                GlobalPingStatus.PingContinuesPLMailSent[i] = false;
                                str = ClassMail.CreateMail(i, false);
                                UpdateWindowsWithOuput("message", i, "", "", "", "## " + str + " ##", 2);
                            }
                            GlobalPingStatus.PingContinuesPL[i] = 0;
                            GlobalPingStatus.PingContinuesPLMailSent[i] = false;
                        }
                    }
             
                }
                else if (action == "run" || action == "load")// Ping error
                {
                    if (HostIP.Length == 0)// If loading HostIP can be blank
                        HostIP = GlobalConfig.Hosts.IP[i];
                    if (GlobalConfig.General.PingResultsViewMinimal)
                        str = ".";
                    else if (GlobalConfig.General.PingResultsViewCompact)
                        str = "[" + MSG + "]";
                    else if (GlobalConfig.General.PingResultsViewBasic)
                        str = CurrentTime + timeSeperator + MSG + Environment.NewLine;
                    else if (GlobalConfig.General.PingResultsViewDetailed)
                        str = CurrentTime + timeSeperator + MSG + Environment.NewLine;
                    else
                        str = "Error in PingResultsView " + Environment.NewLine;

                    GlobalPingStatus.PingPL[i]++;
                    
                    richTextBoxArr[i].SelectionColor = Color.Red;

                    richTextBoxArr[i].AppendText(str);
                    richTextBoxArr[i].SelectionColor = Color.White;

                    if (action == "run")
                    {
                        if (TSbtnSounds.Checked)
                            BackgroundBeep.Beep();
                        toolStriplblPLArr[i].Text = Convert.ToString(GlobalPingStatus.PingPL[i]);
                        if (GlobalPingStatus.PingPL[i] == 1)// change color at first PL !
                            toolStriplblPLArr[i].ForeColor = Color.Red;
                        if (GlobalConfig.Mail.MailAlerts)
                        {
                            if ((++GlobalPingStatus.PingContinuesPL[i]) >= GlobalConfig.Mail.SendAfter)
                            {
                                if (!(GlobalPingStatus.PingContinuesPLMailSent[i])) // Need to send mail alert
                                {
                                    GlobalPingStatus.PingContinuesPLMailSent[i] = true;
                                    str = ClassMail.CreateMail(i, true);
                                    UpdateWindowsWithOuput("message", i, "", "", "", "## " + str + " ##", 2);
                                }
                            }
                        }
                    }
                }
                else // message
                {

                    if (GlobalConfig.General.PingResultsViewMinimal || GlobalConfig.General.PingResultsViewCompact)
                        str=Environment.NewLine;
                    else
                        str="";
                    str += CurrentTime + timeSeperator + MSG + Environment.NewLine;
                    if (action == "AddBookmark" || action == "loadBookmark")// User bookmark
                    {
                        richTextBoxArr[i].SelectionColor = Color.Yellow;
                        BuildMenuItems(MSG);
                    }
                    else
                    {
                        if (severity == 1)
                            richTextBoxArr[i].SelectionColor = Color.Red;
                        else if (severity == 2)
                            richTextBoxArr[i].SelectionColor = Color.Brown;
                        else if (severity == 3)
                            richTextBoxArr[i].SelectionColor = Color.Orange;
                        else
                            richTextBoxArr[i].SelectionColor = Color.Green;
                    }
                    
                    richTextBoxArr[i].AppendText(str);
                    richTextBoxArr[i].SelectionColor = Color.White;
                }

                if (action == "run")
                    if (GlobalPingStatus.PingPL[i] > 0)
                        toolStriplblPrArr[i].Text = Convert.ToString((GlobalPingStatus.PingPL[i] * 100) / (GlobalPingStatus.PingOK[i] + GlobalPingStatus.PingPL[i]));
            });
           

        }


        private void BuildMenuItems(string BookmarkName)
        {
            //if (Array.IndexOf(GlobalBookmarks.Bookmarks, BookmarkName)>-1)// BookmarkName allready exists
            if (GlobalBookmarks.Bookmarks.Find(x => x == BookmarkName) !=null)// BookmarkName allready exists
                return;

            ToolStripMenuItem[] items = new ToolStripMenuItem[2];
            int BookmarkIndex = GlobalBookmarks.Bookmarks.Count;
            //Array.Resize( ref GlobalBookmarks.Bookmarks, BookmarkIndex + 1);

            GlobalBookmarks.Bookmarks.Add(BookmarkName);
            //GlobalBookmarks.Bookmarks.()

            items[0] = new ToolStripMenuItem();
            items[0].Name = BookmarkIndex + "_BookmarkToolStripMenuItem";
            items[0].Tag = BookmarkName;
            items[0].Text = BookmarkName;
            items[0].Click += new EventHandler(MenuItemClickHandler);

            goToBookmarkToolStripMenuItem.DropDownItems.Add(items[0]);
        }

        private void MenuItemClickHandler(object sender, EventArgs e)
        {
            ToolStripMenuItem clickedItem = (ToolStripMenuItem)sender;
            //MessageBox.Show(clickedItem.ToString());
            int index;
            string Bookmark;
            string timeSeperator;

            if (!GlobalConfig.General.ShowTime)
                timeSeperator = "";
            else
                timeSeperator = ": ";

            this.ActiveControl = TSTBCaretHome.Control; // remove focus from RichTextBoxes
            for (int i = 0; i < 9; i++)
            {
                if (GlobalConfig.General.AutoScroll)// Cancel AutoScroll if enabled
                    autoScrollToolStripMenuItem_Click(sender, e);

                Bookmark=clickedItem.ToString();
                if (richTextBoxArr[i].TextLength > 0)
                {
                    index = richTextBoxArr[i].Find(timeSeperator + Bookmark + (char)13);
                    if (index > -1)
                    {
                        //richTextBoxArr[i].SelectionStart = index;
                        richTextBoxArr[i].ScrollToCaret();
                        richTextBoxArr[i].SelectionStart=richTextBoxArr[i].TextLength;
                    }
                }
            }
            
        }


        private void TSbtnStart_Click(object sender, EventArgs e)
        {
            startAllToolStripMenuItem_Click(sender, e);
        }

        private void TSbtnStop_Click(object sender, EventArgs e)
        {
            stopAllToolStripMenuItem_Click(sender, e);
        }

        private void TSbtnOptions_Click(object sender, EventArgs e)
        {
            optionsToolStripMenuItem_Click(sender, e);
        }

        

        private void TSbtnClear_Click(object sender, EventArgs e)
        {
            clearAllWindowsToolStripMenuItem_Click(sender, e);
        }
        private void ClearWindow(int i)
        {
            //for (int i = 0; i < 9; i++)
            //{
                richTextBoxArr[i].Text = "";
                toolStriplblPLArr[i].Text = "0";
                toolStriplblOKArr[i].Text = "0";
                toolStriplblPrArr[i].Text = "0";
                toolStriplblPLArr[i].ForeColor = Color.White;
                GlobalPingStatus.PingOK[i] = 0;
                GlobalPingStatus.PingPL[i] = 0;
           // }
        }
        private void TSbtnAutoScroll_Click(object sender, EventArgs e)
        {
            autoScrollToolStripMenuItem_Click(sender, e);
        }
        private void TSbtnSounds_Click(object sender, EventArgs e)
        {
            soundsToolStripMenuItem_Click(sender, e);
        }

        private void TSbtnShowTime_Click(object sender, EventArgs e)
        {
            showTimeToolStripMenuItem_Click(sender, e);
        }

        private void FormMain_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (CurrentProfileInUse)
                return;
            //MessageBox.Show("OK");
            for (int i = 0; i < 9; i++)
            {
                datafilesWriter[i].Close();// with close - it write all data in buffers
            }
        }

        private void TSbtnLogToFiles_Click(object sender, EventArgs e)
        {
            logToFilesToolStripMenuItem_Click(sender, e);
        }

        private void TSbtnOpenLogsFolder_Click(object sender, EventArgs e)
        {
            openLogsFolderToolStripMenuItem_Click(sender, e);
        }

        private void TSbtnMails_Click(object sender, EventArgs e)
        {
            MailsToolStripMenuItem_Click(sender, e);
        }
        

        private void TSBtnResize1_Click(object sender, EventArgs e){ResizeWindow(0,"");}
        private void TSBtnResize2_Click(object sender, EventArgs e) { ResizeWindow(1, ""); }
        private void TSBtnResize3_Click(object sender, EventArgs e) { ResizeWindow(2, ""); }
        private void TSBtnResize4_Click(object sender, EventArgs e) { ResizeWindow(3, ""); }
        private void TSBtnResize5_Click(object sender, EventArgs e) { ResizeWindow(4, ""); }
        private void TSBtnResize6_Click(object sender, EventArgs e) { ResizeWindow(5, ""); }
        private void TSBtnResize7_Click(object sender, EventArgs e) { ResizeWindow(6, ""); }
        private void TSBtnResize8_Click(object sender, EventArgs e) { ResizeWindow(7, ""); }
        private void TSBtnResize9_Click(object sender, EventArgs e) { ResizeWindow(8, ""); }

        private void TSBtnRestoreAllWindows_Click(object sender, EventArgs e)
        {
            restoreWindowsToolStripMenuItem_Click(sender, e);
        }

        private void ResizeWindow(int i, string action)
        {
            if (toolStripbtnResizeArr[i].Text == "Maximize" && action != "restore")
            {
                toolStripArr[i].Parent = toolStripArr[i].Parent.Parent.Parent;
                richTextBoxArr[i].BringToFront();
                richTextBoxArr[i].Parent = toolStripArr[i].Parent;
                richTextBoxArr[i].BringToFront();
                if (toolStripArr[i].Parent.GetType().ToString().IndexOf("FormMain") > 0) // Can't maximize any more
                {
                    toolStripbtnResizeArr[i].Image = imageList1.Images["Restore.bmp"];
                    toolStripbtnResizeArr[i].Text = "Restore";
                }
            }
            else
            {
                richTextBoxArr[i].Parent = richTextBoxOriginalParent[i];
                toolStripArr[i].Parent = richTextBoxOriginalParent[i];
                toolStripbtnResizeArr[i].Image = imageList1.Images["Max.bmp"];
                toolStripbtnResizeArr[i].Text = "Maximize";
            }
        }


        private void minimalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            minimalViewToolStripMenuItem_Click(sender, e);
        }

        private void detailedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            detailedViewToolStripMenuItem_Click(sender, e);
        }
        private void basicToolStripMenuItem_Click(object sender, EventArgs e)
        {
            basicViewToolStripMenuItem_Click(sender, e);
        }

        private void compactToolStripMenuItem_Click(object sender, EventArgs e)
        {
            compactViewToolStripMenuItem_Click(sender, e);
        }
        private void startAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TSbtnStart.Enabled = startAllToolStripMenuItem.Enabled = !startAllToolStripMenuItem.Enabled;
            TSbtnStop.Enabled = stopAllToolStripMenuItem.Enabled = !stopAllToolStripMenuItem.Enabled;
            GlobalPingStatus.PingStopClicked = false;
            string CurrentTime = Convert.ToString(DateTime.Now) + ": ";
            bool PingStarted=false;

            for (int i = 0; i < 9; i++)
            {
                if (GlobalConfig.Hosts.Enabled[i] && GlobalConfig.Hosts.IP[i].Length > 0)
                {
                    PingStarted = true;
                    UpdateWindowsWithOuput("message", i, "", "", "", "## Ping started ##");
                    TimerArr[i] = new System.Timers.Timer(10000);
                    switch (i)
                    {
                        case 0: TimerArr[i].Elapsed += new ElapsedEventHandler(OnTimedEvent1); break;
                        case 1: TimerArr[i].Elapsed += new ElapsedEventHandler(OnTimedEvent2); break;
                        case 2: TimerArr[i].Elapsed += new ElapsedEventHandler(OnTimedEvent3); break;
                        case 3: TimerArr[i].Elapsed += new ElapsedEventHandler(OnTimedEvent4); break;
                        case 4: TimerArr[i].Elapsed += new ElapsedEventHandler(OnTimedEvent5); break;
                        case 5: TimerArr[i].Elapsed += new ElapsedEventHandler(OnTimedEvent6); break;
                        case 6: TimerArr[i].Elapsed += new ElapsedEventHandler(OnTimedEvent7); break;
                        case 7: TimerArr[i].Elapsed += new ElapsedEventHandler(OnTimedEvent8); break;
                        case 8: TimerArr[i].Elapsed += new ElapsedEventHandler(OnTimedEvent9); break;
                    }

                    TimerArr[i].Interval = GlobalConfig.Ping.PingTimeDelta;
                    TimerArr[i].Enabled = true;
                }
            }
            if (!PingStarted)
            {
                MessageBox.Show("Please configure hosts to ping" + Environment.NewLine + "At View->Options", "No host configured for pinging", MessageBoxButtons.OK, MessageBoxIcon.Information);
                TSbtnStart.Enabled = startAllToolStripMenuItem.Enabled = true;
                TSbtnStop.Enabled = stopAllToolStripMenuItem.Enabled = false;
            
            }
        }

        private void stopAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TSbtnStop.Enabled = stopAllToolStripMenuItem.Enabled = !stopAllToolStripMenuItem.Enabled;
            TSbtnStart.Enabled = startAllToolStripMenuItem.Enabled = !startAllToolStripMenuItem.Enabled;
            
            GlobalPingStatus.PingStopClicked = true;
            string CurrentTime = Convert.ToString(DateTime.Now) + ": ";
            for (int i = 0; i < 9; i++)
            {
                if (TimerArr[i] != null)
                {
                    TimerArr[i].Stop();
                    TimerArr[i].Close();
                    if (!CurrentProfileInUse)
                        datafilesWriter[i].Flush();
                    UpdateWindowsWithOuput("message", i, "", "", "", "## Ping Stoped ##");
                    //toolStriplblArr[i].Text = GlobalConfig.Hosts.IP[i] + " - Stoped";
                }
            }
        }

        private void clearAllWindowsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < 9; i++)
            {
                ClearWindow(i);
            }
        }

        

        private void restoreWindowsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < 9; i++)
            {
                ResizeWindow(i, "restore");
            }
        }

        private void optionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CurrentProfileInUse = false;
            GlobalAction.LastAction = "Options";
            bool boolProfileInUse = false;
            string[] LastIP = new string[9];
            string lastProfileName = GlobalConfig.CurrentProfileName;
            // store old hosts to check if host is changed
            for (int i = 0; i < 9; i++)
            {
                LastIP[i] = GlobalConfig.Hosts.IP[i];
            }

            FormOptions formOptions = new FormOptions();
            DialogResult Result=formOptions.ShowDialog();
            if (Result == DialogResult.Cancel)
            {
                GlobalConfig.CurrentProfileName = lastProfileName;
                GlobalConfig.Init();
                return;
            }

            UpdateStatusButtons();

            if (lastProfileName != GlobalConfig.CurrentProfileName) // Profile chened
            {
                if (GlobalPingStatus.PingStopClicked == false)
                {
                    stopAllToolStripMenuItem_Click(sender, e);
                }
                for (int i = 0; i < 9; i++)
                {
                    ClearWindow(i);
                }
                toolStripMain.Enabled = false;
                initDataFilesPath();

                
                if (!LoadDataFilesToWindowsCheck())
                    ProfileInUse();

                if (!OpenDataFilesForWriting())
                    boolProfileInUse=true;
                toolStripMain.Enabled = true;
            }
            else// Check if host has Changed
            {
                for (int i = 0; i < 9; i++)
                {
                    if (LastIP[i] != GlobalConfig.Hosts.IP[i])
                    {
                        if (File.Exists(datafilesPath[i]))
                        {
                            ClearWindow(i);
                            if (!CurrentProfileInUse)
                            {
                                datafilesWriter[i].Close();
                                File.Delete(datafilesPath[i]);
                                if (!OpenDataFilesForWriting(i))
                                    boolProfileInUse = true;
                            }
                        }
                        UpdateWindowsWithOuput("message", i, "", "", "", "## Host has Changed, Data file and screen cleaned ##");

                    }

                }
            }
            if (boolProfileInUse)
                ProfileInUse();
        }

        private void autoScrollToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TSbtnAutoScroll.Checked = autoScrollToolStripMenuItem.Checked = !autoScrollToolStripMenuItem.Checked;
            UpdateStatusConfig();
        }

        private void soundsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TSbtnSounds.Checked = soundsToolStripMenuItem.Checked = !soundsToolStripMenuItem.Checked;
            UpdateStatusConfig();
        }

        private void showTimeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TSbtnShowTime.Checked = showTimeToolStripMenuItem.Checked= !showTimeToolStripMenuItem.Checked;
            MessageBox.Show("Bookmarks will not work for existing bookmark until refresh button is clicked", "Bookmarks", MessageBoxButtons.OK, MessageBoxIcon.Information);
            //UpdateWindowsWithOuput("message", i, "", "", "", "## Press refresh to reload old results in minimal view ##", 3);
            UpdateStatusConfig();
        }

        private void logToFilesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            logToFilesToolStripMenuItem.Checked = !logToFilesToolStripMenuItem.Checked;
            if (!logToFilesToolStripMenuItem.Checked)
                MessageBox.Show("Saving Ping results disabled, at refresh only old data will reload", "Log disabled", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            UpdateStatusConfig();
        }

        private void openLogsFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string myPath = GlobalConfig.Path.ProfilesPath + GlobalConfig.CurrentProfileName;
            System.Diagnostics.Process prc = new System.Diagnostics.Process();
            prc.StartInfo.FileName = myPath;

            prc.Start();
        }
        private void MailsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TSbtnMails.Checked = MailsToolStripMenuItem.Checked = !MailsToolStripMenuItem.Checked;
            UpdateStatusConfig();
        }
        private void toolStripTextBoxAddBookmark_KeyUp(object sender, KeyEventArgs e)
        {
            string BookmarkName;
            if (e.KeyCode  == Keys.Enter)
            {
                BookmarkName = toolStripTextBoxAddBookmark.Text;
                toolStripTextBoxAddBookmark.Text="";
                if (BookmarkName.Length > 0)
                {
                   // if (Array.IndexOf(GlobalBookmarks.Bookmarks, BookmarkName) > -1)// BookmarkName allready exists
                    if (GlobalBookmarks.Bookmarks.Find(x => x == BookmarkName) != null)
                    {
                        MessageBox.Show("Bookmark - " + BookmarkName + " allready exist","Error");
                        return;
                    }
                    for (int i = 0; i < 9; i++)
                    {
                        UpdateWindowsWithOuput("AddBookmark", i, "", "", "", BookmarkName);
                    }
                }
                this.ActiveControl = TSTBCaretHome.Control;
            }

            //TSDDBtnMainMenu.Select();
            TSTBCaretHome.Select();

        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormAbout formAbout = new FormAbout();
            formAbout.ShowDialog();
        }

        private void clearWindowsAndFilesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bool boolProfileInUse = false;
            if (MessageBox.Show("Delete ping history ?", "Delete", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                for (int i = 0; i < 9; i++)
                {
                    ClearWindow(i);
                    if (!CurrentProfileInUse)
                    {
                        if (File.Exists(datafilesPath[i]))
                        {
                            datafilesWriter[i].Close();
                            File.Delete(datafilesPath[i]);
                            if (!OpenDataFilesForWriting(i))
                                boolProfileInUse = true;
                        }
                    }
                }
            }
            if (boolProfileInUse)
                ProfileInUse();
        }

        private void reloadPingResultsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bool boolProfileInUse = false;
            //goToBookmarkToolStripMenuItem.DropDownItems.Clear();
            for (int i = 0; i < 9; i++)
            {
                ClearWindow(i);
                if (!CurrentProfileInUse)
                    datafilesWriter[i].Close();
                if (!LoadDataFileToWindow(i))
                    boolProfileInUse = true;
                if (!CurrentProfileInUse)
                    if (!OpenDataFilesForWriting(i))
                        boolProfileInUse = true;
            }
            if (boolProfileInUse)
                ProfileInUse();
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            reloadPingResultsToolStripMenuItem_Click(sender, e);
        }

        private void manageBookmarksToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!GlobalPingStatus.PingStopClicked)
            {
                MessageBox.Show("Please stop ping first", "Manage bookmarks", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (!GlobalConfig.General.LogToFiles)
                MessageBox.Show("Log to files disabled !!, Changes will not be saved", "Manage bookmarks", MessageBoxButtons.OK, MessageBoxIcon.Information);

            

            string textOrig, textNew, strOrig, strNew, timeSeperator;
            int charPos, linePos, charLineStart;
            FormManageBookmarks formManageBookmarks = new FormManageBookmarks();
            DialogResult Result = formManageBookmarks.ShowDialog();
            if (!GlobalConfig.General.ShowTime)
                timeSeperator = "";
            else
                timeSeperator = ": ";

            if (Result == DialogResult.OK)
            {
                int bookmarks_new_count=GlobalBookmarks.Bookmarks.Count;
                int bookmarks_orig_count = goToBookmarkToolStripMenuItem.DropDownItems.Count;

                for (int i = 0; i < bookmarks_orig_count; i++)
                {
                    textOrig=goToBookmarkToolStripMenuItem.DropDownItems[i].Text;
                    if (GlobalBookmarks.Bookmarks[i].Length>0) // rename bookmarks
                    {
                        textNew=GlobalBookmarks.Bookmarks[i];
                        if (textNew != textOrig)
                        {
                            // fix bookmarks at menu
                            goToBookmarkToolStripMenuItem.DropDownItems[i].Text = textNew;

                            // fix bookmarks at RichtextBox
                            if (!GlobalConfig.General.LogToFiles)
                            {
                                for (int j = 0; j < 9; j++)
                                {
                                    strOrig = timeSeperator + textOrig + (char)13;
                                    strNew = timeSeperator + textNew + (char)13;
                                    charPos = richTextBoxArr[j].Find(textOrig);
                                    if (charPos > -1)
                                    {
                                        charPos = charPos - 2; // I do not know why :), but i neet it
                                        richTextBoxArr[j].SelectionStart = charPos;
                                        richTextBoxArr[j].SelectionLength = strOrig.Length;
                                        richTextBoxArr[j].SelectedText = strNew;
                                    }
                                }
                            }
                            else// fix bookmark at files (and automatic at RichtextBox due to files reload)
                            {
                                for (int k = 0; k < 9; k++)
                                {
                                    ClearWindow(k);
                                    datafileReplaceString(k, textOrig, textNew, false);

                                }
                            }
                        }
                    }
                    else // remove bookmarks
                    {
                        // fix bookmarks at menu
                        goToBookmarkToolStripMenuItem.DropDownItems[i].Text=""; //RemoveAt(i);
                        // fix bookmarks at RichtextBox
                        if (!GlobalConfig.General.LogToFiles)
                        {
                            for (int j = 0; j < 9; j++)
                            {
                                strOrig = timeSeperator + textOrig + (char)13;
                                charPos = richTextBoxArr[j].Find(strOrig);
                                if (charPos > -1)
                                {
                                    linePos = richTextBoxArr[j].GetLineFromCharIndex(charPos);
                                    if (linePos > -1)
                                    {
                                        charLineStart = richTextBoxArr[j].GetFirstCharIndexFromLine(linePos);
                                        richTextBoxArr[j].SelectionStart = charLineStart;
                                        richTextBoxArr[j].SelectionLength = strOrig.Length - 1 + charPos - charLineStart;
                                        richTextBoxArr[j].SelectedText = "-- Bookmark removed --";
                                    }
                                }

                            }
                        }
                        else// fix bookmark at files
                        {
                            for (int k = 0; k < 9; k++)
                            {
                                ClearWindow(k);
                                datafileReplaceString(k, textOrig, "", true);

                            }
                        }
                    }
                }
            }
        }

        private void datafileReplaceString(int i, string textOrig, string textNew, bool DeleteLine)
        {
            string HostInFile,line,action;
            string[] arr;
            bool addLine;
            richTextBoxArr[i].Visible = false;
            if (!CurrentProfileInUse)
                datafilesWriter[i].Close();
            
            if (File.Exists(datafilesPathTemp[i]))
                File.Delete(datafilesPathTemp[i]);
            File.Move(datafilesPath[i] , datafilesPathTemp[i]);

            datafilesReader[i] = new StreamReader(datafilesPathTemp[i]);
            HostInFile = datafilesReader[i].ReadLine();
            
            if (OpenDataFilesForWriting())
                ProfileInUse();

            while ((line = datafilesReader[i].ReadLine()) != null)
            {
                addLine=true;
                arr = line.Split(',');
                if (arr[1] == "P")
                    action = "run";
                else if (arr[1] == "B")
                    action = "AddBookmark";
                else
                    action = "";

                if (arr[3] == textOrig)// replace string
                { 
                    if (DeleteLine)
                        addLine = false;
                    else
                        arr[3] = textNew;
                    
                }
                if (addLine)
                    UpdateWindowsWithOuput(action, i, arr[0], "", arr[2], arr[3]);
            }

            datafilesReader[i].Close();
            richTextBoxArr[i].Visible = true;
            
            
        }

        private void checkForUpdateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GlobalParam.AppVersionCurrentCheckAuto = false;
            webBrowserVersionCheck.Navigate("http://version-check.sharontools.com/9ping/?type=manual&v=" + GlobalParam.AppVersion);
        }


        private void detailedViewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (detailedViewToolStripMenuItem.Checked)
                return;
            basicViewToolStripMenuItem.Checked = basicToolStripMenuItem.Checked = false;
            compactViewToolStripMenuItem.Checked = compactToolStripMenuItem.Checked = false;
            minimalViewToolStripMenuItem.Checked = minimalToolStripMenuItem.Checked = false;
            detailedViewToolStripMenuItem.Checked = detailedToolStripMenuItem.Checked = true;
            toolStripDDBViewType.Image = detailedToolStripMenuItem.Image;
            TSbtnShowTime.Enabled = showTimeToolStripMenuItem.Enabled = true;
            UpdateStatusConfig();
            for (int i = 0; i < 9; i++)
            {
                richTextBoxArr[i].WordWrap = false;
                richTextBoxArr[i].AppendText(Environment.NewLine);
                UpdateWindowsWithOuput("message", i, "", "", "", "## Press refresh to reload old results in detailed view ##",3);
            }

        }

        private void minimalViewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (minimalViewToolStripMenuItem.Checked)
                return;
            basicToolStripMenuItem.Checked = basicToolStripMenuItem.Checked = false;
            compactViewToolStripMenuItem.Checked = compactToolStripMenuItem.Checked = false;
            minimalViewToolStripMenuItem.Checked = minimalToolStripMenuItem.Checked = true;
            detailedViewToolStripMenuItem.Checked = detailedToolStripMenuItem.Checked = false;
            TSbtnShowTime.Enabled = showTimeToolStripMenuItem.Enabled = false;
            toolStripDDBViewType.Image = minimalToolStripMenuItem.Image;
            UpdateStatusConfig();
            for (int i = 0; i < 9; i++)
            {
                richTextBoxArr[i].WordWrap = true;
                richTextBoxArr[i].AppendText(Environment.NewLine);
                UpdateWindowsWithOuput("message", i, "", "", "", "## Press refresh to reload old results in minimal view ##",3);

            }
        }

        private void basicViewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (basicViewToolStripMenuItem.Checked)
                return;
            basicViewToolStripMenuItem.Checked = basicToolStripMenuItem.Checked = true;
            compactViewToolStripMenuItem.Checked = compactToolStripMenuItem.Checked = false;
            minimalViewToolStripMenuItem.Checked = minimalToolStripMenuItem.Checked = false;
            detailedViewToolStripMenuItem.Checked = detailedToolStripMenuItem.Checked = false;
            TSbtnShowTime.Enabled = showTimeToolStripMenuItem.Enabled = false;
            toolStripDDBViewType.Image = basicToolStripMenuItem.Image;
            UpdateStatusConfig();
            for (int i = 0; i < 9; i++)
            {
                richTextBoxArr[i].WordWrap = false;
                richTextBoxArr[i].AppendText(Environment.NewLine);
                UpdateWindowsWithOuput("message", i, "", "", "", "## Press refresh to reload old results in basic view ##",3);

            }
        }
        private void compactViewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (compactViewToolStripMenuItem.Checked)
                return;
            compactViewToolStripMenuItem.Checked = compactToolStripMenuItem.Checked = true;
            basicViewToolStripMenuItem.Checked = basicToolStripMenuItem.Checked = false;
            minimalViewToolStripMenuItem.Checked = minimalToolStripMenuItem.Checked = false;
            detailedViewToolStripMenuItem.Checked = detailedToolStripMenuItem.Checked = false;
            TSbtnShowTime.Enabled = showTimeToolStripMenuItem.Enabled = false;
            toolStripDDBViewType.Image = compactToolStripMenuItem.Image;
            UpdateStatusConfig();
            for (int i = 0; i < 9; i++)
            {
                richTextBoxArr[i].WordWrap = true;
                richTextBoxArr[i].AppendText(Environment.NewLine);
                UpdateWindowsWithOuput("message", i, "", "", "", "## Press refresh to reload old results in comapct view ##", 3);

            }
        }

        private void startAnother9PingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string app = AppDomain.CurrentDomain.BaseDirectory + AppDomain.CurrentDomain.FriendlyName;
            System.Diagnostics.Process.Start(app);
            //MessageBox.Show(app);
        }

        

        

        

        

        
    }
}
