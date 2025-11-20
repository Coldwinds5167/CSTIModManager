using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace CSTIModManager
{
    partial class FormMain
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private IContainer components = null;

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
            this.components = new Container();
            ComponentResourceManager resources = new ComponentResourceManager(typeof(FormMain));
            this.textBoxDirectory = new TextBox();
            this.textBoxDirectory2 = new TextBox();
            this.buttonFolderBrowser = new Button();
            this.buttonFolderBrowser2 = new Button();
            this.label1 = new Label();
            this.label2 = new Label();
            this.buttonInstall = new Button();
            this.labelStatus = new Label();
            this.tabControlMain = new TabControl();
            this.CSTI = new TabPage();
            this.CSFF = new TabPage();
            this.listViewModsCSTI = new ListView();
            this.listViewModsCSFF = new ListView();
            this.columnHeaderNameCSTI = ((ColumnHeader)(new ColumnHeader()));
            this.columnHeaderVersionCSTI = ((ColumnHeader)(new ColumnHeader()));
            this.columnHeaderLocalVersionCSTI = ((ColumnHeader)(new ColumnHeader()));
            this.columnHeaderAuthorCSTI = ((ColumnHeader)(new ColumnHeader()));
            this.columnHeaderNameCSFF = ((ColumnHeader)(new ColumnHeader()));
            this.columnHeaderAuthorCSFF = ((ColumnHeader)(new ColumnHeader()));
            this.columnHeaderVersionCSFF = ((ColumnHeader)(new ColumnHeader()));
            this.columnHeaderLocalVersionCSFF = ((ColumnHeader)(new ColumnHeader()));
            this.contextMenuStripMain = new ContextMenuStrip(this.components);
            this.viewInfoToolStripMenuItem = new ToolStripMenuItem();
            this.Utilities = new TabPage();
            this.labelVersion = new Label();
            this.pictureBox1 = new PictureBox();
            this.buttonOpenWiki = new Button();
            this.buttonDiscordLink = new Button();
            this.buttonLanguage = new Button();
            this.groupBox1 = new GroupBox();
            this.buttonOpenLogFolderCSFF = new Button();
            this.buttonOpenLogFolderCSTI = new Button();
            this.labelOpen = new Label();
            this.buttonModInfo = new Button();
            this.buttonToggleMods = new Button();
            this.tabControlMain.SuspendLayout();
            this.CSTI.SuspendLayout();
            this.contextMenuStripMain.SuspendLayout();
            this.Utilities.SuspendLayout();
            ((ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // textBoxDirectory
            // 
            this.textBoxDirectory.Anchor = ((AnchorStyles)(((AnchorStyles.Top | AnchorStyles.Left) 
                                                            | AnchorStyles.Right)));
            this.textBoxDirectory.Enabled = false;
            this.textBoxDirectory.Location = new Point(10, 25);
            this.textBoxDirectory.Name = "textBoxDirectory";
            this.textBoxDirectory.Size = new Size(508, 22);
            this.textBoxDirectory.TabIndex = 0;
            // 
            // buttonFolderBrowser
            // 
            this.buttonFolderBrowser.Anchor = ((AnchorStyles)((AnchorStyles.Top | AnchorStyles.Right)));
            this.buttonFolderBrowser.Location = new Point(524, 25);
            this.buttonFolderBrowser.Name = "buttonFolderBrowser";
            this.buttonFolderBrowser.Size = new Size(26, 23);
            this.buttonFolderBrowser.TabIndex = 1;
            this.buttonFolderBrowser.Text = "..";
            this.buttonFolderBrowser.UseVisualStyleBackColor = true;
            this.buttonFolderBrowser.Click += new EventHandler(this.buttonFolderBrowserCSTI_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new Point(9, 9);
            this.label1.Name = "label1";
            this.label1.Size = new Size(127, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "热带岛屿游戏路径:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new Point(9, 54);
            this.label2.Name = "label2";
            this.label2.Size = new Size(127, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "奇幻森林游戏路径:";
            // 
            // textBoxDirectory2
            // 
            this.textBoxDirectory2.Anchor = ((AnchorStyles)(((AnchorStyles.Top | AnchorStyles.Left) 
                                                             | AnchorStyles.Right)));
            this.textBoxDirectory2.Enabled = false;
            this.textBoxDirectory2.Location = new Point(10, 70);
            this.textBoxDirectory2.Name = "textBoxDirectory2";
            this.textBoxDirectory2.Size = new Size(508, 22);
            this.textBoxDirectory2.TabIndex = 0;
            // 
            // buttonFolderBrowser2
            // 
            this.buttonFolderBrowser2.Anchor = ((AnchorStyles)((AnchorStyles.Top | AnchorStyles.Right)));
            this.buttonFolderBrowser2.Location = new Point(524, 70);
            this.buttonFolderBrowser2.Name = "buttonFolderBrowser2";
            this.buttonFolderBrowser2.Size = new Size(26, 23);
            this.buttonFolderBrowser2.TabIndex = 1;
            this.buttonFolderBrowser2.Text = "..";
            this.buttonFolderBrowser2.UseVisualStyleBackColor = true;
            this.buttonFolderBrowser2.Click += new EventHandler(this.buttonFolderBrowserCSFF_Click);
            // 
            // buttonInstall
            // 
            this.buttonInstall.Anchor = ((AnchorStyles)((AnchorStyles.Bottom | AnchorStyles.Right)));
            this.buttonInstall.Enabled = false;
            this.buttonInstall.Location = new Point(440, 376);
            this.buttonInstall.Name = "buttonInstall";
            this.buttonInstall.Size = new Size(112, 23);
            this.buttonInstall.TabIndex = 4;
            this.buttonInstall.Text = "安装/更新";
            this.buttonInstall.UseVisualStyleBackColor = true;
            this.buttonInstall.Click += new EventHandler(this.buttonInstall_Click);
            // 
            // labelStatus
            // 
            this.labelStatus.Anchor = ((AnchorStyles)((AnchorStyles.Bottom | AnchorStyles.Left)));
            this.labelStatus.AutoSize = true;
            this.labelStatus.Location = new Point(7, 381);
            this.labelStatus.Name = "labelStatus";
            this.labelStatus.Size = new Size(66, 13);
            this.labelStatus.TabIndex = 5;
            this.labelStatus.Text = "状态: 无";
            // 
            // tabControlMain
            // 
            this.tabControlMain.Anchor = ((AnchorStyles)((((AnchorStyles.Top | AnchorStyles.Bottom) 
                                                           | AnchorStyles.Left) 
                                                          | AnchorStyles.Right)));
            this.tabControlMain.Controls.Add(this.CSTI);
            this.tabControlMain.Controls.Add(this.CSFF);
            this.tabControlMain.Controls.Add(this.Utilities);
            this.tabControlMain.Enabled = false;
            this.tabControlMain.Location = new Point(10, 98);
            this.tabControlMain.Name = "tabControlMain";
            this.tabControlMain.SelectedIndex = 0;
            this.tabControlMain.Size = new Size(544, 272);
            this.tabControlMain.TabIndex = 8;
            // 
            // Plugins
            // 
            this.CSTI.Controls.Add(this.listViewModsCSTI);
            this.CSTI.Location = new Point(4, 22);
            this.CSTI.Name = "CSTI";
            this.CSTI.Padding = new Padding(3);
            this.CSTI.Size = new Size(536, 256);
            this.CSTI.TabIndex = 0;
            this.CSTI.Text = "热带岛屿";
            this.CSTI.UseVisualStyleBackColor = true;
	    // 
            // listViewMods
            // 
            this.listViewModsCSTI.Anchor = ((AnchorStyles)((((AnchorStyles.Top | AnchorStyles.Bottom) 
                                                             | AnchorStyles.Left) 
                                                            | AnchorStyles.Right)));
            this.listViewModsCSTI.CheckBoxes = true;
            this.listViewModsCSTI.Columns.AddRange(new ColumnHeader[] {
            this.columnHeaderNameCSTI,
            this.columnHeaderAuthorCSTI,
            this.columnHeaderVersionCSTI,
            this.columnHeaderLocalVersionCSTI});
            this.listViewModsCSTI.ContextMenuStrip = this.contextMenuStripMain;
            this.listViewModsCSTI.FullRowSelect = true;
            this.listViewModsCSTI.HideSelection = false;
            this.listViewModsCSTI.Location = new Point(6, 6);
            this.listViewModsCSTI.Name = "listViewModsCSTI";
            this.listViewModsCSTI.Size = new Size(524, 244);
            this.listViewModsCSTI.TabIndex = 0;
            this.listViewModsCSTI.UseCompatibleStateImageBehavior = false;
            this.listViewModsCSTI.View = View.Details;
            this.listViewModsCSTI.ItemChecked += new ItemCheckedEventHandler(this.listViewMods_ItemChecked);
            //this.listViewMods.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.listViewMods_ItemSelectionChanged);
            this.listViewModsCSTI.DoubleClick += new EventHandler(this.listViewMods_DoubleClick);
            // 
            // columnHeaderName
            // 
            this.columnHeaderNameCSTI.Text = "名称";
            this.columnHeaderNameCSTI.Width = 200;
            // 
            // columnHeaderVersion
            // 
            this.columnHeaderVersionCSTI.Text = "云端版本";
            this.columnHeaderVersionCSTI.Width = 75;
            // 
            // columnHeaderLocalVersion
            // 
            this.columnHeaderLocalVersionCSTI.Text = "本地版本";
            this.columnHeaderLocalVersionCSTI.Width = 75;
            // 
            // columnHeaderAuthor
            // 
            this.columnHeaderAuthorCSTI.Text = "作者";
            this.columnHeaderAuthorCSTI.Width = 125;
            // 
            // contextMenuStripMain
            // 
            this.contextMenuStripMain.Items.AddRange(new ToolStripItem[] {
            this.viewInfoToolStripMenuItem});
            this.contextMenuStripMain.Name = "contextMenuStripMain";
            this.contextMenuStripMain.Size = new Size(124, 26);
            // 
            // viewInfoToolStripMenuItem
            // 
            this.viewInfoToolStripMenuItem.Name = "viewInfoToolStripMenuItem";
            this.viewInfoToolStripMenuItem.Size = new Size(123, 22);
            this.viewInfoToolStripMenuItem.Text = "查看信息";
            this.viewInfoToolStripMenuItem.Click += new EventHandler(this.viewInfoToolStripMenuItem_Click);
            // 
            // Utilities
            // 
            this.Utilities.Controls.Add(this.labelVersion);
            this.Utilities.Controls.Add(this.pictureBox1);
            this.Utilities.Controls.Add(this.buttonOpenWiki);
            this.Utilities.Controls.Add(this.buttonDiscordLink);
            this.Utilities.Controls.Add(this.buttonLanguage);
            this.Utilities.Controls.Add(this.groupBox1);
            this.Utilities.Location = new Point(4, 22);
            this.Utilities.Name = "Utilities";
            this.Utilities.Size = new Size(536, 256);
            this.Utilities.TabIndex = 1;
            this.Utilities.Text = "杂项";
            this.Utilities.UseVisualStyleBackColor = true;
            // 
            // labelVersion
            // 
            this.labelVersion.Anchor = ((AnchorStyles)((AnchorStyles.Top | AnchorStyles.Bottom)));
            this.labelVersion.AutoSize = true;
            this.labelVersion.Location = new Point(188, 209);
            this.labelVersion.Name = "labelVersion";
            this.labelVersion.Size = new Size(119, 13);
            this.labelVersion.TabIndex = 11;
            this.labelVersion.Text = "CS Mod Manager";
            this.labelVersion.TextAlign = ContentAlignment.BottomCenter;
            this.labelVersion.UseMnemonic = false;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = ((Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new Point(170, 43);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new Size(186, 163);
            this.pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 10;
            this.pictureBox1.TabStop = false;
            // 
            // buttonOpenWiki
            // 
            this.buttonOpenWiki.Location = new Point(379, 183);
            this.buttonOpenWiki.Name = "buttonOpenWiki";
            this.buttonOpenWiki.Size = new Size(134, 23);
            this.buttonOpenWiki.TabIndex = 9;
            this.buttonOpenWiki.Text = "查看wiki";
            this.buttonOpenWiki.UseVisualStyleBackColor = true;
            this.buttonOpenWiki.Click += new EventHandler(this.buttonOpenWiki_Click);
            // 
            // buttonDiscordLink
            // 
            this.buttonDiscordLink.Location = new Point(379, 153);
            this.buttonDiscordLink.Name = "buttonDiscordLink";
            this.buttonDiscordLink.Size = new Size(134, 23);
            this.buttonDiscordLink.TabIndex = 8;
            this.buttonDiscordLink.Text = "加入Discord!";
            this.buttonDiscordLink.UseVisualStyleBackColor = true;
            this.buttonDiscordLink.Click += new EventHandler(this.buttonDiscordLink_Click);
            // 
            // buttonLanguage
            // 
            this.buttonLanguage.Location = new Point(379, 213);
            this.buttonLanguage.Name = "buttonDiscordLink";
            this.buttonLanguage.Size = new Size(134, 23);
            this.buttonLanguage.TabIndex = 8;
            this.buttonLanguage.Text = "语言/Language";
            this.buttonLanguage.UseVisualStyleBackColor = true;
            this.buttonLanguage.Click += new EventHandler(this.buttonLanguage_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.buttonOpenLogFolderCSFF);
            this.groupBox1.Controls.Add(this.buttonOpenLogFolderCSTI);
            this.groupBox1.Controls.Add(this.labelOpen);
            this.groupBox1.Location = new Point(373, 16);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new Size(146, 130);
            this.groupBox1.TabIndex = 7;
            this.groupBox1.TabStop = false;
            // 
            // buttonOpenLogFolderCSFF
            // 
            this.buttonOpenLogFolderCSFF.Location = new Point(6, 67);
            this.buttonOpenLogFolderCSFF.Name = "buttonOpenLogFolderCSFF";
            this.buttonOpenLogFolderCSFF.Size = new Size(134, 23);
            this.buttonOpenLogFolderCSFF.TabIndex = 5;
            this.buttonOpenLogFolderCSFF.Text = "奇幻森林log目录";
            this.buttonOpenLogFolderCSFF.UseVisualStyleBackColor = true;
            this.buttonOpenLogFolderCSFF.Click += new EventHandler(this.buttonOpenLogFolderCSFF_Click);
            // 
            // buttonOpenLogFolderCSTI
            // 
            this.buttonOpenLogFolderCSTI.Location = new Point(6, 38);
            this.buttonOpenLogFolderCSTI.Name = "buttonOpenLogFolderCSTI";
            this.buttonOpenLogFolderCSTI.Size = new Size(134, 23);
            this.buttonOpenLogFolderCSTI.TabIndex = 5;
            this.buttonOpenLogFolderCSTI.Text = "热带岛屿log目录";
            this.buttonOpenLogFolderCSTI.UseVisualStyleBackColor = true;
            this.buttonOpenLogFolderCSTI.Click += new EventHandler(this.buttonOpenLogFolderCSTI_Click);
            // 
            // labelOpen
            // 
            this.labelOpen.AutoSize = true;
            this.labelOpen.Location = new Point(23, 15);
            this.labelOpen.Name = "labelOpen";
            this.labelOpen.Size = new Size(99, 13);
            this.labelOpen.TabIndex = 6;
            this.labelOpen.Text = "重要路径";
            // 
            // buttonModInfo
            // 
            this.buttonModInfo.Anchor = ((AnchorStyles)((AnchorStyles.Bottom | AnchorStyles.Right)));
            this.buttonModInfo.Enabled = false;
            this.buttonModInfo.Location = new Point(322, 376);
            this.buttonModInfo.Name = "buttonModInfo";
            this.buttonModInfo.Size = new Size(112, 23);
            this.buttonModInfo.TabIndex = 9;
            this.buttonModInfo.Text = "删除Mods";
            this.buttonModInfo.UseVisualStyleBackColor = true;
            this.buttonModInfo.Click += new EventHandler(this.buttonModInfo_Click);
            // 
            // buttonToggleMods
            // 
            this.buttonToggleMods.Anchor = ((AnchorStyles)((AnchorStyles.Bottom | AnchorStyles.Right)));
            this.buttonToggleMods.Enabled = false;
            this.buttonToggleMods.Location = new Point(204, 376);
            this.buttonToggleMods.Name = "buttonToggleMods";
            this.buttonToggleMods.Size = new Size(112, 23);
            this.buttonToggleMods.TabIndex = 10;
            this.buttonToggleMods.Text = "禁用/启用";
            this.buttonToggleMods.UseVisualStyleBackColor = true;
            this.buttonToggleMods.Click += new EventHandler(this.buttonToggleMods_Click);
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new SizeF(6F, 13F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new Size(566, 411);
            this.Controls.Add(this.buttonToggleMods);
            this.Controls.Add(this.buttonModInfo);
            this.Controls.Add(this.tabControlMain);
            this.Controls.Add(this.labelStatus);
            this.Controls.Add(this.buttonInstall);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.buttonFolderBrowser);
            this.Controls.Add(this.textBoxDirectory);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.buttonFolderBrowser2);
            this.Controls.Add(this.textBoxDirectory2);
            this.Font = new Font("Segoe UI", 8.25F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FormMain";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "CS Mod Manager";
            this.Load += new EventHandler(this.FormMain_Load);
            this.tabControlMain.ResumeLayout(false);
            this.CSTI.ResumeLayout(false);
            this.contextMenuStripMain.ResumeLayout(false);
            this.Utilities.ResumeLayout(false);
            this.Utilities.PerformLayout();
            ((ISupportInitialize)(this.pictureBox1)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
            // 
            // Skins (新增的页面)
            // 
            this.CSFF.Controls.Add(this.listViewModsCSFF); // 使用新的ListView
            this.CSFF.Location = new Point(4, 22);
            this.CSFF.Name = "CSFF";
            this.CSFF.Padding = new Padding(3);
            this.CSFF.Size = new Size(536, 256);
            this.CSFF.TabIndex = 2; // 注意索引号变化
            this.CSFF.Text = "奇幻森林"; // 标签页标题
            this.CSFF.UseVisualStyleBackColor = true;
            // 
            // listViewSkins (新增的ListView控件)
            // 
            this.listViewModsCSFF.Anchor = ((AnchorStyles)((((AnchorStyles.Top | AnchorStyles.Bottom) 
                                                             | AnchorStyles.Left) 
                                                            | AnchorStyles.Right)));
            this.listViewModsCSFF.CheckBoxes = true;
            this.listViewModsCSFF.Columns.AddRange(new ColumnHeader[] {
            this.columnHeaderNameCSFF,
            this.columnHeaderAuthorCSFF,
            this.columnHeaderVersionCSFF,
            this.columnHeaderLocalVersionCSFF});
            this.listViewModsCSFF.ContextMenuStrip = this.contextMenuStripMain; // 使用相同的上下文菜单
            this.listViewModsCSFF.FullRowSelect = true;
            this.listViewModsCSFF.HideSelection = false;
            this.listViewModsCSFF.Location = new Point(6, 6);
            this.listViewModsCSFF.Name = "listViewModsCSFF";
            this.listViewModsCSFF.Size = new Size(524, 244);
            this.listViewModsCSFF.TabIndex = 0;
            this.listViewModsCSFF.UseCompatibleStateImageBehavior = false;
            this.listViewModsCSFF.View = View.Details;
            // 绑定相同的事件处理程序
            this.listViewModsCSFF.ItemChecked += new ItemCheckedEventHandler(this.listViewMods_ItemChecked);
            this.listViewModsCSFF.DoubleClick += new EventHandler(this.listViewMods_DoubleClick);
            
            // 列标题文本
            this.columnHeaderNameCSFF.Text = "名称";
            this.columnHeaderNameCSFF.Width = 200;
            this.columnHeaderAuthorCSFF.Text = "作者";
            this.columnHeaderAuthorCSFF.Width = 125;
            this.columnHeaderVersionCSFF.Text = "云端版本";
            this.columnHeaderVersionCSFF.Width = 75;
            this.columnHeaderLocalVersionCSFF.Text = "本地版本";
            this.columnHeaderLocalVersionCSFF.Width = 75;
            
            this.tabControlMain.SelectedIndexChanged += new EventHandler(this.tabControlMain_SelectedIndexChanged);
        }

        #endregion
        private TabPage CSFF;
        private ListView listViewModsCSFF;
        private ColumnHeader columnHeaderNameCSFF;
        private ColumnHeader columnHeaderAuthorCSFF;
        private ColumnHeader columnHeaderVersionCSFF;
        private ColumnHeader columnHeaderLocalVersionCSFF;
        private TextBox textBoxDirectory;
        private TextBox textBoxDirectory2;
        private Button buttonFolderBrowser;
        private Button buttonFolderBrowser2;
        private Label label1;
        private Label label2;
        private Button buttonInstall;
        private Label labelStatus;
        private TabControl tabControlMain;
        private TabPage CSTI;
        private ListView listViewModsCSTI;
        private ColumnHeader columnHeaderNameCSTI;
        private ColumnHeader columnHeaderVersionCSTI;
        private ColumnHeader columnHeaderLocalVersionCSTI;
        private ColumnHeader columnHeaderAuthorCSTI;
        private ContextMenuStrip contextMenuStripMain;
        private ToolStripMenuItem viewInfoToolStripMenuItem;
        private Button buttonModInfo;
        private TabPage Utilities;
        private Label labelOpen;
        private GroupBox groupBox1;
        private Button buttonOpenLogFolderCSFF;
        private Button buttonOpenLogFolderCSTI;
        private Button buttonOpenWiki;
        private Button buttonDiscordLink;
        private Button buttonLanguage;
        private PictureBox pictureBox1;
        private Label labelVersion;
        private Button buttonToggleMods;
    }
}