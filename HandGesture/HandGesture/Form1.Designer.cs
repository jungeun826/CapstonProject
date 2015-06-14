namespace HandGesture
{
    partial class Form1
    {
        /// <summary>
        /// 필수 디자이너 변수입니다.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 사용 중인 모든 리소스를 정리합니다.
        /// </summary>
        /// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 디자이너에서 생성한 코드

        /// <summary>
        /// 디자이너 지원에 필요한 메서드입니다.
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마십시오.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.pointLabel = new System.Windows.Forms.Label();
            this.button_basic = new System.Windows.Forms.Button();
            this.button_fps = new System.Windows.Forms.Button();
            this.button_racing = new System.Windows.Forms.Button();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.메뉴ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.사용법ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.정보ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.종료ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.WCC = new System.Windows.Forms.NotifyIcon(this.components);
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.open = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.BASIC = new System.Windows.Forms.ToolStripMenuItem();
            this.FPS = new System.Windows.Forms.ToolStripMenuItem();
            this.RACING = new System.Windows.Forms.ToolStripMenuItem();
            this.INFO = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.EXIT = new System.Windows.Forms.ToolStripMenuItem();
            this.label1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.menuStrip1.SuspendLayout();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(5, 26);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(330, 250);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // pointLabel
            // 
            this.pointLabel.AutoSize = true;
            this.pointLabel.Location = new System.Drawing.Point(1008, 319);
            this.pointLabel.Name = "pointLabel";
            this.pointLabel.Size = new System.Drawing.Size(0, 12);
            this.pointLabel.TabIndex = 8;
            // 
            // button_basic
            // 
            this.button_basic.Location = new System.Drawing.Point(6, 287);
            this.button_basic.Name = "button_basic";
            this.button_basic.Size = new System.Drawing.Size(105, 50);
            this.button_basic.TabIndex = 9;
            this.button_basic.Text = "BASIC";
            this.button_basic.UseVisualStyleBackColor = true;
            this.button_basic.Click += new System.EventHandler(this.button_basic_Click);
            // 
            // button_fps
            // 
            this.button_fps.Location = new System.Drawing.Point(119, 287);
            this.button_fps.Name = "button_fps";
            this.button_fps.Size = new System.Drawing.Size(105, 50);
            this.button_fps.TabIndex = 10;
            this.button_fps.Text = "FPS";
            this.button_fps.UseVisualStyleBackColor = true;
            this.button_fps.Click += new System.EventHandler(this.button_fps_Click);
            // 
            // button_racing
            // 
            this.button_racing.Location = new System.Drawing.Point(232, 287);
            this.button_racing.Name = "button_racing";
            this.button_racing.Size = new System.Drawing.Size(105, 50);
            this.button_racing.TabIndex = 11;
            this.button_racing.Text = "RACING";
            this.button_racing.UseVisualStyleBackColor = true;
            this.button_racing.Click += new System.EventHandler(this.button_racing_Click);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.메뉴ToolStripMenuItem,
            this.종료ToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(342, 24);
            this.menuStrip1.TabIndex = 12;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // 메뉴ToolStripMenuItem
            // 
            this.메뉴ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.사용법ToolStripMenuItem,
            this.정보ToolStripMenuItem});
            this.메뉴ToolStripMenuItem.Name = "메뉴ToolStripMenuItem";
            this.메뉴ToolStripMenuItem.Size = new System.Drawing.Size(43, 20);
            this.메뉴ToolStripMenuItem.Text = "메뉴";
            // 
            // 사용법ToolStripMenuItem
            // 
            this.사용법ToolStripMenuItem.Name = "사용법ToolStripMenuItem";
            this.사용법ToolStripMenuItem.Size = new System.Drawing.Size(110, 22);
            this.사용법ToolStripMenuItem.Text = "사용법";
            this.사용법ToolStripMenuItem.Click += new System.EventHandler(this.사용법ToolStripMenuItem_Click);
            // 
            // 정보ToolStripMenuItem
            // 
            this.정보ToolStripMenuItem.Name = "정보ToolStripMenuItem";
            this.정보ToolStripMenuItem.Size = new System.Drawing.Size(110, 22);
            this.정보ToolStripMenuItem.Text = "정보";
            this.정보ToolStripMenuItem.Click += new System.EventHandler(this.정보ToolStripMenuItem_Click);
            // 
            // 종료ToolStripMenuItem
            // 
            this.종료ToolStripMenuItem.Name = "종료ToolStripMenuItem";
            this.종료ToolStripMenuItem.Size = new System.Drawing.Size(43, 20);
            this.종료ToolStripMenuItem.Text = "종료";
            this.종료ToolStripMenuItem.Click += new System.EventHandler(this.종료ToolStripMenuItem_Click);
            // 
            // WCC
            // 
            this.WCC.Icon = ((System.Drawing.Icon)(resources.GetObject("WCC.Icon")));
            this.WCC.Text = "notifyIcon1";
            this.WCC.Visible = true;
            this.WCC.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.notifyIcon1_MouseDoubleClick);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.open,
            this.toolStripSeparator2,
            this.BASIC,
            this.FPS,
            this.RACING,
            this.INFO,
            this.toolStripSeparator1,
            this.EXIT});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(118, 148);
            // 
            // open
            // 
            this.open.Name = "open";
            this.open.Size = new System.Drawing.Size(117, 22);
            this.open.Text = "열기";
            this.open.Click += new System.EventHandler(this.open_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(114, 6);
            // 
            // BASIC
            // 
            this.BASIC.Name = "BASIC";
            this.BASIC.Size = new System.Drawing.Size(117, 22);
            this.BASIC.Text = "BASIC";
            this.BASIC.Click += new System.EventHandler(this.BASIC_Click);
            // 
            // FPS
            // 
            this.FPS.Name = "FPS";
            this.FPS.Size = new System.Drawing.Size(117, 22);
            this.FPS.Text = "FPS";
            this.FPS.Click += new System.EventHandler(this.FPS_Click);
            // 
            // RACING
            // 
            this.RACING.Name = "RACING";
            this.RACING.Size = new System.Drawing.Size(117, 22);
            this.RACING.Text = "RACING";
            this.RACING.Click += new System.EventHandler(this.RACING_Click);
            // 
            // INFO
            // 
            this.INFO.Name = "INFO";
            this.INFO.Size = new System.Drawing.Size(117, 22);
            this.INFO.Text = "정보";
            this.INFO.Click += new System.EventHandler(this.INFO_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(114, 6);
            // 
            // EXIT
            // 
            this.EXIT.Name = "EXIT";
            this.EXIT.Size = new System.Drawing.Size(117, 22);
            this.EXIT.Text = "종료";
            this.EXIT.Click += new System.EventHandler(this.EXIT_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("돋움", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label1.Location = new System.Drawing.Point(279, 271);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(54, 15);
            this.label1.TabIndex = 13;
            this.label1.Text = "MODE";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(342, 345);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button_racing);
            this.Controls.Add(this.button_fps);
            this.Controls.Add(this.button_basic);
            this.Controls.Add(this.pointLabel);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Form1";
            this.Text = "Motion Recognition";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Label pointLabel;
        private System.Windows.Forms.Button button_basic;
        private System.Windows.Forms.Button button_fps;
        private System.Windows.Forms.Button button_racing;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem 메뉴ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 사용법ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 정보ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 종료ToolStripMenuItem;
        private System.Windows.Forms.NotifyIcon WCC;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem open;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem BASIC;
        private System.Windows.Forms.ToolStripMenuItem FPS;
        private System.Windows.Forms.ToolStripMenuItem RACING;
        private System.Windows.Forms.ToolStripMenuItem INFO;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem EXIT;
        private System.Windows.Forms.Label label1;
    }
}

