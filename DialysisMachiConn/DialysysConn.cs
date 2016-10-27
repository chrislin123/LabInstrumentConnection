using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using LabMachiLib;
using System.Threading;
using System.Configuration;

namespace DialysisMachiConn
{
    public partial class DialysysConn : Form
    {
        EzMySQL SQL = new EzMySQL(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
        LabComm oLabComm = new LabComm(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
        string ssql = string.Empty;
        NetworkDetection nd = new NetworkDetection();
        int RouteTime = 1000;
        const string sFirmType = "Dialysys";
        Dictionary<string, string> pPar = new Dictionary<string, string>();

        public DialysysConn()
        {
            InitializeComponent();
        }

        private void DialysysConn_Load(object sender, EventArgs e)
        {
            //判斷程式是否重複執行
            oLabComm.CheckProcess();

            Form.CheckForIllegalCrossThreadCalls = false;

            //取得院所名稱
            string sConnEnv = oLabComm.getHospName();

            //顯示標題
            this.Text = oLabComm.getTitleText(this.Text, this.GetType().Assembly.Location);

            if (sConnEnv == "長安")
            {
                RouteTime = 10000;
            }

            //timer1.Interval = RouteTime;
           
            //格式化 參數資料
            pPar.Add("bMachiOriLog", "N");
        }

     
        private void btnStart_Click(object sender, EventArgs e)
        {
                
            //string sss = "02.35";
            ////sss = "00100";
            //double dddd = Convert.ToDouble(sss);
            //int iii = Convert.ToInt32(dddd);

            lblStatus.Text = "轉檔中";
            lblStartTime.Text = DateTime.Now.ToShortDateString() + DateTime.Now.ToShortTimeString();

            if (timer1.Enabled == false)
            {
                timer1.Enabled = true;   
            }
        }

        private void bwDialysis_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            Dialysys oDialysys = new Dialysys();

            worker.WorkerReportsProgress = true;

           
            if (bwDialysis.CancellationPending == true)
            {
                //lblStatus.Text = "stop";
                //Application.DoEvents();
                e.Cancel = true;
                //break;
            }                

            ssql = " select setdata from appsetting where setkind = '{0}' and setType = '{1}' ";
            ssql = string.Format(ssql, "dial", "maip");
                
            DataTable dt = SQL.Get_DataTable(ssql);              
                
            foreach (DataRow dr in dt.Rows)
            {
                string host = dr["setdata"].ToString();

                List<string> lip = host.Split('.').ToList();
                if (lip.Count == 4)
                {
                    worker.ReportProgress(Convert.ToInt32(lip[3]));

                    //string sss = "我的測試傳遞參數";

                    //worker.ReportProgress(999, sss);
                }

                //啟動
                oDialysys.Start(host, 1401, worker,pPar);
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void DialysysConn_FormClosed(object sender, FormClosedEventArgs e)
        {
            //这是最彻底的退出方式，不管什么线程都被强制退出，把程序结束的很干净。              
            Environment.Exit(Environment.ExitCode);          
        }

        private void DialysysConn_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (bwDialysis.IsBusy == true)
            {
                bwDialysis.CancelAsync();
            }       
        }


        private void btnTest_Click(object sender, EventArgs e)
        {

            //SQL.ExecuteSQL(" delete from appsetting where setkind = 'test' and setType = 't1' ");




            //SQL.ExecuteSQL(" delete from appsetting where setkind = 'test' and setType = 't1' ");

            //string sss = SQL.Get_Scalar(" select setType from appsetting where setkind = 'test' and setType = 't1' ");


            //DataSet dt = SQL.Get_Dataset(" select setType from appsetting where setkind = 'test' ");

            //return;

            //測試格式化
            //Dialysys oDialysys = new Dialysys();

            //string sData = "K2143A00.30B00.00C00.00D00210E001.0F031.6G013.7H 0039I 0209J-0100K00085L00000a0b0c0d0e0f0g0h0M1N1O00.00P00.00Q00.00R000.0S000/00T0000/U0000/V0000/i027";

            //DialysysData oDialysysData = oDialysys.formatDialysysData(sData);

            ////寫入資料庫
            //oDialysys.InsDialysysdata(oDialysysData);

            string clientRequest = "";
            Dialysys oDialysys = new Dialysys();

            //模擬資料
            //clientRequest = "K2143A05.80B05.80C00.00D00000E000.0F031.6G013.7H-0039I-0209J-0244K00085L00000a0b0c0d0e0f0g0h0M1N1O00.00P00.00Q00.00R000.0S102030T00110U00061V00094i027";

            //格式化儀器資料
            DialysysData oDialysysData = oDialysys.formatDialysysData(clientRequest);

            //1051005 排除==沒有血壓時間的資料
            if (oDialysysData.S == "000/00") return;

            //1051007 排除==未治療中資料
            if (oDialysysData.M == "0") return;

            //補上資料
            oDialysysData.machinip = "";
            oDialysysData.chdate = oLabComm.GetNowDate();
            oDialysysData.chdt = oLabComm.GetNowFullDateTime();

            //原始資料寫入資料庫
            if (pPar["bMachiOriLog"] == "Y")
            {
                oLabComm.insertErrlog(sFirmType, clientRequest);
            }

            //寫入資料庫
            oDialysys.InsDialysysdata(oDialysysData);
            //bw.ReportProgress(999, "寫入資料庫");

            //寫入HIS系統
            //oDialysys.InsDialysysdataToHis(oDialysysData);
            //bw.ReportProgress(999, "寫入HIS系統");

            return;

            Socket _mySocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            //取得IP
            string host = "192.168.10.192";
            //取得Port
            int port = 1401;

            try
            {
                IPAddress[] IPs = Dns.GetHostAddresses(host);

                //Socket連線
                oLabComm.FormMsgShow(txtMsg, "測試連線");
                Application.DoEvents();


                if (nd.ByTcpIp(IPs[0].ToString(), port, 1) == false)
                //if (nd.ByPing(IPs[0].ToString()) == false)
                {
                    oLabComm.FormMsgShow(txtMsg, string.Format("無法連線 {0}:{1}", IPs[0].ToString(), port));
                    Application.DoEvents();

                    return;
                }

                //Socket連線
                oLabComm.FormMsgShow(txtMsg, "連線");
                Application.DoEvents();


                _mySocket.Connect(IPs[0], port);

                byte[] readBuffer = new byte[_mySocket.ReceiveBufferSize];
                int count = 0;

                if (_mySocket.Connected == true)
                {
                    oLabComm.FormMsgShow(txtMsg, "送信");
                    Application.DoEvents();

                    int byteCount = _mySocket.Send(Encoding.GetEncoding("Big5").GetBytes("K\r\n"));

                    oLabComm.FormMsgShow(txtMsg, byteCount.ToString());
                    Application.DoEvents();


                    //int count = 0;
                    //returnMsg = new byte[_mySocket.ReceiveBufferSize];

                    oLabComm.FormMsgShow(txtMsg, "取得資料");
                    Application.DoEvents();

                    count = _mySocket.Receive(readBuffer);
                    oLabComm.FormMsgShow(txtMsg, count.ToString());
                    Application.DoEvents();

                    clientRequest = Encoding.ASCII.GetString(readBuffer, 0, count);
                    oLabComm.FormMsgShow(txtMsg, clientRequest);
                    Application.DoEvents();
                }

            }
            catch (Exception ex)
            {
                oLabComm.FormMsgShow(txtMsg, ex.ToString());
            }
            finally
            {
                //關閉Socket
                //_mySocket.Close();
                oLabComm.CloseSocket(_mySocket);

                if (_mySocket.Connected == false)
                {
                    oLabComm.FormMsgShow(txtMsg, "已關閉");
                    Application.DoEvents();
                }

            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            
            //timer1.Stop();
            try
            {
                Dictionary<string, string> dConn = oLabComm.ConvertConnString(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);

                string sDbIp = dConn["server"];
                if (nd.ByTcpIp(sDbIp, 3306, 1) == false)
                {
                    //資料庫連線失敗
                    oLabComm.FormMsgShow(txtMsg, "資料庫連線失敗。");
                    return;
                }

                bwDialysis.WorkerSupportsCancellation = true;
                if (bwDialysis.IsBusy == false)
                {
                    oLabComm.FormMsgShow(txtMsg, "轉檔開始。");
                    bwDialysis.RunWorkerAsync();
                }
                else
                {
                    //oLabComm.FormMsgShow(txtMsg, "已在執行中。");
                }
            }
            finally
            {
                //timer1.Start();
            }
        }

        private void bwDialysis_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            oLabComm.FormMsgShow(txtMsg, "轉檔完畢。");
        }

        private void bwDialysis_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            //限制行數
            int iMaxLines = 13;

            if (e.ProgressPercentage == 999)
            {
                if (e.UserState != null)
                {
                    oLabComm.FormMsgShow(txtMsg, e.UserState.ToString());
                }
            }
            else
            {
                string sip = "192.168.10." + e.ProgressPercentage.ToString();

                oLabComm.FormMsgShow(txtMsg, string.Format("機器：{0} 資料轉檔。", sip));
            }

            //限制行數
            if (txtMsg.Lines.Length >= iMaxLines)
            {
                List<string> ltxt = txtMsg.Lines.ToList<string>();
                ltxt.RemoveRange(0, ltxt.Count - iMaxLines);
                txtMsg.Lines = ltxt.ToArray();
            }
        }

        private void chk1_CheckedChanged(object sender, EventArgs e)
        {
            if (chk1.Checked == true)
            {
                pPar["bMachiOriLog"] = "Y";
            }
            else
            {
                pPar["bMachiOriLog"] = "N";                    
            }
        }
        
    }
}
