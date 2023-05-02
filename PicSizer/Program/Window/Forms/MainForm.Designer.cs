
namespace PicSizer.Window.Forms
{
    partial class MainForm
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.button_StartResize = new System.Windows.Forms.Button();
            this.button_Set = new System.Windows.Forms.Button();
            this.label_SelectByTotal = new System.Windows.Forms.Label();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.文件ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.添加文件ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.打开文件夹ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.退出ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.选择ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.全选ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.反选ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.移除ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.选中项ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.已完成ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.错误项ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.全部项ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.帮助ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.关于ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.button_OpenFolder = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.textBox_OutputDir = new System.Windows.Forms.TextBox();
            this.radioButton_OutputDirection = new System.Windows.Forms.RadioButton();
            this.radioButton_OutputStructure = new System.Windows.Forms.RadioButton();
            this.radioButton_CoverOrigin = new System.Windows.Forms.RadioButton();
            this.PicListView = new PicSizer.Window.Assemble.PicListView();
            this.fileName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.fullPath = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.size = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.state = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.menuStrip1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // button_StartResize
            // 
            resources.ApplyResources(this.button_StartResize, "button_StartResize");
            this.button_StartResize.Name = "button_StartResize";
            this.button_StartResize.UseVisualStyleBackColor = true;
            this.button_StartResize.Click += new System.EventHandler(this.OnResizeClick);
            // 
            // button_Set
            // 
            resources.ApplyResources(this.button_Set, "button_Set");
            this.button_Set.Name = "button_Set";
            this.button_Set.UseVisualStyleBackColor = true;
            this.button_Set.Click += new System.EventHandler(this.OnSetClick);
            // 
            // label_SelectByTotal
            // 
            resources.ApplyResources(this.label_SelectByTotal, "label_SelectByTotal");
            this.label_SelectByTotal.BackColor = System.Drawing.SystemColors.Control;
            this.label_SelectByTotal.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label_SelectByTotal.Name = "label_SelectByTotal";
            // 
            // menuStrip1
            // 
            this.menuStrip1.BackColor = System.Drawing.SystemColors.Control;
            resources.ApplyResources(this.menuStrip1, "menuStrip1");
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.文件ToolStripMenuItem,
            this.选择ToolStripMenuItem,
            this.帮助ToolStripMenuItem});
            this.menuStrip1.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.HorizontalStackWithOverflow;
            this.menuStrip1.Name = "menuStrip1";
            // 
            // 文件ToolStripMenuItem
            // 
            this.文件ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.添加文件ToolStripMenuItem,
            this.打开文件夹ToolStripMenuItem,
            this.退出ToolStripMenuItem});
            this.文件ToolStripMenuItem.Name = "文件ToolStripMenuItem";
            resources.ApplyResources(this.文件ToolStripMenuItem, "文件ToolStripMenuItem");
            // 
            // 添加文件ToolStripMenuItem
            // 
            this.添加文件ToolStripMenuItem.Name = "添加文件ToolStripMenuItem";
            resources.ApplyResources(this.添加文件ToolStripMenuItem, "添加文件ToolStripMenuItem");
            this.添加文件ToolStripMenuItem.Click += new System.EventHandler(this.OnFileItemClick);
            // 
            // 打开文件夹ToolStripMenuItem
            // 
            this.打开文件夹ToolStripMenuItem.Name = "打开文件夹ToolStripMenuItem";
            resources.ApplyResources(this.打开文件夹ToolStripMenuItem, "打开文件夹ToolStripMenuItem");
            this.打开文件夹ToolStripMenuItem.Click += new System.EventHandler(this.OnFileItemClick);
            // 
            // 退出ToolStripMenuItem
            // 
            this.退出ToolStripMenuItem.Name = "退出ToolStripMenuItem";
            resources.ApplyResources(this.退出ToolStripMenuItem, "退出ToolStripMenuItem");
            this.退出ToolStripMenuItem.Click += new System.EventHandler(this.OnFileItemClick);
            // 
            // 选择ToolStripMenuItem
            // 
            this.选择ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.全选ToolStripMenuItem,
            this.反选ToolStripMenuItem,
            this.移除ToolStripMenuItem});
            this.选择ToolStripMenuItem.Name = "选择ToolStripMenuItem";
            resources.ApplyResources(this.选择ToolStripMenuItem, "选择ToolStripMenuItem");
            // 
            // 全选ToolStripMenuItem
            // 
            this.全选ToolStripMenuItem.Name = "全选ToolStripMenuItem";
            resources.ApplyResources(this.全选ToolStripMenuItem, "全选ToolStripMenuItem");
            this.全选ToolStripMenuItem.Click += new System.EventHandler(this.OnSelectItemClick);
            // 
            // 反选ToolStripMenuItem
            // 
            this.反选ToolStripMenuItem.Name = "反选ToolStripMenuItem";
            resources.ApplyResources(this.反选ToolStripMenuItem, "反选ToolStripMenuItem");
            this.反选ToolStripMenuItem.Click += new System.EventHandler(this.OnSelectItemClick);
            // 
            // 移除ToolStripMenuItem
            // 
            this.移除ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.选中项ToolStripMenuItem,
            this.已完成ToolStripMenuItem,
            this.错误项ToolStripMenuItem,
            this.全部项ToolStripMenuItem});
            this.移除ToolStripMenuItem.Name = "移除ToolStripMenuItem";
            resources.ApplyResources(this.移除ToolStripMenuItem, "移除ToolStripMenuItem");
            // 
            // 选中项ToolStripMenuItem
            // 
            this.选中项ToolStripMenuItem.Name = "选中项ToolStripMenuItem";
            resources.ApplyResources(this.选中项ToolStripMenuItem, "选中项ToolStripMenuItem");
            this.选中项ToolStripMenuItem.Click += new System.EventHandler(this.OnRemoveItemClick);
            // 
            // 已完成ToolStripMenuItem
            // 
            this.已完成ToolStripMenuItem.Name = "已完成ToolStripMenuItem";
            resources.ApplyResources(this.已完成ToolStripMenuItem, "已完成ToolStripMenuItem");
            this.已完成ToolStripMenuItem.Click += new System.EventHandler(this.OnRemoveItemClick);
            // 
            // 错误项ToolStripMenuItem
            // 
            this.错误项ToolStripMenuItem.Name = "错误项ToolStripMenuItem";
            resources.ApplyResources(this.错误项ToolStripMenuItem, "错误项ToolStripMenuItem");
            this.错误项ToolStripMenuItem.Click += new System.EventHandler(this.OnRemoveItemClick);
            // 
            // 全部项ToolStripMenuItem
            // 
            this.全部项ToolStripMenuItem.Name = "全部项ToolStripMenuItem";
            resources.ApplyResources(this.全部项ToolStripMenuItem, "全部项ToolStripMenuItem");
            this.全部项ToolStripMenuItem.Click += new System.EventHandler(this.OnRemoveItemClick);
            // 
            // 帮助ToolStripMenuItem
            // 
            this.帮助ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.关于ToolStripMenuItem});
            this.帮助ToolStripMenuItem.Name = "帮助ToolStripMenuItem";
            resources.ApplyResources(this.帮助ToolStripMenuItem, "帮助ToolStripMenuItem");
            // 
            // 关于ToolStripMenuItem
            // 
            this.关于ToolStripMenuItem.Name = "关于ToolStripMenuItem";
            resources.ApplyResources(this.关于ToolStripMenuItem, "关于ToolStripMenuItem");
            this.关于ToolStripMenuItem.Tag = "3";
            this.关于ToolStripMenuItem.Click += new System.EventHandler(this.OnHelpMenuClick);
            // 
            // groupBox1
            // 
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Controls.Add(this.button_OpenFolder);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.textBox_OutputDir);
            this.groupBox1.Controls.Add(this.radioButton_OutputDirection);
            this.groupBox1.Controls.Add(this.radioButton_OutputStructure);
            this.groupBox1.Controls.Add(this.radioButton_CoverOrigin);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // button_OpenFolder
            // 
            resources.ApplyResources(this.button_OpenFolder, "button_OpenFolder");
            this.button_OpenFolder.Name = "button_OpenFolder";
            this.button_OpenFolder.UseVisualStyleBackColor = true;
            this.button_OpenFolder.Click += new System.EventHandler(this.button_OpenFolder_Click);
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // textBox_OutputDir
            // 
            this.textBox_OutputDir.AllowDrop = true;
            resources.ApplyResources(this.textBox_OutputDir, "textBox_OutputDir");
            this.textBox_OutputDir.Name = "textBox_OutputDir";
            this.textBox_OutputDir.DragDrop += new System.Windows.Forms.DragEventHandler(this.textBox_OutputDir_DragDrop);
            this.textBox_OutputDir.DragEnter += new System.Windows.Forms.DragEventHandler(this.textBox_OutputDir_DragEnter);
            // 
            // radioButton_OutputDirection
            // 
            resources.ApplyResources(this.radioButton_OutputDirection, "radioButton_OutputDirection");
            this.radioButton_OutputDirection.Checked = true;
            this.radioButton_OutputDirection.Name = "radioButton_OutputDirection";
            this.radioButton_OutputDirection.TabStop = true;
            this.radioButton_OutputDirection.UseVisualStyleBackColor = true;
            this.radioButton_OutputDirection.CheckedChanged += new System.EventHandler(this.radioButton_CheckedChanged);
            // 
            // radioButton_OutputStructure
            // 
            resources.ApplyResources(this.radioButton_OutputStructure, "radioButton_OutputStructure");
            this.radioButton_OutputStructure.Name = "radioButton_OutputStructure";
            this.radioButton_OutputStructure.UseVisualStyleBackColor = true;
            this.radioButton_OutputStructure.CheckedChanged += new System.EventHandler(this.radioButton_CheckedChanged);
            // 
            // radioButton_CoverOrigin
            // 
            resources.ApplyResources(this.radioButton_CoverOrigin, "radioButton_CoverOrigin");
            this.radioButton_CoverOrigin.Name = "radioButton_CoverOrigin";
            this.radioButton_CoverOrigin.UseVisualStyleBackColor = true;
            this.radioButton_CoverOrigin.CheckedChanged += new System.EventHandler(this.radioButton_CheckedChanged);
            // 
            // PicListView
            // 
            this.PicListView.AllowDrop = true;
            resources.ApplyResources(this.PicListView, "PicListView");
            this.PicListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.fileName,
            this.fullPath,
            this.size,
            this.state});
            this.PicListView.FullRowSelect = true;
            this.PicListView.GridLines = true;
            this.PicListView.HideSelection = false;
            this.PicListView.Name = "PicListView";
            this.PicListView.UseCompatibleStateImageBehavior = false;
            this.PicListView.View = System.Windows.Forms.View.Details;
            // 
            // fileName
            // 
            resources.ApplyResources(this.fileName, "fileName");
            // 
            // fullPath
            // 
            resources.ApplyResources(this.fullPath, "fullPath");
            // 
            // size
            // 
            resources.ApplyResources(this.size, "size");
            // 
            // state
            // 
            resources.ApplyResources(this.state, "state");
            // 
            // MainForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.PicListView);
            this.Controls.Add(this.label_SelectByTotal);
            this.Controls.Add(this.button_Set);
            this.Controls.Add(this.button_StartResize);
            this.Controls.Add(this.menuStrip1);
            this.DoubleBuffered = true;
            this.KeyPreview = true;
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "MainForm";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.OnAppExit);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button button_StartResize;
        private System.Windows.Forms.Button button_Set;
        private System.Windows.Forms.Label label_SelectByTotal;
        private System.Windows.Forms.ColumnHeader fileName;
        private System.Windows.Forms.ColumnHeader fullPath;
        private System.Windows.Forms.ColumnHeader size;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem 文件ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 添加文件ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 退出ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 选择ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 全选ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 反选ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 移除ToolStripMenuItem;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ColumnHeader state;
        private Window.Assemble.PicListView PicListView;
        private System.Windows.Forms.ToolStripMenuItem 选中项ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 已完成ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 错误项ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 全部项ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 打开文件夹ToolStripMenuItem;
        private System.Windows.Forms.TextBox textBox_OutputDir;
        private System.Windows.Forms.RadioButton radioButton_OutputDirection;
        private System.Windows.Forms.RadioButton radioButton_OutputStructure;
        private System.Windows.Forms.RadioButton radioButton_CoverOrigin;
        private System.Windows.Forms.ToolStripMenuItem 帮助ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 关于ToolStripMenuItem;
        private System.Windows.Forms.Button button_OpenFolder;
        private System.Windows.Forms.Label label1;
    }
}

