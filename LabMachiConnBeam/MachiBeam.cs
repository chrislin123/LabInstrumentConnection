using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Configuration;
using LabMachiLib;
using System.Threading;


namespace LabMachiConnBeam
{
    public partial class MachiBeam : Form
    {
        string ssql = string.Empty;
        EzMySQL SQL = new EzMySQL(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);        
        LabComm oLabComm = new LabComm(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);        
        //NetworkDetection nd = new NetworkDetection();
       
        public MachiBeam()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            

            Form.CheckForIllegalCrossThreadCalls = false;

            //取得執行檔最後修改時間
            string sExePath = this.GetType().Assembly.Location;
            DateTime dt = System.IO.File.GetLastWriteTime(sExePath);

            //依照connection判斷正式主機還是測試主機
            string sConnEnv = "";
            string Connection = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            if (Connection.Contains("192.168.72.20")) sConnEnv = "測試區";
            if (Connection.Contains("192.168.150.53")) sConnEnv = "長安";

            //顯示標題
            this.Text = string.Format("{0}[{1}]-版本[{2}]", this.Text, sConnEnv, dt.ToString());
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            lblStatus.Text = "轉檔中";
            lblStartTime.Text = DateTime.Now.ToShortDateString() + DateTime.Now.ToShortTimeString();

            if (timer1.Enabled == false)
            {
                timer1.Enabled = true;
            }         
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            string stest = string.Empty;


            //測試


            /*
                 1.this.Close();   只是关闭当前窗口，若不是主窗体的话，是无法退出程序的，另外若有托管线程（非主线程），也无法干净地退出；

                 2.Application.Exit();  强制所有消息中止，退出所有的窗体，但是若有托管线程（非主线程），也无法干净地退出；

                 3.Application.ExitThread(); 强制中止调用线程上的所有消息，同样面临其它线程无法正确退出的问题；

                 4.System.Environment.Exit(Environment.ExitCode);   这是最彻底的退出方式，不管什么线程都被强制退出，把程序结束的很干净。            
             */

            this.Close();
            //Application.Exit();
                        
            //System.Environment.Exit(0); 
        }

        private void btnTest_Click(object sender, EventArgs e)
        {

            //nd.ByTcpIp("192.168.72.20", 3306, 1);

            //return;

            while (true) {

                //if (nd.ByTcpIp("192.168.72.20", 3306, 1) == false)
                //{
                //    //資料庫連線失敗
                //    oLabComm.FormMsgShow(txtMsg, "資料庫 telnet 失敗。");
                //    return;
                //}

                try
                {
                    ssql = " select * from patientdata ";

                    DataTable dt = SQL.Get_DataTable(ssql);
                    //oLabComm.FormMsgShow(txtMsg, "資料庫連線成功。");

                }
                catch (Exception ex)
                {
                    //資料庫連線失敗
                    oLabComm.FormMsgShow(txtMsg, "資料庫連線失敗。");
                    //return;                    
                }

                oLabComm.FormMsgShow(txtMsg, "資料庫 telnet 成功。");
                Application.DoEvents();
                Thread.Sleep(500);
            }

            return;

      
            


            if (bwBeam.IsBusy == true)
            {
                bwBeam.CancelAsync();
            }
        }

        private void MachiAdvia_FormClosed(object sender, FormClosedEventArgs e)
        {
            //这是最彻底的退出方式，不管什么线程都被强制退出，把程序结束的很干净。              
            Environment.Exit(Environment.ExitCode);     
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            Beam oBeam = new Beam();

            worker.WorkerReportsProgress = true;

            //啟動
            oBeam.Start(worker);

        }

        private void MachiBeam_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (bwBeam.IsBusy == true)
            {
                bwBeam.CancelAsync();               
            }           
        }

        private void txtMsg_TextChanged(object sender, EventArgs e)
        {

            if (txtMsg.Lines.Length > 12)
            {
                //txtMsg.Text = txtMsg.Text.Substring(txtMsg.Lines[0].Length + 1);
                
                //txtMsg.ScrollToCaret();

            }	
        }

        private void txtErrorMsg_TextChanged(object sender, EventArgs e)
        {
            //固定只顯示幾行資訊
            if (txtErrorMsg.Lines.Length > 12)
            {
                txtErrorMsg.Text = txtErrorMsg.Text.Substring(txtErrorMsg.Lines[0].Length + 1);
            }	
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            //Dictionary<string, string> dConn = oLabComm.ConvertConnString(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);

            //string sDbIp = dConn["server"];
            //if (nd.ByTcpIp(sDbIp, 3306, 1) == false)
            //{
            //    //資料庫連線失敗
            //    oLabComm.FormMsgShow(txtMsg, "資料庫連線失敗。");
            //    return;
            //}

            bwBeam.WorkerSupportsCancellation = true;
            if (bwBeam.IsBusy == false)
            {
                //oLabComm.FormMsgShow(txtMsg, "轉檔開始。");
                bwBeam.RunWorkerAsync();
            }
            else
            {
                oLabComm.FormMsgShow(txtMsg, "已在執行中。");
            }

        }

        private void bwBeam_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (e.ProgressPercentage == 999) //訊息
            {
                if (e.UserState != null)
                {
                    oLabComm.FormMsgShow(txtMsg, e.UserState.ToString());
                }
            }

            if (e.ProgressPercentage == 888) //錯誤
            {
                if (e.UserState != null)
                {
                    oLabComm.FormMsgShow(txtErrorMsg, e.UserState.ToString());
                }
            }

            if (e.ProgressPercentage == 777) //狀態
            {
                if (e.UserState != null)
                {
                    lblStatus.Text = e.UserState.ToString();                    
                }
            }
        }



    }
}
