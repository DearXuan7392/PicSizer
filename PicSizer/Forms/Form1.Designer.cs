
namespace PicSizer
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
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.button_StartResize = new System.Windows.Forms.Button();
            this.button_Add = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.button_Choose = new System.Windows.Forms.Button();
            this.button_Set = new System.Windows.Forms.Button();
            this.button_Author = new System.Windows.Forms.Button();
            this.button_Remove = new System.Windows.Forms.Button();
            this.button_SelectAll = new System.Windows.Forms.Button();
            this.button_SelectReverse = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // listBox1
            // 
            this.listBox1.AllowDrop = true;
            this.listBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listBox1.FormattingEnabled = true;
            this.listBox1.HorizontalScrollbar = true;
            this.listBox1.ItemHeight = 12;
            this.listBox1.Location = new System.Drawing.Point(12, 12);
            this.listBox1.Name = "listBox1";
            this.listBox1.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.listBox1.Size = new System.Drawing.Size(479, 280);
            this.listBox1.TabIndex = 0;
            this.listBox1.SelectedValueChanged += new System.EventHandler(this.listBox1_SelectedValueChanged);
            this.listBox1.DragDrop += new System.Windows.Forms.DragEventHandler(this.listBox1_DragDrop);
            this.listBox1.DragEnter += new System.Windows.Forms.DragEventHandler(this.DragEnter);
            // 
            // button_StartResize
            // 
            this.button_StartResize.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button_StartResize.Location = new System.Drawing.Point(497, 99);
            this.button_StartResize.Name = "button_StartResize";
            this.button_StartResize.Size = new System.Drawing.Size(75, 23);
            this.button_StartResize.TabIndex = 1;
            this.button_StartResize.Text = "开始压缩";
            this.button_StartResize.UseVisualStyleBackColor = true;
            this.button_StartResize.Click += new System.EventHandler(this.OnResizeClick);
            // 
            // button_Add
            // 
            this.button_Add.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button_Add.Location = new System.Drawing.Point(497, 12);
            this.button_Add.Name = "button_Add";
            this.button_Add.Size = new System.Drawing.Size(75, 23);
            this.button_Add.TabIndex = 2;
            this.button_Add.Text = "添加";
            this.button_Add.UseVisualStyleBackColor = true;
            this.button_Add.Click += new System.EventHandler(this.OnAddClick);
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 331);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(47, 12);
            this.label1.TabIndex = 3;
            this.label1.Text = "输出到:";
            // 
            // textBox1
            // 
            this.textBox1.AllowDrop = true;
            this.textBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox1.Location = new System.Drawing.Point(65, 326);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(426, 21);
            this.textBox1.TabIndex = 4;
            this.textBox1.DragDrop += new System.Windows.Forms.DragEventHandler(this.textBox1_DragDrop);
            this.textBox1.DragEnter += new System.Windows.Forms.DragEventHandler(this.DragEnter);
            // 
            // button_Choose
            // 
            this.button_Choose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button_Choose.Location = new System.Drawing.Point(497, 326);
            this.button_Choose.Name = "button_Choose";
            this.button_Choose.Size = new System.Drawing.Size(75, 23);
            this.button_Choose.TabIndex = 5;
            this.button_Choose.Text = "选择";
            this.button_Choose.UseVisualStyleBackColor = true;
            this.button_Choose.Click += new System.EventHandler(this.OnChooseClick);
            // 
            // button_Set
            // 
            this.button_Set.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button_Set.Location = new System.Drawing.Point(497, 41);
            this.button_Set.Name = "button_Set";
            this.button_Set.Size = new System.Drawing.Size(75, 23);
            this.button_Set.TabIndex = 6;
            this.button_Set.Text = "设置";
            this.button_Set.UseVisualStyleBackColor = true;
            this.button_Set.Click += new System.EventHandler(this.OnSetClick);
            // 
            // button_Author
            // 
            this.button_Author.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button_Author.Location = new System.Drawing.Point(497, 70);
            this.button_Author.Name = "button_Author";
            this.button_Author.Size = new System.Drawing.Size(75, 23);
            this.button_Author.TabIndex = 7;
            this.button_Author.Text = "作者";
            this.button_Author.UseVisualStyleBackColor = true;
            this.button_Author.Click += new System.EventHandler(this.OnAuthorClick);
            // 
            // button_Remove
            // 
            this.button_Remove.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button_Remove.Location = new System.Drawing.Point(416, 298);
            this.button_Remove.Name = "button_Remove";
            this.button_Remove.Size = new System.Drawing.Size(75, 23);
            this.button_Remove.TabIndex = 8;
            this.button_Remove.Text = "移除";
            this.button_Remove.UseVisualStyleBackColor = true;
            this.button_Remove.Click += new System.EventHandler(this.OnRemoveClick);
            // 
            // button_SelectAll
            // 
            this.button_SelectAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button_SelectAll.Location = new System.Drawing.Point(254, 298);
            this.button_SelectAll.Name = "button_SelectAll";
            this.button_SelectAll.Size = new System.Drawing.Size(75, 23);
            this.button_SelectAll.TabIndex = 9;
            this.button_SelectAll.Text = "全选";
            this.button_SelectAll.UseVisualStyleBackColor = true;
            this.button_SelectAll.Click += new System.EventHandler(this.OnSelectAllClick);
            // 
            // button_SelectReverse
            // 
            this.button_SelectReverse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button_SelectReverse.Location = new System.Drawing.Point(335, 298);
            this.button_SelectReverse.Name = "button_SelectReverse";
            this.button_SelectReverse.Size = new System.Drawing.Size(75, 23);
            this.button_SelectReverse.TabIndex = 10;
            this.button_SelectReverse.Text = "反选";
            this.button_SelectReverse.UseVisualStyleBackColor = true;
            this.button_SelectReverse.Click += new System.EventHandler(this.OnSelectReverseClick);
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 303);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(23, 12);
            this.label2.TabIndex = 11;
            this.label2.Text = "0/0";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(584, 361);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.button_SelectReverse);
            this.Controls.Add(this.button_SelectAll);
            this.Controls.Add(this.button_Remove);
            this.Controls.Add(this.button_Author);
            this.Controls.Add(this.button_Set);
            this.Controls.Add(this.button_Choose);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button_Add);
            this.Controls.Add(this.button_StartResize);
            this.Controls.Add(this.listBox1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(450, 300);
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "PicSizer";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.Button button_StartResize;
        private System.Windows.Forms.Button button_Add;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button button_Choose;
        private System.Windows.Forms.Button button_Set;
        private System.Windows.Forms.Button button_Author;
        private System.Windows.Forms.Button button_Remove;
        private System.Windows.Forms.Button button_SelectAll;
        private System.Windows.Forms.Button button_SelectReverse;
        private System.Windows.Forms.Label label2;
    }
}

