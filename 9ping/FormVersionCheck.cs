
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ninePing
{
    public partial class FormVersionCheck : Form
    {
        public FormVersionCheck()
        {
            InitializeComponent();
        }

        private void FormVersionCheck_Load(object sender, EventArgs e)
        {
            string VerStatus=Functions.CompareVersions(GlobalParam.AppLatestVersion);
            labelVer.Text = GlobalParam.AppVersion;
            labelRevision.Text = GlobalParam.AppVersionRevision;

            LinkLabel.Link link4 = new LinkLabel.Link();
            link4.LinkData = linkLabelDownload.Text;
            linkLabelDownload.Links.Add(link4);

            if (VerStatus=="OK")
                groupBoxVerOK.Show();
            else if (VerStatus == "upgrade")
            {
                labelNewVer.Text = GlobalParam.AppLatestVersion;
                groupBoxVerNew.Show();
            }
            else
                groupBoxVerErr.Show();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void linkLabelDownload_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(e.Link.LinkData as string);
        }



       
    }
}

