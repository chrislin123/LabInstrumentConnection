using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

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


    }
}
