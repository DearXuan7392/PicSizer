
namespace PicSizer.Forms
{
    partial class Form1
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.button_StartResize = new System.Windows.Forms.Button();
            this.button_Set = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
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
            this.作者ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.文档ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.反馈和建议ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.关于ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.检查更新ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.picDirPathText1 = new PicSizer_ControlLibrary.PicDirPathText();
            this.listView1 = new PicSizer_ControlLibrary.PicListView();
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
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.BackColor = System.Drawing.SystemColors.Control;
            this.label2.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label2.Name = "label2";
            // 
            // menuStrip1
            // 
            this.menuStrip1.BackColor = System.Drawing.SystemColors.Control;
            resources.ApplyResources(this.menuStrip1, "menuStrip1");
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
            this.作者ToolStripMenuItem,
            this.文档ToolStripMenuItem,
            this.反馈和建议ToolStripMenuItem,
            this.关于ToolStripMenuItem,
            this.检查更新ToolStripMenuItem});
            this.帮助ToolStripMenuItem.Name = "帮助ToolStripMenuItem";
            resources.ApplyResources(this.帮助ToolStripMenuItem, "帮助ToolStripMenuItem");
            // 
            // 作者ToolStripMenuItem
            // 
            this.作者ToolStripMenuItem.Name = "作者ToolStripMenuItem";
            resources.ApplyResources(this.作者ToolStripMenuItem, "作者ToolStripMenuItem");
            this.作者ToolStripMenuItem.Tag = "0";
            this.作者ToolStripMenuItem.Click += new System.EventHandler(this.OnHelpMenuClick);
            // 
            // 文档ToolStripMenuItem
            // 
            this.文档ToolStripMenuItem.Name = "文档ToolStripMenuItem";
            resources.ApplyResources(this.文档ToolStripMenuItem, "文档ToolStripMenuItem");
            this.文档ToolStripMenuItem.Tag = "1";
            this.文档ToolStripMenuItem.Click += new System.EventHandler(this.OnHelpMenuClick);
            // 
            // 反馈和建议ToolStripMenuItem
            // 
            this.反馈和建议ToolStripMenuItem.Name = "反馈和建议ToolStripMenuItem";
            resources.ApplyResources(this.反馈和建议ToolStripMenuItem, "反馈和建议ToolStripMenuItem");
            this.反馈和建议ToolStripMenuItem.Tag = "2";
            this.反馈和建议ToolStripMenuItem.Click += new System.EventHandler(this.OnHelpMenuClick);
            // 
            // 关于ToolStripMenuItem
            // 
            this.关于ToolStripMenuItem.Name = "关于ToolStripMenuItem";
            resources.ApplyResources(this.关于ToolStripMenuItem, "关于ToolStripMenuItem");
            this.关于ToolStripMenuItem.Tag = "3";
            this.关于ToolStripMenuItem.Click += new System.EventHandler(this.OnHelpMenuClick);
            // 
            // 检查更新ToolStripMenuItem
            // 
            this.检查更新ToolStripMenuItem.Name = "检查更新ToolStripMenuItem";
            resources.ApplyResources(this.检查更新ToolStripMenuItem, "检查更新ToolStripMenuItem");
            this.检查更新ToolStripMenuItem.Tag = "4";
            this.检查更新ToolStripMenuItem.Click += new System.EventHandler(this.OnHelpMenuClick);
            // 
            // groupBox1
            // 
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Controls.Add(this.picDirPathText1);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // picDirPathText1
            // 
            this.picDirPathText1.AllowDrop = true;
            resources.ApplyResources(this.picDirPathText1, "picDirPathText1");
            this.picDirPathText1.Name = "picDirPathText1";
            // 
            // listView1
            // 
            this.listView1.AllowDrop = true;
            resources.ApplyResources(this.listView1, "listView1");
            this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.fileName,
            this.fullPath,
            this.size,
            this.state});
            this.listView1.FullRowSelect = true;
            this.listView1.GridLines = true;
            this.listView1.HideSelection = false;
            this.listView1.Name = "listView1";
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.Details;
            this.listView1.OnSelectUpdate += new PicSizer_ControlLibrary.PicListView.OnSelectUpdateEvent(this.UpdateSelectTotalNumLabel);
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
            // Form1
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.listView1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.button_Set);
            this.Controls.Add(this.button_StartResize);
            this.Controls.Add(this.menuStrip1);
            this.DoubleBuffered = true;
            this.KeyPreview = true;
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Form1";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.OnAppExit);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button button_StartResize;
        private System.Windows.Forms.Button button_Set;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ColumnHeader fileName;
        private System.Windows.Forms.ColumnHeader fullPath;
        private System.Windows.Forms.ColumnHeader size;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem 文件ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 添加文件ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 退出ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 帮助ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 作者ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 文档ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 关于ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 选择ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 全选ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 反选ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 移除ToolStripMenuItem;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ColumnHeader state;
        public PicSizer_ControlLibrary.PicListView listView1;
        private System.Windows.Forms.ToolStripMenuItem 选中项ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 已完成ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 错误项ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 全部项ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 打开文件夹ToolStripMenuItem;
        private PicSizer_ControlLibrary.PicDirPathText picDirPathText1;
        private System.Windows.Forms.ToolStripMenuItem 反馈和建议ToolStripMenuItem;
        public System.Windows.Forms.ToolStripMenuItem 检查更新ToolStripMenuItem;
    }
}

