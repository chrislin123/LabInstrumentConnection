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
using System.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


namespace LabMachiConnCobas
{
    public partial class MachiCobas : Form
    {
        Cobas oCobas = new Cobas(8083);

        public MachiCobas()
        {
            InitializeComponent();
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            //cobas start
            if (!bwCobas.IsBusy)
            {
                lblStatus.Text = "轉檔中";
                lblStartTime.Text = DateTime.Now.ToShortDateString() + DateTime.Now.ToShortTimeString();
                bwCobas.RunWorkerAsync();
            }
        }

        private void bwCobas_DoWork(object sender, DoWorkEventArgs e)
        {
            

            //cobas BgWorker event => start socket 
            oCobas.Start(txtMsg, txtErrorMsg);
        }

        private void MachiCobas_FormClosed(object sender, FormClosedEventArgs e)
        {
            //这是最彻底的退出方式，不管什么线程都被强制退出，把程序结束的很干净。              
            Environment.Exit(Environment.ExitCode);     
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            oCobas.Close();
            this.Close();         
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void btnStartNew_Click(object sender, EventArgs e)
        {
            //LabComm oLabComm = new LabComm(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);


            //dynamic dymTransFinish = new JObject();
            //dymTransFinish.Add("tubeNo", "201612190007");
            //dymTransFinish.Add("seqNo", "1"); //非對應預設帶1
            //dymTransFinish.Add("macPscCode", "MCH");
            //dymTransFinish.Add("txTime", DateTime.Now.ToString("yyyy-MM-dd HH:mm"));

            //string jTransFinish = JsonConvert.SerializeObject(dymTransFinish);

            ////回傳完成訊息至HIS系統
            //oLabComm.PostTxResult(jTransFinish);

            

            //return;


            //cobas start
            if (!bwCobas.IsBusy)
            {
                lblMode.Text = "新系統轉檔";
                lblStatus.Text = "轉檔中";
                lblStartTime.Text = DateTime.Now.ToShortDateString() + DateTime.Now.ToShortTimeString();
                
                bwCobasNew.RunWorkerAsync();
            }
        }

        private void bwCobasNew_DoWork(object sender, DoWorkEventArgs e)
        {
            oCobas.StartNew(txtMsg, txtErrorMsg);
        }


    }
}
