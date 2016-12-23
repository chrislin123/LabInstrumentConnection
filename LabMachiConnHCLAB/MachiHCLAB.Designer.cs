namespace LabMachiConnHCLAB
{
    partial class MachiHCLAB
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
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.label23 = new System.Windows.Forms.Label();
            this.HCLABSendErrorMsg = new System.Windows.Forms.RichTextBox();
            this.HCLABSendMsg = new System.Windows.Forms.RichTextBox();
            this.label24 = new System.Windows.Forms.Label();
            this.label25 = new System.Windows.Forms.Label();
            this.label26 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.hclabSendLastStartTime = new System.Windows.Forms.Label();
            this.hclabSendStatus = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.button10 = new System.Windows.Forms.Button();
            this.button11 = new System.Windows.Forms.Button();
            this.HCLABReceiveErrorMsg = new System.Windows.Forms.RichTextBox();
            this.HCLABReceiveMsg = new System.Windows.Forms.RichTextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.label27 = new System.Windows.Forms.Label();
            this.label28 = new System.Windows.Forms.Label();
            this.label29 = new System.Windows.Forms.Label();
            this.label30 = new System.Windows.Forms.Label();
            this.label31 = new System.Windows.Forms.Label();
            this.hclabReceiveLastStartTime = new System.Windows.Forms.Label();
            this.hclabReceiveStatus = new System.Windows.Forms.Label();
            this.label34 = new System.Windows.Forms.Label();
            this.button12 = new System.Windows.Forms.Button();
            this.button13 = new System.Windows.Forms.Button();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Multiline = true;
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(706, 520);
            this.tabControl1.TabIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.label23);
            this.tabPage1.Controls.Add(this.HCLABSendErrorMsg);
            this.tabPage1.Controls.Add(this.HCLABSendMsg);
            this.tabPage1.Controls.Add(this.label24);
            this.tabPage1.Controls.Add(this.label25);
            this.tabPage1.Controls.Add(this.label26);
            this.tabPage1.Controls.Add(this.label15);
            this.tabPage1.Controls.Add(this.label10);
            this.tabPage1.Controls.Add(this.label11);
            this.tabPage1.Controls.Add(this.hclabSendLastStartTime);
            this.tabPage1.Controls.Add(this.hclabSendStatus);
            this.tabPage1.Controls.Add(this.label14);
            this.tabPage1.Controls.Add(this.button10);
            this.tabPage1.Controls.Add(this.button11);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(698, 494);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "HCLAB(傳送)";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.HCLABReceiveErrorMsg);
            this.tabPage2.Controls.Add(this.HCLABReceiveMsg);
            this.tabPage2.Controls.Add(this.label12);
            this.tabPage2.Controls.Add(this.label13);
            this.tabPage2.Controls.Add(this.label27);
            this.tabPage2.Controls.Add(this.label28);
            this.tabPage2.Controls.Add(this.label29);
            this.tabPage2.Controls.Add(this.label30);
            this.tabPage2.Controls.Add(this.label31);
            this.tabPage2.Controls.Add(this.hclabReceiveLastStartTime);
            this.tabPage2.Controls.Add(this.hclabReceiveStatus);
            this.tabPage2.Controls.Add(this.label34);
            this.tabPage2.Controls.Add(this.button12);
            this.tabPage2.Controls.Add(this.button13);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(698, 494);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "HCLAB(接收)";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // label23
            // 
            this.label23.AutoSize = true;
            this.label23.Font = new System.Drawing.Font("新細明體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label23.Location = new System.Drawing.Point(136, 45);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(281, 16);
            this.label23.TabIndex = 57;
            this.label23.Text = "鄭弘彬 0988-139820 王先生 0963501856";
            // 
            // HCLABSendErrorMsg
            // 
            this.HCLABSendErrorMsg.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.HCLABSendErrorMsg.Font = new System.Drawing.Font("新細明體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.HCLABSendErrorMsg.Location = new System.Drawing.Point(57, 352);
            this.HCLABSendErrorMsg.Name = "HCLABSendErrorMsg";
            this.HCLABSendErrorMsg.ReadOnly = true;
            this.HCLABSendErrorMsg.Size = new System.Drawing.Size(588, 124);
            this.HCLABSendErrorMsg.TabIndex = 56;
            this.HCLABSendErrorMsg.Text = "";
            // 
            // HCLABSendMsg
            // 
            this.HCLABSendMsg.Font = new System.Drawing.Font("新細明體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.HCLABSendMsg.Location = new System.Drawing.Point(57, 153);
            this.HCLABSendMsg.Name = "HCLABSendMsg";
            this.HCLABSendMsg.ReadOnly = true;
            this.HCLABSendMsg.Size = new System.Drawing.Size(588, 193);
            this.HCLABSendMsg.TabIndex = 55;
            this.HCLABSendMsg.Text = "";
            // 
            // label24
            // 
            this.label24.AutoSize = true;
            this.label24.Font = new System.Drawing.Font("新細明體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label24.Location = new System.Drawing.Point(70, 45);
            this.label24.Name = "label24";
            this.label24.Size = new System.Drawing.Size(60, 16);
            this.label24.TabIndex = 54;
            this.label24.Text = "聯絡人:";
            // 
            // label25
            // 
            this.label25.AutoSize = true;
            this.label25.Font = new System.Drawing.Font("新細明體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label25.Location = new System.Drawing.Point(136, 19);
            this.label25.Name = "label25";
            this.label25.Size = new System.Drawing.Size(250, 16);
            this.label25.TabIndex = 53;
            this.label25.Text = "三東儀器股份有限公司(07)3421801";
            // 
            // label26
            // 
            this.label26.AutoSize = true;
            this.label26.Font = new System.Drawing.Font("新細明體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label26.Location = new System.Drawing.Point(86, 19);
            this.label26.Name = "label26";
            this.label26.Size = new System.Drawing.Size(44, 16);
            this.label26.TabIndex = 52;
            this.label26.Text = "公司:";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Font = new System.Drawing.Font("新細明體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label15.Location = new System.Drawing.Point(136, 71);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(242, 16);
            this.label15.TabIndex = 51;
            this.label15.Text = "CA-560,XN-2000,HA8180,AX-4030";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("新細明體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label10.Location = new System.Drawing.Point(86, 71);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(44, 16);
            this.label10.TabIndex = 50;
            this.label10.Text = "儀器:";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("新細明體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label11.Location = new System.Drawing.Point(54, 123);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(76, 16);
            this.label11.TabIndex = 49;
            this.label11.Text = "開始時間:";
            // 
            // hclabSendLastStartTime
            // 
            this.hclabSendLastStartTime.AutoSize = true;
            this.hclabSendLastStartTime.Font = new System.Drawing.Font("新細明體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.hclabSendLastStartTime.Location = new System.Drawing.Point(136, 123);
            this.hclabSendLastStartTime.Name = "hclabSendLastStartTime";
            this.hclabSendLastStartTime.Size = new System.Drawing.Size(18, 16);
            this.hclabSendLastStartTime.TabIndex = 48;
            this.hclabSendLastStartTime.Text = "--";
            // 
            // hclabSendStatus
            // 
            this.hclabSendStatus.AutoSize = true;
            this.hclabSendStatus.Font = new System.Drawing.Font("新細明體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.hclabSendStatus.Location = new System.Drawing.Point(136, 97);
            this.hclabSendStatus.Name = "hclabSendStatus";
            this.hclabSendStatus.Size = new System.Drawing.Size(56, 16);
            this.hclabSendStatus.TabIndex = 47;
            this.hclabSendStatus.Text = "未轉檔";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Font = new System.Drawing.Font("新細明體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label14.Location = new System.Drawing.Point(86, 97);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(44, 16);
            this.label14.TabIndex = 46;
            this.label14.Text = "狀態:";
            // 
            // button10
            // 
            this.button10.Font = new System.Drawing.Font("新細明體", 12F);
            this.button10.Location = new System.Drawing.Point(469, 112);
            this.button10.Name = "button10";
            this.button10.Size = new System.Drawing.Size(87, 35);
            this.button10.TabIndex = 45;
            this.button10.Text = "停止轉檔";
            this.button10.UseVisualStyleBackColor = true;
            this.button10.Visible = false;
            // 
            // button11
            // 
            this.button11.Font = new System.Drawing.Font("新細明體", 12F);
            this.button11.Location = new System.Drawing.Point(558, 112);
            this.button11.Name = "button11";
            this.button11.Size = new System.Drawing.Size(87, 35);
            this.button11.TabIndex = 44;
            this.button11.Text = "開始轉檔";
            this.button11.UseVisualStyleBackColor = true;
            // 
            // HCLABReceiveErrorMsg
            // 
            this.HCLABReceiveErrorMsg.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.HCLABReceiveErrorMsg.Font = new System.Drawing.Font("新細明體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.HCLABReceiveErrorMsg.Location = new System.Drawing.Point(57, 352);
            this.HCLABReceiveErrorMsg.Name = "HCLABReceiveErrorMsg";
            this.HCLABReceiveErrorMsg.ReadOnly = true;
            this.HCLABReceiveErrorMsg.Size = new System.Drawing.Size(588, 124);
            this.HCLABReceiveErrorMsg.TabIndex = 55;
            this.HCLABReceiveErrorMsg.Text = "";
            // 
            // HCLABReceiveMsg
            // 
            this.HCLABReceiveMsg.Font = new System.Drawing.Font("新細明體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.HCLABReceiveMsg.Location = new System.Drawing.Point(57, 153);
            this.HCLABReceiveMsg.Name = "HCLABReceiveMsg";
            this.HCLABReceiveMsg.ReadOnly = true;
            this.HCLABReceiveMsg.Size = new System.Drawing.Size(588, 193);
            this.HCLABReceiveMsg.TabIndex = 54;
            this.HCLABReceiveMsg.Text = "";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Font = new System.Drawing.Font("新細明體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label12.Location = new System.Drawing.Point(136, 45);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(281, 16);
            this.label12.TabIndex = 53;
            this.label12.Text = "鄭弘彬 0988-139820 王先生 0963501856";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Font = new System.Drawing.Font("新細明體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label13.Location = new System.Drawing.Point(70, 45);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(60, 16);
            this.label13.TabIndex = 52;
            this.label13.Text = "聯絡人:";
            // 
            // label27
            // 
            this.label27.AutoSize = true;
            this.label27.Font = new System.Drawing.Font("新細明體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label27.Location = new System.Drawing.Point(136, 19);
            this.label27.Name = "label27";
            this.label27.Size = new System.Drawing.Size(250, 16);
            this.label27.TabIndex = 51;
            this.label27.Text = "三東儀器股份有限公司(07)3421801";
            // 
            // label28
            // 
            this.label28.AutoSize = true;
            this.label28.Font = new System.Drawing.Font("新細明體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label28.Location = new System.Drawing.Point(86, 19);
            this.label28.Name = "label28";
            this.label28.Size = new System.Drawing.Size(44, 16);
            this.label28.TabIndex = 50;
            this.label28.Text = "公司:";
            // 
            // label29
            // 
            this.label29.AutoSize = true;
            this.label29.Font = new System.Drawing.Font("新細明體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label29.Location = new System.Drawing.Point(136, 71);
            this.label29.Name = "label29";
            this.label29.Size = new System.Drawing.Size(242, 16);
            this.label29.TabIndex = 49;
            this.label29.Text = "CA-560,XN-2000,HA8180,AX-4030";
            // 
            // label30
            // 
            this.label30.AutoSize = true;
            this.label30.Font = new System.Drawing.Font("新細明體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label30.Location = new System.Drawing.Point(86, 71);
            this.label30.Name = "label30";
            this.label30.Size = new System.Drawing.Size(44, 16);
            this.label30.TabIndex = 48;
            this.label30.Text = "儀器:";
            // 
            // label31
            // 
            this.label31.AutoSize = true;
            this.label31.Font = new System.Drawing.Font("新細明體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label31.Location = new System.Drawing.Point(54, 123);
            this.label31.Name = "label31";
            this.label31.Size = new System.Drawing.Size(76, 16);
            this.label31.TabIndex = 47;
            this.label31.Text = "開始時間:";
            // 
            // hclabReceiveLastStartTime
            // 
            this.hclabReceiveLastStartTime.AutoSize = true;
            this.hclabReceiveLastStartTime.Font = new System.Drawing.Font("新細明體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.hclabReceiveLastStartTime.Location = new System.Drawing.Point(136, 123);
            this.hclabReceiveLastStartTime.Name = "hclabReceiveLastStartTime";
            this.hclabReceiveLastStartTime.Size = new System.Drawing.Size(18, 16);
            this.hclabReceiveLastStartTime.TabIndex = 46;
            this.hclabReceiveLastStartTime.Text = "--";
            // 
            // hclabReceiveStatus
            // 
            this.hclabReceiveStatus.AutoSize = true;
            this.hclabReceiveStatus.Font = new System.Drawing.Font("新細明體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.hclabReceiveStatus.Location = new System.Drawing.Point(136, 97);
            this.hclabReceiveStatus.Name = "hclabReceiveStatus";
            this.hclabReceiveStatus.Size = new System.Drawing.Size(56, 16);
            this.hclabReceiveStatus.TabIndex = 45;
            this.hclabReceiveStatus.Text = "未轉檔";
            // 
            // label34
            // 
            this.label34.AutoSize = true;
            this.label34.Font = new System.Drawing.Font("新細明體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label34.Location = new System.Drawing.Point(86, 97);
            this.label34.Name = "label34";
            this.label34.Size = new System.Drawing.Size(44, 16);
            this.label34.TabIndex = 44;
            this.label34.Text = "狀態:";
            // 
            // button12
            // 
            this.button12.Font = new System.Drawing.Font("新細明體", 12F);
            this.button12.Location = new System.Drawing.Point(469, 112);
            this.button12.Name = "button12";
            this.button12.Size = new System.Drawing.Size(87, 35);
            this.button12.TabIndex = 43;
            this.button12.Text = "停止轉檔";
            this.button12.UseVisualStyleBackColor = true;
            this.button12.Visible = false;
            // 
            // button13
            // 
            this.button13.Font = new System.Drawing.Font("新細明體", 12F);
            this.button13.Location = new System.Drawing.Point(558, 112);
            this.button13.Name = "button13";
            this.button13.Size = new System.Drawing.Size(87, 35);
            this.button13.TabIndex = 42;
            this.button13.Text = "開始轉檔";
            this.button13.UseVisualStyleBackColor = true;
            // 
            // MachiHCLAB
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(706, 520);
            this.Controls.Add(this.tabControl1);
            this.Name = "MachiHCLAB";
            this.Text = "Lab-HCLAB儀器";
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.Label label23;
        private System.Windows.Forms.RichTextBox HCLABSendErrorMsg;
        private System.Windows.Forms.RichTextBox HCLABSendMsg;
        private System.Windows.Forms.Label label24;
        private System.Windows.Forms.Label label25;
        private System.Windows.Forms.Label label26;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label hclabSendLastStartTime;
        private System.Windows.Forms.Label hclabSendStatus;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Button button10;
        private System.Windows.Forms.Button button11;
        private System.Windows.Forms.RichTextBox HCLABReceiveErrorMsg;
        private System.Windows.Forms.RichTextBox HCLABReceiveMsg;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label27;
        private System.Windows.Forms.Label label28;
        private System.Windows.Forms.Label label29;
        private System.Windows.Forms.Label label30;
        private System.Windows.Forms.Label label31;
        private System.Windows.Forms.Label hclabReceiveLastStartTime;
        private System.Windows.Forms.Label hclabReceiveStatus;
        private System.Windows.Forms.Label label34;
        private System.Windows.Forms.Button button12;
        private System.Windows.Forms.Button button13;
    }
}

