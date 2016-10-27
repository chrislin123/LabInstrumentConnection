using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace LabInstrumentConnection
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
        /// <summary>
        /// AVDIA為Socket Client 設計因此在New時不須指定PORT,而是看資料庫設定，PORT資料存在DB中
        /// </summary>
        /// <param name="port"></param>
        public ADIVACentaur(int port = 0)
        {
            //_port = port;
        }
        public void Start(System.Windows.Forms.RichTextBox msgText, System.Windows.Forms.RichTextBox errorText)
        {
            closeSymbol = false;
            EzMySQL SQL = new EzMySQL();
            //string tubeError = "";
            //Centaur IP
            string host = SQL.Get_Scalar("select chparamtext from basedata.systemctrl where chparamcode = 'ADVIACentaurIP'");
            //string host = "127.0.0.1";
            //AVDIA ORDER Port
            int port = int.Parse(SQL.Get_Scalar("select chparamtext from basedata.systemctrl where chparamcode = 'ADVIACentaurPort'"));
            //int port = 8081;           

            //byte[] STX = Encoding.GetEncoding("Big5").GetBytes("\u0002");
            //byte[] ETX = Encoding.GetEncoding("Big5").GetBytes("\u0003");
            //byte[] EOT = Encoding.GetEncoding("Big5").GetBytes("\u0004");
            //byte[] ENQ = Encoding.GetEncoding("Big5").GetBytes("\u0005");
            //byte[] CR = Encoding.GetEncoding("Big5").GetBytes("\r");
            //byte[] ACK = Encoding.GetEncoding("Big5").GetBytes("\u0006");
            //byte[] NAK = Encoding.GetEncoding("Big5").GetBytes("\u0015");

            IPAddress[] IPs = Dns.GetHostAddresses(host);
            _mySocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _mySocket.Connect(IPs[0], port);

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
                        //以防網路不穩
                        if (!_mySocket.Connected)
                        {
                            _mySocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                            _mySocket.Connect(IPs[0], port);
                        }


                        //1.儀器所傳送資料不為空
                        string msg = receiveMsg(msgText);
                        if (msg != "")
                        {
                            string[] data = msg.Split('|');

                            if (data[0].ToString().IndexOf("H", StringComparison.OrdinalIgnoreCase) >= 0)
                            {
                                //ACK
                                //_mySocket.Send(Encoding.GetEncoding("Big5").GetBytes("\u0006"));

                            }

                            if (data[0].ToString().IndexOf("P", StringComparison.OrdinalIgnoreCase) >= 0)
                            {
                                msgText.AppendText("時間:" + DateTime.Now.ToShortTimeString() + "     " + "姓名:" + data[5].ToString() + " 傳送中...。" + "\r\n");
                                msgText.ScrollToCaret();
                                //ACK
                                //_mySocket.Send(Encoding.GetEncoding("Big5").GetBytes("\u0006"));

                            }

                            if (data[0].ToString().IndexOf("BR", StringComparison.OrdinalIgnoreCase) >= 0)
                            {
                                orderID = data[3].ToString();
                                chlabemrno = SQL.Get_Scalar("select chlabemrno from opd.tubemapping where chtubeno = '" + orderID + "'");
                                //ACK
                                //_mySocket.Send(Encoding.GetEncoding("Big5").GetBytes("\u0006"));
                            }

                            if (data[0].ToString().IndexOf("BX", StringComparison.OrdinalIgnoreCase) >= 0)
                            {
                                string chMachineMapping = data[3].ToString().Split('^')[0];
                                SQL.ExecuteSQL("update opd.labdrep set chRepValue='" + data[5].ToString() + "', chUnit='" + data[6].ToString() + "', chAbNormal='', chValueDttm='" + DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString().PadLeft(2, '0') + DateTime.Now.Day.ToString().PadLeft(2, '0') + DateTime.Now.Hour.ToString().PadLeft(2, '0') + DateTime.Now.Minute.ToString().PadLeft(2, '0') + "' where chlabemrno = '" + chlabemrno + "' and chMachineMapping='" + chMachineMapping + "' and chRepValue = '' ");
                                //ACK
                                //_mySocket.Send(Encoding.GetEncoding("Big5").GetBytes("\u0006"));
                                //SQL.ExecuteSQL("insert into opd.labinstrumentlog (type,log,time) values('AVDIA','" + chMachineMapping + "','" + DateTime.Now.ToLongDateString() + DateTime.Now.ToLongTimeString() + "')");

                            }
                        }
                        //前人經驗，先送個ACK
                        _mySocket.Send(Encoding.GetEncoding("Big5").GetBytes("\u0006"));



                        ////這個socket收送一起，因此先收再送 
                        //DataTable dt = SQL.Get_DataTable("select distinct(chtubeno),chlabemrno,chemg from opd.tubemapping where chIsTransmit = 'N' and chischeck != 'C' and chMachineNo in ('" + instructmentCode.C.ToString() + "') ");
                        //if (dt.Rows.Count > 0)
                        //{
                        //    for (int i = 0; i < dt.Rows.Count; i++)
                        //    {
                        //        //tubeError = dt.Rows[i]["chtubeno"].ToString();
                        //        //1. 先送EOT 再送==>ENQ                      
                        //        _mySocket.Send(Encoding.GetEncoding("Big5").GetBytes("\u0004"));
                        //        _mySocket.Send(Encoding.GetEncoding("Big5").GetBytes("\u0005"));
                        //        //<==ACK
                        //        if (!receiveMsgSuccess())
                        //        {
                        //            errorText.AppendText("時間:" + DateTime.Now.ToShortTimeString() + " 1.EOT 傳送錯誤 " + "\r\n");
                        //            errorText.ScrollToCaret();
                        //            continue;
                        //        };

                        //        //2.==> Header   
                        //        //檢查碼
                        //        string checkSum = CalculateChecksumChinese("1H|^~\\&|||||||||||1" + "\r\u0003");
                        //        _mySocket.Send(Encoding.GetEncoding("Big5").GetBytes("\u0002" + "1H|^~\\&|||||||||||1" + "\r\u0003" + checkSum + "\r\n"));
                        //        SQL.ExecuteSQL("insert into opd.labinstrumentlog (type,log,time) values('AVDIA','" + "1H|^~\\&|||||||||||1" + "\r\u0003" + checkSum + "','" + DateTime.Now.ToLongDateString() + DateTime.Now.ToLongTimeString() + "')");

                        //        //<==ACK
                        //        if (!receiveMsgSuccess())
                        //        {
                        //            errorText.AppendText("時間:" + DateTime.Now.ToShortTimeString() + " 2.Header 傳送錯誤 開單號:" + dt.Rows[i]["chtubeno"].ToString() + " 標籤號:" + dt.Rows[i]["chtubeno"].ToString() + "。" + "\r\n");
                        //            errorText.ScrollToCaret();
                        //            continue;
                        //        };

                        //        //3.==> patient   
                        //        //病歷號
                        //        string chmrno = SQL.Get_Scalar("select chmrno from opd.ptlabemr where chlabemrno = '" + dt.Rows[i]["chlabemrno"].ToString() + "'");
                        //        //病患資料
                        //        DataTable patientDT = SQL.Get_DataTable("select * from basedata.patientdata where chmrno = '" + chmrno + "'");
                        //        //性別轉換
                        //        string sex = sexConvert(patientDT.Rows[0]["chsex"].ToString());
                        //        //當天時間
                        //        string today = DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString().PadLeft(2, '0') + DateTime.Now.Day.ToString().PadLeft(2, '0');
                        //        //開單醫師
                        //        string dr = get_apply_dr(dt.Rows[i]["chlabemrno"].ToString());
                        //        //單別
                        //        string kind = get_apply_kind(dt.Rows[i]["chlabemrno"].ToString());
                        //        //地點
                        //        string location = get_apply_location(dt.Rows[i]["chlabemrno"].ToString());
                        //        //檢查碼
                        //        checkSum = CalculateChecksumChinese("2P|1|" + patientDT.Rows[0]["chmrno"].ToString() + "|||" + patientDT.Rows[0]["chname"].ToString() + "||" + patientDT.Rows[0]["chbirthday"].ToString() + "|" + sex + "|||||" + dr + "|||0.0|0.0|||||||" + kind + "|" + location + "|||||||" + today + "||" + "\r\u0003");
                        //        _mySocket.Send(Encoding.GetEncoding("Big5").GetBytes("\u0002" + "2P|1|" + patientDT.Rows[0]["chmrno"].ToString() + "|||" + patientDT.Rows[0]["chname"].ToString() + "||" + patientDT.Rows[0]["chbirthday"].ToString() + "|" + sex + "|||||" + dr + "|||0.0|0.0|||||||" + kind + "|" + location + "|||||||" + today + "||" + "\r\u0003" + checkSum + "\r\n"));
                        //        SQL.ExecuteSQL("insert into opd.labinstrumentlog (type,log,time) values('AVIDA','" + "2P|1|" + patientDT.Rows[0]["chmrno"].ToString() + "|||" + patientDT.Rows[0]["chname"].ToString() + "||" + patientDT.Rows[0]["chbirthday"].ToString() + "|" + sex + "|||||" + dr + "|||0.0|0.0|||||||" + kind + "|" + location + "|||||||" + today + "||" + "\r\u0003" + checkSum + "','" + DateTime.Now.ToLongDateString() + DateTime.Now.ToLongTimeString() + "')");
                        //        //<==ACK
                        //        if (!receiveMsgSuccess())
                        //        {
                        //            errorText.AppendText("時間:" + DateTime.Now.ToShortTimeString() + " 3.Patient 傳送錯誤 開單號:" + dt.Rows[i]["chtubeno"].ToString() + " 標籤號:" + dt.Rows[i]["chtubeno"].ToString() + "。" + "\r\n");
                        //            errorText.ScrollToCaret();
                        //            continue;
                        //        };
                        //        //4.==> ORDER (先CANCEL單，前人經驗)    
                        //        string order = getLabItemString(dt.Rows[i]["chtubeno"].ToString());
                        //        string samplecollectiondt = DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString().PadLeft(2, '0') + DateTime.Now.Day.ToString().PadLeft(2, '0') + DateTime.Now.Hour.ToString().PadLeft(2, '0') + DateTime.Now.Minute.ToString().PadLeft(2, '0') + DateTime.Now.Second.ToString().PadLeft(2, '0');//採檢時間一律以現在時間為準
                        //        string chemg = convertOrderStatus(dt.Rows[i]["chemg"].ToString());
                        //        //檢查碼
                        //        checkSum = CalculateChecksumChinese("3OBR|1|" + dt.Rows[i]["chtubeno"].ToString() + "||" + order + "|" + chemg + "||" + samplecollectiondt + "||||C|||" + samplecollectiondt + "|||||||||||||" + "\r\u0003");
                        //        _mySocket.Send(Encoding.GetEncoding("Big5").GetBytes("\u0002" + "3OBR|1|" + dt.Rows[i]["chtubeno"].ToString() + "||" + order + "|S||" + samplecollectiondt + "||||C|||" + samplecollectiondt + "|||||||||||||" + "\r\u0003" + checkSum + "\r\n"));
                        //        SQL.ExecuteSQL("insert into opd.labinstrumentlog (type,log,time) values('AVIDA','" + "3OBR|1|" + dt.Rows[i]["chtubeno"].ToString() + "||" + order + "|S||" + samplecollectiondt + "||||C|||" + samplecollectiondt + "|||||||||||||" + "','" + DateTime.Now.ToLongDateString() + DateTime.Now.ToLongTimeString() + "')");
                        //        //<==ACK
                        //        if (!receiveMsgSuccess())
                        //        {
                        //            errorText.AppendText("時間:" + DateTime.Now.ToShortTimeString() + " 4.ORDER CANCEL 傳送錯誤 開單號:" + dt.Rows[i]["chtubeno"].ToString() + " 標籤號:" + dt.Rows[i]["chtubeno"].ToString() + "。" + "\r\n");
                        //            errorText.ScrollToCaret();
                        //            continue;
                        //        };

                        //        //4.==> ORDER (再加單)    
                        //        order = getLabItemString(dt.Rows[i]["chtubeno"].ToString());
                        //        //samplecollectiondt = DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString().PadLeft(2, '0') + DateTime.Now.Day.ToString().PadLeft(2, '0') + DateTime.Now.Hour.ToString().PadLeft(2, '0') + DateTime.Now.Minute.ToString().PadLeft(2, '0') + DateTime.Now.Second.ToString().PadLeft(2, '0');//採檢時間一律以現在時間為準
                        //        //檢查碼
                        //        checkSum = CalculateChecksumChinese("4OBR|2|" + dt.Rows[i]["chtubeno"].ToString() + "||" + order + "|" + chemg + "||" + samplecollectiondt + "||||A|||" + samplecollectiondt + "|||||||||||||" + "\r\u0003");
                        //        _mySocket.Send(Encoding.GetEncoding("Big5").GetBytes("\u0002" + "4OBR|2|" + dt.Rows[i]["chtubeno"].ToString() + "||" + order + "|S||" + samplecollectiondt + "||||A|||" + samplecollectiondt + "|||||||||||||" + "\r\u0003" + checkSum + "\r\n"));
                        //        SQL.ExecuteSQL("insert into opd.labinstrumentlog (type,log,time) values('AVIDA','" + "4OBR|2|" + dt.Rows[i]["chtubeno"].ToString() + "||" + order + "|S||" + samplecollectiondt + "||||A|||" + samplecollectiondt + "|||||||||||||" + "','" + DateTime.Now.ToLongDateString() + DateTime.Now.ToLongTimeString() + "')");

                        //        //<==ACK
                        //        if (!receiveMsgSuccess())
                        //        {
                        //            errorText.AppendText("時間:" + DateTime.Now.ToShortTimeString() + " 4.ORDER ADD 傳送錯誤 開單號:" + dt.Rows[i]["chtubeno"].ToString() + " 標籤號:" + dt.Rows[i]["chtubeno"].ToString() + "。" + "\r\n");
                        //            errorText.ScrollToCaret();
                        //            continue;
                        //        };
                        //        //檢查碼
                        //        checkSum = CalculateChecksumChinese("5L|1||1|4\r\u0003");

                        //        //5.==>結束符號 5L|1||2|8
                        //        _mySocket.Send(Encoding.GetEncoding("Big5").GetBytes("\u0002" + "5L|1||1|4\r\u0003" + checkSum + "\r\n"));
                        //        SQL.ExecuteSQL("insert into opd.labinstrumentlog (type,log,time) values('AVIDA','" + "5L|1||1|4\r\u0003" + checkSum + "','" + DateTime.Now.ToLongDateString() + DateTime.Now.ToLongTimeString() + "')");
                        //        //<==ACK
                        //        if (!receiveMsgSuccess())
                        //        {
                        //            errorText.AppendText("時間:" + DateTime.Now.ToShortTimeString() + " 5.End Mark 傳送錯誤 開單號:" + dt.Rows[i]["chtubeno"].ToString() + " 標籤號:" + dt.Rows[i]["chtubeno"].ToString() + "。" + "\r\n");
                        //            errorText.ScrollToCaret();
                        //            continue;
                        //        };
                        //        //結束符號 EOT
                        //        _mySocket.Send(Encoding.GetEncoding("Big5").GetBytes("\u0004"));
                        //        //處理完畢後，刪除所有處理資料,並斷開連結
                        //        //_mySocket.Close();

                        //        msgText.AppendText("傳送時間:" + DateTime.Now.Year.ToString() + "年" + DateTime.Now.Month.ToString().PadLeft(2, '0') + "月" + DateTime.Now.Day.ToString().PadLeft(2, '0') + "日" + DateTime.Now.Hour.ToString().PadLeft(2, '0') + "時" + DateTime.Now.Minute.ToString().PadLeft(2, '0') + "分" + DateTime.Now.Second.ToString().PadLeft(2, '0') + "秒" + "     " + "標籤號:" + dt.Rows[i]["chtubeno"].ToString() + "傳送成功。" + "\r\n");
                        //        msgText.ScrollToCaret();
                        //        //將標籤號註記已上傳
                        //        SQL.ExecuteSQL("update opd.tubemapping set chistransmit = 'Y', chtransmittime = '" + samplecollectiondt + "' where chtubeno = '" + dt.Rows[i]["chtubeno"].ToString() + "'");
                        //    }
                        //}
                    }
                    catch (Exception ex)
                    {
                        errorText.AppendText("時間:" + DateTime.Now.ToShortTimeString() + " Exception 傳送錯誤: 標籤號:" + orderID + "訊息" + ex.Message + "。" + "\r\n");
                        errorText.ScrollToCaret();
                        SQL.ExecuteSQL("insert into opd.labinstrumentlog (type,log,time) values('AVDIA','" + ex.Message + "','" + DateTime.Now.ToLongDateString() + DateTime.Now.ToLongTimeString() + "')");
                        //_mySocket.Send(Encoding.GetEncoding("Big5").GetBytes("\u0015"));
                        continue;
                    }
                }
            }
            catch (Exception ex)
            {
                errorText.AppendText("時間:" + DateTime.Now.ToShortTimeString() + " Exception 傳送錯誤: 標籤號:" + orderID + "訊息" + ex.Message + "。" + "\r\n");
                errorText.ScrollToCaret();
                SQL.ExecuteSQL("insert into opd.labinstrumentlog (type,log,time) values('AVDIA','" + ex.Message + "','" + DateTime.Now.ToLongDateString() + DateTime.Now.ToLongTimeString() + "')");

                // _mySocket.Send(Encoding.GetEncoding("Big5").GetBytes("\u0015"));              
            }
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
        public void Close()
        {
            closeSymbol = true;

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
            EzMySQL SQL = new EzMySQL();

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
            EzMySQL SQL = new EzMySQL();

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
            EzMySQL SQL = new EzMySQL();

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
            EzMySQL SQL = new EzMySQL();
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
        public string receiveMsg(System.Windows.Forms.RichTextBox obj)
        {
            byte[] returnMsg = new byte[_mySocket.ReceiveBufferSize];

            int count = 0;
            if ((count = _mySocket.Receive(returnMsg)) != 0)
            {
                //_mySocket.Send(Encoding.GetEncoding("Big5").GetBytes("\u0006"));
                //回傳值(04:EOT)
                string result = ASCIITOHex(Encoding.ASCII.GetString(returnMsg, 0, count));
                if (result == "04")
                {
                    obj.AppendText("傳送結束。" + "\r\n");
                    obj.ScrollToCaret();
                }
                //判斷回傳字串
                if (result == "04" || result == "05")
                //if (result == "04")
                {
                    //chlabemrno = "";
                    //orderID = "";
                    //ACK
                    //_mySocket.Send(Encoding.GetEncoding("Big5").GetBytes("\u0006"));
                    //return Encoding.GetEncoding("Big5").GetString(returnMsg);
                    return "";
                }
                else
                {
                    string finalReturn = Encoding.GetEncoding("Big5").GetString(returnMsg);
                    EzMySQL SQL = new EzMySQL();
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
    }
}
