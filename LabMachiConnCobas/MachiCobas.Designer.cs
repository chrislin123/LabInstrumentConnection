namespace LabMachiConnCobas
{
    partial class MachiCobas
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
            this.lblConnPort = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.bwCobas = new System.ComponentModel.BackgroundWorker();
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
            this.btnStartNew = new System.Windows.Forms.Button();
            this.bwCobasNew = new System.ComponentModel.BackgroundWorker();
            this.lblMode = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lblConnPort
            // 
            this.lblConnPort.AutoSize = true;
            this.lblConnPort.Font = new System.Drawing.Font("新細明體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lblConnPort.Location = new System.Drawing.Point(549, 34);
            this.lblConnPort.Name = "lblConnPort";
            this.lblConnPort.Size = new System.Drawing.Size(48, 16);
            this.lblConnPort.TabIndex = 119;
            this.lblConnPort.Text = "00000";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("新細明體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label2.Location = new System.Drawing.Point(351, 34);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(76, 16);
            this.label2.TabIndex = 117;
            this.label2.Text = "連線位址:";
            // 
            // bwCobas
            // 
            this.bwCobas.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bwCobas_DoWork);
            // 
            // lblConnHost
            // 
            this.lblConnHost.AutoSize = true;
            this.lblConnHost.Font = new System.Drawing.Font("新細明體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lblConnHost.Location = new System.Drawing.Point(433, 34);
            this.lblConnHost.Name = "lblConnHost";
            this.lblConnHost.Size = new System.Drawing.Size(116, 16);
            this.lblConnHost.TabIndex = 118;
            this.lblConnHost.Text = "000.000.000.000";
            // 
            // btnTest
            // 
            this.btnTest.Font = new System.Drawing.Font("新細明體", 12F);
            this.btnTest.Location = new System.Drawing.Point(421, 101);
            this.btnTest.Name = "btnTest";
            this.btnTest.Size = new System.Drawing.Size(87, 35);
            this.btnTest.TabIndex = 116;
            this.btnTest.Text = "測試";
            this.btnTest.UseVisualStyleBackColor = true;
            this.btnTest.Visible = false;
            // 
            // txtErrorMsg
            // 
            this.txtErrorMsg.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.txtErrorMsg.Font = new System.Drawing.Font("新細明體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.txtErrorMsg.Location = new System.Drawing.Point(13, 341);
            this.txtErrorMsg.Name = "txtErrorMsg";
            this.txtErrorMsg.ReadOnly = true;
            this.txtErrorMsg.Size = new System.Drawing.Size(588, 124);
            this.txtErrorMsg.TabIndex = 115;
            this.txtErrorMsg.Text = "";
            // 
            // txtMsg
            // 
            this.txtMsg.Font = new System.Drawing.Font("新細明體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.txtMsg.Location = new System.Drawing.Point(13, 142);
            this.txtMsg.Name = "txtMsg";
            this.txtMsg.ReadOnly = true;
            this.txtMsg.Size = new System.Drawing.Size(588, 193);
            this.txtMsg.TabIndex = 114;
            this.txtMsg.Text = "";
            // 
            // label40
            // 
            this.label40.AutoSize = true;
            this.label40.Font = new System.Drawing.Font("新細明體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label40.Location = new System.Drawing.Point(92, 34);
            this.label40.Name = "label40";
            this.label40.Size = new System.Drawing.Size(145, 16);
            this.label40.TabIndex = 113;
            this.label40.Text = "張翔峻 0960-554496";
            // 
            // label41
            // 
            this.label41.AutoSize = true;
            this.label41.Font = new System.Drawing.Font("新細明體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label41.Location = new System.Drawing.Point(26, 34);
            this.label41.Name = "label41";
            this.label41.Size = new System.Drawing.Size(60, 16);
            this.label41.TabIndex = 112;
            this.label41.Text = "聯絡人:";
            // 
            // label43
            // 
            this.label43.AutoSize = true;
            this.label43.Font = new System.Drawing.Font("新細明體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label43.Location = new System.Drawing.Point(92, 8);
            this.label43.Name = "label43";
            this.label43.Size = new System.Drawing.Size(271, 16);
            this.label43.TabIndex = 111;
            this.label43.Text = "威力德生醫股份有限公司-(07)3800773";
            // 
            // label44
            // 
            this.label44.AutoSize = true;
            this.label44.Font = new System.Drawing.Font("新細明體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label44.Location = new System.Drawing.Point(42, 8);
            this.label44.Name = "label44";
            this.label44.Size = new System.Drawing.Size(44, 16);
            this.label44.TabIndex = 110;
            this.label44.Text = "公司:";
            // 
            // label45
            // 
            this.label45.AutoSize = true;
            this.label45.Font = new System.Drawing.Font("新細明體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label45.Location = new System.Drawing.Point(92, 60);
            this.label45.Name = "label45";
            this.label45.Size = new System.Drawing.Size(84, 16);
            this.label45.TabIndex = 109;
            this.label45.Text = "Cobas-b221";
            // 
            // label46
            // 
            this.label46.AutoSize = true;
            this.label46.Font = new System.Drawing.Font("新細明體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label46.Location = new System.Drawing.Point(42, 60);
            this.label46.Name = "label46";
            this.label46.Size = new System.Drawing.Size(44, 16);
            this.label46.TabIndex = 108;
            this.label46.Text = "儀器:";
            // 
            // label47
            // 
            this.label47.AutoSize = true;
            this.label47.Font = new System.Drawing.Font("新細明體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label47.Location = new System.Drawing.Point(10, 112);
            this.label47.Name = "label47";
            this.label47.Size = new System.Drawing.Size(76, 16);
            this.label47.TabIndex = 107;
            this.label47.Text = "開始時間:";
            // 
            // lblStartTime
            // 
            this.lblStartTime.AutoSize = true;
            this.lblStartTime.Font = new System.Drawing.Font("新細明體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lblStartTime.Location = new System.Drawing.Point(92, 112);
            this.lblStartTime.Name = "lblStartTime";
            this.lblStartTime.Size = new System.Drawing.Size(18, 16);
            this.lblStartTime.TabIndex = 106;
            this.lblStartTime.Text = "--";
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.Font = new System.Drawing.Font("新細明體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lblStatus.Location = new System.Drawing.Point(92, 86);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(56, 16);
            this.lblStatus.TabIndex = 105;
            this.lblStatus.Text = "未轉檔";
            // 
            // label50
            // 
            this.label50.AutoSize = true;
            this.label50.Font = new System.Drawing.Font("新細明體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label50.Location = new System.Drawing.Point(42, 86);
            this.label50.Name = "label50";
            this.label50.Size = new System.Drawing.Size(44, 16);
            this.label50.TabIndex = 104;
            this.label50.Text = "狀態:";
            // 
            // btnStart
            // 
            this.btnStart.Font = new System.Drawing.Font("新細明體", 12F);
            this.btnStart.Location = new System.Drawing.Point(514, 101);
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
            this.btnClose.Location = new System.Drawing.Point(511, 471);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(90, 34);
            this.btnClose.TabIndex = 102;
            this.btnClose.Text = "關閉程式";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btnStartNew
            // 
            this.btnStartNew.Font = new System.Drawing.Font("新細明體", 12F);
            this.btnStartNew.Location = new System.Drawing.Point(293, 101);
            this.btnStartNew.Name = "btnStartNew";
            this.btnStartNew.Size = new System.Drawing.Size(122, 35);
            this.btnStartNew.TabIndex = 120;
            this.btnStartNew.Text = "新系統轉檔";
            this.btnStartNew.UseVisualStyleBackColor = true;
            this.btnStartNew.Click += new System.EventHandler(this.btnStartNew_Click);
            // 
            // bwCobasNew
            // 
            this.bwCobasNew.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bwCobasNew_DoWork);
            // 
            // lblMode
            // 
            this.lblMode.AutoSize = true;
            this.lblMode.Font = new System.Drawing.Font("新細明體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lblMode.Location = new System.Drawing.Point(433, 60);
            this.lblMode.Name = "lblMode";
            this.lblMode.Size = new System.Drawing.Size(23, 16);
            this.lblMode.TabIndex = 122;
            this.lblMode.Text = "---";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("新細明體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label3.Location = new System.Drawing.Point(383, 60);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(44, 16);
            this.label3.TabIndex = 121;
            this.label3.Text = "模式:";
            // 
            // MachiCobas
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(611, 513);
            this.Controls.Add(this.lblMode);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.btnStartNew);
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
            this.Name = "MachiCobas";
            this.Text = "Lab-Cobas";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MachiCobas_FormClosed);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblConnPort;
        private System.Windows.Forms.Label label2;
        private System.ComponentModel.BackgroundWorker bwCobas;
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
        private System.Windows.Forms.Button btnStartNew;
        private System.ComponentModel.BackgroundWorker bwCobasNew;
        private System.Windows.Forms.Label lblMode;
        private System.Windows.Forms.Label label3;
    }
}

