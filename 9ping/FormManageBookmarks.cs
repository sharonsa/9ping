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
    public partial class FormManageBookmarks : Form
    {
        public static List<string> BookmarksTemp = GlobalBookmarks.Bookmarks;
        public FormManageBookmarks()
        {
            InitializeComponent();
        }

        private void FormManageBookmarks_Load(object sender, EventArgs e)
        {
            LoadBookmarks();
            
        }
        private void LoadBookmarks()
        {
            int TotalBookmarks=GlobalBookmarks.Bookmarks.Count;
            for (int i = 0; i < TotalBookmarks; i++)
            {
                treeViewBookmarks.Nodes.Add(GlobalBookmarks.Bookmarks[i]);
            }
        }

        private void buttonBookmarkNewCancel_Click(object sender, EventArgs e)
        {
            groupBoxBookmarkRename.Hide();
            textBoxBookmarkNewName.Text = "";
            treeViewBookmarks.Enabled = true;
        }

        private void buttonBookmarkNewOK_Click(object sender, EventArgs e)
        {
            int TotalBookmarks = treeViewBookmarks.GetNodeCount(false);
            string str=textBoxBookmarkNewName.Text;
            if (str.Length == 0)
                return;
            for (int i = 0; i < TotalBookmarks; i++)
            {
                if (treeViewBookmarks.Nodes[i].Text==str)
                {
                    MessageBox.Show("Bookmark " + str + " allready exist","Error", MessageBoxButtons.OK,MessageBoxIcon.Error);
                    return;
                }
            }
            treeViewBookmarks.SelectedNode.Text = textBoxBookmarkNewName.Text;
            BookmarksTemp[treeViewBookmarks.SelectedNode.Index] = textBoxBookmarkNewName.Text;
            groupBoxBookmarkRename.Hide();
            textBoxBookmarkNewName.Text = "";
            treeViewBookmarks.Enabled = true;
        }

        private void buttonFolderRename_Click(object sender, EventArgs e)
        {
            if (treeViewBookmarks.SelectedNode == null)
                return;
            treeViewBookmarks.Enabled = false;
            string BMName=treeViewBookmarks.SelectedNode.Text;
            if (treeViewBookmarks.SelectedNode.Text.Length>0){
                textBoxBookmarkNewName.Text = BMName;
                groupBoxBookmarkRename.Show();
                this.ActiveControl = textBoxBookmarkNewName;
            }
            
        }

        private void buttonProfileRemove_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult;
            if (treeViewBookmarks.SelectedNode == null)
                return;
            if (treeViewBookmarks.SelectedNode.ForeColor == Color.Gray)
            {
                MessageBox.Show("This Bookmark allready deleted !", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            dialogResult=MessageBox.Show("Remove bookmark " + treeViewBookmarks.SelectedNode.Text + Environment.NewLine + "Are you sure?", "Warning", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);
            if (dialogResult == DialogResult.Yes)
            {
                BookmarksTemp[treeViewBookmarks.SelectedNode.Index] = "";
                treeViewBookmarks.SelectedNode.Text = " - (" + treeViewBookmarks.SelectedNode.Text + ")";
                treeViewBookmarks.SelectedNode.ForeColor = Color.Gray;
                treeViewBookmarks.SelectedNode.BackColor = Color.Silver;
                //treeViewBookmarks.SelectedNode.NodeFont.Strikeout = true;

            }
        }

        private void button_OK_Click(object sender, EventArgs e)
        {
            int bookmarks_orig_count=GlobalBookmarks.Bookmarks.Count;
            //int bookmarks_new_count=treeViewBookmarks.GetNodeCount(false);
            for(int i=0 ; i<bookmarks_orig_count; i++)
            {
                //if (i < bookmarks_new_count)
                GlobalBookmarks.Bookmarks[i] = BookmarksTemp[i];
                //else
                //    GlobalBookmarks.Bookmarks.RemoveAt(i);
            }

        
            
        }
    }
}
