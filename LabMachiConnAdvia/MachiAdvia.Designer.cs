namespace LabMachiConnAdvia
{
    partial class MachiAdvia
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MachiAdvia));
            this.txtErrorMsg = new System.Windows.Forms.RichTextBox();
            this.txtMsg = new System.Windows.Forms.RichTextBox();
            this.label40 = new System.Windows.Forms.Label();
            this.label41 = new System.Windows.Forms.Label();
            this.label43 = new System.Windows.Forms.Label();
            this.label44 = new System.Windows.Forms.Label();
            this.label45 = new System.Windows.Forms.Label();
            this.label46 = new System.Windows.Forms.Label();
            this.label47 = new System.Windows.Forms.Label();
            this.lblAdviaStartTime = new System.Windows.Forms.Label();
            this.lblAdviaStatus = new System.Windows.Forms.Label();
            this.label50 = new System.Windows.Forms.Label();
            this.btnStart = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.btnTest = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.pbConnStatus = new System.Windows.Forms.PictureBox();
            this.NetworkDetector = new System.ComponentModel.BackgroundWorker();
            this.label2 = new System.Windows.Forms.Label();
            this.lblConnHost = new System.Windows.Forms.Label();
            this.lblConnPort = new System.Windows.Forms.Label();
            this.timerConnStatus = new System.Windows.Forms.Timer(this.components);
            this.timerAdvia = new System.Windows.Forms.Timer(this.components);
            this.bwAdvia = new System.ComponentModel.BackgroundWorker();
            this.bwSend = new System.ComponentModel.BackgroundWorker();
            this.timerSend = new System.Windows.Forms.Timer(this.components);
            this.btnStartNew = new System.Windows.Forms.Button();
            this.timerAdviaNew = new System.Windows.Forms.Timer(this.components);
            this.bwAdviaNew = new System.ComponentModel.BackgroundWorker();
            ((System.ComponentModel.ISupportInitialize)(this.pbConnStatus)).BeginInit();
            this.SuspendLayout();
            // 
            // txtErrorMsg
            // 
            this.txtErrorMsg.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.txtErrorMsg.Font = new System.Drawing.Font("新細明體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.txtErrorMsg.Location = new System.Drawing.Point(12, 429);
            this.txtErrorMsg.Name = "txtErrorMsg";
            this.txtErrorMsg.ReadOnly = true;
            this.txtErrorMsg.Size = new System.Drawing.Size(588, 89);
            this.txtErrorMsg.TabIndex = 82;
            this.txtErrorMsg.Text = "";
            // 
            // txtMsg
            // 
            this.txtMsg.Font = new System.Drawing.Font("新細明體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.txtMsg.Location = new System.Drawing.Point(12, 135);
            this.txtMsg.Name = "txtMsg";
            this.txtMsg.ReadOnly = true;
            this.txtMsg.Size = new System.Drawing.Size(588, 288);
            this.txtMsg.TabIndex = 81;
            this.txtMsg.Text = "";
            // 
            // label40
            // 
            this.label40.AutoSize = true;
            this.label40.Font = new System.Drawing.Font("新細明體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label40.Location = new System.Drawing.Point(91, 35);
            this.label40.Name = "label40";
            this.label40.Size = new System.Drawing.Size(140, 16);
            this.label40.TabIndex = 80;
            this.label40.Text = "陳琮勝 0920430047";
            // 
            // label41
            // 
            this.label41.AutoSize = true;
            this.label41.Font = new System.Drawing.Font("新細明體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label41.Location = new System.Drawing.Point(25, 35);
            this.label41.Name = "label41";
            this.label41.Size = new System.Drawing.Size(60, 16);
            this.label41.TabIndex = 79;
            this.label41.Text = "聯絡人:";
            // 
            // label43
            // 
            this.label43.AutoSize = true;
            this.label43.Font = new System.Drawing.Font("新細明體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label43.Location = new System.Drawing.Point(91, 9);
            this.label43.Name = "label43";
            this.label43.Size = new System.Drawing.Size(260, 16);
            this.label43.TabIndex = 78;
            this.label43.Text = "開榮股份有限公司  台中 0423848999";
            // 
            // label44
            // 
            this.label44.AutoSize = true;
            this.label44.Font = new System.Drawing.Font("新細明體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label44.Location = new System.Drawing.Point(41, 9);
            this.label44.Name = "label44";
            this.label44.Size = new System.Drawing.Size(44, 16);
            this.label44.TabIndex = 77;
            this.label44.Text = "公司:";
            // 
            // label45
            // 
            this.label45.AutoSize = true;
            this.label45.Font = new System.Drawing.Font("新細明體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label45.Location = new System.Drawing.Point(91, 61);
            this.label45.Name = "label45";
            this.label45.Size = new System.Drawing.Size(110, 16);
            this.label45.TabIndex = 76;
            this.label45.Text = "ADVIA Centaur";
            // 
            // label46
            // 
            this.label46.AutoSize = true;
            this.label46.Font = new System.Drawing.Font("新細明體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label46.Location = new System.Drawing.Point(41, 61);
            this.label46.Name = "label46";
            this.label46.Size = new System.Drawing.Size(44, 16);
            this.label46.TabIndex = 75;
            this.label46.Text = "儀器:";
            // 
            // label47
            // 
            this.label47.AutoSize = true;
            this.label47.Font = new System.Drawing.Font("新細明體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label47.Location = new System.Drawing.Point(9, 113);
            this.label47.Name = "label47";
            this.label47.Size = new System.Drawing.Size(76, 16);
            this.label47.TabIndex = 74;
            this.label47.Text = "開始時間:";
            // 
            // lblAdviaStartTime
            // 
            this.lblAdviaStartTime.AutoSize = true;
            this.lblAdviaStartTime.Font = new System.Drawing.Font("新細明體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lblAdviaStartTime.Location = new System.Drawing.Point(91, 113);
            this.lblAdviaStartTime.Name = "lblAdviaStartTime";
            this.lblAdviaStartTime.Size = new System.Drawing.Size(18, 16);
            this.lblAdviaStartTime.TabIndex = 73;
            this.lblAdviaStartTime.Text = "--";
            // 
            // lblAdviaStatus
            // 
            this.lblAdviaStatus.AutoSize = true;
            this.lblAdviaStatus.Font = new System.Drawing.Font("新細明體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lblAdviaStatus.Location = new System.Drawing.Point(91, 87);
            this.lblAdviaStatus.Name = "lblAdviaStatus";
            this.lblAdviaStatus.Size = new System.Drawing.Size(56, 16);
            this.lblAdviaStatus.TabIndex = 72;
            this.lblAdviaStatus.Text = "未轉檔";
            // 
            // label50
            // 
            this.label50.AutoSize = true;
            this.label50.Font = new System.Drawing.Font("新細明體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label50.Location = new System.Drawing.Point(41, 87);
            this.label50.Name = "label50";
            this.label50.Size = new System.Drawing.Size(44, 16);
            this.label50.TabIndex = 71;
            this.label50.Text = "狀態:";
            // 
            // btnStart
            // 
            this.btnStart.Font = new System.Drawing.Font("新細明體", 12F);
            this.btnStart.Location = new System.Drawing.Point(424, 94);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(87, 35);
            this.btnStart.TabIndex = 70;
            this.btnStart.Text = "開始轉檔";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // btnClose
            // 
            this.btnClose.Font = new System.Drawing.Font("新細明體", 12F);
            this.btnClose.Location = new System.Drawing.Point(517, 95);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(90, 34);
            this.btnClose.TabIndex = 69;
            this.btnClose.Text = "關閉程式";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btnTest
            // 
            this.btnTest.Font = new System.Drawing.Font("新細明體", 12F);
            this.btnTest.Location = new System.Drawing.Point(517, 54);
            this.btnTest.Name = "btnTest";
            this.btnTest.Size = new System.Drawing.Size(87, 35);
            this.btnTest.TabIndex = 83;
            this.btnTest.Text = "測試";
            this.btnTest.UseVisualStyleBackColor = true;
            this.btnTest.Click += new System.EventHandler(this.btnTest_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("新細明體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label1.Location = new System.Drawing.Point(358, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(108, 16);
            this.label1.TabIndex = 84;
            this.label1.Text = "儀器連線狀態:";
            // 
            // pbConnStatus
            // 
            this.pbConnStatus.Image = global::LabMachiConnAdvia.Properties.Resources.Ball_Green;
            this.pbConnStatus.Location = new System.Drawing.Point(472, 2);
            this.pbConnStatus.Name = "pbConnStatus";
            this.pbConnStatus.Size = new System.Drawing.Size(30, 30);
            this.pbConnStatus.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pbConnStatus.TabIndex = 85;
            this.pbConnStatus.TabStop = false;
            this.pbConnStatus.Visible = false;
            // 
            // NetworkDetector
            // 
            this.NetworkDetector.DoWork += new System.ComponentModel.DoWorkEventHandler(this.NetworkDetector_DoWork);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("新細明體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label2.Location = new System.Drawing.Point(358, 35);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(76, 16);
            this.label2.TabIndex = 87;
            this.label2.Text = "連線位址:";
            // 
            // lblConnHost
            // 
            this.lblConnHost.AutoSize = true;
            this.lblConnHost.Font = new System.Drawing.Font("新細明體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lblConnHost.Location = new System.Drawing.Point(440, 35);
            this.lblConnHost.Name = "lblConnHost";
            this.lblConnHost.Size = new System.Drawing.Size(116, 16);
            this.lblConnHost.TabIndex = 88;
            this.lblConnHost.Text = "127.127.127.127";
            // 
            // lblConnPort
            // 
            this.lblConnPort.AutoSize = true;
            this.lblConnPort.Font = new System.Drawing.Font("新細明體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lblConnPort.Location = new System.Drawing.Point(556, 35);
            this.lblConnPort.Name = "lblConnPort";
            this.lblConnPort.Size = new System.Drawing.Size(48, 16);
            this.lblConnPort.TabIndex = 89;
            this.lblConnPort.Text = "60000";
            // 
            // timerConnStatus
            // 
            this.timerConnStatus.Interval = 5000;
            this.timerConnStatus.Tick += new System.EventHandler(this.timerConnStatus_Tick);
            // 
            // timerAdvia
            // 
            this.timerAdvia.Interval = 5000;
            this.timerAdvia.Tick += new System.EventHandler(this.timerAdvia_Tick);
            // 
            // bwAdvia
            // 
            this.bwAdvia.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bwAdvia_DoWork);
            this.bwAdvia.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.bwAdvia_ProgressChanged);
            this.bwAdvia.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bwAdvia_RunWorkerCompleted);
            // 
            // bwSend
            // 
            this.bwSend.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bwSend_DoWork);
            this.bwSend.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.bwSend_ProgressChanged);
            this.bwSend.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bwSend_RunWorkerCompleted);
            // 
            // timerSend
            // 
            this.timerSend.Interval = 10000;
            this.timerSend.Tick += new System.EventHandler(this.timerSend_Tick);
            // 
            // btnStartNew
            // 
            this.btnStartNew.Font = new System.Drawing.Font("新細明體", 12F);
            this.btnStartNew.Location = new System.Drawing.Point(313, 95);
            this.btnStartNew.Name = "btnStartNew";
            this.btnStartNew.Size = new System.Drawing.Size(105, 35);
            this.btnStartNew.TabIndex = 90;
            this.btnStartNew.Text = "新系統轉檔";
            this.btnStartNew.UseVisualStyleBackColor = true;
            this.btnStartNew.Click += new System.EventHandler(this.btnStartNew_Click);
            // 
            // timerAdviaNew
            // 
            this.timerAdviaNew.Interval = 5000;
            this.timerAdviaNew.Tick += new System.EventHandler(this.timerAdviaNew_Tick);
            // 
            // bwAdviaNew
            // 
            this.bwAdviaNew.WorkerReportsProgress = true;
            this.bwAdviaNew.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bwAdviaNew_DoWork);
            this.bwAdviaNew.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.bwAdviaNew_ProgressChanged);
            this.bwAdviaNew.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bwAdviaNew_RunWorkerCompleted);
            // 
            // MachiAdvia
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(611, 530);
            this.Controls.Add(this.btnStartNew);
            this.Controls.Add(this.lblConnPort);
            this.Controls.Add(this.lblConnHost);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.pbConnStatus);
            this.Controls.Add(this.label1);
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
            this.Controls.Add(this.lblAdviaStartTime);
            this.Controls.Add(this.lblAdviaStatus);
            this.Controls.Add(this.label50);
            this.Controls.Add(this.btnStart);
            this.Controls.Add(this.btnClose);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MachiAdvia";
            this.Text = "Lab-ADVIA免疫機";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MachiAdvia_FormClosed);
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pbConnStatus)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RichTextBox txtErrorMsg;
        private System.Windows.Forms.RichTextBox txtMsg;
        private System.Windows.Forms.Label label40;
        private System.Windows.Forms.Label label41;
        private System.Windows.Forms.Label label43;
        private System.Windows.Forms.Label label44;
        private System.Windows.Forms.Label label45;
        private System.Windows.Forms.Label label46;
        private System.Windows.Forms.Label label47;
        private System.Windows.Forms.Label lblAdviaStartTime;
        private System.Windows.Forms.Label lblAdviaStatus;
        private System.Windows.Forms.Label label50;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Button btnTest;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.PictureBox pbConnStatus;
        private System.ComponentModel.BackgroundWorker NetworkDetector;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lblConnHost;
        private System.Windows.Forms.Label lblConnPort;
        private System.Windows.Forms.Timer timerConnStatus;
        private System.Windows.Forms.Timer timerAdvia;
        private System.ComponentModel.BackgroundWorker bwAdvia;
        private System.ComponentModel.BackgroundWorker bwSend;
        private System.Windows.Forms.Timer timerSend;
        private System.Windows.Forms.Button btnStartNew;
        private System.Windows.Forms.Timer timerAdviaNew;
        private System.ComponentModel.BackgroundWorker bwAdviaNew;
    }
}

