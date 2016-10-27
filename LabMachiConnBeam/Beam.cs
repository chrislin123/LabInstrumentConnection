using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using LabMachiLib;
using System.Configuration;
using System.Windows.Forms;
using System.ComponentModel;

namespace LabMachiConnBeam
{
    public class Beam
    {
        EzMySQL SQL = new EzMySQL(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
        LabComm oLabComm = new LabComm(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
        string ssql = string.Empty;

        const int CodeSendMsg = 999;
        const int CodeSendErr = 888;
        const int CodeSendStatus = 777;

        //廠商類別
        string sFirmType = "Beam";

        //beam 所負責的2台儀器名稱
        private enum instructmentName
        {
            H7180, HMJACK
        }
        //beam 所負責的2台儀器代碼
        private enum instructmentCode
        {
            H = instructmentName.H7180,
            S = instructmentName.HMJACK,
        }

        public void Start(BackgroundWorker bw)
        {           
            StringBuilder sb = new StringBuilder();
            
            bw.ReportProgress(CodeSendStatus, string.Format("待機中..({0})", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")));
            
            //1.先找出所有沒有傳送的管子
            ssql = "select distinct(chtubeno) from opd.tubemapping where chmachineno in ('" + instructmentCode.H.ToString() + "','" + instructmentCode.S.ToString() + "') and chIsTransmit = 'N' and chischeck != 'C'";

            DataTable receiveDt = new DataTable();
            try
            {
                //預防資料庫無法連線會停機
                receiveDt = SQL.Get_DataTable(ssql);
            }
            catch (Exception ex)
            {
                oLabComm.insertlog(sFirmType, ex.Message);
                bw.ReportProgress(CodeSendErr, "訊息:資料庫連線異常，出現錯誤。");
                //oLabComm.FormMsgShow(errorText, "訊息:資料庫連線異常，出現錯誤。");
                return;
            }
            
            foreach (DataRow dr in receiveDt.Rows)
            {
                try
                {
                    string sTubeNo = dr["chtubeno"].ToString();

                    bw.ReportProgress(CodeSendStatus, string.Format("傳送中..檢體號：{0}({1})", sTubeNo, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")));
                    
                    //傳送資料到middleware
                    SendData(dr);


                    //int iii = int.Parse("");
                    bw.ReportProgress(CodeSendMsg, "檢體號:" + sTubeNo + "訊息:傳送完成。");
                    
                }
                catch (Exception ex)
                {
                    oLabComm.insertlog(sFirmType, ex.Message);
                    bw.ReportProgress(CodeSendErr, "訊息:出現錯誤，請查詢labinstrumentlog。");                    
                }
            }

            //2.報告結果回傳
            ssql = " select * from middleware.labmachineresult where chfinshdttm = '' and chrepvalue != '' ";
            DataTable reportDt = new DataTable();
            try
            {
                //預防資料庫無法連線會停機
                reportDt = SQL.Get_DataTable(ssql);
            }
            catch (Exception ex)
            {
                oLabComm.insertlog(sFirmType, ex.Message);
                bw.ReportProgress(CodeSendErr, "訊息:資料庫連線異常，出現錯誤。");                
            }
            
            foreach (DataRow dr in reportDt.Rows)
            {
                try
                {
                    string sLabemrno = dr["chlabemrno"].ToString();

                    bw.ReportProgress(CodeSendStatus, string.Format("接收中..開單號：{0}({1})", sLabemrno, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")));
                    
                    //接收報告到HIS系統
                    ReceiveData(dr);

                    bw.ReportProgress(CodeSendMsg, "開單號:" + sLabemrno + "訊息:接收完成");                    
                }
                catch (Exception ex)
                {
                    oLabComm.insertlog(sFirmType, ex.Message);
                    bw.ReportProgress(CodeSendErr, "訊息:資料庫連線異常，出現錯誤。");                    
                }
            }    
        }


        /// <summary>
        /// 接收報告到HIS系統
        /// </summary>
        public void ReceiveData(DataRow dr)
        {

            //重點中的重點 labsetaqcnt 中 chamchineno 等於 H S 他們的chmachinemapping chmachineorder 一定要一樣
            ssql = "update opd.labdrep set chrepvalue = '" + dr["chrepvalue"].ToString() + "',chunit='" + dr["chunit"].ToString() + "' where chlabemrno = '" + dr["chlabemrno"].ToString() + "' and intseq = '" + dr["intseq"].ToString() + "' and chmachinemapping = '" + dr["chdiacode"].ToString() + "' and chrepvalue = ''";
            SQL.ExecuteSQL(ssql);

            string chfinshdttm = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");

            //押上完成註記
            ssql = "update middleware.labmachineresult set chfinshdttm = '" + chfinshdttm + "' where chlabemrno = '" + dr["chlabemrno"].ToString() + "' and intseq = '" + dr["intseq"].ToString() + "' and chdiacode = '" + dr["chdiacode"].ToString() + "' and chsampleid = '" + dr["chsampleid"].ToString() + "' and chonmachineid = '" + dr["chonmachineid"].ToString() + "'";
            SQL.ExecuteSQL(ssql);

        }

        /// <summary>
        /// 傳送資料到middleware
        /// </summary>
        public void SendData(DataRow dr)
        {
            StringBuilder sb = new StringBuilder();

            string sTubeNo = dr["chtubeno"].ToString();


            //取得目前時間
            string sNow = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");

            //寫入log檔
            oLabComm.insertlog("Beam", sTubeNo);
            
            //主項
            sb.Clear();
            sb.AppendLine("select lab.chLabEmrNo,lab.intSeq,item.chMachineOrder,'',tube.chtubeno,tube.chemg,'','',lab.chregion,'',item.chUnit,tube.chmachineno,m.chMachineName,'','',''");
            sb.AppendLine("from opd.labemracnt lab");
            sb.AppendLine("inner join opd.tubemapping tube on lab.chlabemrno = tube.chlabemrno and lab.intseq = tube.intseq");
            sb.AppendLine("inner join opd.labsetacnt item on lab.chlabsetid = item.chlabsetid and lab.chdiacode = item.chLabCode");
            sb.AppendLine("inner join opd.labmachine m on tube.chMachineNo = m.chMachineNo");
            sb.AppendLine("where tube.chtubeno = '" + sTubeNo + "'");
            sb.AppendLine("and lab.chdc = 'N' and tube.chMachineNo in ('" + instructmentCode.H.ToString() + "','" + instructmentCode.S.ToString() + "') and chIsTransmit = 'N' and chischeck != 'C'");
            sb.AppendLine("and item.chinreportitem = 'Y'");
            //主項項目
            DataTable masterOrderDt = SQL.Get_DataTable(sb.ToString());

            //塞進middleware 
            foreach (DataRow drMasOrder in masterOrderDt.Rows)
            {

                LabMachineResult oLabMachineResult = new LabMachineResult();
                oLabMachineResult.chLabEmrNo = drMasOrder["chLabEmrNo"].ToString(); //1
                oLabMachineResult.intSeq = drMasOrder["intSeq"].ToString(); //2
                oLabMachineResult.chDiaCode = drMasOrder["chMachineOrder"].ToString(); //3
                oLabMachineResult.chDiaCodeOfMachineNo = ""; //4
                oLabMachineResult.chSampleID = drMasOrder["chtubeno"].ToString(); //5
                oLabMachineResult.chEmg = drMasOrder["chemg"].ToString(); //6
                oLabMachineResult.chRepValue = ""; //7
                oLabMachineResult.chRepValueStatus = "0"; //8
                oLabMachineResult.chSampleType = drMasOrder["chregion"].ToString(); //9
                oLabMachineResult.chDataType = ""; //10
                oLabMachineResult.chUnit = drMasOrder["chUnit"].ToString(); //11
                oLabMachineResult.chCreateDttm = sNow; //12
                oLabMachineResult.chOnMachineId = drMasOrder["chmachineno"].ToString(); //13
                oLabMachineResult.chOnMachineName = drMasOrder["chmachinename"].ToString(); //14
                oLabMachineResult.chOnMachineDttm = ""; //15
                oLabMachineResult.chMachineReturnDttm = ""; //16
                oLabMachineResult.chFinshDttm = ""; //17

                //判斷是否資料已經傳送過
                ssql = " select * from  middleware.labmachineresult ";
                ssql += " where 1=1 ";
                ssql += " and chLabEmrNo = '" + oLabMachineResult.chLabEmrNo + "' ";
                ssql += " and intSeq = '" + oLabMachineResult.intSeq + "' ";
                ssql += " and chDiaCode = '" + oLabMachineResult.chDiaCode + "' ";
                ssql += " and chSampleID = '" + oLabMachineResult.chSampleID + "' ";
                ssql += " and chOnMachineId = '" + oLabMachineResult.chOnMachineId + "' ";
                DataTable dt_temp = new DataTable();
                dt_temp = SQL.Get_DataTable(ssql);
                if (dt_temp.Rows.Count > 0)
                {
                    continue;
                }


                sb.Clear();
                sb.AppendLine(" insert into middleware.labmachineresult ");
                sb.AppendLine(" values( ");
                sb.AppendLine(" '" + oLabMachineResult.chLabEmrNo + "' "); //1
                sb.AppendLine(" ,'" + oLabMachineResult.intSeq + "' ");//2
                sb.AppendLine(" ,'" + oLabMachineResult.chDiaCode + "' ");//3
                sb.AppendLine(" ,'" + oLabMachineResult.chDiaCodeOfMachineNo + "' ");//4
                sb.AppendLine(" ,'" + oLabMachineResult.chSampleID + "' ");//5
                sb.AppendLine(" ,'" + oLabMachineResult.chEmg + "' ");//6
                sb.AppendLine(" ,'" + oLabMachineResult.chRepValue + "' ");//7
                sb.AppendLine(" ,'" + oLabMachineResult.chRepValueStatus + "' ");//8
                sb.AppendLine(" ,'" + oLabMachineResult.chSampleType + "' ");//9
                sb.AppendLine(" ,'" + oLabMachineResult.chDataType + "' ");//10
                sb.AppendLine(" ,'" + oLabMachineResult.chUnit + "' ");//11
                sb.AppendLine(" ,'" + oLabMachineResult.chCreateDttm + "' ");//12
                sb.AppendLine(" ,'" + oLabMachineResult.chOnMachineId + "'  ");//13
                sb.AppendLine(" ,'" + oLabMachineResult.chOnMachineName + "' ");//14
                sb.AppendLine(" ,'" + oLabMachineResult.chOnMachineDttm + "' ");//15
                sb.AppendLine(" ,'" + oLabMachineResult.chMachineReturnDttm + "' ");//16
                sb.AppendLine(" ,'" + oLabMachineResult.chFinshDttm + "' ");//17                            
                sb.AppendLine(" ) ");

                SQL.ExecuteSQL(sb.ToString());
            }

            

            //報告項      
            sb.Clear();         
            sb.AppendLine("select lab.chLabEmrNo,lab.intSeq,item.chMachineOrder,'',tube.chtubeno,tube.chemg,'','',lab.chregion,'',item.chUnit,tube.chmachineno,m.chMachineName,'','',''");
            sb.AppendLine("from opd.labemracnt lab");
            sb.AppendLine("inner join opd.tubemapping tube on lab.chlabemrno = tube.chlabemrno and lab.intseq = tube.intseq");
            sb.AppendLine("inner join opd.labsetattach attach on lab.chlabsetid = attach.chLabSetID and lab.chdiacode = attach.chlabcode");
            sb.AppendLine("inner join opd.labsetacnt item on attach.chCombineSetID = item.chlabsetid and attach.chCombineCode = item.chLabCode");
            sb.AppendLine("inner join opd.labmachine m on tube.chMachineNo = m.chMachineNo");
            sb.AppendLine("where tube.chtubeno = '" + sTubeNo + "'");
            sb.AppendLine("and lab.chdc = 'N' and attach.chdc = 'N' and tube.chMachineNo in ('" + instructmentCode.H.ToString() + "','" + instructmentCode.S.ToString() + "') and chIsTransmit = 'N' and chischeck != 'C'");

            //報告項目展開
            DataTable detailOrderDt = SQL.Get_DataTable(sb.ToString());

            //塞進middleware 
            foreach (DataRow drMasOrder in detailOrderDt.Rows)
            {
                LabMachineResult oLabMachineResult = new LabMachineResult();
                oLabMachineResult.chLabEmrNo = drMasOrder["chLabEmrNo"].ToString(); //1
                oLabMachineResult.intSeq = drMasOrder["intSeq"].ToString(); //2
                oLabMachineResult.chDiaCode = drMasOrder["chMachineOrder"].ToString(); //3
                oLabMachineResult.chDiaCodeOfMachineNo = ""; //4
                oLabMachineResult.chSampleID = drMasOrder["chtubeno"].ToString(); //5
                oLabMachineResult.chEmg = drMasOrder["chemg"].ToString(); //6
                oLabMachineResult.chRepValue = ""; //7
                oLabMachineResult.chRepValueStatus = "0"; //8
                oLabMachineResult.chSampleType = drMasOrder["chregion"].ToString(); //9
                oLabMachineResult.chDataType = ""; //10
                oLabMachineResult.chUnit = drMasOrder["chUnit"].ToString(); //11
                oLabMachineResult.chCreateDttm = sNow; //12
                oLabMachineResult.chOnMachineId = drMasOrder["chmachineno"].ToString(); //13
                oLabMachineResult.chOnMachineName = drMasOrder["chmachinename"].ToString(); //14
                oLabMachineResult.chOnMachineDttm = ""; //15
                oLabMachineResult.chMachineReturnDttm = ""; //16
                oLabMachineResult.chFinshDttm = ""; //17

                //判斷是否資料已經傳送過
                ssql = " select * from  middleware.labmachineresult ";
                ssql += " where 1=1 ";
                ssql += " and chLabEmrNo = '" + oLabMachineResult.chLabEmrNo + "' ";
                ssql += " and intSeq = '" + oLabMachineResult.intSeq + "' ";
                ssql += " and chDiaCode = '" + oLabMachineResult.chDiaCode + "' ";
                ssql += " and chSampleID = '" + oLabMachineResult.chSampleID + "' ";
                ssql += " and chOnMachineId = '" + oLabMachineResult.chOnMachineId + "' ";

                DataTable dt_temp = new DataTable();
                dt_temp = SQL.Get_DataTable(ssql);
                if (dt_temp.Rows.Count > 0)
                {
                    continue;
                }

                sb.Clear();
                sb.AppendLine(" insert into middleware.labmachineresult ");
                sb.AppendLine(" values( ");
                sb.AppendLine(" '" + oLabMachineResult.chLabEmrNo + "' "); //1
                sb.AppendLine(" ,'" + oLabMachineResult.intSeq + "' ");//2
                sb.AppendLine(" ,'" + oLabMachineResult.chDiaCode + "' ");//3
                sb.AppendLine(" ,'" + oLabMachineResult.chDiaCodeOfMachineNo + "' ");//4
                sb.AppendLine(" ,'" + oLabMachineResult.chSampleID + "' ");//5
                sb.AppendLine(" ,'" + oLabMachineResult.chEmg + "' ");//6
                sb.AppendLine(" ,'" + oLabMachineResult.chRepValue + "' ");//7
                sb.AppendLine(" ,'" + oLabMachineResult.chRepValueStatus + "' ");//8
                sb.AppendLine(" ,'" + oLabMachineResult.chSampleType + "' ");//9
                sb.AppendLine(" ,'" + oLabMachineResult.chDataType + "' ");//10
                sb.AppendLine(" ,'" + oLabMachineResult.chUnit + "' ");//11
                sb.AppendLine(" ,'" + oLabMachineResult.chCreateDttm + "' ");//12
                sb.AppendLine(" ,'" + oLabMachineResult.chOnMachineId + "'  ");//13
                sb.AppendLine(" ,'" + oLabMachineResult.chOnMachineName + "' ");//14
                sb.AppendLine(" ,'" + oLabMachineResult.chOnMachineDttm + "' ");//15
                sb.AppendLine(" ,'" + oLabMachineResult.chMachineReturnDttm + "' ");//16
                sb.AppendLine(" ,'" + oLabMachineResult.chFinshDttm + "' ");//17                            
                sb.AppendLine(" ) ");

                SQL.ExecuteSQL(sb.ToString());
            }

            sb.Clear();

            //壓已傳送
            ssql = "update opd.tubemapping set chIsTransmit = 'Y',chTransmitTime ='" + sNow + "' where chtubeno = '" + sTubeNo + "'";
            SQL.ExecuteSQL(ssql);
          

        }

    }
}
