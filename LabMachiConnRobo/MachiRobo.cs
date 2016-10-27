using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using LabMachiLib;
using System.Threading;
using System.Configuration;
using System.Diagnostics;

namespace LabMachiConnRobo
{
    public partial class MachiRobo : Form
    {
        string ssql = string.Empty;
        EzMySQL SQL = new EzMySQL(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
        LabComm oLabComm = new LabComm(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);

        public MachiRobo()
        {
            InitializeComponent();
        }

        private void MachiRobo_Load(object sender, EventArgs e)
        {
            //判斷程式是否重複執行
            oLabComm.CheckProcess();

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

            //取得IP            
            lblConnHost.Text = oLabComm.GetSystemCtrlData("LabTubeIP");
            //取得Port            
            lblConnPort.Text = oLabComm.GetSystemCtrlData("LabTubePort");
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            lblStatus.Text = "轉檔中";
            lblStartTime.Text = DateTime.Now.ToShortDateString() + DateTime.Now.ToShortTimeString();

            bwRobo.WorkerSupportsCancellation = true;
            if (bwRobo.IsBusy == false)
            {
                bwRobo.RunWorkerAsync();
            }

        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void MachiRobo_FormClosed(object sender, FormClosedEventArgs e)
        {
            //这是最彻底的退出方式，不管什么线程都被强制退出，把程序结束的很干净。              
            Environment.Exit(Environment.ExitCode);                     
        }

        private void bwRobo_DoWork(object sender, DoWorkEventArgs e)
        {

            ROBO8000 oROBO8000 = new ROBO8000();

            while (true)
            {
                if (bwRobo.CancellationPending == true)
                {
                    lblStatus.Text = "stop";
                    Application.DoEvents();
                    e.Cancel = true;
                    break;
                }

                //每三秒執行一次
                Thread.Sleep(3000);



                //oROBO8000.Close();
                //oROBO8000.closeSymbol = true;

                //啟動
                oROBO8000.Start(txtMsg, txtErrorMsg, lblStatus);

            }
        }

        private void MachiRobo_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (bwRobo.IsBusy == true)
            {
                bwRobo.CancelAsync();
            }           
        }
    }
}
