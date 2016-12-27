using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using LabMachiLib;
using System.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace LabMachiConnHCLAB
{
    public class HCLABSend
    {
        LabComm oLabComm = new LabComm(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
        EzMySQL SQL = new EzMySQL(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
        
        //廠商類別
        string sFirmType = "HCLABSend";
        //hclab 所負責的4台儀器名稱
        private enum instructmentName
        {
            CA560, XN2000, HA8180, AX4030
        }
        //hclab 所負責的4台儀器代碼
        private enum instructmentCode
        {
            P = instructmentName.CA560,
            X = instructmentName.XN2000,
            G = instructmentName.HA8180,
            U = instructmentName.AX4030
        }

        Socket _mySocket;
        int _port = 0;
        bool closeSymbol = false;
        /// <summary>
        /// HCLAB為Socket Client 設計因此在New時不須指定PORT,而是看資料庫設定，PORT資料存在DB中
        /// </summary>
        /// <param name="port"></param>
        public HCLABSend(int port = 0)
        {
            //_port = port;
        }
        public void Start(System.Windows.Forms.RichTextBox msgText, System.Windows.Forms.RichTextBox errorText)
        {
            closeSymbol = false;
            //EzMySQL SQL = new EzMySQL();
            string tubeError = "";

            //HCLAB IP
            string host = SQL.Get_Scalar("select chparamtext from basedata.systemctrl where chparamcode = 'HCLABIP'");
            //string host = "127.0.0.1";
            //HCLAB ORDER Port
            int port = int.Parse(SQL.Get_Scalar("select chparamtext from basedata.systemctrl where chparamcode = 'HCLABOrderPort'"));
            //int port = 8081;

            //byte[] STX = Encoding.GetEncoding("Big5").GetBytes("\u0002");
            //byte[] ETX = Encoding.GetEncoding("Big5").GetBytes("\u0003");
            //byte[] EOT = Encoding.GetEncoding("Big5").GetBytes("\u0004");
            //byte[] ENQ = Encoding.GetEncoding("Big5").GetBytes("\u0005");
            //byte[] CR = Encoding.GetEncoding("Big5").GetBytes("\r");
            //byte[] ACK = Encoding.GetEncoding("Big5").GetBytes("\u0006");
            //byte[] NAK = Encoding.GetEncoding("Big5").GetBytes("\u0015");

            IPAddress[] IPs = Dns.GetHostAddresses(host);

            try
            {
                while (true)
                {
                    if (closeSymbol == true)
                    {
                        _mySocket.Close();
                        break;
                    }
                    try
                    {
                        //3秒執行一次
                        Thread.Sleep(3000);
                        DataTable dt = SQL.Get_DataTable("select distinct(chtubeno),chlabemrno,chemg from opd.tubemapping where chIsTransmit = 'N' and chischeck != 'C' and chMachineNo in ('" + instructmentCode.P.ToString() + "','" + instructmentCode.X.ToString() + "','" + instructmentCode.G.ToString() + "','" + instructmentCode.U.ToString() + "') ");
                        //DataTable dt = SQL.Get_DataTable("select distinct(chtubeno),chlabemrno,chemg from opd.tubemapping where chTubeNo = 'ZBI0006622' and chIsTransmit = 'N' and chischeck != 'C' and chMachineNo in ('" + instructmentCode.P.ToString() + "','" + instructmentCode.X.ToString() + "','" + instructmentCode.G.ToString() + "','" + instructmentCode.U.ToString() + "') ");
                        //確定有檔案需要執行 再開socket
                        if (dt.Rows.Count > 0)
                        {
                            for (int i = 0; i < dt.Rows.Count; i++)
                            {
                                if (dt.Rows[i]["chtubeno"].ToString() == "ZRH0070683")
                                {
                                    string ssss = "";
                                }

                                string chlabkind = SQL.Get_Scalar("select chlabkind from opd.ptlabemr where chlabemrno = '" + dt.Rows[i]["chlabemrno"].ToString() + "'");
                                if (chlabkind != "OPD")
                                {
                                    //沒有簽收就不送middleware
                                    string count = SQL.Get_Scalar("select count(1) as count from opd.tubemapping a inner join opd.labemracnt b on a.chlabemrno = b.chlabemrno and a.intseq = b.intseq where chtubeno = '" + dt.Rows[i]["chtubeno"].ToString() + "' and b.intlabdegree!=5");
                                    if (count != "0") { continue; }
                                }
                                else
                                {
                                    //沒有採集就不送middleware
                                    string count = SQL.Get_Scalar("select count(1) as count from opd.tubemapping a inner join opd.labemracnt b on a.chlabemrno = b.chlabemrno and a.intseq = b.intseq where chtubeno = '" + dt.Rows[i]["chtubeno"].ToString() + "' and b.intlabdegree=4");
                                    if (count == "0") { continue; }

                                }

                                _mySocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                                _mySocket.Connect(IPs[0], port);

                                //1. 先送EOT 再送==>ENQ
                                tubeError = dt.Rows[i]["chtubeno"].ToString();
                                _mySocket.Send(Encoding.GetEncoding("Big5").GetBytes("\u0004"));
                                _mySocket.Send(Encoding.GetEncoding("Big5").GetBytes("\u0005"));
                                //<==ACK
                                if (!receiveMsgSuccess())
                                {
                                    errorText.AppendText("時間:" + DateTime.Now.ToShortTimeString() + " 1.EOT 傳送錯誤 " + "\r\n");
                                    errorText.ScrollToCaret();
                                    continue;
                                };

                                //2.==> Header   
                                //檢查碼
                                string checkSum = CalculateChecksumChinese("1H|^~\\&|||||||||||1" + "\r\u0003");
                                _mySocket.Send(Encoding.GetEncoding("Big5").GetBytes("\u0002" + "1H|^~\\&|||||||||||1" + "\r\u0003" + checkSum + "\r\n"));
                                SQL.ExecuteSQL("insert into opd.labinstrumentlog (type,log,time) values('HCLABSend','" + "1H|^~\\&|||||||||||1" + "\r\u0003" + checkSum + "','" + DateTime.Now.ToLongDateString() + DateTime.Now.ToLongTimeString() + "')");

                                //<==ACK
                                if (!receiveMsgSuccess())
                                {
                                    errorText.AppendText("時間:" + DateTime.Now.ToShortTimeString() + " 2.Header 傳送錯誤 開單號:" + dt.Rows[i]["chtubeno"].ToString() + " 標籤號:" + dt.Rows[i]["chtubeno"].ToString() + "。" + "\r\n");
                                    errorText.ScrollToCaret();
                                    continue;
                                };

                                //3.==> patient   
                                //病歷號
                                string chmrno = SQL.Get_Scalar("select chmrno from opd.ptlabemr where chlabemrno = '" + dt.Rows[i]["chlabemrno"].ToString() + "'");
                                //病患資料
                                DataTable patientDT = SQL.Get_DataTable("select * from basedata.patientdata where chmrno = '" + chmrno + "'");
                                //性別轉換
                                string sex = sexConvert(patientDT.Rows[0]["chsex"].ToString());
                                //當天時間
                                string today = DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString().PadLeft(2, '0') + DateTime.Now.Day.ToString().PadLeft(2, '0');
                                //開單醫師
                                string dr = get_apply_dr(dt.Rows[i]["chlabemrno"].ToString());
                                //單別
                                string kind = get_apply_kind(dt.Rows[i]["chlabemrno"].ToString());
                                //地點
                                string location = get_apply_location(dt.Rows[i]["chlabemrno"].ToString());
                                //檢查碼
                                checkSum = CalculateChecksumChinese("2P|1|" + patientDT.Rows[0]["chmrno"].ToString() + "|||" + patientDT.Rows[0]["chname"].ToString() + "||" + patientDT.Rows[0]["chbirthday"].ToString() + "|" + sex + "|||||" + dr + "|||0.0|0.0|||||||" + kind + "|" + location + "|||||||" + today + "||" + "\r\u0003");
                                _mySocket.Send(Encoding.GetEncoding("Big5").GetBytes("\u0002" + "2P|1|" + patientDT.Rows[0]["chmrno"].ToString() + "|||" + patientDT.Rows[0]["chname"].ToString() + "||" + patientDT.Rows[0]["chbirthday"].ToString() + "|" + sex + "|||||" + dr + "|||0.0|0.0|||||||" + kind + "|" + location + "|||||||" + today + "||" + "\r\u0003" + checkSum + "\r\n"));
                                SQL.ExecuteSQL("insert into opd.labinstrumentlog (type,log,time) values('HCLABSend','" + "2P|1|" + patientDT.Rows[0]["chmrno"].ToString() + "|||" + patientDT.Rows[0]["chname"].ToString() + "||" + patientDT.Rows[0]["chbirthday"].ToString() + "|" + sex + "|||||" + dr + "|||0.0|0.0|||||||" + kind + "|" + location + "|||||||" + today + "||" + "\r\u0003" + checkSum + "','" + DateTime.Now.ToLongDateString() + DateTime.Now.ToLongTimeString() + "')");
                                //<==ACK
                                if (!receiveMsgSuccess())
                                {
                                    errorText.AppendText("時間:" + DateTime.Now.ToShortTimeString() + " 3.Patient 傳送錯誤 開單號:" + dt.Rows[i]["chtubeno"].ToString() + " 標籤號:" + dt.Rows[i]["chtubeno"].ToString() + "。" + "\r\n");
                                    errorText.ScrollToCaret();
                                    continue;
                                };
                                //4.==> ORDER (先CANCEL單，前人經驗)    
                                string order = getLabItemString(dt.Rows[i]["chtubeno"].ToString());
                                string samplecollectiondt = DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString().PadLeft(2, '0') + DateTime.Now.Day.ToString().PadLeft(2, '0') + DateTime.Now.Hour.ToString().PadLeft(2, '0') + DateTime.Now.Minute.ToString().PadLeft(2, '0') + DateTime.Now.Second.ToString().PadLeft(2, '0');//採檢時間一律以現在時間為準
                                string chemg = convertOrderStatus(dt.Rows[i]["chemg"].ToString());
                                //檢查碼
                                checkSum = CalculateChecksumChinese("3OBR|1|" + dt.Rows[i]["chtubeno"].ToString() + "||" + order + "|" + chemg + "||" + samplecollectiondt + "||||C|||" + samplecollectiondt + "|||||||||||||" + "\r\u0003");
                                _mySocket.Send(Encoding.GetEncoding("Big5").GetBytes("\u0002" + "3OBR|1|" + dt.Rows[i]["chtubeno"].ToString() + "||" + order + "|S||" + samplecollectiondt + "||||C|||" + samplecollectiondt + "|||||||||||||" + "\r\u0003" + checkSum + "\r\n"));
                                SQL.ExecuteSQL("insert into opd.labinstrumentlog (type,log,time) values('HCLABSend','" + "3OBR|1|" + dt.Rows[i]["chtubeno"].ToString() + "||" + order + "|S||" + samplecollectiondt + "||||C|||" + samplecollectiondt + "|||||||||||||" + "','" + DateTime.Now.ToLongDateString() + DateTime.Now.ToLongTimeString() + "')");
                                //<==ACK
                                if (!receiveMsgSuccess())
                                {
                                    errorText.AppendText("時間:" + DateTime.Now.ToShortTimeString() + " 4.ORDER CANCEL 傳送錯誤 開單號:" + dt.Rows[i]["chtubeno"].ToString() + " 標籤號:" + dt.Rows[i]["chtubeno"].ToString() + "。" + "\r\n");
                                    errorText.ScrollToCaret();
                                    continue;
                                };

                                //4.==> ORDER (再加單)    
                                order = getLabItemString(dt.Rows[i]["chtubeno"].ToString());
                                //samplecollectiondt = DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString().PadLeft(2, '0') + DateTime.Now.Day.ToString().PadLeft(2, '0') + DateTime.Now.Hour.ToString().PadLeft(2, '0') + DateTime.Now.Minute.ToString().PadLeft(2, '0') + DateTime.Now.Second.ToString().PadLeft(2, '0');//採檢時間一律以現在時間為準
                                //檢查碼
                                checkSum = CalculateChecksumChinese("4OBR|2|" + dt.Rows[i]["chtubeno"].ToString() + "||" + order + "|" + chemg + "||" + samplecollectiondt + "||||A|||" + samplecollectiondt + "|||||||||||||" + "\r\u0003");
                                _mySocket.Send(Encoding.GetEncoding("Big5").GetBytes("\u0002" + "4OBR|2|" + dt.Rows[i]["chtubeno"].ToString() + "||" + order + "|S||" + samplecollectiondt + "||||A|||" + samplecollectiondt + "|||||||||||||" + "\r\u0003" + checkSum + "\r\n"));
                                SQL.ExecuteSQL("insert into opd.labinstrumentlog (type,log,time) values('HCLABSend','" + "4OBR|2|" + dt.Rows[i]["chtubeno"].ToString() + "||" + order + "|S||" + samplecollectiondt + "||||A|||" + samplecollectiondt + "|||||||||||||" + "','" + DateTime.Now.ToLongDateString() + DateTime.Now.ToLongTimeString() + "')");

                                //<==ACK
                                if (!receiveMsgSuccess())
                                {
                                    errorText.AppendText("時間:" + DateTime.Now.ToShortTimeString() + " 4.ORDER ADD 傳送錯誤 開單號:" + dt.Rows[i]["chtubeno"].ToString() + " 標籤號:" + dt.Rows[i]["chtubeno"].ToString() + "。" + "\r\n");
                                    errorText.ScrollToCaret();
                                    continue;
                                };
                                //檢查碼
                                checkSum = CalculateChecksumChinese("5L|1||1|4\r\u0003");

                                //5.==>結束符號 5L|1||2|8
                                _mySocket.Send(Encoding.GetEncoding("Big5").GetBytes("\u0002" + "5L|1||1|4\r\u0003" + checkSum + "\r\n"));
                                SQL.ExecuteSQL("insert into opd.labinstrumentlog (type,log,time) values('HCLABSend','" + "5L|1||1|4\r\u0003" + checkSum + "','" + DateTime.Now.ToLongDateString() + DateTime.Now.ToLongTimeString() + "')");
                                //<==ACK
                                if (!receiveMsgSuccess())
                                {
                                    errorText.AppendText("時間:" + DateTime.Now.ToShortTimeString() + " 5.End Mark 傳送錯誤 開單號:" + dt.Rows[i]["chtubeno"].ToString() + " 標籤號:" + dt.Rows[i]["chtubeno"].ToString() + "。" + "\r\n");
                                    errorText.ScrollToCaret();
                                    continue;
                                };
                                //結束符號 EOT
                                _mySocket.Send(Encoding.GetEncoding("Big5").GetBytes("\u0004"));
                                //處理完畢後，刪除所有處理資料,並斷開連結
                                _mySocket.Close();

                                //msgText.AppendText("傳送時間:" + DateTime.Now.Year.ToString() + "年" + DateTime.Now.Month.ToString().PadLeft(2, '0') + "月" + DateTime.Now.Day.ToString().PadLeft(2, '0') + "日" + DateTime.Now.Hour.ToString().PadLeft(2, '0') + "時" + DateTime.Now.Minute.ToString().PadLeft(2, '0') + "分" + DateTime.Now.Second.ToString().PadLeft(2, '0') + "秒" + "     " + "標籤號:" + dt.Rows[i]["chtubeno"].ToString() + "傳送成功。" + "\r\n");
                                msgText.ScrollToCaret();
                                //將標籤號註記已上傳
                                SQL.ExecuteSQL("update opd.tubemapping set chistransmit = 'Y', chtransmittime = '" + samplecollectiondt + "' where chtubeno = '" + dt.Rows[i]["chtubeno"].ToString() + "'");
                            }

                        }
                    }
                    catch (Exception ex)
                    {
                        errorText.AppendText("時間:" + DateTime.Now.ToShortTimeString() + " Exception 傳送錯誤: 標籤號:" + tubeError + "訊息" + ex.Message + "。" + "\r\n");
                        errorText.ScrollToCaret();
                        //_mySocket.Send(Encoding.GetEncoding("Big5").GetBytes("\u0004"));

                        //SQL.ExecuteSQL("insert into opd.labinstrumentlog (type,log,time) values('HCLABSend','" + ex.Message + "','" + DateTime.Now.ToLongDateString() + DateTime.Now.ToLongTimeString() + "')");
                        oLabComm.insertlog(sFirmType, ex.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                errorText.AppendText("時間:" + DateTime.Now.ToShortTimeString() + " Exception 傳送錯誤: 標籤號:" + tubeError + "。" + "訊息" + ex.Message + "\r\n");
                errorText.ScrollToCaret();
                //_mySocket.Send(Encoding.GetEncoding("Big5").GetBytes("\u0004"));
                //SQL.ExecuteSQL("insert into opd.labinstrumentlog (type,log,time) values('HCLABSend','" + ex.Message + "','" + DateTime.Now.ToLongDateString() + DateTime.Now.ToLongTimeString() + "')");
                oLabComm.insertlog(sFirmType, ex.Message);
            }
        }


        public void StartNew(BackgroundWorker bw)
        {
            closeSymbol = false;
            
            string chTubeNo = "";

            //1051223 配合新系統改寫，寫死資料            
            //HCLAB IP            
            string host = "192.168.12.25";
            //HCLAB ORDER Port                    
            int port = 9081;           

            IPAddress[] IPs = Dns.GetHostAddresses(host);

            try
            {
                while (true)
                {
                    if (closeSymbol == true)
                    {
                        _mySocket.Close();
                        break;
                    }
                    try
                    {
                        //3秒執行一次
                        Thread.Sleep(3000);

                        //New系統 取得清單資料
                        string sResp = oLabComm.GetListTodo();

                        JObject jo1 = JObject.Parse(sResp);
                        JObject joValue = JObject.Parse(jo1.GetValue("value").ToString());
                        dynamic dynList = joValue as dynamic;
                        var ary2 = ((JArray)dynList.list).Cast<dynamic>().ToArray();

                        //篩選該儀器分類的檢體號
                        //P = instructmentName.CA560,
                        //X = instructmentName.XN2000,
                        //G = instructmentName.HA8180,
                        //U = instructmentName.AX4030


                        if (ary2.Length > 0)
                        {
                            List<string> listTemp = new List<string>();

                            foreach (JObject item in ary2)
                            {
                                dynamic dynItem = item as dynamic;
                                string sMachineCode = dynItem.machineCode;
                                if (sMachineCode == "P" ||sMachineCode == "X" ||sMachineCode == "G" ||sMachineCode == "U")
                                {
                                    listTemp.Add(dynItem.machineCode);
                                }
                            }

                            //去除重複
                            List<string> listTube = listTemp.Distinct().ToList();

                            //依照檢體號傳送資料
                            foreach (string tubeno in listTube)
                            {
                                chTubeNo = tubeno;
                                //病歷號
                                string chMrno = "";                                
                                //性別轉換
                                string sex = "";
                                //當天時間
                                string today = DateTime.Now.ToString("yyyyMMdd");
                                //開單醫師
                                string dr = "";
                                //單別
                                string kind = "";
                                //地點
                                string location = "";
                                //病患姓名
                                string chname = "";
                                //生日
                                string chbirthday = "";
                                //是否急件
                                string sEmg = "";
                                //檢體儀器項目
                                string order = getLabItemStringNew(tubeno, ary2);

                                //取得檢體基本資料基本資料
                                foreach (dynamic item in ary2)
                                {
                                    if (item.tubeNo == tubeno)
                                    {
                                        //病歷號
                                        chMrno = item.resumeNo;                                        
                                        //性別轉換
                                        sex = sexConvert(item.gender);                                       
                                        //開單醫師
                                        dr = item.docName;
                                        //單別
                                        kind = get_apply_kind(item.dept);
                                        //地點
                                        location = item.roomNo;
                                        break;
                                    }
                                }


                                
                                //與儀器溝通
                                _mySocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                                _mySocket.Connect(IPs[0], port);

                                //1. 先送EOT 再送==>ENQ                            
                                _mySocket.Send(Encoding.GetEncoding("Big5").GetBytes("\u0004"));
                                _mySocket.Send(Encoding.GetEncoding("Big5").GetBytes("\u0005"));
                                //<==ACK
                                if (!receiveMsgSuccess())
                                {
                                    oLabComm.FeedbackMsg(bw, msgtype.msgGeneral, string.Format("1.EOT 傳送錯誤。"));
                                    continue;
                                };

                                //2.==> Header   
                                //檢查碼
                                string checkSum = CalculateChecksumChinese("1H|^~\\&|||||||||||1" + "\r\u0003");
                                _mySocket.Send(Encoding.GetEncoding("Big5").GetBytes("\u0002" + "1H|^~\\&|||||||||||1" + "\r\u0003" + checkSum + "\r\n"));
                                SQL.ExecuteSQL("insert into opd.labinstrumentlog (type,log,time) values('HCLABSend','" + "1H|^~\\&|||||||||||1" + "\r\u0003" + checkSum + "','" + DateTime.Now.ToLongDateString() + DateTime.Now.ToLongTimeString() + "')");

                                //<==ACK
                                if (!receiveMsgSuccess())
                                {
                                    oLabComm.FeedbackMsg(bw, msgtype.msgGeneral, string.Format("2.Header 傳送錯誤 開單號:{0} 標籤號:{1}。", chTubeNo, chTubeNo));
                                    continue;
                                };

                                //3.==> patient   
                                //檢查碼
                                checkSum = CalculateChecksumChinese("2P|1|" + chMrno + "|||" + chname + "||" + chbirthday + "|" + sex + "|||||" + dr + "|||0.0|0.0|||||||" + kind + "|" + location + "|||||||" + today + "||" + "\r\u0003");
                                _mySocket.Send(Encoding.GetEncoding("Big5").GetBytes("\u0002" + "2P|1|" + chMrno + "|||" + chname + "||" + chbirthday + "|" + sex + "|||||" + dr + "|||0.0|0.0|||||||" + kind + "|" + location + "|||||||" + today + "||" + "\r\u0003" + checkSum + "\r\n"));
                                SQL.ExecuteSQL("insert into opd.labinstrumentlog (type,log,time) values('HCLABSend','" + "2P|1|" + chMrno + "|||" + chname + "||" + chbirthday + "|" + sex + "|||||" + dr + "|||0.0|0.0|||||||" + kind + "|" + location + "|||||||" + today + "||" + "\r\u0003" + checkSum + "','" + DateTime.Now.ToLongDateString() + DateTime.Now.ToLongTimeString() + "')");
                                //<==ACK
                                if (!receiveMsgSuccess())
                                {
                                    oLabComm.FeedbackMsg(bw, msgtype.msgGeneral, string.Format("2.Patient 傳送錯誤 開單號:{0} 標籤號:{1}。", chTubeNo, chTubeNo));
                                    continue;
                                };

                                //4.==> ORDER (先CANCEL單，前人經驗)    
                                //string order = getLabItemString(chTubeNo);
                                string samplecollectiondt = DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString().PadLeft(2, '0') + DateTime.Now.Day.ToString().PadLeft(2, '0') + DateTime.Now.Hour.ToString().PadLeft(2, '0') + DateTime.Now.Minute.ToString().PadLeft(2, '0') + DateTime.Now.Second.ToString().PadLeft(2, '0');//採檢時間一律以現在時間為準
                                string chemg = convertOrderStatus(sEmg);
                                //檢查碼
                                checkSum = CalculateChecksumChinese("3OBR|1|" + chTubeNo + "||" + order + "|" + chemg + "||" + samplecollectiondt + "||||C|||" + samplecollectiondt + "|||||||||||||" + "\r\u0003");
                                _mySocket.Send(Encoding.GetEncoding("Big5").GetBytes("\u0002" + "3OBR|1|" + chTubeNo + "||" + order + "|S||" + samplecollectiondt + "||||C|||" + samplecollectiondt + "|||||||||||||" + "\r\u0003" + checkSum + "\r\n"));
                                SQL.ExecuteSQL("insert into opd.labinstrumentlog (type,log,time) values('HCLABSend','" + "3OBR|1|" + chTubeNo + "||" + order + "|S||" + samplecollectiondt + "||||C|||" + samplecollectiondt + "|||||||||||||" + "','" + DateTime.Now.ToLongDateString() + DateTime.Now.ToLongTimeString() + "')");
                                //<==ACK
                                if (!receiveMsgSuccess())
                                {
                                    oLabComm.FeedbackMsg(bw, msgtype.msgGeneral, string.Format("3.ORDER CANCEL 傳送錯誤 開單號:{0} 標籤號:{1}。", chTubeNo, chTubeNo));
                                    continue;
                                };

                                //4.==> ORDER (再加單)    
                                //order = getLabItemString(chTubeNo);
                                //samplecollectiondt = DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString().PadLeft(2, '0') + DateTime.Now.Day.ToString().PadLeft(2, '0') + DateTime.Now.Hour.ToString().PadLeft(2, '0') + DateTime.Now.Minute.ToString().PadLeft(2, '0') + DateTime.Now.Second.ToString().PadLeft(2, '0');//採檢時間一律以現在時間為準
                                //檢查碼
                                checkSum = CalculateChecksumChinese("4OBR|2|" + chTubeNo + "||" + order + "|" + chemg + "||" + samplecollectiondt + "||||A|||" + samplecollectiondt + "|||||||||||||" + "\r\u0003");
                                _mySocket.Send(Encoding.GetEncoding("Big5").GetBytes("\u0002" + "4OBR|2|" + chTubeNo + "||" + order + "|S||" + samplecollectiondt + "||||A|||" + samplecollectiondt + "|||||||||||||" + "\r\u0003" + checkSum + "\r\n"));
                                SQL.ExecuteSQL("insert into opd.labinstrumentlog (type,log,time) values('HCLABSend','" + "4OBR|2|" + chTubeNo + "||" + order + "|S||" + samplecollectiondt + "||||A|||" + samplecollectiondt + "|||||||||||||" + "','" + DateTime.Now.ToLongDateString() + DateTime.Now.ToLongTimeString() + "')");

                                //<==ACK
                                if (!receiveMsgSuccess())
                                {
                                    oLabComm.FeedbackMsg(bw, msgtype.msgGeneral, string.Format("4.ORDER ADD 傳送錯誤 開單號:{0} 標籤號:{1}。", chTubeNo, chTubeNo));
                                    continue;
                                };
                                //檢查碼
                                checkSum = CalculateChecksumChinese("5L|1||1|4\r\u0003");

                                //5.==>結束符號 5L|1||2|8
                                _mySocket.Send(Encoding.GetEncoding("Big5").GetBytes("\u0002" + "5L|1||1|4\r\u0003" + checkSum + "\r\n"));
                                SQL.ExecuteSQL("insert into opd.labinstrumentlog (type,log,time) values('HCLABSend','" + "5L|1||1|4\r\u0003" + checkSum + "','" + DateTime.Now.ToLongDateString() + DateTime.Now.ToLongTimeString() + "')");
                                //<==ACK
                                if (!receiveMsgSuccess())
                                {
                                    oLabComm.FeedbackMsg(bw, msgtype.msgGeneral, string.Format("5.End Mark 傳送錯誤 開單號:{0} 標籤號:{1}。", chTubeNo, chTubeNo));
                                    continue;
                                };
                                //結束符號 EOT
                                _mySocket.Send(Encoding.GetEncoding("Big5").GetBytes("\u0004"));
                                //處理完畢後，刪除所有處理資料,並斷開連結
                                _mySocket.Close();

                                oLabComm.FeedbackMsg(bw, msgtype.msgGeneral, string.Format("開單號:{0} 標籤號:{1} 傳送成功。", chTubeNo, chTubeNo));

                                //將標籤號註記已上傳
                                SQL.ExecuteSQL("update opd.tubemapping set chistransmit = 'Y', chtransmittime = '" + samplecollectiondt + "' where chtubeno = '" + chTubeNo + "'");


                            }
                        }

                        

                    }
                    catch (Exception ex)
                    {
                        oLabComm.FeedbackMsg(bw, msgtype.msgErr, string.Format("Exception 傳送錯誤: 標籤號:{0} 訊息{1}。", chTubeNo, ex.Message));
                        oLabComm.insertlog(sFirmType, ex.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                oLabComm.FeedbackMsg(bw, msgtype.msgErr, string.Format("Exception 傳送錯誤: 標籤號:{0} 訊息{1}。", chTubeNo, ex.Message));                                                
                oLabComm.insertlog(sFirmType, ex.Message);
            }
        }


        public void Close()
        {
            closeSymbol = true;

        }
        /// <summary>
        /// 轉換件別代碼
        /// </summary>
        /// <param name="chemg"></param>
        /// <returns></returns>
        private string convertOrderStatus(string chemg)
        {
            if (chemg == "Y") { return "S"; }
            return "S";
        }

        /// <summary>
        /// 計算checksum(英文)
        /// </summary>
        /// <param name="dataToCalculate"></param>
        /// <returns></returns>
        private string CalculateChecksum(string dataToCalculate)
        {
            byte[] byteToCalculate = Encoding.ASCII.GetBytes(dataToCalculate);
            int checksum = 0;
            foreach (byte chData in byteToCalculate)
            {
                checksum += chData;
            }
            checksum &= 0xff;
            return checksum.ToString("X2");
        }
        /// <summary>
        /// 計算checksum(中文)  有中文或怕有中文的都用這個
        /// </summary>
        /// <param name="dataToCalculate"></param>
        /// <returns></returns>
        private string CalculateChecksumChinese(string dataToCalculate)
        {
            byte[] utf8 = Encoding.Unicode.GetBytes(dataToCalculate);
            byte[] big5 = Encoding.Convert(Encoding.Unicode, Encoding.Default, utf8);

            int checksum = 0;
            foreach (byte chData in big5)
            {
                checksum += chData;
            }
            checksum &= 0xff;
            return checksum.ToString("X2");
        }

        /// <summary>
        /// 回傳開單地點
        /// </summary>
        /// <param name="chlabemrno"></param>
        /// <returns></returns>
        public string get_apply_location(string chlabemrno)
        {
            //EzMySQL SQL = new EzMySQL();

            DataTable data = SQL.Get_DataTable("select chroom,chlabkind from opd.ptlabemr where chlabemrno = '" + chlabemrno + "'");

            if (data.Rows.Count > 0)
            {
                return data.Rows[0]["chroom"].ToString() + "^" + data.Rows[0]["chlabkind"].ToString();
            }
            else
            {
                return "-^-";
            }
        }

        /// <summary>
        /// 回傳開單醫師
        /// </summary>
        /// <param name="chlabemrno"></param>
        /// <returns></returns>
        public string get_apply_dr(string chlabemrno)
        {
            string drCode = "";
            string drName = "";
            //EzMySQL SQL = new EzMySQL();

            drCode = SQL.Get_Scalar("select chemp from opd.ptlabemr where chlabemrno = '" + chlabemrno + "'");
            drName = SQL.Get_Scalar("select chempname from basedata.employeelist where chempno = '" + drCode + "'");

            return drCode + "^" + drName;

        }
        /// <summary>
        /// 回傳開單別
        /// </summary>
        /// <param name="chlabemrno"></param>
        /// <returns></returns>
        public string get_apply_kind(string chlabemrno)
        {
            string kind = "";
            //EzMySQL SQL = new EzMySQL();

            kind = SQL.Get_Scalar("select chlabkind from opd.ptlabemr where chlabemrno = '" + chlabemrno + "'");

            //轉換為儀器格式
            if (kind == "INP")
            {
                kind = "IP";
            }
            else if (kind == "EMG")
            {
                kind = "ER";
            }
            else
            {
                kind = "OP";
            }

            return kind;

        }

        /// <summary>
        /// 新系統 診別轉換[診別。參數數值的意義為：1:門診 2:急診 3:住院。]
        /// </summary>
        /// <param name="dept"></param>
        /// <returns></returns>
        public string get_apply_kind_New(string dept)
        {
            string kind = "";

            //門診
            if (dept == "1") kind = "OP";
            //急診
            if (dept == "2") kind = "ER";
            //住院
            if (dept == "3") kind = "IP";          
            
            return kind;
        }

        /// <summary>
        /// 取得該標籤號的所有檢驗代碼與名稱=>轉換為儀器格式
        /// </summary>
        /// <returns></returns>
        public string getLabItemString(string chtubeno)
        {
            //EzMySQL SQL = new EzMySQL();
            DataTable dt = SQL.Get_DataTable("select * from opd.tubemapping where chtubeno = '" + chtubeno + "'");
            string result = "";

            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    DataTable detail = SQL.Get_DataTable("select chlabsetid,chdiacode from opd.labemracnt where chlabemrno='" + dt.Rows[i]["chlabemrno"].ToString() + "' and intseq = '" + dt.Rows[i]["intseq"].ToString() + "'");
                    DataTable data = SQL.Get_DataTable("select chmachineorder,chlabname from opd.labsetacnt where chlabsetid = '" + detail.Rows[0]["chlabsetid"].ToString() + "' and chlabcode = '" + detail.Rows[0]["chdiacode"].ToString() + "'");
                    //防止醫令重複 廠商無法處理(等待廠商處理，indexof很不安全，好死不死代碼差一個字就中招了)
                    //if (result.ToString().IndexOf(data.Rows[0]["chmachineorder"].ToString(), StringComparison.OrdinalIgnoreCase) >= 0) { continue; }
                    if (result != "") { result = result + "~"; }
                    //result = detail.Rows[0]["chdiacode"].ToString() + "^" + chlabname;
                    result = result + data.Rows[0]["chmachineorder"].ToString() + "^" + data.Rows[0]["chmachineorder"].ToString();//一律以廠商檢驗代碼為準
                }
            }
            else
            {
                return result;
            }
            return result;
        }
        
        
        public string getLabItemStringNew(string tubeno,dynamic[] dlist)
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

        /// <summary>
        /// 性別轉換
        /// </summary>
        /// <param name="sex"></param>
        /// <returns></returns>
        public string sexConvert(string sex)
        {
            string result = "";
            if (sex == "0")
            {
                result = "F";
            }
            else if (sex == "1")
            {
                result = "M";
            }
            else
            {
                result = "U";
            }
            return result;
        }
        /// <summary>
        /// Hex = > ASCII 轉碼
        /// </summary>
        /// <param name="hex"></param>
        /// <returns></returns>
        public string HEX2ASCII(string hex)
        {
            string res = String.Empty;
            for (int a = 0; a < hex.Length; a = a + 2)
            {
                string Char2Convert = hex.Substring(a, 2);
                int n = Convert.ToInt32(Char2Convert, 16);
                char c = (char)n;
                res += c.ToString();
            }
            return res;
        }
        /// <summary>
        /// ASCII => HEX 轉碼
        /// </summary>
        /// <param name="ascii"></param>
        /// <returns></returns>
        public string ASCIITOHex(string ascii)
        {
            StringBuilder sb = new StringBuilder();
            byte[] inputBytes = Encoding.UTF8.GetBytes(ascii);

            foreach (byte b in inputBytes)
            {
                sb.Append(string.Format("{0:x2}", b));
            }
            return sb.ToString();

        }
        /// <summary>
        /// 儀器是否傳輸成功
        /// </summary>
        /// <returns></returns>
        public bool receiveMsgSuccess()
        {
            byte[] returnMsg = new byte[_mySocket.ReceiveBufferSize];
            int count = 0;
            if ((count = _mySocket.Receive(returnMsg)) != 0)
            {
                //回傳值(06:ACK 15:NAK)
                string result = ASCIITOHex(Encoding.ASCII.GetString(returnMsg, 0, count));
                if (result != "06")
                {
                    _mySocket.Send(Encoding.GetEncoding("Big5").GetBytes("\u0004"));
                    return false;
                }
                return true;
            }
            //沒收到資料，繼續下一筆
            else
            {
                _mySocket.Send(Encoding.GetEncoding("Big5").GetBytes("\u0004"));
                return false;
            }
        }
    }
}
