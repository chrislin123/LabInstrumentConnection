namespace DialysisMachiConn
{
    partial class DialysysConn
    {
        /// <summary>
        /// 設計工具所需的變數。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清除任何使用中的資源。
        /// </summary>
        /// <param name="disposing">如果應該處置 Managed 資源則為 true，否則為 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 設計工具產生的程式碼

        /// <summary>
        /// 此為設計工具支援所需的方法 - 請勿使用程式碼編輯器
        /// 修改這個方法的內容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.lblConnPort = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.bwDialysis = new System.ComponentModel.BackgroundWorker();
            this.lblConnHost = new System.Windows.Forms.Label();
            this.btnTest = new System.Windows.Forms.Button();
            this.txtErrorMsg = new System.Windows.Forms.RichTextBox();
            this.txtMsg = new System.Windows.Forms.RichTextBox();
            this.label40 = new System.Windows.Forms.Label();
            this.label41 = new System.Windows.Forms.Label();
            this.label43 = new System.Windows.Forms.Label();
            this.label44 = new System.Windows.Forms.Label();
            this.label45 = new System.Windows.Forms.Label();
            this.label46 = new System.Windows.Forms.Label();
            this.label47 = new System.Windows.Forms.Label();
            this.lblStartTime = new System.Windows.Forms.Label();
            this.lblStatus = new System.Windows.Forms.Label();
            this.label50 = new System.Windows.Forms.Label();
            this.btnStart = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.chk1 = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // lblConnPort
            // 
            this.lblConnPort.AutoSize = true;
            this.lblConnPort.Font = new System.Drawing.Font("新細明體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lblConnPort.Location = new System.Drawing.Point(550, 35);
            this.lblConnPort.Name = "lblConnPort";
            this.lblConnPort.Size = new System.Drawing.Size(48, 16);
            this.lblConnPort.TabIndex = 119;
            this.lblConnPort.Text = "60000";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("新細明體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label2.Location = new System.Drawing.Point(352, 35);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(76, 16);
            this.label2.TabIndex = 117;
            this.label2.Text = "連線位址:";
            // 
            // bwDialysis
            // 
            this.bwDialysis.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bwDialysis_DoWork);
            this.bwDialysis.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.bwDialysis_ProgressChanged);
            this.bwDialysis.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bwDialysis_RunWorkerCompleted);
            // 
            // lblConnHost
            // 
            this.lblConnHost.AutoSize = true;
            this.lblConnHost.Font = new System.Drawing.Font("新細明體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lblConnHost.Location = new System.Drawing.Point(434, 35);
            this.lblConnHost.Name = "lblConnHost";
            this.lblConnHost.Size = new System.Drawing.Size(116, 16);
            this.lblConnHost.TabIndex = 118;
            this.lblConnHost.Text = "127.127.127.127";
            // 
            // btnTest
            // 
            this.btnTest.Font = new System.Drawing.Font("新細明體", 12F);
            this.btnTest.Location = new System.Drawing.Point(422, 102);
            this.btnTest.Name = "btnTest";
            this.btnTest.Size = new System.Drawing.Size(87, 35);
            this.btnTest.TabIndex = 116;
            this.btnTest.Text = "測試";
            this.btnTest.UseVisualStyleBackColor = true;
            this.btnTest.Visible = false;
            this.btnTest.Click += new System.EventHandler(this.btnTest_Click);
            // 
            // txtErrorMsg
            // 
            this.txtErrorMsg.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.txtErrorMsg.Font = new System.Drawing.Font("新細明體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.txtErrorMsg.Location = new System.Drawing.Point(14, 440);
            this.txtErrorMsg.Name = "txtErrorMsg";
            this.txtErrorMsg.ReadOnly = true;
            this.txtErrorMsg.Size = new System.Drawing.Size(588, 67);
            this.txtErrorMsg.TabIndex = 115;
            this.txtErrorMsg.Text = "";
            // 
            // txtMsg
            // 
            this.txtMsg.Font = new System.Drawing.Font("新細明體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.txtMsg.Location = new System.Drawing.Point(14, 143);
            this.txtMsg.Name = "txtMsg";
            this.txtMsg.ReadOnly = true;
            this.txtMsg.Size = new System.Drawing.Size(588, 291);
            this.txtMsg.TabIndex = 114;
            this.txtMsg.Text = "";
            // 
            // label40
            // 
            this.label40.AutoSize = true;
            this.label40.Font = new System.Drawing.Font("新細明體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label40.Location = new System.Drawing.Point(93, 35);
            this.label40.Name = "label40";
            this.label40.Size = new System.Drawing.Size(40, 16);
            this.label40.TabIndex = 113;
            this.label40.Text = "未知";
            // 
            // label41
            // 
            this.label41.AutoSize = true;
            this.label41.Font = new System.Drawing.Font("新細明體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label41.Location = new System.Drawing.Point(27, 35);
            this.label41.Name = "label41";
            this.label41.Size = new System.Drawing.Size(60, 16);
            this.label41.TabIndex = 112;
            this.label41.Text = "聯絡人:";
            // 
            // label43
            // 
            this.label43.AutoSize = true;
            this.label43.Font = new System.Drawing.Font("新細明體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label43.Location = new System.Drawing.Point(93, 9);
            this.label43.Name = "label43";
            this.label43.Size = new System.Drawing.Size(292, 16);
            this.label43.TabIndex = 111;
            this.label43.Text = "股份有限公司    台北總公司: 台中營業處:";
            // 
            // label44
            // 
            this.label44.AutoSize = true;
            this.label44.Font = new System.Drawing.Font("新細明體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label44.Location = new System.Drawing.Point(43, 9);
            this.label44.Name = "label44";
            this.label44.Size = new System.Drawing.Size(44, 16);
            this.label44.TabIndex = 110;
            this.label44.Text = "公司:";
            // 
            // label45
            // 
            this.label45.AutoSize = true;
            this.label45.Font = new System.Drawing.Font("新細明體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label45.Location = new System.Drawing.Point(93, 61);
            this.label45.Name = "label45";
            this.label45.Size = new System.Drawing.Size(90, 16);
            this.label45.TabIndex = 109;
            this.label45.Text = "Dialysis儀器";
            // 
            // label46
            // 
            this.label46.AutoSize = true;
            this.label46.Font = new System.Drawing.Font("新細明體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label46.Location = new System.Drawing.Point(43, 61);
            this.label46.Name = "label46";
            this.label46.Size = new System.Drawing.Size(44, 16);
            this.label46.TabIndex = 108;
            this.label46.Text = "儀器:";
            // 
            // label47
            // 
            this.label47.AutoSize = true;
            this.label47.Font = new System.Drawing.Font("新細明體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label47.Location = new System.Drawing.Point(11, 113);
            this.label47.Name = "label47";
            this.label47.Size = new System.Drawing.Size(76, 16);
            this.label47.TabIndex = 107;
            this.label47.Text = "開始時間:";
            // 
            // lblStartTime
            // 
            this.lblStartTime.AutoSize = true;
            this.lblStartTime.Font = new System.Drawing.Font("新細明體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lblStartTime.Location = new System.Drawing.Point(93, 113);
            this.lblStartTime.Name = "lblStartTime";
            this.lblStartTime.Size = new System.Drawing.Size(18, 16);
            this.lblStartTime.TabIndex = 106;
            this.lblStartTime.Text = "--";
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.Font = new System.Drawing.Font("新細明體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lblStatus.Location = new System.Drawing.Point(93, 87);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(56, 16);
            this.lblStatus.TabIndex = 105;
            this.lblStatus.Text = "未轉檔";
            // 
            // label50
            // 
            this.label50.AutoSize = true;
            this.label50.Font = new System.Drawing.Font("新細明體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label50.Location = new System.Drawing.Point(43, 87);
            this.label50.Name = "label50";
            this.label50.Size = new System.Drawing.Size(44, 16);
            this.label50.TabIndex = 104;
            this.label50.Text = "狀態:";
            // 
            // btnStart
            // 
            this.btnStart.Font = new System.Drawing.Font("新細明體", 12F);
            this.btnStart.Location = new System.Drawing.Point(515, 102);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(87, 35);
            this.btnStart.TabIndex = 103;
            this.btnStart.Text = "開始轉檔";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // btnClose
            // 
            this.btnClose.Font = new System.Drawing.Font("新細明體", 12F);
            this.btnClose.Location = new System.Drawing.Point(512, 513);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(90, 34);
            this.btnClose.TabIndex = 102;
            this.btnClose.Text = "關閉程式";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // timer1
            // 
            this.timer1.Interval = 10000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // chk1
            // 
            this.chk1.AutoSize = true;
            this.chk1.Font = new System.Drawing.Font("新細明體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.chk1.Location = new System.Drawing.Point(355, 60);
            this.chk1.Name = "chk1";
            this.chk1.Size = new System.Drawing.Size(191, 20);
            this.chk1.TabIndex = 120;
            this.chk1.Text = "寫入儀器原始格式進log";
            this.chk1.UseVisualStyleBackColor = true;
            this.chk1.CheckedChanged += new System.EventHandler(this.chk1_CheckedChanged);
            // 
            // DialysysConn
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(616, 559);
            this.Controls.Add(this.chk1);
            this.Controls.Add(this.lblConnPort);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.lblConnHost);
            this.Controls.Add(this.btnTest);
            this.Controls.Add(this.txtErrorMsg);
            this.Controls.Add(this.txtMsg);
            this.Controls.Add(this.label40);
            this.Controls.Add(this.label41);
            this.Controls.Add(this.label43);
            this.Controls.Add(this.label44);
            this.Controls.Add(this.label45);
            this.Controls.Add(this.label46);
            this.Controls.Add(this.label47);
            this.Controls.Add(this.lblStartTime);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.label50);
            this.Controls.Add(this.btnStart);
            this.Controls.Add(this.btnClose);
            this.Name = "DialysysConn";
            this.Text = "透析儀器連線";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.DialysysConn_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.DialysysConn_FormClosed);
            this.Load += new System.EventHandler(this.DialysysConn_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblConnPort;
        private System.Windows.Forms.Label label2;
        private System.ComponentModel.BackgroundWorker bwDialysis;
        private System.Windows.Forms.Label lblConnHost;
        private System.Windows.Forms.Button btnTest;
        private System.Windows.Forms.RichTextBox txtErrorMsg;
        private System.Windows.Forms.RichTextBox txtMsg;
        private System.Windows.Forms.Label label40;
        private System.Windows.Forms.Label label41;
        private System.Windows.Forms.Label label43;
        private System.Windows.Forms.Label label44;
        private System.Windows.Forms.Label label45;
        private System.Windows.Forms.Label label46;
        private System.Windows.Forms.Label label47;
        private System.Windows.Forms.Label lblStartTime;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.Label label50;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.CheckBox chk1;
    }
}

