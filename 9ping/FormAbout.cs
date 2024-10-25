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
    public partial class FormAbout : Form
    {
        public FormAbout()
        {
            InitializeComponent();
        }

        private void About_Load(object sender, EventArgs e)
        {
            //string[] ProductVersion= Application.ProductVersion.Split('.');

            labelVer.Text = Functions.GetVersion();
            labelRevision.Text = Functions.GetVersionRevision();

            LinkLabel.Link link1 = new LinkLabel.Link();
            LinkLabel.Link link2 = new LinkLabel.Link();
            LinkLabel.Link link3 = new LinkLabel.Link();


            link1.LinkData = "mailto:" + linkLabelEmail.Text;
            linkLabelEmail.Links.Add(link1);

            link2.LinkData = linkLabelWeb.Text;
            linkLabelWeb.Links.Add(link2);

            link3.LinkData = linkLabelLinkedin.Text;
            linkLabelLinkedin.Links.Add(link3);

           }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void linkLabelEmail_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(e.Link.LinkData as string);
        }

        private void linkLabelWeb_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(e.Link.LinkData as string);
        }

        private void linkLabelLinkedin_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(e.Link.LinkData as string);
        }

        private void linkLabelDownload_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(e.Link.LinkData as string);
        }



       
    }
}
