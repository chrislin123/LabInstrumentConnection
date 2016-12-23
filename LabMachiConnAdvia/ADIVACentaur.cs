using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using LabMachiLib;
using System.Configuration;
using System.ComponentModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace LabMachiConnAdvia
{
    //免疫機
    public class ADIVACentaur
    {
        //AVDIA 所負責的4台儀器名稱
        private enum instructmentName
        {
            Centaur
        }
        //AVDIA 所負責的4台儀器代碼
        private enum instructmentCode
        {
            C = instructmentName.Centaur
        }

        Socket _mySocket;
        int _port = 0;
        bool closeSymbol = false;
        string orderID = "";//標籤號
        string chlabemrno = "";//開單號
        string LabType = "AVDIA";
        string ssql = string.Empty;
        EzMySQL SQL = new EzMySQL(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);        
        LabComm oLabComm = new LabComm(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
        Dictionary<string, string> dicList = new Dictionary<string, string>();

        /// <summary>
        /// AVDIA為Socket Client 設計因此在New時不須指定PORT,而是看資料庫設定，PORT資料存在DB中
        /// </summary>
        /// <param name="port"></param>
        public ADIVACentaur(int port = 0)
        {
            //_port = port;

            dicList = CreateLabSet();
        }
        public void Start(BackgroundWorker bw)
        {
            closeSymbol = false;
            
            //Centaur IP
            string host = SQL.Get_Scalar("select chparamtext from basedata.systemctrl where chparamcode = 'ADVIACentaurIP'");
            //host = "123.123.123.123";            

            //AVDIA ORDER Port
            int port = int.Parse(SQL.Get_Scalar("select chparamtext from basedata.systemctrl where chparamcode = 'ADVIACentaurPort'"));
            //int port = 8081;         

            //SOH[01]=Beginning of Header
            //STX[02]=Beginning of Text Sending
            //ETX[03]=End of Text Sending
            //EOT[04]=End of Sending
            //ENQ[05]=Inquiry
            //ACK[06]=Positive Response
            //CR[]=
            //NAK[15]=Negative Response

            //byte[] STX = Encoding.GetEncoding("Big5").GetBytes("\u0002");            
            //byte[] ETX = Encoding.GetEncoding("Big5").GetBytes("\u0003");            
            //byte[] EOT = Encoding.GetEncoding("Big5").GetBytes("\u0004");            
            //byte[] ENQ = Encoding.GetEncoding("Big5").GetBytes("\u0005");            
            //byte[] CR = Encoding.GetEncoding("Big5").GetBytes("\r");            
            //byte[] ACK = Encoding.GetEncoding("Big5").GetBytes("\u0006");            
            //byte[] NAK = Encoding.GetEncoding("Big5").GetBytes("\u0015");


            IPAddress[] IPs = Dns.GetHostAddresses(host);
            _mySocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                //未連線則進行連線
                if (_mySocket.Connected == false) _mySocket.Connect(IPs[0], port);

                //連線失敗則跳出
                if (_mySocket.Connected == false)
                {
                    oLabComm.FeedbackMsg(bw, msgtype.msgGeneral, string.Format("Socket:{0} {1} 連線失敗。", host, port));
                    return;
                }

                ////前人經驗，先送個ACK
                _mySocket.Send(Encoding.GetEncoding("Big5").GetBytes("\u0006"));

                //取得完整免疫資料                
                AdivaData oAdivaData = ReceiveProc(bw);                

                //儀器資料轉至HIS系統                
                MachineDataToHis(oAdivaData,bw);

            }
            catch (Exception ex)
            {
                oLabComm.FeedbackMsg(bw, msgtype.msgErr, ex.Message);                               

                //寫入異常紀錄
                oLabComm.insertlog(LabType, "Error==" + ex.Message);
            }
            finally
            {
                //關閉Socket                
                oLabComm.CloseSocket(_mySocket);            
            }
        }

        public void StartNew(BackgroundWorker bw)
        {
            closeSymbol = false;

            //Centaur IP
            string host = SQL.Get_Scalar("select chparamtext from basedata.systemctrl where chparamcode = 'ADVIACentaurIP'");
            //host = "123.123.123.123";            

            //AVDIA ORDER Port
            int port = int.Parse(SQL.Get_Scalar("select chparamtext from basedata.systemctrl where chparamcode = 'ADVIACentaurPort'"));
            //int port = 8081;         

            //SOH[01]=Beginning of Header
            //STX[02]=Beginning of Text Sending
            //ETX[03]=End of Text Sending
            //EOT[04]=End of Sending
            //ENQ[05]=Inquiry
            //ACK[06]=Positive Response
            //CR[]=
            //NAK[15]=Negative Response

            //byte[] STX = Encoding.GetEncoding("Big5").GetBytes("\u0002");            
            //byte[] ETX = Encoding.GetEncoding("Big5").GetBytes("\u0003");            
            //byte[] EOT = Encoding.GetEncoding("Big5").GetBytes("\u0004");            
            //byte[] ENQ = Encoding.GetEncoding("Big5").GetBytes("\u0005");            
            //byte[] CR = Encoding.GetEncoding("Big5").GetBytes("\r");            
            //byte[] ACK = Encoding.GetEncoding("Big5").GetBytes("\u0006");            
            //byte[] NAK = Encoding.GetEncoding("Big5").GetBytes("\u0015");


            IPAddress[] IPs = Dns.GetHostAddresses(host);
            _mySocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                //未連線則進行連線
                if (_mySocket.Connected == false) _mySocket.Connect(IPs[0], port);

                //連線失敗則跳出
                if (_mySocket.Connected == false)
                {
                    oLabComm.FeedbackMsg(bw, msgtype.msgGeneral, string.Format("Socket:{0} {1} 連線失敗。", host, port));
                    return;
                }

                ////前人經驗，先送個ACK
                _mySocket.Send(Encoding.GetEncoding("Big5").GetBytes("\u0006"));

                //取得完整免疫資料                
                AdivaData oAdivaData = ReceiveProc(bw);

                //儀器資料轉至HIS系統                
                MachineDataToHisNew(oAdivaData, bw);

            }
            catch (Exception ex)
            {
                oLabComm.FeedbackMsg(bw, msgtype.msgErr, ex.Message);

                //寫入異常紀錄
                oLabComm.insertlog(LabType, "Error==" + ex.Message);
            }
            finally
            {
                //關閉Socket                
                oLabComm.CloseSocket(_mySocket);
            }
        }

        public void Send(BackgroundWorker bw)
        {
            closeSymbol = false;

            //Centaur IP
            string host = SQL.Get_Scalar("select chparamtext from basedata.systemctrl where chparamcode = 'ADVIACentaurIP'");
            //host = "123.123.123.123";            

            //AVDIA ORDER Port
            int port = int.Parse(SQL.Get_Scalar("select chparamtext from basedata.systemctrl where chparamcode = 'ADVIACentaurPort'"));
            //int port = 8081;         

            //SOH[01]=Beginning of Header
            //STX[02]=Beginning of Text Sending
            //ETX[03]=End of Text Sending
            //EOT[04]=End of Sending
            //ENQ[05]=Inquiry
            //ACK[06]=Positive Response
            //CR[]=
            //NAK[15]=Negative Response

            //byte[] STX = Encoding.GetEncoding("Big5").GetBytes("\u0002");            
            //byte[] ETX = Encoding.GetEncoding("Big5").GetBytes("\u0003");            
            //byte[] EOT = Encoding.GetEncoding("Big5").GetBytes("\u0004");            
            //byte[] ENQ = Encoding.GetEncoding("Big5").GetBytes("\u0005");            
            //byte[] CR = Encoding.GetEncoding("Big5").GetBytes("\r");            
            //byte[] ACK = Encoding.GetEncoding("Big5").GetBytes("\u0006");            
            //byte[] NAK = Encoding.GetEncoding("Big5").GetBytes("\u0015");

            string CommCR  = "\r";
            string CommSTX = "\u0002";
            string CommETX = "\u0003";
            string CommEOT = "\u0004";
            string CommENQ = "\u0005";
            string CommACK = "\u0006";
            string CommNAK = "\u0015";


            IPAddress[] IPs = Dns.GetHostAddresses(host);
            _mySocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);


            byte[] returnMsg = new byte[_mySocket.ReceiveBufferSize];
            int count = 0;
            string stmp = "";
            string sSkResult = "";
            try
            {
                //未連線則進行連線
                if (_mySocket.Connected == false) _mySocket.Connect(IPs[0], port);

                //連線失敗則跳出
                if (_mySocket.Connected == false)
                {
                    oLabComm.FeedbackMsg(bw, msgtype.msgGeneral, string.Format("Socket:{0} {1} 連線失敗。", host, port));
                    return;
                }



                /*
                _mySocket.Send(Encoding.GetEncoding("Big5").GetBytes("\u0004"));
                _mySocket.Send(Encoding.GetEncoding("Big5").GetBytes("\u0005"));


                //1.==>前人經驗，先送個ACK                
                stmp = CommACK;
                _mySocket.Send(Encoding.GetEncoding("Big5").GetBytes(stmp));
                oLabComm.insertlog(LabType, stmp);
                
                Thread.Sleep(1000);
                sSkResult = getReturn(_mySocket);
                oLabComm.insertlog(LabType, sSkResult);

                //2.==> Header   
                stmp = @"H|\^&| | |ADVCNT_LIS| | | | |LIS_ID| |P|1" + CommCR;
                _mySocket.Send(Encoding.GetEncoding("Big5").GetBytes(stmp));
                oLabComm.insertlog(LabType, stmp);

                Thread.Sleep(1000);
                sSkResult = getReturn(_mySocket);
                oLabComm.insertlog(LabType, sSkResult);


                //3.==> patient   
                stmp = "P|12|A123-45-6789| | |Jones^Mary^A| |19540601|F| | | | |12B| | | | | | | | | | | |ER1" + CommCR;
                _mySocket.Send(Encoding.GetEncoding("Big5").GetBytes(stmp));
                oLabComm.insertlog(LabType, stmp);

                Thread.Sleep(1000);
                sSkResult = getReturn(_mySocket);
                oLabComm.insertlog(LabType, sSkResult);


                //4.==> ORDER (先CANCEL單，前人經驗)   
                //stmp = "\u0006";
                //_mySocket.Send(Encoding.GetEncoding("Big5").GetBytes(stmp));
                //oLabComm.insertlog(LabType, stmp);

                //Thread.Sleep(1000);
                //sSkResult = getReturn(_mySocket);
                //oLabComm.insertlog(LabType, sSkResult);


                //4.==> ORDER (再加單)   
                stmp = @"O|1|18653| |^^^T4\^^^HCG\^^^P1234|R| | | | | |N| | | | | | | | | | | | | |O\Q"+ CommCR;
                _mySocket.Send(Encoding.GetEncoding("Big5").GetBytes(stmp));
                oLabComm.insertlog(LabType, stmp);

                Thread.Sleep(1000);
                sSkResult = getReturn(_mySocket);
                oLabComm.insertlog(LabType, sSkResult);
 
                
                //5.==>結束符號 5L|1||2|8
                stmp = "L|1|F" + CommCR;
                _mySocket.Send(Encoding.GetEncoding("Big5").GetBytes(stmp));
                oLabComm.insertlog(LabType, stmp);

                Thread.Sleep(1000);
                sSkResult = getReturn(_mySocket);
                oLabComm.insertlog(LabType, sSkResult);








                return;
                */










             

                //取得完整免疫資料               


                //這個socket收送一起，因此先收再送 
                ssql = " select distinct(chtubeno),chlabemrno,chemg from opd.tubemapping " 
                     + " where chIsTransmit = 'N' and chischeck != 'C' and chMachineNo in ('" + instructmentCode.C.ToString() + "') "
                     + " and chtubeno = 'ZRC0052301' ";
                DataTable dt = SQL.Get_DataTable(ssql);
                
                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        //tubeError = dt.Rows[i]["chtubeno"].ToString();
                        //1. 先送EOT 再送==>ENQ                      
                        _mySocket.Send(Encoding.GetEncoding("Big5").GetBytes("\u0004"));
                        _mySocket.Send(Encoding.GetEncoding("Big5").GetBytes("\u0005"));
                        //<==ACK
                        Thread.Sleep(1000);
                        if (_mySocket.Available != 0)
                        {
                            count = 0;
                            returnMsg = new byte[_mySocket.ReceiveBufferSize];

                            if ((count = _mySocket.Receive(returnMsg)) != 0)
                            {
                                //回傳值(04:EOT)=End of Sending
                                //回傳值(05:ENQ)=Inquiry
                                string result = ASCIITOHex(Encoding.ASCII.GetString(returnMsg, 0, count));
                                oLabComm.insertlog(LabType, result);
                            }
                        }

                        //2.==> Header   
                        //檢查碼
                        string sSend = @"1H|\\^&|||||||||||1";

                        string checkSum = CalculateChecksumChinese(sSend + "\r\u0003");
                        _mySocket.Send(Encoding.GetEncoding("Big5").GetBytes("\u0002" + sSend + "\r\u0003" + checkSum + "\r\n"));


                        stmp = sSend + "\r\u0003" + checkSum;
                        oLabComm.insertlog(LabType, stmp);
                        //SQL.ExecuteSQL("insert into opd.labinstrumentlog (type,log,time) values('AVDIA','" + "1H|^~\\&|||||||||||1" + "\r\u0003" + checkSum + "','" + DateTime.Now.ToLongDateString() + DateTime.Now.ToLongTimeString() + "')");

                        //<==ACK
                        //if (!receiveMsgSuccess())
                        //{
                        //    //errorText.AppendText("時間:" + DateTime.Now.ToShortTimeString() + " 2.Header 傳送錯誤 開單號:" + dt.Rows[i]["chtubeno"].ToString() + " 標籤號:" + dt.Rows[i]["chtubeno"].ToString() + "。" + "\r\n");
                        //    //errorText.ScrollToCaret();
                        //    continue;
                        //};
                        Thread.Sleep(1000);
                        if (_mySocket.Available != 0)
                        {
                            count = 0;
                            returnMsg = new byte[_mySocket.ReceiveBufferSize];

                            if ((count = _mySocket.Receive(returnMsg)) != 0)
                            {
                                //回傳值(04:EOT)=End of Sending
                                //回傳值(05:ENQ)=Inquiry
                                string result = ASCIITOHex(Encoding.ASCII.GetString(returnMsg, 0, count));
                                oLabComm.insertlog(LabType, result);
                            }
                        }

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

                        stmp = "2P|1|" + patientDT.Rows[0]["chmrno"].ToString() + "|||" + patientDT.Rows[0]["chname"].ToString() + "||" + patientDT.Rows[0]["chbirthday"].ToString() + "|" + sex + "|||||" + dr + "|||0.0|0.0|||||||" + kind + "|" + location + "|||||||" + today + "||" + "\r\u0003" + checkSum;
                        oLabComm.insertlog(LabType, stmp);
                        //oLabComm
                        //SQL.ExecuteSQL("insert into opd.labinstrumentlog (type,log,time) values('AVIDA','" + "2P|1|" + patientDT.Rows[0]["chmrno"].ToString() + "|||" + patientDT.Rows[0]["chname"].ToString() + "||" + patientDT.Rows[0]["chbirthday"].ToString() + "|" + sex + "|||||" + dr + "|||0.0|0.0|||||||" + kind + "|" + location + "|||||||" + today + "||" + "\r\u0003" + checkSum + "','" + DateTime.Now.ToLongDateString() + DateTime.Now.ToLongTimeString() + "')");
                        //<==ACK
                        //if (!receiveMsgSuccess())
                        //{
                        //    //errorText.AppendText("時間:" + DateTime.Now.ToShortTimeString() + " 3.Patient 傳送錯誤 開單號:" + dt.Rows[i]["chtubeno"].ToString() + " 標籤號:" + dt.Rows[i]["chtubeno"].ToString() + "。" + "\r\n");
                        //    //errorText.ScrollToCaret();
                        //    continue;
                        //};
                        Thread.Sleep(1000);
                        if (_mySocket.Available != 0)
                        {
                            count = 0;
                            returnMsg = new byte[_mySocket.ReceiveBufferSize];

                            if ((count = _mySocket.Receive(returnMsg)) != 0)
                            {
                                //回傳值(04:EOT)=End of Sending
                                //回傳值(05:ENQ)=Inquiry
                                string result = ASCIITOHex(Encoding.ASCII.GetString(returnMsg, 0, count));
                                oLabComm.insertlog(LabType, result);
                            }
                        }

                        //4.==> ORDER (先CANCEL單，前人經驗)    
                        string order = getLabItemString(dt.Rows[i]["chtubeno"].ToString());
                        string samplecollectiondt = DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString().PadLeft(2, '0') + DateTime.Now.Day.ToString().PadLeft(2, '0') + DateTime.Now.Hour.ToString().PadLeft(2, '0') + DateTime.Now.Minute.ToString().PadLeft(2, '0') + DateTime.Now.Second.ToString().PadLeft(2, '0');//採檢時間一律以現在時間為準
                        string chemg = convertOrderStatus(dt.Rows[i]["chemg"].ToString());
                        //檢查碼
                        checkSum = CalculateChecksumChinese("3OBR|1|" + dt.Rows[i]["chtubeno"].ToString() + "||" + order + "|" + chemg + "||" + samplecollectiondt + "||||C|||" + samplecollectiondt + "|||||||||||||" + "\r\u0003");
                        _mySocket.Send(Encoding.GetEncoding("Big5").GetBytes("\u0002" + "3OBR|1|" + dt.Rows[i]["chtubeno"].ToString() + "||" + order + "|S||" + samplecollectiondt + "||||C|||" + samplecollectiondt + "|||||||||||||" + "\r\u0003" + checkSum + "\r\n"));

                        stmp = "3OBR|1|" + dt.Rows[i]["chtubeno"].ToString() + "||" + order + "|S||" + samplecollectiondt + "||||C|||" + samplecollectiondt + "|||||||||||||";
                        oLabComm.insertlog(LabType, stmp);
                        //SQL.ExecuteSQL("insert into opd.labinstrumentlog (type,log,time) values('AVIDA','" + "3OBR|1|" + dt.Rows[i]["chtubeno"].ToString() + "||" + order + "|S||" + samplecollectiondt + "||||C|||" + samplecollectiondt + "|||||||||||||" + "','" + DateTime.Now.ToLongDateString() + DateTime.Now.ToLongTimeString() + "')");
                        //<==ACK
                        //if (!receiveMsgSuccess())
                        //{
                        //    //errorText.AppendText("時間:" + DateTime.Now.ToShortTimeString() + " 4.ORDER CANCEL 傳送錯誤 開單號:" + dt.Rows[i]["chtubeno"].ToString() + " 標籤號:" + dt.Rows[i]["chtubeno"].ToString() + "。" + "\r\n");
                        //    //errorText.ScrollToCaret();
                        //    continue;
                        //};
                        Thread.Sleep(1000);
                        if (_mySocket.Available != 0)
                        {
                            count = 0;
                            returnMsg = new byte[_mySocket.ReceiveBufferSize];

                            if ((count = _mySocket.Receive(returnMsg)) != 0)
                            {
                                //回傳值(04:EOT)=End of Sending
                                //回傳值(05:ENQ)=Inquiry
                                string result = ASCIITOHex(Encoding.ASCII.GetString(returnMsg, 0, count));
                                oLabComm.insertlog(LabType, result);
                            }
                        }

                        //4.==> ORDER (再加單)    
                        order = getLabItemString(dt.Rows[i]["chtubeno"].ToString());
                        //samplecollectiondt = DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString().PadLeft(2, '0') + DateTime.Now.Day.ToString().PadLeft(2, '0') + DateTime.Now.Hour.ToString().PadLeft(2, '0') + DateTime.Now.Minute.ToString().PadLeft(2, '0') + DateTime.Now.Second.ToString().PadLeft(2, '0');//採檢時間一律以現在時間為準
                        //檢查碼
                        checkSum = CalculateChecksumChinese("4OBR|2|" + dt.Rows[i]["chtubeno"].ToString() + "||" + order + "|" + chemg + "||" + samplecollectiondt + "||||A|||" + samplecollectiondt + "|||||||||||||" + "\r\u0003");
                        _mySocket.Send(Encoding.GetEncoding("Big5").GetBytes("\u0002" + "4OBR|2|" + dt.Rows[i]["chtubeno"].ToString() + "||" + order + "|S||" + samplecollectiondt + "||||A|||" + samplecollectiondt + "|||||||||||||" + "\r\u0003" + checkSum + "\r\n"));

                        stmp = "4OBR|2|" + dt.Rows[i]["chtubeno"].ToString() + "||" + order + "|S||" + samplecollectiondt + "||||A|||" + samplecollectiondt + "|||||||||||||";
                        oLabComm.insertlog(LabType, stmp);
                        //SQL.ExecuteSQL("insert into opd.labinstrumentlog (type,log,time) values('AVIDA','" + "4OBR|2|" + dt.Rows[i]["chtubeno"].ToString() + "||" + order + "|S||" + samplecollectiondt + "||||A|||" + samplecollectiondt + "|||||||||||||" + "','" + DateTime.Now.ToLongDateString() + DateTime.Now.ToLongTimeString() + "')");

                        //<==ACK
                        //if (!receiveMsgSuccess())
                        //{
                        //    //errorText.AppendText("時間:" + DateTime.Now.ToShortTimeString() + " 4.ORDER ADD 傳送錯誤 開單號:" + dt.Rows[i]["chtubeno"].ToString() + " 標籤號:" + dt.Rows[i]["chtubeno"].ToString() + "。" + "\r\n");
                        //    //errorText.ScrollToCaret();
                        //    continue;
                        //};
                        Thread.Sleep(1000);
                        if (_mySocket.Available != 0)
                        {
                            count = 0;
                            returnMsg = new byte[_mySocket.ReceiveBufferSize];

                            if ((count = _mySocket.Receive(returnMsg)) != 0)
                            {
                                //回傳值(04:EOT)=End of Sending
                                //回傳值(05:ENQ)=Inquiry
                                string result = ASCIITOHex(Encoding.ASCII.GetString(returnMsg, 0, count));
                                oLabComm.insertlog(LabType, result);
                            }
                        }
                        //檢查碼
                        checkSum = CalculateChecksumChinese("5L|1||1|4\r\u0003");

                        //5.==>結束符號 5L|1||2|8
                        _mySocket.Send(Encoding.GetEncoding("Big5").GetBytes("\u0002" + "5L|1||1|4\r\u0003" + checkSum + "\r\n"));

                        stmp = "5L|1||1|4\r\u0003" + checkSum;
                        oLabComm.insertlog(LabType, stmp);

                        //SQL.ExecuteSQL("insert into opd.labinstrumentlog (type,log,time) values('AVIDA','" + "5L|1||1|4\r\u0003" + checkSum + "','" + DateTime.Now.ToLongDateString() + DateTime.Now.ToLongTimeString() + "')");
                        //<==ACK
                        //if (!receiveMsgSuccess())
                        //{
                        //    //errorText.AppendText("時間:" + DateTime.Now.ToShortTimeString() + " 5.End Mark 傳送錯誤 開單號:" + dt.Rows[i]["chtubeno"].ToString() + " 標籤號:" + dt.Rows[i]["chtubeno"].ToString() + "。" + "\r\n");
                        //    //errorText.ScrollToCaret();
                        //    continue;
                        //};
                        Thread.Sleep(1000);
                        if (_mySocket.Available != 0)
                        {
                            count = 0;
                            returnMsg = new byte[_mySocket.ReceiveBufferSize];

                            if ((count = _mySocket.Receive(returnMsg)) != 0)
                            {
                                //回傳值(04:EOT)=End of Sending
                                //回傳值(05:ENQ)=Inquiry
                                string result = ASCIITOHex(Encoding.ASCII.GetString(returnMsg, 0, count));
                                oLabComm.insertlog(LabType, result);
                            }
                        }

                        //結束符號 EOT
                        _mySocket.Send(Encoding.GetEncoding("Big5").GetBytes("\u0004"));
                        //處理完畢後，刪除所有處理資料,並斷開連結
                        //_mySocket.Close();

                        Thread.Sleep(1000);
                        if (_mySocket.Available != 0)
                        {
                            count = 0;
                            returnMsg = new byte[_mySocket.ReceiveBufferSize];

                            if ((count = _mySocket.Receive(returnMsg)) != 0)
                            {
                                //回傳值(04:EOT)=End of Sending
                                //回傳值(05:ENQ)=Inquiry
                                string result = ASCIITOHex(Encoding.ASCII.GetString(returnMsg, 0, count));
                                oLabComm.insertlog(LabType, result);
                            }
                        }

                        //msgText.AppendText("傳送時間:" + DateTime.Now.Year.ToString() + "年" + DateTime.Now.Month.ToString().PadLeft(2, '0') + "月" + DateTime.Now.Day.ToString().PadLeft(2, '0') + "日" + DateTime.Now.Hour.ToString().PadLeft(2, '0') + "時" + DateTime.Now.Minute.ToString().PadLeft(2, '0') + "分" + DateTime.Now.Second.ToString().PadLeft(2, '0') + "秒" + "     " + "標籤號:" + dt.Rows[i]["chtubeno"].ToString() + "傳送成功。" + "\r\n");
                        //msgText.ScrollToCaret();
                        //將標籤號註記已上傳
                        //SQL.ExecuteSQL("update opd.tubemapping set chistransmit = 'Y', chtransmittime = '" + samplecollectiondt + "' where chtubeno = '" + dt.Rows[i]["chtubeno"].ToString() + "'");
                    }
                }



                //AdivaData oAdivaData = ReceiveProc(bw);

                //儀器資料轉至HIS系統                
                //MachineDataToHis(oAdivaData, bw);

            }
            catch (Exception ex)
            {
                oLabComm.FeedbackMsg(bw, msgtype.msgErr, ex.Message);

                //寫入異常紀錄
                oLabComm.insertlog(LabType, "Error==" + ex.Message);
            }
            finally
            {
                //關閉Socket                
                oLabComm.CloseSocket(_mySocket);
            }
        }


        private string getReturn(Socket sk)
        {
            int count = 0;
            byte[] returnMsg = new byte[sk.ReceiveBufferSize];
            string result = "";
            if (sk.Available != 0)
            {
                count = 0;
                //returnMsg = new byte[_mySocket.ReceiveBufferSize];

                if ((count = sk.Receive(returnMsg)) != 0)
                {
                    //回傳值(04:EOT)=End of Sending
                    //回傳值(05:ENQ)=Inquiry
                    result = ASCIITOHex(Encoding.ASCII.GetString(returnMsg, 0, count));
                    //oLabComm.insertlog(LabType, result);
                }
            }

            return result;

        }

        /// <summary>儀器資料轉至HIS系統
        /// 
        /// </summary>
        private void MachineDataToHis(AdivaData oAdivaData, BackgroundWorker bw)
        {
            if (oAdivaData.SampleID != "")
            {
                oLabComm.FeedbackMsg(bw, msgtype.msgStatus, "寫入HIS系統中..");

                oLabComm.FeedbackMsg(bw, msgtype.msgGeneral, string.Format("寫入HIS系統中..檢體號:{0}", oAdivaData.SampleID));           

                LabMachineData oMachData = new LabMachineData();
                string TransmitTime = DateTime.Now.ToString("yyyyMMddHHmmss");
                //bool bIsINTR = false;
                

                foreach (string sLisCode in oAdivaData.OrderList)
                {
                    //寫入His系統的數值
                    string sValue = string.Empty;
                    //目前儀器傳出來的項目數值
                    string sValue_INTR = string.Empty;
                    string sValue_INDX = string.Empty;
                    string sValue_DOSE = string.Empty;
                    string sValue_COFF = string.Empty;
                    string sValue_RLU = string.Empty;

                    //取得管子資料
                    ssql = " select * from opd.tubemapping a "
                         + " left join opd.labdrep b on a.chLabEmrNo = b.chlabemrno and a.intSeq = b.intSeq "
                         + " where chtubeno = '" + oAdivaData.SampleID + "' "
                         + " and chMachineMapping = '" + sLisCode + "' ";

                    DataTable dt = SQL.Get_DataTable(ssql);
                    foreach (DataRow dr in dt.Rows)
                    {
                        oMachData.chLabEmrNo = dr["chLabEmrNo"].ToString();
                        oMachData.chTubeNo = dr["chTubeNo"].ToString();
                        oMachData.intSeq = dr["intSeq"].ToString();
                        oMachData.chMachineNo = dr["chMachineNo"].ToString();
                        oMachData.chTransmitTime = TransmitTime;
                        oMachData.chMachineMapping = sLisCode;
                    }

                    //寫入記錄檔
                    foreach (AdivaRecordData item in oAdivaData.RecordList)
                    {
                        if (item.LIScode != sLisCode) continue;

                        //判斷是否INTR
                        if (item.ResultAspects == "INTR")
                        {
                            sValue_INTR = item.DataValue == "React" ? "Reactive" : "Non-reactive";
                        }


                        if (item.ResultAspects == "INDX") sValue_INDX = item.DataValue;
                        if (item.ResultAspects == "DOSE") sValue_DOSE = item.DataValue;
                        if (item.ResultAspects == "COFF") sValue_COFF = item.DataValue;
                        if (item.ResultAspects == "RLU") sValue_RLU = item.DataValue;

                        oMachData.chValue = "";
                        oMachData.chValuetype = "";
                        oMachData.chUnit = "";
                        oMachData.chOnBoardTime = "";

                        oMachData.chValue = item.DataValue;
                        oMachData.chValuetype = item.ResultAspects;
                        oMachData.chUnit = item.DataUnits;

                        ssql = " insert into opd.labmachinedata "
                             + " (chLabEmrNo,intSeq,chTubeNo,chCombineCode "
                             + " ,chMachineNo,chMachineMapping,chValuetype,chValue "
                             + " ,chUnit,chTransmitTime,chOnBoardTime,chUpdToHisTime) "
                             + " values ( "
                             + " '" + oMachData.chLabEmrNo + "' "
                             + " ,'" + oMachData.intSeq + "' "
                            //+ " ,'" + oMachData.chTubeNo + "' "
                             + " ,'" + oAdivaData.SampleID + "' "  //選擇儀器sampleno(管號或試管號)                                 
                             + " ,'" + oMachData.chCombineCode + "' "
                             + " ,'" + oMachData.chMachineNo + "' "
                             + " ,'" + oMachData.chMachineMapping + "' "
                             + " ,'" + oMachData.chValuetype + "' "
                             + " ,'" + oMachData.chValue + "' "
                             + " ,'" + oMachData.chUnit + "' "
                             + " ,'" + oMachData.chTransmitTime + "' "
                             + " ,'" + oMachData.chOnBoardTime + "' "
                             + " ,'' "
                             + " ) "
                             ;
                        SQL.ExecuteSQL(ssql);
                    }

                    //調整非線性數值資料


                    //取得數值組合設定檔
                    ssql = " select usKey from msutilset "
                         + " where usType = 'LabA' "
                         + " and usValue='" + oMachData.chMachineMapping + "' "
                         ;
                    string ValueType = SQL.Get_Scalar(ssql);


                    if (ValueType == "Tp1") //DOSE
                    {
                        //非線性資料規則
                        sValue_DOSE = NonLineValueRule(oMachData.chMachineMapping, sValue_DOSE);
                        sValue = sValue_DOSE;
                    }
                    else if (ValueType == "Tp2") //INTR(DOSE)
                    {
                        //非線性資料規則
                        sValue_DOSE = NonLineValueRule(oMachData.chMachineMapping, sValue_DOSE);
                        sValue = string.Format("{0}({1})", sValue_INTR, sValue_DOSE);
                    }
                    else if (ValueType == "Tp3") //INTR(INDX)
                    {
                        //非線性資料規則
                        sValue_INDX = NonLineValueRule(oMachData.chMachineMapping, sValue_INDX);
                        sValue = string.Format("{0}({1})", sValue_INTR, sValue_INDX);
                    }
                    else if (ValueType == "Tp4") //INTR(COFF)
                    {
                        //非線性資料規則
                        sValue_COFF = NonLineValueRule(oMachData.chMachineMapping, sValue_COFF);
                        sValue = string.Format("{0}({1})", sValue_INTR, sValue_COFF);
                    }
                    else //數值預設空白
                    {
                        sValue = "";
                        //sValue = sValue_DOSE;
                    }


                    //1050627 修改為由資料庫設定數值類別
                    //if (sValue_INTR != "" && sValue != "")
                    //{
                    //    if (sValue_INTR == "React") sValue_INTR = "Reactive";
                    //    if (sValue_INTR == "NR") sValue_INTR = "Non-reactive";
                    //    sValue = string.Format("{0}({1})", sValue_INTR, sValue_DOSE);
                    //}

                    //if (sValue_INTR != "" && sValue_INDX != "")
                    //{
                    //    if (sValue_INTR == "React") sValue_INTR = "Reactive";
                    //    if (sValue_INTR == "NR") sValue_INTR = "Non-reactive";
                    //    sValue = string.Format("{0}({1})", sValue_INTR, sValue_INDX);
                    //}                

                    ssql = " select * from opd.labdrep "
                         + " where chlabemrno = '" + oMachData.chLabEmrNo + "' "
                         + " and chMachineMapping='" + oMachData.chMachineMapping + "' "
                         + " and intSeq='" + oMachData.intSeq + "' "
                         + " and chRepValue = '' "
                         ;
                    dt.Clear();
                    dt = SQL.Get_DataTable(ssql);
                    if (dt.Rows.Count > 0)
                    {
                        //更新報告報告項目
                        string updatetime = DateTime.Now.ToString("yyyyMMddHHmm");

                        ssql = " update opd.labdrep set "
                             + " chRepValue='" + sValue + "', chValueDttm='" + updatetime + "' "
                             + " where chlabemrno = '" + oMachData.chLabEmrNo + "' and chMachineMapping='" + oMachData.chMachineMapping + "' "
                             + " and chRepValue = '' and intSeq = '" + oMachData.intSeq + "' ";
                        SQL.ExecuteSQL(ssql);

                        //更新後記錄於Tubemapping
                        ssql = " update opd.labmachinedata set "
                             + " chUpdToHisTime='" + TransmitTime + "' "
                             + " where chLabEmrNo = '" + oMachData.chLabEmrNo + "' "
                             + " and intseq = '" + oMachData.intSeq + "' "
                             + " and chtubeno = '" + oMachData.chTubeNo + "' "
                             + " and chMachineMapping='" + oMachData.chMachineMapping + "' "
                             ;
                        SQL.ExecuteSQL(ssql);

                        string sTemp = string.Format("開單號:{0},序號:{1},儀器碼:{2},數值:{3}"
                            , oMachData.chLabEmrNo
                            , oMachData.intSeq
                            , oMachData.chMachineMapping
                            , sValue
                            );
                        oLabComm.FeedbackMsg(bw, msgtype.msgGeneral, sTemp);

                    }

                }

                
            }

        }

        /// <summary>儀器資料轉至HIS系統
        /// 
        /// </summary>
        private void MachineDataToHisNew(AdivaData oAdivaData, BackgroundWorker bw)
        {
            if (oAdivaData.SampleID != "")
            {
                oLabComm.FeedbackMsg(bw, msgtype.msgStatus, "寫入HIS系統中..");

                oLabComm.FeedbackMsg(bw, msgtype.msgGeneral, string.Format("寫入HIS系統中..檢體號:{0}", oAdivaData.SampleID));

                LabMachineData oMachData = new LabMachineData();
                string TransmitTime = DateTime.Now.ToString("yyyyMMddHHmmss");
                //bool bIsINTR = false;


                foreach (string sLisCode in oAdivaData.OrderList)
                {
                    //寫入His系統的數值
                    string sValue = string.Empty;
                    //目前儀器傳出來的項目數值
                    string sValue_INTR = string.Empty;
                    string sValue_INDX = string.Empty;
                    string sValue_DOSE = string.Empty;
                    string sValue_COFF = string.Empty;
                    string sValue_RLU = string.Empty;


                    /* 
                    //取得管子資料
                    ssql = " select * from opd.tubemapping a "
                         + " left join opd.labdrep b on a.chLabEmrNo = b.chlabemrno and a.intSeq = b.intSeq "
                         + " where chtubeno = '" + oAdivaData.SampleID + "' "
                         + " and chMachineMapping = '" + sLisCode + "' ";

                    DataTable dt = SQL.Get_DataTable(ssql);
                    foreach (DataRow dr in dt.Rows)
                    {
                        oMachData.chLabEmrNo = dr["chLabEmrNo"].ToString();
                        oMachData.chTubeNo = dr["chTubeNo"].ToString();
                        oMachData.intSeq = dr["intSeq"].ToString();
                        oMachData.chMachineNo = dr["chMachineNo"].ToString();
                        oMachData.chTransmitTime = TransmitTime;
                        oMachData.chMachineMapping = sLisCode;
                    }

                    //寫入記錄檔
                    foreach (AdivaRecordData item in oAdivaData.RecordList)
                    {
                        if (item.LIScode != sLisCode) continue;

                        //判斷是否INTR
                        if (item.ResultAspects == "INTR")
                        {
                            sValue_INTR = item.DataValue == "React" ? "Reactive" : "Non-reactive";
                        }


                        if (item.ResultAspects == "INDX") sValue_INDX = item.DataValue;
                        if (item.ResultAspects == "DOSE") sValue_DOSE = item.DataValue;
                        if (item.ResultAspects == "COFF") sValue_COFF = item.DataValue;
                        if (item.ResultAspects == "RLU") sValue_RLU = item.DataValue;

                        oMachData.chValue = "";
                        oMachData.chValuetype = "";
                        oMachData.chUnit = "";
                        oMachData.chOnBoardTime = "";

                        oMachData.chValue = item.DataValue;
                        oMachData.chValuetype = item.ResultAspects;
                        oMachData.chUnit = item.DataUnits;

                        ssql = " insert into opd.labmachinedata "
                             + " (chLabEmrNo,intSeq,chTubeNo,chCombineCode "
                             + " ,chMachineNo,chMachineMapping,chValuetype,chValue "
                             + " ,chUnit,chTransmitTime,chOnBoardTime,chUpdToHisTime) "
                             + " values ( "
                             + " '" + oMachData.chLabEmrNo + "' "
                             + " ,'" + oMachData.intSeq + "' "
                            //+ " ,'" + oMachData.chTubeNo + "' "
                             + " ,'" + oAdivaData.SampleID + "' "  //選擇儀器sampleno(管號或試管號)                                 
                             + " ,'" + oMachData.chCombineCode + "' "
                             + " ,'" + oMachData.chMachineNo + "' "
                             + " ,'" + oMachData.chMachineMapping + "' "
                             + " ,'" + oMachData.chValuetype + "' "
                             + " ,'" + oMachData.chValue + "' "
                             + " ,'" + oMachData.chUnit + "' "
                             + " ,'" + oMachData.chTransmitTime + "' "
                             + " ,'" + oMachData.chOnBoardTime + "' "
                             + " ,'' "
                             + " ) "
                             ;
                        SQL.ExecuteSQL(ssql);
                    }
                    */
                    //調整非線性數值資料


                    //取得數值組合設定檔                    
                    //ssql = " select usKey from msutilset "
                    //     + " where usType = 'LabA' "
                    //     + " and usValue='" + oMachData.chMachineMapping + "' "
                    //     ;
                    //string ValueType = SQL.Get_Scalar(ssql);

                    //1051221 配合新系統改寫，設定檔寫死在程式碼中
                    string ValueType = dicList[oMachData.chMachineMapping].ToString();


                    if (ValueType == "Tp1") //DOSE
                    {
                        //非線性資料規則
                        sValue_DOSE = NonLineValueRule(oMachData.chMachineMapping, sValue_DOSE);
                        sValue = sValue_DOSE;
                    }
                    else if (ValueType == "Tp2") //INTR(DOSE)
                    {
                        //非線性資料規則
                        sValue_DOSE = NonLineValueRule(oMachData.chMachineMapping, sValue_DOSE);
                        sValue = string.Format("{0}({1})", sValue_INTR, sValue_DOSE);
                    }
                    else if (ValueType == "Tp3") //INTR(INDX)
                    {
                        //非線性資料規則
                        sValue_INDX = NonLineValueRule(oMachData.chMachineMapping, sValue_INDX);
                        sValue = string.Format("{0}({1})", sValue_INTR, sValue_INDX);
                    }
                    else if (ValueType == "Tp4") //INTR(COFF)
                    {
                        //非線性資料規則
                        sValue_COFF = NonLineValueRule(oMachData.chMachineMapping, sValue_COFF);
                        sValue = string.Format("{0}({1})", sValue_INTR, sValue_COFF);
                    }
                    else //數值預設空白
                    {
                        sValue = "";
                        //sValue = sValue_DOSE;
                    }


                    //1051221 

                    //檢體號
                    string chTubeNo = oAdivaData.SampleID;

                    //儀器數值代碼                    
                    string chMachineMapping = oMachData.chMachineMapping;

                    //數值
                    string chRepValue = sValue;

                    dynamic dymSaveData = new JObject();
                    dymSaveData.Add("tubeNo", chTubeNo);
                    dymSaveData.Add("seqNo", "1"); //非對應預設帶1
                    dymSaveData.Add("macPscCode", chMachineMapping);
                    dymSaveData.Add("repValue", chRepValue);

                    string jSaveData = JsonConvert.SerializeObject(dymSaveData);

                    //回傳數值至HIS系統
                    oLabComm.PostSaveResult(jSaveData);

                    dynamic dymTransFinish = new JObject();
                    dymTransFinish.Add("tubeNo", chTubeNo);
                    dymTransFinish.Add("seqNo", "1"); //非對應預設帶1
                    dymTransFinish.Add("macPscCode", chMachineMapping);
                    dymTransFinish.Add("txTime", DateTime.Now.ToString("yyyy-MM-dd HH:mm"));

                    string jTransFinish = JsonConvert.SerializeObject(dymTransFinish);

                    //回傳完成訊息至HIS系統
                    oLabComm.PostTxResult(jTransFinish);


                    string sTemp = string.Format("開單號:{0},序號:{1},儀器碼:{2},數值:{3}"
                        , oMachData.chLabEmrNo
                        , oMachData.intSeq
                        , oMachData.chMachineMapping
                        , sValue
                        );
                    oLabComm.FeedbackMsg(bw, msgtype.msgGeneral, sTemp);

                    //ssql = " select * from opd.labdrep "
                    //     + " where chlabemrno = '" + oMachData.chLabEmrNo + "' "
                    //     + " and chMachineMapping='" + oMachData.chMachineMapping + "' "
                    //     + " and intSeq='" + oMachData.intSeq + "' "
                    //     + " and chRepValue = '' "
                    //     ;
                    //dt.Clear();
                    //dt = SQL.Get_DataTable(ssql);
                    //if (dt.Rows.Count > 0)
                    //{
                    //    //更新報告報告項目
                    //    string updatetime = DateTime.Now.ToString("yyyyMMddHHmm");

                    //    ssql = " update opd.labdrep set "
                    //         + " chRepValue='" + sValue + "', chValueDttm='" + updatetime + "' "
                    //         + " where chlabemrno = '" + oMachData.chLabEmrNo + "' and chMachineMapping='" + oMachData.chMachineMapping + "' "
                    //         + " and chRepValue = '' and intSeq = '" + oMachData.intSeq + "' ";
                    //    SQL.ExecuteSQL(ssql);

                    //    //更新後記錄於Tubemapping
                    //    ssql = " update opd.labmachinedata set "
                    //         + " chUpdToHisTime='" + TransmitTime + "' "
                    //         + " where chLabEmrNo = '" + oMachData.chLabEmrNo + "' "
                    //         + " and intseq = '" + oMachData.intSeq + "' "
                    //         + " and chtubeno = '" + oMachData.chTubeNo + "' "
                    //         + " and chMachineMapping='" + oMachData.chMachineMapping + "' "
                    //         ;
                    //    SQL.ExecuteSQL(ssql);

                    //    string sTemp = string.Format("開單號:{0},序號:{1},儀器碼:{2},數值:{3}"
                    //        , oMachData.chLabEmrNo
                    //        , oMachData.intSeq
                    //        , oMachData.chMachineMapping
                    //        , sValue
                    //        );
                    //    oLabComm.FeedbackMsg(bw, msgtype.msgGeneral, sTemp);

                    //}

                }


            }

        }

        /// <summary>非線性資料規則
        /// 
        /// </summary>        
        /// <returns></returns>
        private string NonLineValueRule(string MachineMapping,string sValue)
        {
            string sResult = sValue;

            ssql = " select MdfEmp from msutilset "
                     + " where usType = 'LabN' "
                     + " and usKey= 'NLin' "
                     + " and usValue='" + MachineMapping + "' "
                     ;
            string sSetValue = SQL.Get_Scalar(ssql);

            //設定值
            decimal dSetValue = 0;
            //儀器數值
            decimal dValue = 0;

            //設定值轉換有誤，回傳原值
            if (decimal.TryParse(sSetValue, out dSetValue) == false) return sValue;
            //儀器數值轉換有誤，回傳原值
            if (decimal.TryParse(sValue, out dValue) == false) return sValue;

            if (dValue < dSetValue)
            {
                sResult = "<" + dSetValue.ToString();
            }            

            return sResult;
        }

        /// <summary>
        /// Adiva接收儀器資料程序
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="lblStatus"></param>
        /// <returns></returns>
        private AdivaData ReceiveProc(BackgroundWorker bw)
        {
            AdivaData oAdivaData = new AdivaData();
            Boolean bContinue = true;
            byte[] returnMsg = new byte[_mySocket.ReceiveBufferSize];
            StringBuilder sb = new StringBuilder();

            string ssssssss = string.Empty;

            int count = 0;
            int iStatusCount = 0;

            try
            {
                while (bContinue)
                {
                    Thread.Sleep(1000);
                    
                    //前人經驗，先送個ACK
                    _mySocket.Send(Encoding.GetEncoding("Big5").GetBytes("\u0006"));

                    if (_mySocket.Available == 0)
                    {
                        string sMsg = string.Format("接收中..({0})", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
                        oLabComm.FeedbackMsg(bw, msgtype.msgStatus, sMsg);
                        continue;
                    }

                    //歸零
                    count = 0;
                    returnMsg = new byte[_mySocket.ReceiveBufferSize];
                    if ((count = _mySocket.Receive(returnMsg)) != 0)
                    {   
                        //回傳值(04:EOT)=End of Sending
                        //回傳值(05:ENQ)=Inquiry
                        string result = ASCIITOHex(Encoding.ASCII.GetString(returnMsg, 0, count));

                        if (result == "04" || result == "0405")
                        {
                            bContinue = false;
                        }

                        //判斷回傳字串
                        if (result == "04" || result == "05" || result == "0405")
                        {
                            oLabComm.insertlog(LabType, result);
                        }
                        else
                        {
                            string finalReturn = Encoding.GetEncoding("Big5").GetString(returnMsg);
                            ssssssss = finalReturn;
                            //系統紀錄
                            oLabComm.insertlog(LabType, finalReturn);

                            //去除空白
                            finalReturn = finalReturn.Replace("\0", "");
                            //資料格式化
                            if (finalReturn.Length != 0)
                            {
                                if (finalReturn.Length >= 2) finalReturn = finalReturn.Substring(2, finalReturn.Length - 2);
                                if (finalReturn.Length >= 5) finalReturn = finalReturn.Substring(0, finalReturn.Length - 5);

                                //加入StringBuilder
                                sb.Append(finalReturn);
                            }
                        }
                    }
                }

                oLabComm.FeedbackMsg(bw, msgtype.msgStatus, "Socket 資料解析中..");                

                string sData = sb.ToString();

                //格式化後資料寫入log
                oLabComm.insertlog(LabType, "format");
                oLabComm.insertlog(LabType, sData);
                //記錄格式化後的儀器資料
                oLabComm.insertErrlog(LabType, sData);


                oAdivaData = DataConvertToAdivaData(sData);

                //顯示訊息                            
                //oLabComm.FeedbackMsg(bw, msgtype.msgGeneral, "儀器傳送結束。");
            }
            catch (Exception ex)
            {
                //oLabComm.FormMsgShow(obj, "時間:" + DateTime.Now.ToShortTimeString() + " Exception 傳送錯誤: 標籤號:" + orderID + "訊息" + ex.Message + "。" + "\r\n");

                oLabComm.insertlog(LabType, "Error==" + ex.Message);

                return new AdivaData();
            }

            return oAdivaData;
        }

        /// <summary>機器資料格式轉為Adiva物件
        /// 
        /// </summary>
        /// <param name="sData"></param>
        /// <returns></returns>
        private AdivaData DataConvertToAdivaData(string sData)
        {
            AdivaData oAdivaData = new AdivaData();

            var serials = sData.Replace("\n", "").Split('\r').ToList<string>();

            foreach (string item in serials)
            {
                string[] data = item.Split('|');
                //沒資料則跳過
                if (data.Length == 0) continue;
                //取得sample資料(O)_管號
                if (data[0] == "O")
                {
                    if (data.Length == 26) //O26
                    {
                        string[] SampleData = data[2].Split('^');
                        if (SampleData.Length == 3)
                        {
                            //管號
                            oAdivaData.SampleID = SampleData[0];
                            //架號
                            oAdivaData.RackNum = SampleData[1];
                            //位置
                            oAdivaData.SamplePo = SampleData[2];
                        }

                        string[] SampleData4 = data[4].Split('^');
                        if (SampleData4.Length >= 4)
                        {
                            //機器對應碼
                            //oAdivaData.MachineMapping = SampleData4[3];
                        }
                    }
                }

                //取得Record資料(R)_項目結果
                if (data[0] == "R")
                {
                    if (data.Length == 13) //R13
                    {
                        AdivaRecordData oAdivaRecordData = new AdivaRecordData();

                        oAdivaRecordData.DataValue = data[3];
                        oAdivaRecordData.DataUnits = data[4];
                        oAdivaRecordData.DataStatus = data[8];

                        string[] RecordData = data[2].Split('^');
                        if (RecordData.Length == 8) //R3.8
                        {
                            oAdivaRecordData.LIScode = RecordData[3];
                            oAdivaRecordData.ResultAspects = RecordData[7];
                        }

                        oAdivaData.AddRecord(oAdivaRecordData);
                    }
                }

            }

            //紀錄該檢體做的項目清單
            foreach (AdivaRecordData item in oAdivaData.RecordList)
            {
                if (oAdivaData.OrderList.Contains(item.LIScode) == false)
                {
                    oAdivaData.OrderList.Add(item.LIScode);
                }
            }

            return oAdivaData;
        }

        /// <summary>轉換件別代碼
        /// 
        /// </summary>
        /// <param name="chemg"></param>
        /// <returns></returns>
        private string convertOrderStatus(string chemg)
        {
            if (chemg == "Y") { return "S"; }
            return "S";
        }
        /// <summary>儀器是否傳輸成功
        /// 
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
        public void Close()
        {
            closeSymbol = true;

        }

        /// <summary>計算checksum(英文)
        /// 
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
        /// <summary>計算checksum(中文)  有中文或怕有中文的都用這個
        /// 
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
        /// 取得該標籤號的所有檢驗代碼與名稱=>轉換為儀器格式
        /// </summary>
        /// <returns></returns>
        public string getLabItemString(string chtubeno)
        {   
            DataTable dt = SQL.Get_DataTable("select * from opd.tubemapping where chtubeno = '" + chtubeno + "'");
            string result = "";

            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    DataTable detail = SQL.Get_DataTable("select chlabsetid,chdiacode from opd.labemracnt where chlabemrno='" + dt.Rows[i]["chlabemrno"].ToString() + "' and intseq = '" + dt.Rows[i]["intseq"].ToString() + "'");
                    string chlabname = SQL.Get_Scalar("select chlabname from opd.labsetacnt where chlabsetid = '" + detail.Rows[0]["chlabsetid"].ToString() + "' and chlabcode = '" + detail.Rows[0]["chdiacode"].ToString() + "'");

                    if (result != "") { result = result + "~"; }
                    result = detail.Rows[0]["chdiacode"].ToString() + "^" + chlabname;
                }
            }
            else
            {
                return result;
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
        /// 儀器所傳送資料
        /// </summary>
        /// <returns></returns>
        private string receiveMsg(System.Windows.Forms.RichTextBox obj)
        {


            byte[] returnMsg = new byte[_mySocket.ReceiveBufferSize];

            int count = 0;

            try
            {

                //if (_mySocket.Available == 0)
                //{
                //    return "";
                //}


                if ((count = _mySocket.Receive(returnMsg)) != 0)
                {
                    //_mySocket.Send(Encoding.GetEncoding("Big5").GetBytes("\u0006"));
                    //回傳值(04:EOT)=End of Sending
                    //回傳值(05:ENQ)=Inquiry
                    string result = ASCIITOHex(Encoding.ASCII.GetString(returnMsg, 0, count));



                    if (result == "04")
                    {
                        //obj.AppendText("傳送結束。" + "\r\n");
                        //obj.ScrollToCaret();
                    }




                    //判斷回傳字串
                    if (result == "04" || result == "05" || result == "0405")
                    //if (result == "04")
                    {
                        //chlabemrno = "";
                        //orderID = "";
                        //ACK
                        //_mySocket.Send(Encoding.GetEncoding("Big5").GetBytes("\u0006"));
                        //return Encoding.GetEncoding("Big5").GetString(returnMsg);

                        SQL.ExecuteSQL("insert into opd.labinstrumentlog (type,log,time) values('AVDIA','" + result + "','" + DateTime.Now.ToLongDateString() + DateTime.Now.ToLongTimeString() + "')");

                        return "";
                    }
                    else
                    {
                        string finalReturn = Encoding.GetEncoding("Big5").GetString(returnMsg);                        
                        SQL.ExecuteSQL("insert into opd.labinstrumentlog (type,log,time) values('AVDIA','" + finalReturn + "','" + DateTime.Now.ToLongDateString() + DateTime.Now.ToLongTimeString() + "')");
                        //傳儀器訊息回去
                        return finalReturn;
                    }
                }
                //沒收到資料，繼續下一筆
                else
                {
                    return "";
                }
            }
            catch (Exception)
            {

                throw;
            }


        }


        public string test(System.Windows.Forms.Label ADVIAStatus)
        {
            string kind = SQL.Get_Scalar("select log from opd.labinstrumentlog where autoinc = '3810'");
            //string kind1 = SQL.Get_Scalar("select log from opd.labinstrumentlog where autoinc = '4736'");        

            AdivaData oAdivaData = DataConvertToAdivaData(kind);


            BackgroundWorker bw = new BackgroundWorker();
            bw.WorkerReportsProgress = true;
            MachineDataToHis(oAdivaData, bw);
            
            return "";
        }

        private Dictionary<string, string> CreateLabSet()
        {
            Dictionary<string, string> dicList = new Dictionary<string, string>();

            dicList.Add("TSH3UL", "Tp1");
            dicList.Add("PSA", "Tp1");
            dicList.Add("FT4", "Tp1");
            dicList.Add("aTPO", "Tp1");
            dicList.Add("TnIUltra", "Tp1");
            dicList.Add("AFP", "Tp1");
            dicList.Add("T3", "Tp1");
            dicList.Add("BNP", "Tp1");
            dicList.Add("T4", "Tp1");
            dicList.Add("FER", "Tp1");
            dicList.Add("IgE", "Tp1");
            dicList.Add("HCY", "Tp1");
            dicList.Add("PCT", "Tp1");
            dicList.Add("CEA", "Tp1");
            dicList.Add("CA125", "Tp1");
            dicList.Add("CA199", "Tp1");
            dicList.Add("CA153", "Tp1");
            dicList.Add("iPTH", "Tp1");
            dicList.Add("THCG", "Tp1");
            dicList.Add("COR", "Tp1");
            dicList.Add("PCT", "Tp1");

            dicList.Add("aHBs2", "Tp2");
            dicList.Add("RubG", "Tp2");
            dicList.Add("aHAVT", "Tp2");
            dicList.Add("aHAVM", "Tp2");
            dicList.Add("HBs", "Tp3");
            dicList.Add("aHCV", "Tp3");

            return dicList;
        }        

        
    }



}
