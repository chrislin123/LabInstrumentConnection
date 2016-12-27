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

namespace LabMachiConnHCLAB
{
    public partial class MachiHCLAB : Form
    {
        LabComm oLabComm = new LabComm(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);

        //HCLAB Send 9081
        HCLABSend _Job3 = new HCLABSend();
        //HCLAB Receive 9082
        HCLABReceive _Job4 = new HCLABReceive();

        public MachiHCLAB()
        {
            InitializeComponent();
        }


        #region OldHIS

        private void btnSend_Click(object sender, EventArgs e)
        {
            //hclabsend start
            if (!hclabSendBgWork.IsBusy)
            {
                hclabSendStatus.Text = "轉檔中";
                hclabSendLastStartTime.Text = DateTime.Now.ToShortDateString() + DateTime.Now.ToShortTimeString();
                hclabSendBgWork.RunWorkerAsync();
            }
        }



        private void btnRec_Click(object sender, EventArgs e)
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

        private void hclabSendBgWork_DoWork(object sender, DoWorkEventArgs e)
        {
            //hclabsend BgWorker event => start socket 
            _Job3.Start(HCLABSendMsg, HCLABSendErrorMsg);
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

        #endregion

        private void btnSendClose_Click(object sender, EventArgs e)
        {
            //hclabsend closing
            hclabSendClosing();
            //newhis
            hclabSendClosingNew();
        }

        private void btnRecClose_Click(object sender, EventArgs e)
        {
            //hclabreceive 
            hclabReceiveClosing();
            //newhis
            hclabReceiveClosingNew();
        }


        

        #region NewHIS       

        private void btnSendNew_Click(object sender, EventArgs e)
        {
            //hclabsend start
            if (!hclabSendBgWorkNew.IsBusy)
            {
                hclabSendStatus.Text = "轉檔中";
                hclabSendLastStartTime.Text = DateTime.Now.ToShortDateString() + DateTime.Now.ToShortTimeString();
                hclabSendBgWorkNew.RunWorkerAsync();
            }
        }

        private void hclabSendBgWorkNew_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;

            try
            {
                //hclabsend BgWorker event => start socket 
                _Job3.StartNew(worker);
            }
            catch (Exception ex)
            {
                oLabComm.FeedbackMsg(worker, msgtype.msgErr, ex.Message);
            }       

            
            //_Job3.StartNew();

        }

        private void hclabSendClosingNew()
        {
            hclabSendStatus.Text = "未轉檔";
            _Job3.Close();
            hclabSendBgWorkNew.CancelAsync();
        }

        private void btnRecNew_Click(object sender, EventArgs e)
        {
            //hclabreceive start
            if (!hclabReceiveBgWorkNew.IsBusy)
            {
                hclabReceiveStatus.Text = "轉檔中";
                hclabReceiveLastStartTime.Text = DateTime.Now.ToShortDateString() + DateTime.Now.ToShortTimeString();
                hclabReceiveBgWorkNew.RunWorkerAsync();
            }
        }

        private void hclabReceiveBgWorkNew_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;

            try
            {
                //hclabrecevie BgWorker event => start socket 
                _Job4.StartNew(worker);
            }
            catch (Exception ex)
            {
                oLabComm.FeedbackMsg(worker, msgtype.msgErr, ex.Message);
            }       
        }

        private void hclabReceiveClosingNew()
        {
            hclabReceiveStatus.Text = "未轉檔";
            _Job4.Close();
            hclabReceiveBgWorkNew.CancelAsync();
        }

        #endregion

        private void MachiHCLAB_FormClosed(object sender, FormClosedEventArgs e)
        {
            Environment.Exit(Environment.ExitCode);     
        }


        public string getLabItemStringNew(string tubeno, dynamic[] dlist)
        {
            string result = "";


            foreach (dynamic item in dlist)
            {
                if (item.tubeNo != tubeno) continue;
                
                if (result != "") result = result + "~";

                result = result + string.Format("{0}^{1}", item.macPscCode, item.macPscCode);
            }
            return result;
        }

        private void btnCloseSend_Click(object sender, EventArgs e)
        {

            //New 取得清單資料
            string sResp = oLabComm.GetListTodo();

            JObject jo1 = JObject.Parse(sResp);
            JObject joValue = JObject.Parse(jo1.GetValue("value").ToString());            
            dynamic dynList = joValue as dynamic;
            var ary2 = ((JArray)dynList.list).Cast<dynamic>().ToArray();

            List<string> listTemp = new List<string>();
            if (ary2.Length > 0)
            {
                

                foreach (JObject item in ary2)
                {
                    dynamic dynItem = item as dynamic;
                    string sMachineCode = dynItem.machineCode;
                    if (sMachineCode == "H" || sMachineCode == "S" )
                    {
                        listTemp.Add(Convert.ToString(dynItem.tubeNo));
                    }
                }
            }

            List<string> listTube = listTemp.Distinct().ToList();



            

            foreach (string tubeno in listTube)
            {

                string ssss = getLabItemStringNew(tubeno, ary2);


                listTube.Remove(tubeno);
            }



            return;




            //foreach (JObject item in ary2)
            //{
            //    dynamic dynItem = item as dynamic;
            //    MessageBox.Show(Convert.ToString(dynItem.tubeNo));
            //}

            return;

            this.Close();
        }

        private void btnCloseRec_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void hclabReceiveBgWorkNew_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            //一般訊息
            if (e.ProgressPercentage == (int)msgtype.msgGeneral)
            {
                if (e.UserState != null)
                {
                    oLabComm.FormMsgShow(HCLABReceiveMsg, e.UserState.ToString());
                }
            }

            //錯誤訊息
            if (e.ProgressPercentage == (int)msgtype.msgErr)
            {
                if (e.UserState != null)
                {
                    oLabComm.FormMsgShow(HCLABReceiveErrorMsg, e.UserState.ToString());
                }
            }

            //狀態
            if (e.ProgressPercentage == (int)msgtype.msgStatus)
            {
                if (e.UserState != null)
                {
                    hclabReceiveStatus.Text = e.UserState.ToString();
                }
            }

            //顯示行數
            oLabComm.LimitLines(HCLABReceiveMsg, 50);
        }

        private void hclabSendBgWorkNew_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            //一般訊息
            if (e.ProgressPercentage == (int)msgtype.msgGeneral)
            {
                if (e.UserState != null)
                {
                    oLabComm.FormMsgShow(HCLABSendMsg, e.UserState.ToString());
                }
            }
            
            //錯誤訊息
            if (e.ProgressPercentage == (int)msgtype.msgErr)
            {
                if (e.UserState != null)
                {
                    oLabComm.FormMsgShow(HCLABSendErrorMsg, e.UserState.ToString());
                }
            }
            
            //狀態
            if (e.ProgressPercentage == (int)msgtype.msgStatus)
            {
                if (e.UserState != null)
                {
                    hclabSendStatus.Text = e.UserState.ToString();
                }
            }

            //顯示行數
            oLabComm.LimitLines(HCLABSendMsg, 50);
        }

        

        

        
        

      
    }
}
