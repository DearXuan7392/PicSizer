
namespace PicSizer.Forms
{
    partial class UpdateSaveForm
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
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.button_Cancel = new System.Windows.Forms.Button();
            this.label_Speed = new System.Windows.Forms.Label();
            this.checkBox_RunWhenFinish = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // progressBar1
            // 
            this.progressBar1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBar1.Location = new System.Drawing.Point(12, 12);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(474, 23);
            this.progressBar1.TabIndex = 0;
            // 
            // button_Cancel
            // 
            this.button_Cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button_Cancel.Location = new System.Drawing.Point(411, 41);
            this.button_Cancel.Name = "button_Cancel";
            this.button_Cancel.Size = new System.Drawing.Size(75, 23);
            this.button_Cancel.TabIndex = 1;
            this.button_Cancel.Text = "取消";
            this.button_Cancel.UseVisualStyleBackColor = true;
            this.button_Cancel.Click += new System.EventHandler(this.OnStopClick);
            // 
            // label_Speed
            // 
            this.label_Speed.Location = new System.Drawing.Point(12, 41);
            this.label_Speed.Name = "label_Speed";
            this.label_Speed.Size = new System.Drawing.Size(255, 23);
            this.label_Speed.TabIndex = 2;
            this.label_Speed.Text = "0Bps / 0B";
            this.label_Speed.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // checkBox_RunWhenFinish
            // 
            this.checkBox_RunWhenFinish.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.checkBox_RunWhenFinish.AutoSize = true;
            this.checkBox_RunWhenFinish.Checked = true;
            this.checkBox_RunWhenFinish.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox_RunWhenFinish.Location = new System.Drawing.Point(273, 45);
            this.checkBox_RunWhenFinish.Name = "checkBox_RunWhenFinish";
            this.checkBox_RunWhenFinish.Size = new System.Drawing.Size(132, 16);
            this.checkBox_RunWhenFinish.TabIndex = 3;
            this.checkBox_RunWhenFinish.Text = "下载完成后立即运行";
            this.checkBox_RunWhenFinish.UseVisualStyleBackColor = true;
            // 
            // UpdateSaveForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(498, 68);
            this.ControlBox = false;
            this.Controls.Add(this.checkBox_RunWhenFinish);
            this.Controls.Add(this.label_Speed);
            this.Controls.Add(this.button_Cancel);
            this.Controls.Add(this.progressBar1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "UpdateSaveForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "正在建立连接...";
            this.Load += new System.EventHandler(this.UpdateSaveForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Button button_Cancel;
        private System.Windows.Forms.Label label_Speed;
        private System.Windows.Forms.CheckBox checkBox_RunWhenFinish;
    }
}