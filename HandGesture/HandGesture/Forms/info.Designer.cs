namespace HandGesture
{
    partial class info
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
            this.label_ver = new System.Windows.Forms.Label();
            this.label_date = new System.Windows.Forms.Label();
            this.label_developers = new System.Windows.Forms.Label();
            this.pictureBox_icon = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_icon)).BeginInit();
            this.SuspendLayout();
            // 
            // label_ver
            // 
            this.label_ver.AutoSize = true;
            this.label_ver.Font = new System.Drawing.Font("굴림", 11F);
            this.label_ver.ForeColor = System.Drawing.Color.Teal;
            this.label_ver.Location = new System.Drawing.Point(112, 132);
            this.label_ver.Name = "label_ver";
            this.label_ver.Size = new System.Drawing.Size(45, 15);
            this.label_ver.TabIndex = 0;
            this.label_ver.Text = "Ver. 1";
            // 
            // label_date
            // 
            this.label_date.AutoSize = true;
            this.label_date.Location = new System.Drawing.Point(103, 160);
            this.label_date.Name = "label_date";
            this.label_date.Size = new System.Drawing.Size(61, 12);
            this.label_date.TabIndex = 1;
            this.label_date.Text = "2015 06 14";
            // 
            // label_developers
            // 
            this.label_developers.AutoSize = true;
            this.label_developers.ForeColor = System.Drawing.Color.Teal;
            this.label_developers.Location = new System.Drawing.Point(91, 182);
            this.label_developers.Name = "label_developers";
            this.label_developers.Size = new System.Drawing.Size(93, 12);
            this.label_developers.TabIndex = 2;
            this.label_developers.Text = "12091441 김용래";
            // 
            // pictureBox_icon
            // 
            this.pictureBox_icon.Location = new System.Drawing.Point(77, 13);
            this.pictureBox_icon.Name = "pictureBox_icon";
            this.pictureBox_icon.Size = new System.Drawing.Size(113, 113);
            this.pictureBox_icon.TabIndex = 3;
            this.pictureBox_icon.TabStop = false;
            // 
            // info
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Controls.Add(this.pictureBox_icon);
            this.Controls.Add(this.label_developers);
            this.Controls.Add(this.label_date);
            this.Controls.Add(this.label_ver);
            this.Name = "info";
            this.Text = "정보";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_icon)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label_ver;
        private System.Windows.Forms.Label label_date;
        private System.Windows.Forms.Label label_developers;
        private System.Windows.Forms.PictureBox pictureBox_icon;
    }
}