using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace ninePing
{
    public partial class FormOptions : Form
    {
        TextBox[] textBoxIP, textBoxName;
        CheckBox[] checkBoxIP;
        //Boolean LoadCompeled = false;
        public FormOptions()
        {
            InitializeComponent();
        }
        private void FormOptions_Load(object sender, EventArgs e)
        {
            textBoxIP =   new TextBox[]  { textBoxIP1, textBoxIP2, textBoxIP3, textBoxIP4, textBoxIP5, textBoxIP6, textBoxIP7, textBoxIP8, textBoxIP9 };
            textBoxName = new TextBox[] { textBoxName1, textBoxName2, textBoxName3, textBoxName4, textBoxName5, textBoxName6, textBoxName7, textBoxName8, textBoxName9 };
            checkBoxIP =  new CheckBox[] { checkBoxIP1, checkBoxIP2, checkBoxIP3, checkBoxIP4, checkBoxIP5, checkBoxIP6, checkBoxIP7, checkBoxIP8, checkBoxIP9 };

            pictureBoxLogWarnning.Image = imageListDir.Images["Warning.bmp"];
            // Update Profiles comboBox
            UpdateProfilesTreeList();

            // Update Hosts & Config 
            UpdateTabs();
            //LoadCompeled = true;
        }
        
        public void UpdateProfilesTreeList(string path="",TreeNode ParentNode=null )
        {
            //TreeNode CurrentNode;
            if (path.Length == 0)
                path = GlobalConfig.Path.ProfilesPath;
            
            // remove old data info in case of refrase
            if (ParentNode == null && treeViewDir.Nodes.Count>0)
                    treeViewDir.Nodes.Remove(treeViewDir.Nodes[0]);

            string[] paths = Directory.GetDirectories(path);
            string CurrentPath;
            string[] subdir;
            string CurrenFulltPath;
            TreeNode[] nodes = new TreeNode[paths.Length];

            for (int i = 0; i < paths.Length; i++)
            {
                CurrentPath = paths[i];
                CurrenFulltPath = paths[i].Remove(0, GlobalConfig.Path.ProfilesPath.Length);
                paths[i] = paths[i].Remove(0, path.Length);
                if(paths[i].Substring(0,1)==@"\")
                    paths[i] = paths[i].Remove(0, 1);

                nodes[i] = new TreeNode(paths[i]);

                if (ParentNode == null)
                {
                    ParentNode = new TreeNode("Profiles");
                    ParentNode.ImageIndex = 1;
                    ParentNode.SelectedImageIndex = 1;
                    treeViewDir.Nodes.Add(ParentNode);
                }

                subdir = Directory.GetDirectories(CurrentPath);

                if (File.Exists(CurrentPath + @"\config.ini"))
                {
                    nodes[i].ImageIndex = 0;
                    nodes[i].SelectedImageIndex = 0;
                }
                else
                {
                    nodes[i].ImageIndex = 1;
                    nodes[i].SelectedImageIndex = 1;
                }

                ParentNode.Nodes.Add(nodes[i]);
                if (CurrenFulltPath == GlobalConfig.CurrentProfileName)
                    treeViewDir.SelectedNode = nodes[i];

                if (subdir.Length > 0)
                    UpdateProfilesTreeList(CurrentPath, nodes[i]);
            }
        }

        //  #################################################################################
        //  #################################### Update window ##############################
        //  #################################################################################

        public void UpdateTabs()
        {
            //  #################################### Hosts #################################
            for (int i = 0; i < 9; i++)
            {
                textBoxIP[i].Text = GlobalConfig.Hosts.IP[i];
                textBoxName[i].Text = GlobalConfig.Hosts.Name[i];
                checkBoxIP[i].Checked = GlobalConfig.Hosts.Enabled[i];
            }

            //  #################################### Ping #################################
            textBoxPingTimeDelta.Text = Convert.ToString(GlobalConfig.Ping.PingTimeDelta);
            textBoxPingTimeout.Text = Convert.ToString(GlobalConfig.Ping.PingTimeout);

            //  #################################### General #################################

            checkBoxPingAutoStart.Checked = GlobalConfig.General.PingAutoStart;
            checkBoxGeneralAutoScroll.Checked = GlobalConfig.General.AutoScroll;
            checkBoxGeneralPlaySounds.Checked = GlobalConfig.General.PlaySounds;
            checkBoxGeneralShowTime.Checked = GlobalConfig.General.ShowTime;
            checkBoxGeneralLogToFiles.Checked = GlobalConfig.General.LogToFiles;
            radioButtonPingResultsViewDetailed.Checked = GlobalConfig.General.PingResultsViewDetailed;
            radioButtonPingResultsViewMinimal.Checked = GlobalConfig.General.PingResultsViewMinimal;
            radioButtonPingResultsViewBasic.Checked = GlobalConfig.General.PingResultsViewBasic;
            radioButtonPingResultsViewCompact.Checked = GlobalConfig.General.PingResultsViewCompact;
            checkBoxAutoCheckForUpdate.Checked = GlobalConfig.AutoCheckForUpdate;

            //  #################################### Mail #################################
            textBoxMailFrom.Text= GlobalConfig.Mail.MailFrom;
            textBoxMailTo.Text = GlobalConfig.Mail.MailTo;
            textBoxMailSubject.Text = GlobalConfig.Mail.MailSubject;
            richTextBoxMailBody.Text = GlobalConfig.Mail.MailBody;

            textBoxSMTPServer.Text = GlobalConfig.Mail.SMTPServer;
            textBoxSMTPServerPort.Text =Convert.ToString(GlobalConfig.Mail.SMTPServerPort);
            textBoxSMTPUser.Text = GlobalConfig.Mail.SMTPUser;
            textBoxSMTPPass.Text = GlobalConfig.Mail.SMTPPass;
            checkBoxSMTPSSL.Checked = GlobalConfig.Mail.SMTPSSL;

            checkBoxMailAlerts.Checked=GlobalConfig.Mail.MailAlerts;
            textBoxMailSendAfter.Text=GlobalConfig.Mail.SendAfter.ToString();
            textBoxMailMaxPerMinute.Text=GlobalConfig.Mail.MaxPerMinute.ToString();
            textBoxMailMaxPerHour.Text = GlobalConfig.Mail.MaxPerHour.ToString();

        }
        
        private void button_OK_Click(object sender, EventArgs e)
        {
            SaveToFile();
            this.Close();
        }
        private void button_Apply_Click(object sender, EventArgs e)
        {
            SaveToFile();
        }
        private void button_Cancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        //  #################################################################################
        //  #################################### Save to file ###############################
        //  #################################################################################
        public void SaveToFile()
        {

            // saving hosts

            //  #################################### Profile #################################
            // save profile name for next 9Ping starts
            GlobalConfig.Path.iniSystemFile.IniWriteValue("Profile", "Name", GlobalConfig.CurrentProfileName);

            //  #################################### Hosts #################################
            for (int i = 0; i < 9; i++)
            {
                GlobalConfig.Path.iniConfigFile.IniWriteValue("Hosts", "IP-" + Convert.ToString(i), textBoxIP[i].Text);
                GlobalConfig.Path.iniConfigFile.IniWriteValue("Hosts", "Name-" + Convert.ToString(i), textBoxName[i].Text);
                GlobalConfig.Path.iniConfigFile.IniWriteValue("Hosts", "Enabled-" + Convert.ToString(i), Convert.ToString(checkBoxIP[i].Checked));
            }

            //  #################################### Ping #################################
            GlobalConfig.Path.iniConfigFile.IniWriteValue("Ping", "PingTimeDelta", textBoxPingTimeDelta.Text);
            GlobalConfig.Path.iniConfigFile.IniWriteValue("Ping", "PingTimeout", textBoxPingTimeout.Text);

            //  #################################### General #################################

            GlobalConfig.Path.iniConfigFile.IniWriteValue("General", "PingAutoStart", Convert.ToString(checkBoxPingAutoStart.Checked));
            GlobalConfig.Path.iniConfigFile.IniWriteValue("General", "AutoScroll", Convert.ToString(checkBoxGeneralAutoScroll.Checked));
            GlobalConfig.Path.iniConfigFile.IniWriteValue("General", "PlaySounds", Convert.ToString(checkBoxGeneralPlaySounds.Checked));
            GlobalConfig.Path.iniConfigFile.IniWriteValue("General", "ShowTime", Convert.ToString(checkBoxGeneralShowTime.Checked));
            GlobalConfig.Path.iniConfigFile.IniWriteValue("General", "LogToFiles", Convert.ToString(checkBoxGeneralLogToFiles.Checked));
            GlobalConfig.Path.iniConfigFile.IniWriteValue("General", "PingResultsViewDetailed", Convert.ToString(radioButtonPingResultsViewDetailed.Checked));
            GlobalConfig.Path.iniConfigFile.IniWriteValue("General", "PingResultsViewMinimal", Convert.ToString(radioButtonPingResultsViewMinimal.Checked));
            GlobalConfig.Path.iniConfigFile.IniWriteValue("General", "PingResultsViewBasic", Convert.ToString(radioButtonPingResultsViewBasic.Checked));
            GlobalConfig.Path.iniConfigFile.IniWriteValue("General", "PingResultsViewCompact", Convert.ToString(radioButtonPingResultsViewCompact.Checked));

            //  #################################### Program #################################
            GlobalConfig.Path.iniSystemFile.IniWriteValue("Program", "AutoCheckForUpdate", Convert.ToString(checkBoxAutoCheckForUpdate.Checked));

            //  #################################### Mail #################################
            GlobalConfig.Path.iniConfigFile.IniWriteValue("Mail", "MailFrom", textBoxMailFrom.Text);
            GlobalConfig.Path.iniConfigFile.IniWriteValue("Mail", "MailTo", textBoxMailTo.Text);
            GlobalConfig.Path.iniConfigFile.IniWriteValue("Mail", "MailSubject", textBoxMailSubject.Text);
            GlobalConfig.Path.iniConfigFile.IniWriteValue("Mail", "MailBody", richTextBoxMailBody.Text.Replace('\n','|'));
            GlobalConfig.Path.iniConfigFile.IniWriteValue("Mail", "SMTPServer", textBoxSMTPServer.Text);
            GlobalConfig.Path.iniConfigFile.IniWriteValue("Mail", "SMTPServerPort", Convert.ToString(textBoxSMTPServerPort.Text));
            GlobalConfig.Path.iniConfigFile.IniWriteValue("Mail", "SMTPUser", textBoxSMTPUser.Text);
            var PassBytes = Encoding.UTF8.GetBytes(textBoxSMTPPass.Text);
            GlobalConfig.Path.iniConfigFile.IniWriteValue("Mail", "SMTPPass", Convert.ToBase64String(PassBytes));
            GlobalConfig.Path.iniConfigFile.IniWriteValue("Mail", "SMTPSSL", Convert.ToString(checkBoxSMTPSSL.Checked));
            GlobalConfig.Path.iniConfigFile.IniWriteValue("Mail", "MailAlerts", Convert.ToString(checkBoxMailAlerts.Checked));
            GlobalConfig.Path.iniConfigFile.IniWriteValue("Mail", "SendAfter", textBoxMailSendAfter.Text);
            GlobalConfig.Path.iniConfigFile.IniWriteValue("Mail", "MaxPerMinute", textBoxMailMaxPerMinute.Text);
            GlobalConfig.Path.iniConfigFile.IniWriteValue("Mail", "MaxPerHour", textBoxMailMaxPerHour.Text);

            GlobalConfig.Init();
        }

        private void buttonProfileNew_Click(object sender, EventArgs e)
        {
            treeViewDir.Enabled = false;
            tabControl1.Enabled = false;
            labelNew.Text = "New profile name";
            labelNewType.Text = "profile";
            textBoxProfileNewName.Text = "";
            groupBoxProfileNewName.Visible = true;
            this.ActiveControl = textBoxProfileNewName;
        }

        private void buttonProfileNewCancel_Click(object sender, EventArgs e)
        {
            groupBoxProfileNewName.Visible = false;
            treeViewDir.Enabled = true;
            tabControl1.Enabled = true;
        }
        private void buttonProfileNewOK_Click(object sender, EventArgs e)
        {
            string dir,tmpdir,ConfigFilePath;
            TreeNode TreeNodeToCheck;
            FileStream fs;
            dir = treeViewDir.SelectedNode.FullPath + @"\";

            // Check if name exists
            if (labelNewType.Text == "rename")// Rename
                TreeNodeToCheck = treeViewDir.SelectedNode.Parent;
            else // Adding new folder / profile
                TreeNodeToCheck = treeViewDir.SelectedNode;

            int TotalProfiles = TreeNodeToCheck.GetNodeCount(false);
            string str = textBoxProfileNewName.Text;
            if (str.Length == 0)
                return;
            for (int i = 0; i < TotalProfiles; i++)
            {
                if (TreeNodeToCheck.Nodes[i].Text == str)
                {
                    MessageBox.Show("Profile / Directory - " + str + " allready exist", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }


            if (labelNewType.Text == "rename")  // Rename
            {
                tmpdir = dir.Remove(dir.Length - treeViewDir.SelectedNode.Text.Length - 1);
                Functions.RenameDirectory(dir, tmpdir + textBoxProfileNewName.Text);
                GlobalConfig.CurrentProfileName = tmpdir.Remove(0,treeViewDir.Nodes[0].Text.Length + 1) + textBoxProfileNewName.Text;
            }
            else // Add
            {
                GlobalConfig.CurrentProfileName = GetTreePath(treeViewDir.SelectedNode.FullPath) + @"\" + textBoxProfileNewName.Text;
                Functions.CheckDirectory(dir + textBoxProfileNewName.Text);
                if (labelNewType.Text == "profile")
                {
                    ConfigFilePath = dir + textBoxProfileNewName.Text + @"\Config.ini";
                    if (!File.Exists(ConfigFilePath))
                    {
                        fs =File.Create(ConfigFilePath);
                        fs.Close();
                    }
                }
                
            }

            GlobalConfig.Init();
            UpdateProfilesTreeList();
            UpdateTabs();

           // }
           groupBoxProfileNewName.Visible = false;
           treeViewDir.Enabled = true;
           tabControl1.Enabled = true;
        }
        private void treeViewDir_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (treeViewDir.SelectedNode.Text == "Profiles")
            {
                tabControl1.Enabled = false;
                button_OK.Enabled = false;
                button_Apply.Enabled = false;
                buttonProfileRemove.Enabled = false;
                buttonFolderRename.Enabled = false;
            }
            else
            {
                tabControl1.Enabled = true;
                button_OK.Enabled = true;
                button_Apply.Enabled = true;
                if (treeViewDir.SelectedNode.Text == "Default")
                {
                    buttonProfileRemove.Enabled = false;
                    buttonFolderRename.Enabled = false;
                }
                else
                {
                    buttonProfileRemove.Enabled = true;
                    buttonFolderRename.Enabled = true;
                    if (treeViewDir.SelectedNode.ImageIndex == 1)
                    {
                        tabControl1.Enabled = false;
                        button_OK.Enabled = false;
                        button_Apply.Enabled = false;
                    }
                    else 
                    {
                        tabControl1.Enabled = true;
                        button_OK.Enabled = true;
                        button_Apply.Enabled = true;
                    }

                }
            }
            if (treeViewDir.SelectedNode.ImageIndex == 0)// 0 = profile, 1 = directory
            {
                buttonProfileNew.Enabled = false;
                buttonFolderAdd.Enabled = false;
                GlobalConfig.CurrentProfileName = GetTreePath(treeViewDir.SelectedNode.FullPath);
                GlobalConfig.Init();
            }
            else
            {
                buttonProfileNew.Enabled = true;
                buttonFolderAdd.Enabled = true;
            }
            
            UpdateTabs();
        }

        private void buttonProfileRemove_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Are you sure you want to complitly remove profile" + Environment.NewLine + treeViewDir.SelectedNode.Text + " ?", "Profile remove", MessageBoxButtons.YesNo,MessageBoxIcon.Exclamation);
            if (dialogResult == DialogResult.Yes)
            {
                Functions.DelDirectory(GetTreePath(treeViewDir.SelectedNode.FullPath));
                GlobalConfig.CurrentProfileName = GetTreePath(treeViewDir.SelectedNode.Parent.FullPath);
                GlobalConfig.Init();
                UpdateProfilesTreeList();
                UpdateTabs();
            }
            
        }
        private string GetTreePath(string TreePath)
        {
            TreePath=TreePath.Remove(0, treeViewDir.Nodes[0].Text.Length);
            if (TreePath.Length > 0)
                if (TreePath.Substring(0, 1) == @"\")
                    TreePath = TreePath.Remove(0, 1);
            return TreePath;
        }

        private void buttonFolderAdd_Click(object sender, EventArgs e)
        {
            treeViewDir.Enabled = false;
            tabControl1.Enabled = false;
            labelNew.Text = "New Folder name";
            labelNewType.Text = "folder";
            textBoxProfileNewName.Text = "";
            groupBoxProfileNewName.Visible = true;
            this.ActiveControl = textBoxProfileNewName;
        }

        private void buttonFolderRename_Click(object sender, EventArgs e)
        {
            treeViewDir.Enabled = false;
            labelNew.Text = "Rename Folder / Profile";
            labelNewType.Text = "rename";
            textBoxProfileNewName.Text = treeViewDir.SelectedNode.Text.Replace(treeViewDir.SelectedNode.Parent.Text + @"\", "");
            groupBoxProfileNewName.Visible = true;
            this.ActiveControl = textBoxProfileNewName;
        }

        private void pictureBoxLogWarnning_MouseHover(object sender, EventArgs e)
        {
            ToolTip tt = new ToolTip();
            tt.SetToolTip(pictureBoxLogWarnning, pictureBoxLogWarnning.Tag.ToString());
        }

        private void buttonSendMailTest_Click(object sender, EventArgs e)
        {
            string MailFrom = textBoxMailFrom.Text;
            string MailTo = textBoxMailTo.Text;
            string MailSubject = textBoxMailSubject.Text;
            string MailBody = richTextBoxMailBody.Text;
            string SMTPServer = textBoxSMTPServer.Text;
            int SMTPServerPort = Convert.ToInt32(textBoxSMTPServerPort.Text);
            string SMTPUser = textBoxSMTPUser.Text;
            string SMTPPass = textBoxSMTPPass.Text;
            bool SMTPSSL = checkBoxSMTPSSL.Checked;

            ClassMail.send(MailFrom, MailTo, MailSubject, MailBody, SMTPServer, SMTPServerPort, SMTPUser, SMTPPass, SMTPSSL, true);
        }
    }
}
