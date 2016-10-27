using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Sockets;


namespace LabMachiLib
{
    public enum msgtype
    {
        msgErr=999,
        msgGeneral=888,
        msgStatus=777
    }

    public class LabComm
    {
        EzMySQL SQL;
        string ssql = string.Empty;
        string sConnString = string.Empty;


        public LabComm(string pConnString)
        {
            sConnString = pConnString;
            SQL = new EzMySQL(pConnString);
        }

        public void insertlog(string type, string log)
        {
            try
            {
                //去除單引號雙引號
                log = log.Replace("'", "").Replace(@"""", "");

                string time = DateTime.Now.ToLongDateString() + DateTime.Now.ToLongTimeString();
                ssql = "insert into opd.labinstrumentlog (type,log,time) values('" + type + "','" + log + "','" + time + "')";

                SQL.ExecuteSQL(ssql);
                    
            }
            catch (Exception)
            {   
                
            }
        }

        public void insertErrlog(string type, string log)
        {
            try
            {
                //去除單引號雙引號
                log = log.Replace("'", "").Replace(@"""", "");

                string time = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
                ssql = "insert into opd.machinelog (type,time,log) values('" + type + "','" + time + "','" + log + "')";

                SQL.ExecuteSQL(ssql);

            }
            catch (Exception)
            {

            }
        }

        public void FormMsgShow(System.Windows.Forms.RichTextBox msgText,string pMsg)
        {   
            string sDateTime = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
            msgText.AppendText(sDateTime + "=" + pMsg + "\r\n");
            msgText.ScrollToCaret(); 
            
        }

        public void FeedbackMsg(System.ComponentModel.BackgroundWorker bw,msgtype mt, string pMsg)
        {            
            //string sDateTime = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");

            //string msg = string.Format("{0}={1}", sDateTime, pMsg);

            bw.ReportProgress((int)(msgtype)mt, pMsg);                        
        }

        public string GetNowDataTime()
        {
            //格式：(2016/08/22 09:33:45)
            string snowtest = DateTime.Now.ToString("");



            //格式：(2016/08/22 09:33:45)
            //string sNow = DateTime.Now.Year.ToString() + "/" 
            //            + DateTime.Now.Month.ToString().PadLeft(2, '0') + "/" 
            //            + DateTime.Now.Day.ToString().PadLeft(2, '0') + " " 
            //            + DateTime.Now.Hour.ToString().PadLeft(2, '0') + ":" 
            //            + DateTime.Now.Minute.ToString().PadLeft(2, '0') + ":" 
            //            + DateTime.Now.Second.ToString().PadLeft(2, '0');

            return snowtest;
        }

        /// <summary>
        /// 西元年(YYYYMMDD)
        /// </summary>
        /// <returns></returns>
        public string GetNowDate()
        {
            //格式：(yyyyMMdd)
            string sNowDate = DateTime.Now.ToString("yyyyMMdd");

            return sNowDate;
        }

        /// <summary>
        /// 西元年(yyyyMMddHHmmss)
        /// </summary>
        /// <returns></returns>
        public string GetNowFullDateTime()
        {
            //格式：(2016/08/22 09:33:45)
            string sNowFullDateTime = DateTime.Now.ToString("yyyyMMddHHmmss");

            return sNowFullDateTime;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pType"></param>
        /// <returns>
        /// pType=1,格式：(20160822093345)
        /// pType=2,格式：(2016/08/22 09:33:45)
        /// </returns>
        public string GetNowFullDateTime(string pType)
        {
            string sNowFullDateTime = "";

            //格式：(20160822093345)
            if (pType == "1") sNowFullDateTime = DateTime.Now.ToString("yyyyMMddHHmmss");
            //格式：(2016/08/22 09:33:45)
            if (pType == "2") sNowFullDateTime = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
            

            return sNowFullDateTime;
        }



        public string GetSystemCtrlData(string sParamCode)
        {
            string sResult = string.Empty;

            ssql = "select chParamText from basedata.systemctrl where chparamcode = '{0}'";
            ssql = string.Format(ssql, sParamCode);

            sResult = SQL.Get_Scalar(ssql);
            

            return sResult;                
        }

        /// <summary>
        /// 判斷是否有相同應用程式執行
        /// </summary>
        /// <returns></returns>
        public void CheckProcess()
        {
            //取得此process的名稱
            String name = Process.GetCurrentProcess().ProcessName;
            //取得所有與目前process名稱相同的process
            Process[] ps = Process.GetProcessesByName(name);
            //ps.Length > 1 表示此proces以重複執行
            if (ps.Length > 1)
            {
                MessageBox.Show("應用程式執行中，請關閉後再執行。" + Environment.NewLine + "若無視窗，請開啟工作管理員關閉應用程式。");
                System.Environment.Exit(2);
            }
        }

        /// <summary>
        /// 完整關閉Socket
        /// </summary>
        /// <param name="s"></param>
        public void CloseSocket(Socket s)
        {
            if (s.Connected)
            {
                s.Shutdown(SocketShutdown.Both);

                s.Disconnect(false);

                s.Close();
            }            
        } 

        public Dictionary<string,string> ConvertConnString(string sConnString)
        {   
            Dictionary<string, string> dConn = new Dictionary<string, string>();

            List<string> lType = sConnString.Split(';').ToList();

            foreach (string sItem in lType)
            {
                List<string> lItem = sItem.Split('=').ToList();

                if (lItem.Count == 2){
                    dConn.Add(lItem[0], lItem[1]);
                }
            }

            return dConn;
        }

        /// <summary>
        /// 限制RichTextBox，顯示行數
        /// </summary>
        /// <param name="rtb">RichTextBox 元件</param>
        /// <param name="iMaxLines">限制顯示行數</param>
        public void LimitLines(RichTextBox rtb,int iMaxLines)
        {   
            //限制行數
            if (rtb.Lines.Length >= iMaxLines)
            {
                List<string> ltxt = rtb.Lines.ToList<string>();
                ltxt.RemoveRange(0, ltxt.Count - iMaxLines);
                rtb.Lines = ltxt.ToArray();
            }
        }


        public string getTitleText(string sOldTitle,string sExePath)
        {
            string sResult = string.Empty;
            string sConnEnv = getHospName();

            //取得執行檔最後修改時間            
            DateTime dt = System.IO.File.GetLastWriteTime(sExePath);            

            sResult = string.Format("{0}[{1}]-版本[{2}]", sOldTitle, sConnEnv, dt.ToString());

            return sResult;
        }

        public string getHospName()
        {
            string sResult = string.Empty;

            //依照connection判斷正式主機還是測試主機
            if (sConnString.Contains("192.168.72.20")) sResult = "測試";
            if (sConnString.Contains("192.168.150.53")) sResult = "長安";


            return sResult;
        }


    }
}
