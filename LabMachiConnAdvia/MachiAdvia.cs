using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Configuration;
using LabMachiLib;
using System.Linq;

namespace LabMachiConnAdvia
{
    public partial class MachiAdvia : Form
    {
        string ssql = string.Empty;
        EzMySQL SQL = new EzMySQL(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);        
        LabComm oLabComm = new LabComm(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
        LabMachiLib.NetworkDetection NetDetect = new LabMachiLib.NetworkDetection();
        System.Drawing.Bitmap imgBall_Green = LabMachiConnAdvia.Properties.Resources.Ball_Green;
        System.Drawing.Bitmap imgBall_Red = LabMachiConnAdvia.Properties.Resources.Ball_Red;

        public MachiAdvia()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //判斷程式是否重複執行
            oLabComm.CheckProcess();

            Form.CheckForIllegalCrossThreadCalls = false;

            //取得院所名稱
            string sConnEnv = oLabComm.getHospName();
            
            //顯示標題
            this.Text = oLabComm.getTitleText(this.Text, this.GetType().Assembly.Location);

            //取得免疫機IP與Port
            string host = SQL.Get_Scalar("select chparamtext from basedata.systemctrl where chparamcode = 'ADVIACentaurIP'");
            lblConnHost.Text = host;
            //AVDIA ORDER Port
            int port = int.Parse(SQL.Get_Scalar("select chparamtext from basedata.systemctrl where chparamcode = 'ADVIACentaurPort'"));
            lblConnPort.Text = port.ToString();            

            //啟動測試儀器連線狀態
            //timerConnStatus.Enabled = true;

            if (sConnEnv == "測試")
            { 
                btnTest.Visible = true;
            }

        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            lblAdviaStatus.Text = "轉檔中";
            lblAdviaStartTime.Text = DateTime.Now.ToShortDateString() + DateTime.Now.ToShortTimeString();

            if (timerAdvia.Enabled == false)
            {
                timerAdvia.Enabled = true;
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {         
            this.Close();         
        }

        private void btnTest_Click(object sender, EventArgs e)
        {
            ADIVACentaur oadv = new ADIVACentaur();


            oadv.test(lblAdviaStatus);


            return;

//            string sss = @"H|^&|||NG_LIS|||||LIS_ID||P|1//                            P|1|||||||U//                            O|1|ZSI0052684^0059^B||^^^HBs^^^aHBs2|R||||||||||||||||||||F//                            R|1|^^^aHBs2^^^1^INTR|React|||||FR||||20161019170550//                            C|1|I|Above Check|I//                            R|2|^^^aHBs2^^^1^DOSE|237.80|mIU/mL||||FR||||20161019170550//                            C|1|I|Above Check|I//                            R|3|^^^aHBs2^^^1^COFF|1.00|mIU/mL||||FR||||20161019170550//                            C|1|I|Above Check|I//                            R|4|^^^aHBs2^^^1^RLU|356150|||||FR||||20161019170550//                            C|1|I|Above Check|I//                            R|5|^^^HBs^^^1^INTR|NR|||<||FR||||20161019171406//                            C|1|I|Below Check|I//                            R|6|^^^HBs^^^1^INDX|< 0.10|||<||FR||||20161019171406//                            C|1|I|Below Check|I//                            R|7|^^^HBs^^^1^COFF|1.00|Index||<||FR||||20161019171406//                            C|1|I|Below Check|I//                            R|8|^^^HBs^^^1^RLU|1041|||<||FR||||20161019171406//                            C|1|I|Below Check|I//                            L|1//                            ";


//            AdivaData ad = oadv.DataConvertToAdivaData(sss);





            //timerSend.Enabled = true;

            try
            {
                bwSend.WorkerSupportsCancellation = true;
                if (bwSend.IsBusy == false)
                {
                    oLabComm.FormMsgShow(txtMsg, "開始接收");
                    bwSend.WorkerReportsProgress = true;
                    bwSend.RunWorkerAsync();
                }
            }
            finally
            {
                string aaaaaa = "";
                //timer1.Start();
            }

        }

        private void MachiAdvia_FormClosed(object sender, FormClosedEventArgs e)
        {
            //这是最彻底的退出方式，不管什么线程都被强制退出，把程序结束的很干净。              
            Environment.Exit(Environment.ExitCode);     
        }

        private void NetworkDetector_DoWork(object sender, DoWorkEventArgs e)
        {
            ////偵測網路測試
            //LabMachiLib.NetworkDetection NetDetect = new LabMachiLib.NetworkDetection();
            //System.Drawing.Bitmap imgBall_Green = LabMachiConnAdvia.Properties.Resources.Ball_Green;
            //System.Drawing.Bitmap imgBall_Red = LabMachiConnAdvia.Properties.Resources.Ball_Red;
            //string sConnHost = lblConnHost.Text;
            //int sConnPort = Convert.ToInt32(lblConnPort.Text);
                
            //int i = 0;
            //while (true)
            //{
            //    //閃燈功能
            //    pbConnStatus.Image = null;
            //    pbConnStatus.Refresh();
            //    System.Threading.Thread.Sleep(100);
                
            //    //使用ping 偵測網路
            //    //if (NetDetect.ByPing("168.95.1.1"))


            //    //使用Tcp 偵測網路
            //    if (NetDetect.ByTcpIp(sConnHost, sConnPort,1))
            //    {
            //        pbConnStatus.Image = imgBall_Green;
            //    }
            //    else
            //    {
            //        pbConnStatus.Image = imgBall_Red;
            //    }

            //    pbConnStatus.Refresh();
                
            //    //每一秒偵測一次
            //    System.Threading.Thread.Sleep(1000);
            //}            
        }

        private void timerConnStatus_Tick(object sender, EventArgs e)
        {
            //==偵測網路

            //ip            
            string sConnHost = lblConnHost.Text;
            //port
            int sConnPort = Convert.ToInt32(lblConnPort.Text);

            //閃燈功能
            pbConnStatus.Image = null;
            pbConnStatus.Refresh();
            System.Threading.Thread.Sleep(100);

            //使用ping 偵測網路
            //if (NetDetect.ByPing("168.95.1.1"))


            //使用Tcp 偵測網路
            if (NetDetect.ByTcpIp(sConnHost, sConnPort, 1))
            {
                pbConnStatus.Image = imgBall_Green;
            }
            else
            {
                pbConnStatus.Image = imgBall_Red;
            }

            pbConnStatus.Refresh();

            //每一秒偵測一次
            //System.Threading.Thread.Sleep(1000);                  
        }

        private void bwAdvia_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;

            ADIVACentaur oAdvia = new ADIVACentaur();

            try
            {
                oAdvia.Start(bwAdvia);
            }
            catch (Exception ex)
            {
                oLabComm.FeedbackMsg(worker, msgtype.msgErr, ex.Message);                
            }           
        }

        private void bwAdvia_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            //一般訊息
            if (e.ProgressPercentage == (int)msgtype.msgGeneral)
            {
                if (e.UserState != null)
                {
                    oLabComm.FormMsgShow(txtMsg, e.UserState.ToString());
                }
            }         
 
            //錯誤訊息
            if (e.ProgressPercentage == (int)msgtype.msgErr)
            {
                if (e.UserState != null)
                {
                    oLabComm.FormMsgShow(txtErrorMsg, e.UserState.ToString());
                }
            }         

            //狀態
            if (e.ProgressPercentage == (int)msgtype.msgStatus)
            {
                if (e.UserState != null)
                {
                    lblAdviaStatus.Text = e.UserState.ToString();
                }
            }

            //顯示行數
            oLabComm.LimitLines(txtMsg, 50);
        }

        private void bwAdvia_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            //oLabComm.FormMsgShow(txtMsg, "轉檔完畢。");

        }

        private void timerAdvia_Tick(object sender, EventArgs e)
        {
            try
            {
                bwAdvia.WorkerSupportsCancellation = true;
                if (bwAdvia.IsBusy == false)
                {
                    oLabComm.FormMsgShow(txtMsg, "開始接收");
                    bwAdvia.WorkerReportsProgress = true;
                    bwAdvia.RunWorkerAsync();

                }                
            }
            finally
            {
                //timer1.Start();
            }

        }

        private void bwSend_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;

            ADIVACentaur oAdvia = new ADIVACentaur();

            try
            {
                oAdvia.Send(worker);
            }
            catch (Exception ex)
            {
                oLabComm.FeedbackMsg(worker, msgtype.msgErr, ex.Message);
            }           
        }

        private void bwSend_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            //一般訊息
            if (e.ProgressPercentage == (int)msgtype.msgGeneral)
            {
                if (e.UserState != null)
                {
                    oLabComm.FormMsgShow(txtMsg, e.UserState.ToString());
                }
            }

            //錯誤訊息
            if (e.ProgressPercentage == (int)msgtype.msgErr)
            {
                if (e.UserState != null)
                {
                    oLabComm.FormMsgShow(txtErrorMsg, e.UserState.ToString());
                }
            }

            //狀態
            if (e.ProgressPercentage == (int)msgtype.msgStatus)
            {
                if (e.UserState != null)
                {
                    lblAdviaStatus.Text = e.UserState.ToString();
                }
            }

            //顯示行數
            oLabComm.LimitLines(txtMsg, 8);
        }

        private void bwSend_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            oLabComm.FormMsgShow(txtMsg, "轉檔完畢。");
        }

        private void timerSend_Tick(object sender, EventArgs e)
        {
            try
            {
                bwSend.WorkerSupportsCancellation = true;
                if (bwSend.IsBusy == false)
                {
                    oLabComm.FormMsgShow(txtMsg, "開始接收");
                    bwSend.WorkerReportsProgress = true;
                    bwSend.RunWorkerAsync();
                }
            }
            finally
            {
                //timer1.Start();
            }
        }

        private void btnStartNew_Click(object sender, EventArgs e)
        {
            lblAdviaStatus.Text = "轉檔中";
            lblAdviaStartTime.Text = DateTime.Now.ToShortDateString() + DateTime.Now.ToShortTimeString();

            if (timerAdviaNew.Enabled == false)
            {
                timerAdviaNew.Enabled = true;
            }
        }

        private void timerAdviaNew_Tick(object sender, EventArgs e)
        {
            try
            {


                bwAdviaNew.WorkerSupportsCancellation = true;
                if (bwAdviaNew.IsBusy == false)
                {
                    oLabComm.FormMsgShow(txtMsg, "開始接收");
                    bwAdviaNew.WorkerReportsProgress = true;
                    bwAdviaNew.RunWorkerAsync();

                }
            }
            finally
            {
                //timer1.Start();
            }
        }

        private void bwAdviaNew_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;

            ADIVACentaur oAdvia = new ADIVACentaur();

            try
            {
                oAdvia.Start(bwAdvia);
            }
            catch (Exception ex)
            {
                oLabComm.FeedbackMsg(worker, msgtype.msgErr, ex.Message);
            }           
        }

        private void bwAdviaNew_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            //一般訊息
            if (e.ProgressPercentage == (int)msgtype.msgGeneral)
            {
                if (e.UserState != null)
                {
                    oLabComm.FormMsgShow(txtMsg, e.UserState.ToString());
                }
            }

            //錯誤訊息
            if (e.ProgressPercentage == (int)msgtype.msgErr)
            {
                if (e.UserState != null)
                {
                    oLabComm.FormMsgShow(txtErrorMsg, e.UserState.ToString());
                }
            }

            //狀態
            if (e.ProgressPercentage == (int)msgtype.msgStatus)
            {
                if (e.UserState != null)
                {
                    lblAdviaStatus.Text = e.UserState.ToString();
                }
            }

            //顯示行數
            oLabComm.LimitLines(txtMsg, 50);
        }

        private void bwAdviaNew_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            //oLabComm.FormMsgShow(txtMsg, "轉檔完畢。");
        }      
    }
}
