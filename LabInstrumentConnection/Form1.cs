using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Configuration;
using System.Threading;


namespace LabInstrumentConnection
{
    public partial class Form1 : Form
    {
        //Cobas 8083
        Cobas_b221 _Job1 = new Cobas_b221(8083);
        //ROBO8000 8081
        ROBO8000 _Job2 = new ROBO8000();
        //HCLAB Send 9081
        HCLABSend _Job3 = new HCLABSend();
        //HCLAB Receive 9082
        HCLABReceive _Job4 = new HCLABReceive();
        //醫全實業
        Beam _Job5 = new Beam();
        //免疫機
        ADIVACentaur _Job6 = new ADIVACentaur();

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //EzMySQL SQL = new EzMySQL();
            //DataTable dt =  SQL.Get_DataTable("select * from patientdata");            
            this.Close();
        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            //cobas closing 
            CobasClosing();
            //robo8000 closing
            robo8000Closing();
            //hclabsend closing
            hclabSendClosing();
            //hclabreceive closing
            hclabReceiveClosing();
            //beam closing
            beamClosing();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //cobas start
            if (!cobasBgWork.IsBusy)
            {
                cobasStatus.Text = "轉檔中";
                cobasLastStartTime.Text = DateTime.Now.ToShortDateString() + DateTime.Now.ToShortTimeString();
                cobasBgWork.RunWorkerAsync();
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            //cobas closing
            CobasClosing();
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
            if (Connection.Contains("192.168.72.20")) sConnEnv = "測試";
            if (Connection.Contains("192.168.150.53")) sConnEnv = "長安";

            //顯示標題
            this.Text = string.Format("{0}[{1}]-{2}", this.Text, sConnEnv, dt.ToString());

            string sss = string.Empty;
        }

        private void cobasBgWork_DoWork(object sender, DoWorkEventArgs e)
        {
            //cobas BgWorker event => start socket 
            _Job1.Start(cobasMsg, cobasErrorMsg);
        }
        private void CobasClosing()
        {
            cobasStatus.Text = "未轉檔";
            _Job1.Close();
            cobasBgWork.CancelAsync();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            //robo8000 start
            if (!robo8000BgWork.IsBusy)
            {
                robo8000Status.Text = "轉檔中";
                robo8000LastStartTime.Text = DateTime.Now.ToShortDateString() + DateTime.Now.ToShortTimeString();
                robo8000BgWork.RunWorkerAsync();
            }
        }

        private void robo8000BgWork_DoWork(object sender, DoWorkEventArgs e)
        {
            //robo8000 BgWorker event => start socket 
            _Job2.Start(robo8000Msg, robo8000ErrorMsg);
        }
        private void robo8000Closing()
        {
            robo8000Status.Text = "未轉檔";
            _Job2.Close();
            robo8000BgWork.CancelAsync();
        }
        private void hclabSendClosing()
        {
            hclabSendStatus.Text = "未轉檔";
            _Job3.Close();
            hclabSendBgWork.CancelAsync();
        }
        private void hclabReceiveClosing()
        {
            hclabReceiveStatus.Text = "未轉檔";
            _Job4.Close();
            hclabReceiveBgWork.CancelAsync();
        }
        private void beamClosing()
        {
            beamReceiveStatus.Text = "未轉檔";            
            //_Job4.Close();
            beamBgWork.CancelAsync();            
        }
        private void button6_Click(object sender, EventArgs e)
        {
            //robo8000 closing
            robo8000Closing();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            //robo8000 start
            button7_Click(sender, e);
            //cobas start
            button2_Click(sender, e);
            //hclabsend start
            button11_Click(sender, e);
            //hclabreceive start
            button13_Click(sender, e);
            //beam start
            button5_Click(sender, e);
            //免疫機
            //button5_Click(sender, e);
        }

        private void button9_Click(object sender, EventArgs e)
        {
            //robo8000 closing
            robo8000Closing();
            //cobas closing
            CobasClosing();
            //hclabsend closing
            hclabSendClosing();
            //hclabreceive 
            hclabReceiveClosing();
        }

        private void button11_Click(object sender, EventArgs e)
        {
            //hclabsend start
            if (!hclabSendBgWork.IsBusy)
            {
                hclabSendStatus.Text = "轉檔中";
                hclabSendLastStartTime.Text = DateTime.Now.ToShortDateString() + DateTime.Now.ToShortTimeString();
                hclabSendBgWork.RunWorkerAsync();
            }
        }

        private void hclabSendBgWork_DoWork(object sender, DoWorkEventArgs e)
        {
            //hclabsend BgWorker event => start socket 
            _Job3.Start(HCLABSendMsg, HCLABSendErrorMsg);
        }

        private void button10_Click(object sender, EventArgs e)
        {
            //hclabsend closing
            hclabSendClosing();
        }

        private void button13_Click(object sender, EventArgs e)
        {
            //hclabreceive start
            if (!hclabReceiveBgWork.IsBusy)
            {
                hclabReceiveStatus.Text = "轉檔中";
                hclabReceiveLastStartTime.Text = DateTime.Now.ToShortDateString() + DateTime.Now.ToShortTimeString();
                hclabReceiveBgWork.RunWorkerAsync();
            }
        }

        private void hclabReceiveBgWork_DoWork(object sender, DoWorkEventArgs e)
        {
            //hclabrecevie BgWorker event => start socket 
            _Job4.Start(HCLABReceiveMsg, HCLABReceiveErrorMsg);
        }

        private void button12_Click(object sender, EventArgs e)
        {
            //hclabreceive 
            hclabReceiveClosing();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            EzMySQL SQL = new EzMySQL();
            int count = int.Parse(SQL.Get_Scalar("select count(1) from opd.labinstrumentlog"));

            if (count >= 10000)
            {
                SQL.ExecuteSQL(" truncate table opd.labinstrumentlog");
            }

        }

        private void button5_Click(object sender, EventArgs e)
        {
            //beamBgWork start
            if (!beamBgWork.IsBusy)
            {
                beamReceiveStatus.Text = "轉檔中";
                beamReceiveLastStartTime.Text = DateTime.Now.ToShortDateString() + DateTime.Now.ToShortTimeString();
                beamBgWork.RunWorkerAsync();
            }


        }

        private void button4_Click(object sender, EventArgs e)
        {
            //ADVIABgWork start
            if (!ADVIABgWork.IsBusy)
            {
                ADVIAStatus.Text = "轉檔中";
                ADVIAStartTime.Text = DateTime.Now.ToShortDateString() + DateTime.Now.ToShortTimeString();
                ADVIABgWork.RunWorkerAsync();
            }

        }

        private void beamBgWork_DoWork(object sender, DoWorkEventArgs e)
        {
            _Job5.Start(beamReceiveMsg, beamReceiveErrorMsg);
        }

        private void ADVIABgWork_DoWork(object sender, DoWorkEventArgs e)
        {
            _Job6.Start(ADVIAMsg, ADVIAErrorMsg);
        }

        private void label43_Click(object sender, EventArgs e)
        {

        }

        private void label41_Click(object sender, EventArgs e)
        {

        }

        private void label40_Click(object sender, EventArgs e)
        {

        }

        private void label46_Click(object sender, EventArgs e)
        {

        }

        private void label45_Click(object sender, EventArgs e)
        {

        }

        private void label50_Click(object sender, EventArgs e)
        {

        }

        private void ADVIAStatus_Click(object sender, EventArgs e)
        {

        }

        private void label47_Click(object sender, EventArgs e)
        {

        }

        private void ADVIAStartTime_Click(object sender, EventArgs e)
        {

        }

        private void ADVIAMsg_TextChanged(object sender, EventArgs e)
        {

        }

        private void ADVIAErrorMsg_TextChanged(object sender, EventArgs e)
        {

        }

        private void label44_Click(object sender, EventArgs e)
        {

        }
    }
}
