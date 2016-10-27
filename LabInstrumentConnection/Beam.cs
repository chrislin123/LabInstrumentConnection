using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LabInstrumentConnection
{
    public class Beam
    {
        LabComm oLabComm = new LabComm();
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

        public void Start(System.Windows.Forms.RichTextBox msgText, System.Windows.Forms.RichTextBox errorText)
        {
            
            while (true)
            {
                try
                {
                    //10秒執行一次
                    Thread.Sleep(3000);

                    //1.先送ORDER
                    EzMySQL SQL = new EzMySQL();
                    StringBuilder sb = new StringBuilder();
                    //先找出所有沒有傳送的管子
                    DataTable receiveDt = SQL.Get_DataTable("select distinct(chtubeno) from opd.tubemapping where chmachineno in ('" + instructmentCode.H.ToString() + "','" + instructmentCode.S.ToString() + "') and chIsTransmit = 'N' and chischeck != 'C'");
                    //準備傳送到middleware table
                    if (receiveDt.Rows.Count > 0)
                    {
                        for (int i = 0; i < receiveDt.Rows.Count; i++)
                        {
                            try
                            {
                                string now = DateTime.Now.Year.ToString() + "/" + DateTime.Now.Month.ToString().PadLeft(2, '0') + "/" + DateTime.Now.Day.ToString().PadLeft(2, '0') + " " + DateTime.Now.Hour.ToString().PadLeft(2, '0') + ":" + DateTime.Now.Minute.ToString().PadLeft(2, '0') + ":" + DateTime.Now.Second.ToString().PadLeft(2, '0');

                                SQL.ExecuteSQL("insert into opd.labinstrumentlog (type,log,time) values('Beam','" + receiveDt.Rows[i]["chtubeno"].ToString() + "','" + DateTime.Now.ToLongDateString() + DateTime.Now.ToLongTimeString() + "')");

                                sb.Clear();
                                //主項
                                sb.AppendLine("select lab.chLabEmrNo,lab.intSeq,item.chMachineOrder,'',tube.chtubeno,tube.chemg,'','',lab.chregion,'',item.chUnit,'" + now + "' as inserttime,tube.chmachineno,m.chMachineName,'','',''");
                                sb.AppendLine("from opd.labemracnt lab");
                                sb.AppendLine("inner join opd.tubemapping tube on lab.chlabemrno = tube.chlabemrno and lab.intseq = tube.intseq");
                                sb.AppendLine("inner join opd.labsetacnt item on lab.chlabsetid = item.chlabsetid and lab.chdiacode = item.chLabCode");
                                sb.AppendLine("inner join opd.labmachine m on tube.chMachineNo = m.chMachineNo");
                                sb.AppendLine("where tube.chtubeno = '" + receiveDt.Rows[i]["chtubeno"].ToString() + "'");
                                sb.AppendLine("and lab.chdc = 'N' and tube.chMachineNo in ('" + instructmentCode.H.ToString() + "','" + instructmentCode.S.ToString() + "') and chIsTransmit = 'N' and chischeck != 'C'");
                                sb.AppendLine("and item.chinreportitem = 'Y'");
                                //主項項目
                                DataTable masterOrderDt = SQL.Get_DataTable(sb.ToString());

                                //塞進middleware 
                                for (int j = 0; j < masterOrderDt.Rows.Count; j++)
                                {
                                    sb.Clear();
                                    sb.AppendLine("insert into middleware.labmachineresult");
                                    sb.AppendLine("values('" + masterOrderDt.Rows[j]["chLabEmrNo"].ToString() + "','" + masterOrderDt.Rows[j]["intSeq"].ToString() + "','" + masterOrderDt.Rows[j]["chMachineOrder"].ToString() + "','','" + masterOrderDt.Rows[j]["chtubeno"].ToString() + "','" + masterOrderDt.Rows[j]["chemg"].ToString() + "','','','" + masterOrderDt.Rows[j]["chregion"].ToString() + "','','" + masterOrderDt.Rows[j]["chUnit"].ToString() + "','" + masterOrderDt.Rows[j]["inserttime"].ToString() + "','" + masterOrderDt.Rows[j]["chmachineno"].ToString() + "','" + masterOrderDt.Rows[j]["chmachinename"].ToString() + "','','','')");
                                    SQL.ExecuteSQL(sb.ToString());
                                }


                                sb.Clear();

                                //報告項                           
                                sb.AppendLine("select lab.chLabEmrNo,lab.intSeq,item.chMachineOrder,'',tube.chtubeno,tube.chemg,'','',lab.chregion,'',item.chUnit,'" + now + "' as inserttime,tube.chmachineno,m.chMachineName,'','',''");
                                sb.AppendLine("from opd.labemracnt lab");
                                sb.AppendLine("inner join opd.tubemapping tube on lab.chlabemrno = tube.chlabemrno and lab.intseq = tube.intseq");
                                sb.AppendLine("inner join opd.labsetattach attach on lab.chlabsetid = attach.chLabSetID and lab.chdiacode = attach.chlabcode");
                                sb.AppendLine("inner join opd.labsetacnt item on attach.chCombineSetID = item.chlabsetid and attach.chCombineCode = item.chLabCode");
                                sb.AppendLine("inner join opd.labmachine m on tube.chMachineNo = m.chMachineNo");
                                sb.AppendLine("where tube.chtubeno = '" + receiveDt.Rows[i]["chtubeno"].ToString() + "'");
                                sb.AppendLine("and lab.chdc = 'N' and attach.chdc = 'N' and tube.chMachineNo in ('" + instructmentCode.H.ToString() + "','" + instructmentCode.S.ToString() + "') and chIsTransmit = 'N' and chischeck != 'C'");

                                //報告項目展開
                                DataTable detailOrderDt = SQL.Get_DataTable(sb.ToString());

                                //塞進middleware 
                                for (int j = 0; j < detailOrderDt.Rows.Count; j++)
                                {
                                    sb.Clear();
                                    sb.AppendLine("insert into middleware.labmachineresult");
                                    sb.AppendLine("values('" + detailOrderDt.Rows[j]["chLabEmrNo"].ToString() + "','" + detailOrderDt.Rows[j]["intSeq"].ToString() + "','" + detailOrderDt.Rows[j]["chMachineOrder"].ToString() + "','','" + detailOrderDt.Rows[j]["chtubeno"].ToString() + "','" + detailOrderDt.Rows[j]["chemg"].ToString() + "','','','" + detailOrderDt.Rows[j]["chregion"].ToString() + "','','" + detailOrderDt.Rows[j]["chUnit"].ToString() + "','" + detailOrderDt.Rows[j]["inserttime"].ToString() + "','" + detailOrderDt.Rows[j]["chmachineno"].ToString() + "','" + detailOrderDt.Rows[j]["chmachinename"].ToString() + "','','','')");
                                    SQL.ExecuteSQL(sb.ToString());
                                }

                                sb.Clear();

                                //壓已傳送
                                SQL.ExecuteSQL("update opd.tubemapping set chIsTransmit = 'Y',chTransmitTime ='" + now + "' where chtubeno = '" + receiveDt.Rows[i]["chtubeno"].ToString() + "'");

                                msgText.AppendText("時間:" + DateTime.Now.ToShortTimeString() + " 檢體號:" + receiveDt.Rows[i]["chtubeno"].ToString() + "訊息:" + "傳送完成" + "。" + "\r\n");


                            }
                            catch (Exception ex)
                            {
                                oLabComm.insertlog(sFirmType, ex.Message);
                                errorText.AppendText("時間:" + DateTime.Now.ToShortTimeString() + "訊息:" + "出現錯誤，請查詢labinstrumentlog" + "。" + "\r\n");
                            }
                        }
                    }


                    //2.收Result
                    sb.Clear();
                    sb.AppendLine("select * from middleware.labmachineresult where chfinshdttm = '' and chrepvalue != ''");

                    //報告結果回傳
                    DataTable reportDt = SQL.Get_DataTable(sb.ToString());
                    if (reportDt.Rows.Count > 0)
                    {
                        for (int i = 0; i < reportDt.Rows.Count; i++)
                        {
                            try
                            {
                                //回塞檢驗結果(等廠商測試完成再打開，不然真的會炸)
                                //重點中的重點 labsetaqcnt 中 chamchineno 等於 H S 他們的chmachinemapping chmachineorder 一定要一樣
                                SQL.ExecuteSQL("update opd.labdrep set chrepvalue = '" + reportDt.Rows[i]["chrepvalue"].ToString() + "',chunit='" + reportDt.Rows[i]["chunit"].ToString() + "' where chlabemrno = '" + reportDt.Rows[i]["chlabemrno"].ToString() + "' and intseq = '" + reportDt.Rows[i]["intseq"].ToString() + "' and chmachinemapping = '" + reportDt.Rows[i]["chdiacode"].ToString() + "' and chrepvalue = ''");
                                //SQL.ExecuteSQL(sb.ToString());
                                string chfinshdttm = DateTime.Now.Year.ToString() + "/" + DateTime.Now.Month.ToString().PadLeft(2, '0') + "/" + DateTime.Now.Day.ToString().PadLeft(2, '0') + " " + DateTime.Now.Hour.ToString().PadLeft(2, '0') + ":" + DateTime.Now.Minute.ToString().PadLeft(2, '0') + ":" + DateTime.Now.Second.ToString().PadLeft(2, '0');
                                //押上完成註記
                                SQL.ExecuteSQL("update middleware.labmachineresult set chfinshdttm = '" + chfinshdttm + "' where chlabemrno = '" + reportDt.Rows[i]["chlabemrno"].ToString() + "' and intseq = '" + reportDt.Rows[i]["intseq"].ToString() + "' and chdiacode = '" + reportDt.Rows[i]["chdiacode"].ToString() + "' and chsampleid = '" + reportDt.Rows[i]["chsampleid"].ToString() + "' and chonmachineid = '" + reportDt.Rows[i]["chonmachineid"].ToString() + "'");
                                //SQL.ExecuteSQL(sb.ToString());

                                msgText.AppendText("時間:" + DateTime.Now.ToShortTimeString() + " 開單號:" + reportDt.Rows[i]["chlabemrno"].ToString() + "訊息:" + "接收完成" + "。" + "\r\n");

                            }
                            catch (Exception ex)
                            {
                                oLabComm.insertlog(sFirmType, ex.Message);
                                errorText.AppendText("時間:" + DateTime.Now.ToShortTimeString() + "訊息:" + "出現錯誤，請查詢labinstrumentlog" + "。" + "\r\n");
                            }
                        }
                    }

                }
                catch (Exception ex)
                {
                    //EzMySQL SQL = new EzMySQL();
                    //SQL.ExecuteSQL("insert into opd.labinstrumentlog (type,log,time) values('Beam','" + ex.Message + "','" + DateTime.Now.ToLongDateString() + DateTime.Now.ToLongTimeString() + "')");
                    //1050419 log元件化
                    oLabComm.insertlog(sFirmType, ex.Message);
                    errorText.AppendText("時間:" + DateTime.Now.ToShortTimeString() + "訊息:" + "出現錯誤，請查詢labinstrumentlog" + "。" + "\r\n");
                }
            }
        }
    }
}
