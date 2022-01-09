
namespace PicSizer
{
    partial class SettingForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SettingForm));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.numericUpDown_LimitHeight = new System.Windows.Forms.NumericUpDown();
            this.numericUpDown_LimitWidth = new System.Windows.Forms.NumericUpDown();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.comboBox_ResizeMode = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.comboBox_KB_or_MB = new System.Windows.Forms.ComboBox();
            this.numericUpDown_Size = new System.Windows.Forms.NumericUpDown();
            this.label8 = new System.Windows.Forms.Label();
            this.numericUpDown_Value = new System.Windows.Forms.NumericUpDown();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.comboBox_CompressionMode = new System.Windows.Forms.ComboBox();
            this.button_Save = new System.Windows.Forms.Button();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.comboBox_ExtensionMode = new System.Windows.Forms.ComboBox();
            this.label12 = new System.Windows.Forms.Label();
            this.textBox_CustomRenameStr = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.numericUpDown_StartIndex = new System.Windows.Forms.NumericUpDown();
            this.label10 = new System.Windows.Forms.Label();
            this.comboBox_RenameMode = new System.Windows.Forms.ComboBox();
            this.label9 = new System.Windows.Forms.Label();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.trackBar_Threads = new System.Windows.Forms.TrackBar();
            this.numericUpDown_Threads = new System.Windows.Forms.NumericUpDown();
            this.label15 = new System.Windows.Forms.Label();
            this.checkBox_TopMost = new System.Windows.Forms.CheckBox();
            this.checkBox_AllowAnyExtension = new System.Windows.Forms.CheckBox();
            this.comboBox_DoWhenException = new System.Windows.Forms.ComboBox();
            this.label13 = new System.Windows.Forms.Label();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.checkBox_UseGPU = new System.Windows.Forms.CheckBox();
            this.numericUpDown_Brightness = new System.Windows.Forms.NumericUpDown();
            this.trackBar_Brightness = new System.Windows.Forms.TrackBar();
            this.label14 = new System.Windows.Forms.Label();
            this.button_Export = new System.Windows.Forms.Button();
            this.button_ReadSetting = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_LimitHeight)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_LimitWidth)).BeginInit();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_Size)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_Value)).BeginInit();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_StartIndex)).BeginInit();
            this.groupBox4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar_Threads)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_Threads)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.groupBox5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_Brightness)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar_Brightness)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.numericUpDown_LimitHeight);
            this.groupBox1.Controls.Add(this.numericUpDown_LimitWidth);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.comboBox_ResizeMode);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(228, 146);
            this.groupBox1.TabIndex = 8;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "尺寸";
            // 
            // numericUpDown_LimitHeight
            // 
            this.numericUpDown_LimitHeight.Location = new System.Drawing.Point(65, 77);
            this.numericUpDown_LimitHeight.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.numericUpDown_LimitHeight.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDown_LimitHeight.Name = "numericUpDown_LimitHeight";
            this.numericUpDown_LimitHeight.Size = new System.Drawing.Size(120, 21);
            this.numericUpDown_LimitHeight.TabIndex = 16;
            this.numericUpDown_LimitHeight.Value = new decimal(new int[] {
            1080,
            0,
            0,
            0});
            // 
            // numericUpDown_LimitWidth
            // 
            this.numericUpDown_LimitWidth.Location = new System.Drawing.Point(65, 50);
            this.numericUpDown_LimitWidth.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.numericUpDown_LimitWidth.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDown_LimitWidth.Name = "numericUpDown_LimitWidth";
            this.numericUpDown_LimitWidth.Size = new System.Drawing.Size(120, 21);
            this.numericUpDown_LimitWidth.TabIndex = 13;
            this.numericUpDown_LimitWidth.Value = new decimal(new int[] {
            1920,
            0,
            0,
            0});
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(192, 79);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(29, 12);
            this.label5.TabIndex = 15;
            this.label5.Text = "像素";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(192, 52);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(29, 12);
            this.label4.TabIndex = 14;
            this.label4.Text = "像素";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 79);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(47, 12);
            this.label3.TabIndex = 11;
            this.label3.Text = "Height:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 52);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(41, 12);
            this.label2.TabIndex = 10;
            this.label2.Text = "Width:";
            // 
            // comboBox_ResizeMode
            // 
            this.comboBox_ResizeMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_ResizeMode.FormattingEnabled = true;
            this.comboBox_ResizeMode.Items.AddRange(new object[] {
            "无修正",
            "不小于限定值",
            "不大于限定值",
            "强制修正"});
            this.comboBox_ResizeMode.Location = new System.Drawing.Point(65, 14);
            this.comboBox_ResizeMode.Name = "comboBox_ResizeMode";
            this.comboBox_ResizeMode.Size = new System.Drawing.Size(121, 20);
            this.comboBox_ResizeMode.TabIndex = 9;
            this.comboBox_ResizeMode.SelectedIndexChanged += new System.EventHandler(this.comboBox_ResizeMode_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 17);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 12);
            this.label1.TabIndex = 8;
            this.label1.Text = "尺寸修正";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.comboBox_KB_or_MB);
            this.groupBox2.Controls.Add(this.numericUpDown_Size);
            this.groupBox2.Controls.Add(this.label8);
            this.groupBox2.Controls.Add(this.numericUpDown_Value);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Controls.Add(this.comboBox_CompressionMode);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox2.Location = new System.Drawing.Point(0, 0);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(206, 146);
            this.groupBox2.TabIndex = 9;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "压缩";
            // 
            // comboBox_KB_or_MB
            // 
            this.comboBox_KB_or_MB.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_KB_or_MB.FormattingEnabled = true;
            this.comboBox_KB_or_MB.Items.AddRange(new object[] {
            "KB",
            "MB"});
            this.comboBox_KB_or_MB.Location = new System.Drawing.Point(148, 46);
            this.comboBox_KB_or_MB.Name = "comboBox_KB_or_MB";
            this.comboBox_KB_or_MB.Size = new System.Drawing.Size(40, 20);
            this.comboBox_KB_or_MB.TabIndex = 17;
            this.comboBox_KB_or_MB.SelectedIndexChanged += new System.EventHandler(this.comboBox3_SelectedIndexChanged);
            // 
            // numericUpDown_Size
            // 
            this.numericUpDown_Size.Location = new System.Drawing.Point(66, 46);
            this.numericUpDown_Size.Maximum = new decimal(new int[] {
            1048576,
            0,
            0,
            0});
            this.numericUpDown_Size.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDown_Size.Name = "numericUpDown_Size";
            this.numericUpDown_Size.Size = new System.Drawing.Size(76, 21);
            this.numericUpDown_Size.TabIndex = 16;
            this.numericUpDown_Size.Value = new decimal(new int[] {
            200,
            0,
            0,
            0});
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(6, 48);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(59, 12);
            this.label8.TabIndex = 15;
            this.label8.Text = "指定大小:";
            // 
            // numericUpDown_Value
            // 
            this.numericUpDown_Value.Location = new System.Drawing.Point(66, 73);
            this.numericUpDown_Value.Name = "numericUpDown_Value";
            this.numericUpDown_Value.Size = new System.Drawing.Size(120, 21);
            this.numericUpDown_Value.TabIndex = 14;
            this.numericUpDown_Value.Value = new decimal(new int[] {
            80,
            0,
            0,
            0});
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(6, 75);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(59, 12);
            this.label7.TabIndex = 12;
            this.label7.Text = "指定画质:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(6, 23);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(59, 12);
            this.label6.TabIndex = 11;
            this.label6.Text = "压缩模式:";
            // 
            // comboBox_CompressionMode
            // 
            this.comboBox_CompressionMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_CompressionMode.FormattingEnabled = true;
            this.comboBox_CompressionMode.Items.AddRange(new object[] {
            "指定大小",
            "指定画质"});
            this.comboBox_CompressionMode.Location = new System.Drawing.Point(66, 20);
            this.comboBox_CompressionMode.Name = "comboBox_CompressionMode";
            this.comboBox_CompressionMode.Size = new System.Drawing.Size(121, 20);
            this.comboBox_CompressionMode.TabIndex = 10;
            this.comboBox_CompressionMode.SelectedIndexChanged += new System.EventHandler(this.comboBox_CompressionMode_SelectedIndexChanged);
            // 
            // button_Save
            // 
            this.button_Save.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button_Save.Location = new System.Drawing.Point(371, 475);
            this.button_Save.Name = "button_Save";
            this.button_Save.Size = new System.Drawing.Size(79, 42);
            this.button_Save.TabIndex = 10;
            this.button_Save.Text = "保存";
            this.button_Save.UseVisualStyleBackColor = true;
            this.button_Save.Click += new System.EventHandler(this.button_Save_Click);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.comboBox_ExtensionMode);
            this.groupBox3.Controls.Add(this.label12);
            this.groupBox3.Controls.Add(this.textBox_CustomRenameStr);
            this.groupBox3.Controls.Add(this.label11);
            this.groupBox3.Controls.Add(this.numericUpDown_StartIndex);
            this.groupBox3.Controls.Add(this.label10);
            this.groupBox3.Controls.Add(this.comboBox_RenameMode);
            this.groupBox3.Controls.Add(this.label9);
            this.groupBox3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox3.Location = new System.Drawing.Point(0, 0);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(206, 165);
            this.groupBox3.TabIndex = 11;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "命名";
            // 
            // comboBox_ExtensionMode
            // 
            this.comboBox_ExtensionMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_ExtensionMode.FormattingEnabled = true;
            this.comboBox_ExtensionMode.Items.AddRange(new object[] {
            "JPG/JPEG",
            "PNG",
            "BMP",
            "TIFF",
            "原格式"});
            this.comboBox_ExtensionMode.Location = new System.Drawing.Point(66, 44);
            this.comboBox_ExtensionMode.Name = "comboBox_ExtensionMode";
            this.comboBox_ExtensionMode.Size = new System.Drawing.Size(121, 20);
            this.comboBox_ExtensionMode.TabIndex = 21;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(8, 47);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(59, 12);
            this.label12.TabIndex = 20;
            this.label12.Text = "指定后缀:";
            // 
            // textBox_CustomRenameStr
            // 
            this.textBox_CustomRenameStr.Location = new System.Drawing.Point(66, 71);
            this.textBox_CustomRenameStr.Name = "textBox_CustomRenameStr";
            this.textBox_CustomRenameStr.Size = new System.Drawing.Size(121, 21);
            this.textBox_CustomRenameStr.TabIndex = 19;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(8, 74);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(59, 12);
            this.label11.TabIndex = 18;
            this.label11.Text = "自定名称:";
            // 
            // numericUpDown_StartIndex
            // 
            this.numericUpDown_StartIndex.Location = new System.Drawing.Point(66, 98);
            this.numericUpDown_StartIndex.Maximum = new decimal(new int[] {
            1024,
            0,
            0,
            0});
            this.numericUpDown_StartIndex.Minimum = new decimal(new int[] {
            1024,
            0,
            0,
            -2147483648});
            this.numericUpDown_StartIndex.Name = "numericUpDown_StartIndex";
            this.numericUpDown_StartIndex.Size = new System.Drawing.Size(122, 21);
            this.numericUpDown_StartIndex.TabIndex = 17;
            this.numericUpDown_StartIndex.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(6, 100);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(59, 12);
            this.label10.TabIndex = 2;
            this.label10.Text = "起始下标:";
            // 
            // comboBox_RenameMode
            // 
            this.comboBox_RenameMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_RenameMode.FormattingEnabled = true;
            this.comboBox_RenameMode.Items.AddRange(new object[] {
            "数字",
            "原名",
            "混合方式"});
            this.comboBox_RenameMode.Location = new System.Drawing.Point(66, 18);
            this.comboBox_RenameMode.Name = "comboBox_RenameMode";
            this.comboBox_RenameMode.Size = new System.Drawing.Size(121, 20);
            this.comboBox_RenameMode.TabIndex = 1;
            this.comboBox_RenameMode.SelectedIndexChanged += new System.EventHandler(this.comboBox_RenameMode_SelectedIndexChanged);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(8, 21);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(59, 12);
            this.label9.TabIndex = 0;
            this.label9.Text = "命名方式:";
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.trackBar_Threads);
            this.groupBox4.Controls.Add(this.numericUpDown_Threads);
            this.groupBox4.Controls.Add(this.label15);
            this.groupBox4.Controls.Add(this.checkBox_TopMost);
            this.groupBox4.Controls.Add(this.checkBox_AllowAnyExtension);
            this.groupBox4.Controls.Add(this.comboBox_DoWhenException);
            this.groupBox4.Controls.Add(this.label13);
            this.groupBox4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox4.Location = new System.Drawing.Point(0, 0);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(228, 165);
            this.groupBox4.TabIndex = 13;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "其它";
            // 
            // trackBar_Threads
            // 
            this.trackBar_Threads.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.trackBar_Threads.Location = new System.Drawing.Point(8, 112);
            this.trackBar_Threads.Minimum = 1;
            this.trackBar_Threads.Name = "trackBar_Threads";
            this.trackBar_Threads.Size = new System.Drawing.Size(213, 45);
            this.trackBar_Threads.TabIndex = 6;
            this.trackBar_Threads.Value = 1;
            this.trackBar_Threads.Scroll += new System.EventHandler(this.trackBar_Threads_Scroll);
            // 
            // numericUpDown_Threads
            // 
            this.numericUpDown_Threads.Location = new System.Drawing.Point(83, 85);
            this.numericUpDown_Threads.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.numericUpDown_Threads.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDown_Threads.Name = "numericUpDown_Threads";
            this.numericUpDown_Threads.Size = new System.Drawing.Size(120, 21);
            this.numericUpDown_Threads.TabIndex = 5;
            this.numericUpDown_Threads.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDown_Threads.ValueChanged += new System.EventHandler(this.numericUpDown_Threads_ValueChanged);
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(6, 87);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(71, 12);
            this.label15.TabIndex = 4;
            this.label15.Text = "最大线程数:";
            // 
            // checkBox_TopMost
            // 
            this.checkBox_TopMost.AutoSize = true;
            this.checkBox_TopMost.Location = new System.Drawing.Point(8, 68);
            this.checkBox_TopMost.Name = "checkBox_TopMost";
            this.checkBox_TopMost.Size = new System.Drawing.Size(96, 16);
            this.checkBox_TopMost.TabIndex = 3;
            this.checkBox_TopMost.Text = "置顶PicSizer";
            this.checkBox_TopMost.UseVisualStyleBackColor = true;
            this.checkBox_TopMost.CheckedChanged += new System.EventHandler(this.checkBox_TopMost_CheckedChanged);
            // 
            // checkBox_AllowAnyExtension
            // 
            this.checkBox_AllowAnyExtension.AutoSize = true;
            this.checkBox_AllowAnyExtension.Location = new System.Drawing.Point(8, 46);
            this.checkBox_AllowAnyExtension.Name = "checkBox_AllowAnyExtension";
            this.checkBox_AllowAnyExtension.Size = new System.Drawing.Size(96, 16);
            this.checkBox_AllowAnyExtension.TabIndex = 2;
            this.checkBox_AllowAnyExtension.Text = "允许任意后缀";
            this.checkBox_AllowAnyExtension.UseVisualStyleBackColor = true;
            // 
            // comboBox_DoWhenException
            // 
            this.comboBox_DoWhenException.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_DoWhenException.FormattingEnabled = true;
            this.comboBox_DoWhenException.Items.AddRange(new object[] {
            "忽略并继续编号",
            "忽略并跳过编号",
            "显示并继续编号",
            "显示并跳过编号",
            "显示并立即结束"});
            this.comboBox_DoWhenException.Location = new System.Drawing.Point(83, 18);
            this.comboBox_DoWhenException.Name = "comboBox_DoWhenException";
            this.comboBox_DoWhenException.Size = new System.Drawing.Size(120, 20);
            this.comboBox_DoWhenException.TabIndex = 1;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(6, 21);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(71, 12);
            this.label13.TabIndex = 0;
            this.label13.Text = "发生异常时:";
            // 
            // splitContainer1
            // 
            this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer1.IsSplitterFixed = true;
            this.splitContainer1.Location = new System.Drawing.Point(12, 164);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.groupBox3);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.groupBox4);
            this.splitContainer1.Size = new System.Drawing.Size(438, 165);
            this.splitContainer1.SplitterDistance = 206;
            this.splitContainer1.TabIndex = 14;
            // 
            // splitContainer2
            // 
            this.splitContainer2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer2.IsSplitterFixed = true;
            this.splitContainer2.Location = new System.Drawing.Point(12, 12);
            this.splitContainer2.Name = "splitContainer2";
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.groupBox2);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.groupBox1);
            this.splitContainer2.Size = new System.Drawing.Size(438, 146);
            this.splitContainer2.SplitterDistance = 206;
            this.splitContainer2.TabIndex = 15;
            // 
            // groupBox5
            // 
            this.groupBox5.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox5.Controls.Add(this.checkBox_UseGPU);
            this.groupBox5.Controls.Add(this.numericUpDown_Brightness);
            this.groupBox5.Controls.Add(this.trackBar_Brightness);
            this.groupBox5.Controls.Add(this.label14);
            this.groupBox5.Location = new System.Drawing.Point(12, 335);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(438, 83);
            this.groupBox5.TabIndex = 16;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "图像处理";
            // 
            // checkBox_UseGPU
            // 
            this.checkBox_UseGPU.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.checkBox_UseGPU.AutoSize = true;
            this.checkBox_UseGPU.Enabled = false;
            this.checkBox_UseGPU.Location = new System.Drawing.Point(360, 61);
            this.checkBox_UseGPU.Name = "checkBox_UseGPU";
            this.checkBox_UseGPU.Size = new System.Drawing.Size(72, 16);
            this.checkBox_UseGPU.TabIndex = 3;
            this.checkBox_UseGPU.Text = "硬件加速";
            this.checkBox_UseGPU.UseVisualStyleBackColor = true;
            // 
            // numericUpDown_Brightness
            // 
            this.numericUpDown_Brightness.Location = new System.Drawing.Point(47, 20);
            this.numericUpDown_Brightness.Name = "numericUpDown_Brightness";
            this.numericUpDown_Brightness.Size = new System.Drawing.Size(42, 21);
            this.numericUpDown_Brightness.TabIndex = 2;
            this.numericUpDown_Brightness.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.numericUpDown_Brightness.ValueChanged += new System.EventHandler(this.numericUpDown_Brightness_ValueChanged);
            // 
            // trackBar_Brightness
            // 
            this.trackBar_Brightness.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.trackBar_Brightness.LargeChange = 10;
            this.trackBar_Brightness.Location = new System.Drawing.Point(95, 20);
            this.trackBar_Brightness.Maximum = 100;
            this.trackBar_Brightness.Name = "trackBar_Brightness";
            this.trackBar_Brightness.Size = new System.Drawing.Size(336, 45);
            this.trackBar_Brightness.SmallChange = 5;
            this.trackBar_Brightness.TabIndex = 1;
            this.trackBar_Brightness.TickFrequency = 10;
            this.trackBar_Brightness.Value = 100;
            this.trackBar_Brightness.Scroll += new System.EventHandler(this.trackBar_Brightness_Scroll);
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(6, 22);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(35, 12);
            this.label14.TabIndex = 0;
            this.label14.Text = "亮度:";
            // 
            // button_Export
            // 
            this.button_Export.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button_Export.Location = new System.Drawing.Point(286, 475);
            this.button_Export.Name = "button_Export";
            this.button_Export.Size = new System.Drawing.Size(79, 42);
            this.button_Export.TabIndex = 17;
            this.button_Export.Text = "导出配置";
            this.button_Export.UseVisualStyleBackColor = true;
            this.button_Export.Click += new System.EventHandler(this.button_Export_Click);
            // 
            // button_ReadSetting
            // 
            this.button_ReadSetting.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button_ReadSetting.Location = new System.Drawing.Point(201, 475);
            this.button_ReadSetting.Name = "button_ReadSetting";
            this.button_ReadSetting.Size = new System.Drawing.Size(79, 42);
            this.button_ReadSetting.TabIndex = 18;
            this.button_ReadSetting.Text = "读取配置";
            this.button_ReadSetting.UseVisualStyleBackColor = true;
            this.button_ReadSetting.Click += new System.EventHandler(this.button_ReadSetting_Click);
            // 
            // SettingForm
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(462, 529);
            this.Controls.Add(this.button_ReadSetting);
            this.Controls.Add(this.button_Export);
            this.Controls.Add(this.groupBox5);
            this.Controls.Add(this.splitContainer2);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.button_Save);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SettingForm";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "设置";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SettingForm_FormClosing);
            this.Load += new System.EventHandler(this.SettingForm_Load);
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.SettingForm_DragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.SettingForm_DragEnter);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_LimitHeight)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_LimitWidth)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_Size)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_Value)).EndInit();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_StartIndex)).EndInit();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar_Threads)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_Threads)).EndInit();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_Brightness)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar_Brightness)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox comboBox_ResizeMode;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.ComboBox comboBox_CompressionMode;
        private System.Windows.Forms.NumericUpDown numericUpDown_LimitHeight;
        private System.Windows.Forms.NumericUpDown numericUpDown_LimitWidth;
        private System.Windows.Forms.NumericUpDown numericUpDown_Value;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ComboBox comboBox_KB_or_MB;
        private System.Windows.Forms.NumericUpDown numericUpDown_Size;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Button button_Save;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.ComboBox comboBox_RenameMode;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.ComboBox comboBox_ExtensionMode;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.TextBox textBox_CustomRenameStr;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.NumericUpDown numericUpDown_StartIndex;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.ComboBox comboBox_DoWhenException;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.NumericUpDown numericUpDown_Brightness;
        private System.Windows.Forms.TrackBar trackBar_Brightness;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.CheckBox checkBox_AllowAnyExtension;
        public System.Windows.Forms.CheckBox checkBox_UseGPU;
        private System.Windows.Forms.CheckBox checkBox_TopMost;
        private System.Windows.Forms.NumericUpDown numericUpDown_Threads;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.TrackBar trackBar_Threads;
        private System.Windows.Forms.Button button_Export;
        private System.Windows.Forms.Button button_ReadSetting;
    }
}