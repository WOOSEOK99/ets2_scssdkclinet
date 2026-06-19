namespace SCSSdkClient.Demo {
    partial class SCSSdkClientDemo {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code
        private void InitializeComponent() {
            this.components = new System.ComponentModel.Container();
            this.timerUI = new System.Windows.Forms.Timer(this.components);
            this.panelTop = new System.Windows.Forms.Panel();
            this.lblTitle = new System.Windows.Forms.Label();
            this.btnOpenDashboard = new System.Windows.Forms.Button();
            this.btnConnect = new System.Windows.Forms.Button();
            this.btnDisconnect = new System.Windows.Forms.Button();
            this.panelStatus = new System.Windows.Forms.Panel();
            this.panelSdkLight = new System.Windows.Forms.Panel();
            this.lblSdkLight = new System.Windows.Forms.Label();
            this.panelGameLight = new System.Windows.Forms.Panel();
            this.lblGameLight = new System.Windows.Forms.Label();
            this.panelWsLight = new System.Windows.Forms.Panel();
            this.lblWsLight = new System.Windows.Forms.Label();
            this.panelPausedLight = new System.Windows.Forms.Panel();
            this.lblPausedLight = new System.Windows.Forms.Label();
            this.panelStats = new System.Windows.Forms.Panel();
            this.lblStatSpeed = new System.Windows.Forms.Label();
            this.lblStatRpm = new System.Windows.Forms.Label();
            this.lblStatGear = new System.Windows.Forms.Label();
            this.lblStatFuel = new System.Windows.Forms.Label();
            this.lblStatClients = new System.Windows.Forms.Label();
            this.lblStatUpdateRate = new System.Windows.Forms.Label();
            this.panelLog = new System.Windows.Forms.Panel();
            this.rtbLog = new System.Windows.Forms.RichTextBox();
            this.btnClearLog = new System.Windows.Forms.Button();
            this.panelTop.SuspendLayout();
            this.panelStatus.SuspendLayout();
            this.panelStats.SuspendLayout();
            this.panelLog.SuspendLayout();
            this.SuspendLayout();
            // 
            // timerUI
            // 
            this.timerUI.Interval = 500;
            this.timerUI.Tick += new System.EventHandler(this.timerUI_Tick);
            // 
            // panelTop
            // 
            this.panelTop.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(13)))), ((int)(((byte)(17)))), ((int)(((byte)(23)))));
            this.panelTop.Controls.Add(this.lblTitle);
            this.panelTop.Controls.Add(this.btnClearLog);
            this.panelTop.Controls.Add(this.btnOpenDashboard);
            this.panelTop.Controls.Add(this.btnConnect);
            this.panelTop.Controls.Add(this.btnDisconnect);
            this.panelTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelTop.Location = new System.Drawing.Point(0, 0);
            this.panelTop.Name = "panelTop";
            this.panelTop.Padding = new System.Windows.Forms.Padding(16, 10, 16, 10);
            this.panelTop.Size = new System.Drawing.Size(680, 103);
            this.panelTop.TabIndex = 3;
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.lblTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(88)))), ((int)(((byte)(166)))), ((int)(((byte)(255)))));
            this.lblTitle.Location = new System.Drawing.Point(16, 10);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(372, 25);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "ETS2 / ATS  Telemetry WebSocket Server";
            // 
            // btnOpenDashboard
            // 
            this.btnOpenDashboard.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(111)))), ((int)(((byte)(235)))));
            this.btnOpenDashboard.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnOpenDashboard.FlatAppearance.BorderSize = 0;
            this.btnOpenDashboard.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnOpenDashboard.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnOpenDashboard.ForeColor = System.Drawing.Color.White;
            this.btnOpenDashboard.Location = new System.Drawing.Point(285, 38);
            this.btnOpenDashboard.Name = "btnOpenDashboard";
            this.btnOpenDashboard.Size = new System.Drawing.Size(140, 36);
            this.btnOpenDashboard.TabIndex = 1;
            this.btnOpenDashboard.Text = "🌐 웹 대시보드 열기";
            this.btnOpenDashboard.UseVisualStyleBackColor = false;
            this.btnOpenDashboard.Click += new System.EventHandler(this.btnOpenDashboard_Click);
            // 
            // btnConnect
            // 
            this.btnConnect.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(134)))), ((int)(((byte)(54)))));
            this.btnConnect.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnConnect.FlatAppearance.BorderSize = 0;
            this.btnConnect.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnConnect.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnConnect.ForeColor = System.Drawing.Color.White;
            this.btnConnect.Location = new System.Drawing.Point(440, 38);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(110, 36);
            this.btnConnect.TabIndex = 2;
            this.btnConnect.Text = "▶  연결 시작";
            this.btnConnect.UseVisualStyleBackColor = false;
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // btnDisconnect
            // 
            this.btnDisconnect.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(139)))), ((int)(((byte)(35)))), ((int)(((byte)(35)))));
            this.btnDisconnect.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnDisconnect.Enabled = false;
            this.btnDisconnect.FlatAppearance.BorderSize = 0;
            this.btnDisconnect.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDisconnect.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnDisconnect.ForeColor = System.Drawing.Color.White;
            this.btnDisconnect.Location = new System.Drawing.Point(558, 38);
            this.btnDisconnect.Name = "btnDisconnect";
            this.btnDisconnect.Size = new System.Drawing.Size(110, 36);
            this.btnDisconnect.TabIndex = 3;
            this.btnDisconnect.Text = "■  연결 해제";
            this.btnDisconnect.UseVisualStyleBackColor = false;
            this.btnDisconnect.Click += new System.EventHandler(this.btnDisconnect_Click);
            // 
            // panelStatus
            // 
            this.panelStatus.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(35)))), ((int)(((byte)(43)))));
            this.panelStatus.Controls.Add(this.panelSdkLight);
            this.panelStatus.Controls.Add(this.lblSdkLight);
            this.panelStatus.Controls.Add(this.panelGameLight);
            this.panelStatus.Controls.Add(this.lblGameLight);
            this.panelStatus.Controls.Add(this.panelWsLight);
            this.panelStatus.Controls.Add(this.lblWsLight);
            this.panelStatus.Controls.Add(this.panelPausedLight);
            this.panelStatus.Controls.Add(this.lblPausedLight);
            this.panelStatus.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelStatus.Location = new System.Drawing.Point(0, 103);
            this.panelStatus.Name = "panelStatus";
            this.panelStatus.Padding = new System.Windows.Forms.Padding(16, 0, 16, 0);
            this.panelStatus.Size = new System.Drawing.Size(680, 60);
            this.panelStatus.TabIndex = 2;
            // 
            // panelSdkLight
            // 
            this.panelSdkLight.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(68)))), ((int)(((byte)(68)))));
            this.panelSdkLight.Location = new System.Drawing.Point(20, 24);
            this.panelSdkLight.Name = "panelSdkLight";
            this.panelSdkLight.Size = new System.Drawing.Size(12, 12);
            this.panelSdkLight.TabIndex = 0;
            // 
            // lblSdkLight
            // 
            this.lblSdkLight.AutoSize = true;
            this.lblSdkLight.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblSdkLight.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(139)))), ((int)(((byte)(148)))), ((int)(((byte)(158)))));
            this.lblSdkLight.Location = new System.Drawing.Point(38, 21);
            this.lblSdkLight.Name = "lblSdkLight";
            this.lblSdkLight.Size = new System.Drawing.Size(79, 15);
            this.lblSdkLight.TabIndex = 1;
            this.lblSdkLight.Text = "SDK 플러그인";
            // 
            // panelGameLight
            // 
            this.panelGameLight.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(68)))), ((int)(((byte)(68)))));
            this.panelGameLight.Location = new System.Drawing.Point(160, 24);
            this.panelGameLight.Name = "panelGameLight";
            this.panelGameLight.Size = new System.Drawing.Size(12, 12);
            this.panelGameLight.TabIndex = 2;
            // 
            // lblGameLight
            // 
            this.lblGameLight.AutoSize = true;
            this.lblGameLight.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblGameLight.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(139)))), ((int)(((byte)(148)))), ((int)(((byte)(158)))));
            this.lblGameLight.Location = new System.Drawing.Point(178, 21);
            this.lblGameLight.Name = "lblGameLight";
            this.lblGameLight.Size = new System.Drawing.Size(58, 15);
            this.lblGameLight.TabIndex = 3;
            this.lblGameLight.Text = "게임 연결";
            // 
            // panelWsLight
            // 
            this.panelWsLight.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(68)))), ((int)(((byte)(68)))));
            this.panelWsLight.Location = new System.Drawing.Point(300, 24);
            this.panelWsLight.Name = "panelWsLight";
            this.panelWsLight.Size = new System.Drawing.Size(12, 12);
            this.panelWsLight.TabIndex = 4;
            // 
            // lblWsLight
            // 
            this.lblWsLight.AutoSize = true;
            this.lblWsLight.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblWsLight.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(139)))), ((int)(((byte)(148)))), ((int)(((byte)(158)))));
            this.lblWsLight.Location = new System.Drawing.Point(318, 21);
            this.lblWsLight.Name = "lblWsLight";
            this.lblWsLight.Size = new System.Drawing.Size(93, 15);
            this.lblWsLight.TabIndex = 5;
            this.lblWsLight.Text = "WebSocket 서버";
            // 
            // panelPausedLight
            // 
            this.panelPausedLight.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(68)))), ((int)(((byte)(68)))));
            this.panelPausedLight.Location = new System.Drawing.Point(460, 24);
            this.panelPausedLight.Name = "panelPausedLight";
            this.panelPausedLight.Size = new System.Drawing.Size(12, 12);
            this.panelPausedLight.TabIndex = 6;
            // 
            // lblPausedLight
            // 
            this.lblPausedLight.AutoSize = true;
            this.lblPausedLight.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblPausedLight.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(139)))), ((int)(((byte)(148)))), ((int)(((byte)(158)))));
            this.lblPausedLight.Location = new System.Drawing.Point(478, 21);
            this.lblPausedLight.Name = "lblPausedLight";
            this.lblPausedLight.Size = new System.Drawing.Size(82, 15);
            this.lblPausedLight.TabIndex = 7;
            this.lblPausedLight.Text = "게임 일시정지";
            // 
            // panelStats
            // 
            this.panelStats.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(27)))), ((int)(((byte)(34)))));
            this.panelStats.Controls.Add(this.lblStatSpeed);
            this.panelStats.Controls.Add(this.lblStatRpm);
            this.panelStats.Controls.Add(this.lblStatGear);
            this.panelStats.Controls.Add(this.lblStatFuel);
            this.panelStats.Controls.Add(this.lblStatClients);
            this.panelStats.Controls.Add(this.lblStatUpdateRate);
            this.panelStats.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelStats.Location = new System.Drawing.Point(0, 163);
            this.panelStats.Name = "panelStats";
            this.panelStats.Padding = new System.Windows.Forms.Padding(16, 8, 16, 8);
            this.panelStats.Size = new System.Drawing.Size(680, 90);
            this.panelStats.TabIndex = 1;
            // 
            // lblStatSpeed
            // 
            this.lblStatSpeed.AutoSize = true;
            this.lblStatSpeed.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblStatSpeed.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(201)))), ((int)(((byte)(209)))), ((int)(((byte)(217)))));
            this.lblStatSpeed.Location = new System.Drawing.Point(10, 8);
            this.lblStatSpeed.Name = "lblStatSpeed";
            this.lblStatSpeed.Size = new System.Drawing.Size(77, 15);
            this.lblStatSpeed.TabIndex = 0;
            this.lblStatSpeed.Text = "속도: -  km/h";
            // 
            // lblStatRpm
            // 
            this.lblStatRpm.AutoSize = true;
            this.lblStatRpm.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblStatRpm.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(201)))), ((int)(((byte)(209)))), ((int)(((byte)(217)))));
            this.lblStatRpm.Location = new System.Drawing.Point(140, 8);
            this.lblStatRpm.Name = "lblStatRpm";
            this.lblStatRpm.Size = new System.Drawing.Size(43, 15);
            this.lblStatRpm.TabIndex = 1;
            this.lblStatRpm.Text = "RPM: -";
            // 
            // lblStatGear
            // 
            this.lblStatGear.AutoSize = true;
            this.lblStatGear.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblStatGear.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(201)))), ((int)(((byte)(209)))), ((int)(((byte)(217)))));
            this.lblStatGear.Location = new System.Drawing.Point(260, 8);
            this.lblStatGear.Name = "lblStatGear";
            this.lblStatGear.Size = new System.Drawing.Size(46, 15);
            this.lblStatGear.TabIndex = 2;
            this.lblStatGear.Text = "기어: N";
            // 
            // lblStatFuel
            // 
            this.lblStatFuel.AutoSize = true;
            this.lblStatFuel.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblStatFuel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(201)))), ((int)(((byte)(209)))), ((int)(((byte)(217)))));
            this.lblStatFuel.Location = new System.Drawing.Point(360, 8);
            this.lblStatFuel.Name = "lblStatFuel";
            this.lblStatFuel.Size = new System.Drawing.Size(51, 15);
            this.lblStatFuel.TabIndex = 3;
            this.lblStatFuel.Text = "연료: - L";
            // 
            // lblStatClients
            // 
            this.lblStatClients.AutoSize = true;
            this.lblStatClients.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblStatClients.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(201)))), ((int)(((byte)(209)))), ((int)(((byte)(217)))));
            this.lblStatClients.Location = new System.Drawing.Point(490, 8);
            this.lblStatClients.Name = "lblStatClients";
            this.lblStatClients.Size = new System.Drawing.Size(129, 15);
            this.lblStatClients.TabIndex = 4;
            this.lblStatClients.Text = "HTML 클라이언트: 0 개";
            // 
            // lblStatUpdateRate
            // 
            this.lblStatUpdateRate.AutoSize = true;
            this.lblStatUpdateRate.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblStatUpdateRate.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(201)))), ((int)(((byte)(209)))), ((int)(((byte)(217)))));
            this.lblStatUpdateRate.Location = new System.Drawing.Point(10, 50);
            this.lblStatUpdateRate.Name = "lblStatUpdateRate";
            this.lblStatUpdateRate.Size = new System.Drawing.Size(69, 15);
            this.lblStatUpdateRate.TabIndex = 5;
            this.lblStatUpdateRate.Text = "갱신 주기: -";
            // 
            // panelLog
            // 
            this.panelLog.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(27)))), ((int)(((byte)(34)))));
            this.panelLog.Controls.Add(this.rtbLog);
            this.panelLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelLog.Location = new System.Drawing.Point(0, 253);
            this.panelLog.Name = "panelLog";
            this.panelLog.Padding = new System.Windows.Forms.Padding(16, 8, 16, 16);
            this.panelLog.Size = new System.Drawing.Size(680, 347);
            this.panelLog.TabIndex = 0;
            // 
            // rtbLog
            // 
            this.rtbLog.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(13)))), ((int)(((byte)(17)))), ((int)(((byte)(23)))));
            this.rtbLog.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.rtbLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtbLog.Font = new System.Drawing.Font("Consolas", 9F);
            this.rtbLog.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(201)))), ((int)(((byte)(209)))), ((int)(((byte)(217)))));
            this.rtbLog.Location = new System.Drawing.Point(16, 8);
            this.rtbLog.Name = "rtbLog";
            this.rtbLog.ReadOnly = true;
            this.rtbLog.Size = new System.Drawing.Size(648, 323);
            this.rtbLog.TabIndex = 0;
            this.rtbLog.Text = "";
            this.rtbLog.WordWrap = false;
            // 
            // btnClearLog
            // 
            this.btnClearLog.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClearLog.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(54)))), ((int)(((byte)(61)))));
            this.btnClearLog.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnClearLog.FlatAppearance.BorderSize = 0;
            this.btnClearLog.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClearLog.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.btnClearLog.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(139)))), ((int)(((byte)(148)))), ((int)(((byte)(158)))));
            this.btnClearLog.Location = new System.Drawing.Point(4, 73);
            this.btnClearLog.Name = "btnClearLog";
            this.btnClearLog.Size = new System.Drawing.Size(83, 24);
            this.btnClearLog.TabIndex = 1;
            this.btnClearLog.Text = "로그 지우기";
            this.btnClearLog.UseVisualStyleBackColor = false;
            this.btnClearLog.Click += new System.EventHandler(this.btnClearLog_Click);
            // 
            // SCSSdkClientDemo
            // 
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(27)))), ((int)(((byte)(34)))));
            this.ClientSize = new System.Drawing.Size(680, 600);
            this.Controls.Add(this.panelLog);
            this.Controls.Add(this.panelStats);
            this.Controls.Add(this.panelStatus);
            this.Controls.Add(this.panelTop);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(237)))), ((int)(((byte)(243)))));
            this.MinimumSize = new System.Drawing.Size(680, 600);
            this.Name = "SCSSdkClientDemo";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ETS2/ATS Telemetry Server";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SCSSdkClientDemo_FormClosing);
            this.panelTop.ResumeLayout(false);
            this.panelTop.PerformLayout();
            this.panelStatus.ResumeLayout(false);
            this.panelStatus.PerformLayout();
            this.panelStats.ResumeLayout(false);
            this.panelStats.PerformLayout();
            this.panelLog.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        // ── 컨트롤 선언 ──────────────────────────────────────────────────
        private System.Windows.Forms.Timer  timerUI;
        private System.Windows.Forms.Panel  panelTop;
        private System.Windows.Forms.Panel  panelStatus;
        private System.Windows.Forms.Panel  panelStats;
        private System.Windows.Forms.Panel  panelLog;

        private System.Windows.Forms.Label  lblTitle;
        private System.Windows.Forms.Button btnOpenDashboard;
        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.Button btnDisconnect;

        private System.Windows.Forms.Panel panelSdkLight;
        private System.Windows.Forms.Panel panelGameLight;
        private System.Windows.Forms.Panel panelWsLight;
        private System.Windows.Forms.Panel panelPausedLight;
        private System.Windows.Forms.Label lblSdkLight;
        private System.Windows.Forms.Label lblGameLight;
        private System.Windows.Forms.Label lblWsLight;
        private System.Windows.Forms.Label lblPausedLight;

        private System.Windows.Forms.Label lblStatSpeed;
        private System.Windows.Forms.Label lblStatRpm;
        private System.Windows.Forms.Label lblStatGear;
        private System.Windows.Forms.Label lblStatFuel;
        private System.Windows.Forms.Label lblStatClients;
        private System.Windows.Forms.Label lblStatUpdateRate;
        private System.Windows.Forms.RichTextBox rtbLog;
        private System.Windows.Forms.Button      btnClearLog;

        // 기존 코드 호환용 (삭제하면 Telemetry_Data 에서 에러)
        private System.Windows.Forms.RichTextBox lbGeneral    = new System.Windows.Forms.RichTextBox();
        private System.Windows.Forms.RichTextBox common       = new System.Windows.Forms.RichTextBox();
        private System.Windows.Forms.RichTextBox truck        = new System.Windows.Forms.RichTextBox();
        private System.Windows.Forms.RichTextBox trailer      = new System.Windows.Forms.RichTextBox();
        private System.Windows.Forms.RichTextBox job          = new System.Windows.Forms.RichTextBox();
        private System.Windows.Forms.RichTextBox control      = new System.Windows.Forms.RichTextBox();
        private System.Windows.Forms.RichTextBox navigation   = new System.Windows.Forms.RichTextBox();
        private System.Windows.Forms.RichTextBox substances   = new System.Windows.Forms.RichTextBox();
        private System.Windows.Forms.RichTextBox gameplayevent= new System.Windows.Forms.RichTextBox();
        private System.Windows.Forms.RichTextBox rtb_fuel     = new System.Windows.Forms.RichTextBox();
        private System.Windows.Forms.ToolStripStatusLabel l_updateRate = new System.Windows.Forms.ToolStripStatusLabel();
    }
}
