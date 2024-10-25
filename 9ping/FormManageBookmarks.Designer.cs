namespace ninePing
{
    partial class FormManageBookmarks
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormManageBookmarks));
            this.groupBoxBookmarkRename = new System.Windows.Forms.GroupBox();
            this.labelNewType = new System.Windows.Forms.Label();
            this.buttonBookmarkNewCancel = new System.Windows.Forms.Button();
            this.buttonBookmarkNewOK = new System.Windows.Forms.Button();
            this.labelNew = new System.Windows.Forms.Label();
            this.textBoxBookmarkNewName = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.treeViewBookmarks = new System.Windows.Forms.TreeView();
            this.button_OK = new System.Windows.Forms.Button();
            this.buttonFolderRename = new System.Windows.Forms.Button();
            this.buttonProfileRemove = new System.Windows.Forms.Button();
            this.button_Cancel = new System.Windows.Forms.Button();
            this.groupBoxBookmarkRename.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBoxBookmarkRename
            // 
            this.groupBoxBookmarkRename.Controls.Add(this.labelNewType);
            this.groupBoxBookmarkRename.Controls.Add(this.buttonBookmarkNewCancel);
            this.groupBoxBookmarkRename.Controls.Add(this.buttonBookmarkNewOK);
            this.groupBoxBookmarkRename.Controls.Add(this.labelNew);
            this.groupBoxBookmarkRename.Controls.Add(this.textBoxBookmarkNewName);
            this.groupBoxBookmarkRename.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.groupBoxBookmarkRename.Location = new System.Drawing.Point(54, 66);
            this.groupBoxBookmarkRename.Name = "groupBoxBookmarkRename";
            this.groupBoxBookmarkRename.Size = new System.Drawing.Size(236, 143);
            this.groupBoxBookmarkRename.TabIndex = 35;
            this.groupBoxBookmarkRename.TabStop = false;
            this.groupBoxBookmarkRename.Visible = false;
            // 
            // labelNewType
            // 
            this.labelNewType.AutoSize = true;
            this.labelNewType.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelNewType.Location = new System.Drawing.Point(14, 2);
            this.labelNewType.Name = "labelNewType";
            this.labelNewType.Size = new System.Drawing.Size(63, 13);
            this.labelNewType.TabIndex = 36;
            this.labelNewType.Text = "Bookmark";
            this.labelNewType.Visible = false;
            // 
            // buttonBookmarkNewCancel
            // 
            this.buttonBookmarkNewCancel.Location = new System.Drawing.Point(165, 104);
            this.buttonBookmarkNewCancel.Name = "buttonBookmarkNewCancel";
            this.buttonBookmarkNewCancel.Size = new System.Drawing.Size(51, 23);
            this.buttonBookmarkNewCancel.TabIndex = 35;
            this.buttonBookmarkNewCancel.Text = "Cancel";
            this.buttonBookmarkNewCancel.UseVisualStyleBackColor = true;
            this.buttonBookmarkNewCancel.Click += new System.EventHandler(this.buttonBookmarkNewCancel_Click);
            // 
            // buttonBookmarkNewOK
            // 
            this.buttonBookmarkNewOK.Location = new System.Drawing.Point(108, 104);
            this.buttonBookmarkNewOK.Name = "buttonBookmarkNewOK";
            this.buttonBookmarkNewOK.Size = new System.Drawing.Size(51, 23);
            this.buttonBookmarkNewOK.TabIndex = 35;
            this.buttonBookmarkNewOK.Text = "OK";
            this.buttonBookmarkNewOK.UseVisualStyleBackColor = true;
            this.buttonBookmarkNewOK.Click += new System.EventHandler(this.buttonBookmarkNewOK_Click);
            // 
            // labelNew
            // 
            this.labelNew.AutoSize = true;
            this.labelNew.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelNew.Location = new System.Drawing.Point(28, 46);
            this.labelNew.Name = "labelNew";
            this.labelNew.Size = new System.Drawing.Size(125, 13);
            this.labelNew.TabIndex = 35;
            this.labelNew.Text = "New bookmark name";
            // 
            // textBoxBookmarkNewName
            // 
            this.textBoxBookmarkNewName.Location = new System.Drawing.Point(31, 68);
            this.textBoxBookmarkNewName.Name = "textBoxBookmarkNewName";
            this.textBoxBookmarkNewName.Size = new System.Drawing.Size(185, 20);
            this.textBoxBookmarkNewName.TabIndex = 31;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(12, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(49, 13);
            this.label7.TabIndex = 38;
            this.label7.Text = "Profiles";
            // 
            // treeViewBookmarks
            // 
            this.treeViewBookmarks.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.treeViewBookmarks.HideSelection = false;
            this.treeViewBookmarks.Location = new System.Drawing.Point(15, 16);
            this.treeViewBookmarks.Name = "treeViewBookmarks";
            this.treeViewBookmarks.Size = new System.Drawing.Size(302, 256);
            this.treeViewBookmarks.TabIndex = 37;
            // 
            // button_OK
            // 
            this.button_OK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.button_OK.Location = new System.Drawing.Point(209, 276);
            this.button_OK.Name = "button_OK";
            this.button_OK.Size = new System.Drawing.Size(51, 23);
            this.button_OK.TabIndex = 37;
            this.button_OK.Text = "OK";
            this.button_OK.UseVisualStyleBackColor = true;
            this.button_OK.Click += new System.EventHandler(this.button_OK_Click);
            // 
            // buttonFolderRename
            // 
            this.buttonFolderRename.Location = new System.Drawing.Point(15, 278);
            this.buttonFolderRename.Name = "buttonFolderRename";
            this.buttonFolderRename.Size = new System.Drawing.Size(74, 23);
            this.buttonFolderRename.TabIndex = 42;
            this.buttonFolderRename.Tag = "Rename current Profile / Folder";
            this.buttonFolderRename.Text = "Rename";
            this.buttonFolderRename.UseVisualStyleBackColor = true;
            this.buttonFolderRename.Click += new System.EventHandler(this.buttonFolderRename_Click);
            // 
            // buttonProfileRemove
            // 
            this.buttonProfileRemove.Location = new System.Drawing.Point(95, 278);
            this.buttonProfileRemove.Name = "buttonProfileRemove";
            this.buttonProfileRemove.Size = new System.Drawing.Size(66, 23);
            this.buttonProfileRemove.TabIndex = 41;
            this.buttonProfileRemove.Tag = "Remove current Profile / Folder";
            this.buttonProfileRemove.Text = "Remove";
            this.buttonProfileRemove.UseVisualStyleBackColor = true;
            this.buttonProfileRemove.Click += new System.EventHandler(this.buttonProfileRemove_Click);
            // 
            // button_Cancel
            // 
            this.button_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button_Cancel.Location = new System.Drawing.Point(266, 276);
            this.button_Cancel.Name = "button_Cancel";
            this.button_Cancel.Size = new System.Drawing.Size(51, 23);
            this.button_Cancel.TabIndex = 43;
            this.button_Cancel.Text = "Cancel";
            this.button_Cancel.UseVisualStyleBackColor = true;
            // 
            // FormManageBookmarks
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(335, 311);
            this.Controls.Add(this.groupBoxBookmarkRename);
            this.Controls.Add(this.button_Cancel);
            this.Controls.Add(this.buttonFolderRename);
            this.Controls.Add(this.buttonProfileRemove);
            this.Controls.Add(this.button_OK);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.treeViewBookmarks);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FormManageBookmarks";
            this.Text = "FormManageBookmarks";
            this.Load += new System.EventHandler(this.FormManageBookmarks_Load);
            this.groupBoxBookmarkRename.ResumeLayout(false);
            this.groupBoxBookmarkRename.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBoxBookmarkRename;
        private System.Windows.Forms.Label labelNewType;
        private System.Windows.Forms.Button buttonBookmarkNewCancel;
        private System.Windows.Forms.Button buttonBookmarkNewOK;
        private System.Windows.Forms.Label labelNew;
        private System.Windows.Forms.TextBox textBoxBookmarkNewName;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TreeView treeViewBookmarks;
        private System.Windows.Forms.Button button_OK;
        private System.Windows.Forms.Button buttonFolderRename;
        private System.Windows.Forms.Button buttonProfileRemove;
        private System.Windows.Forms.Button button_Cancel;
    }
}