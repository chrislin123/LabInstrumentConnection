using System;
using System.Collections.Generic;
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
using System.ComponentModel;

namespace LabMachiConnHCLAB
{
    public class HCLABReceive
    {
        LabComm oLabComm = new LabComm(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
        EzMySQL SQL = new EzMySQL(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
                
        //廠商類別
        string sFirmType = "HCLABReceive";
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
        string orderID = "";//標籤號
        string chlabemrno = "";//開單號
        /// <summary>
        /// HCLAB為Socket Client 設計因此在New時不須指定PORT,而是看資料庫設定，PORT資料存在DB中
        /// </summary>
        /// <param name="port"></param>
        public HCLABReceive(int port = 0)
        {
            //_port = port;
        }
        public void Start(System.Windows.Forms.RichTextBox msgText, System.Windows.Forms.RichTextBox errorText)
        {
            closeSymbol = false;
            //EzMySQL SQL = new EzMySQL();

            //HCLAB IP
            string host = SQL.Get_Scalar("select chparamtext from basedata.systemctrl where chparamcode = 'HCLABIP'");
            //string host = "127.0.0.1";
            //HCLAB ORDER Port
            int port = int.Parse(SQL.Get_Scalar("select chparamtext from basedata.systemctrl where chparamcode = 'HCLABResultPort'"));
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

                        //3秒執行一次
                        //Thread.Sleep(3000);
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
                                //msgText.AppendText("時間:" + DateTime.Now.ToShortTimeString() + "     " + "姓名:" + data[5].ToString() + " 傳送中...。" + "\r\n");
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
                                SQL.ExecuteSQL("insert into opd.labinstrumentlog (type,log,time) values('HCLABReceive','" + chMachineMapping + "','" + DateTime.Now.ToLongDateString() + DateTime.Now.ToLongTimeString() + "')");

                            }
                        }
                        //前人經驗，先送個ACK
                        _mySocket.Send(Encoding.GetEncoding("Big5").GetBytes("\u0006"));
                    }
                    catch (Exception ex)
                    {
                        errorText.AppendText("時間:" + DateTime.Now.ToShortTimeString() + " Exception 傳送錯誤: 標籤號:" + orderID + "訊息" + ex.Message + "。" + "\r\n");
                        errorText.ScrollToCaret();
                        //SQL.ExecuteSQL("insert into opd.labinstrumentlog (type,log,time) values('HCLABReceive','" + ex.Message + "','" + DateTime.Now.ToLongDateString() + DateTime.Now.ToLongTimeString() + "')");
                        oLabComm.insertlog(sFirmType, ex.Message);
                        //_mySocket.Send(Encoding.GetEncoding("Big5").GetBytes("\u0015"));
                        continue;
                    }
                }
            }
            catch (Exception ex)
            {
                errorText.AppendText("時間:" + DateTime.Now.ToShortTimeString() + " Exception 傳送錯誤: 標籤號:" + orderID + "訊息" + ex.Message + "。" + "\r\n");
                errorText.ScrollToCaret();
                //SQL.ExecuteSQL("insert into opd.labinstrumentlog (type,log,time) values('HCLABReceive','" + ex.Message + "','" + DateTime.Now.ToLongDateString() + DateTime.Now.ToLongTimeString() + "')");
                oLabComm.insertlog(sFirmType, ex.Message);
               // _mySocket.Send(Encoding.GetEncoding("Big5").GetBytes("\u0015"));              
            }
        }

        public void StartNew(BackgroundWorker bw)
        {
            closeSymbol = false;
            
            //HCLAB IP            
            //1051223 配合新系統改寫，寫死資料
            string host = "192.168.12.25";            
            
            //HCLAB ORDER Port            
            //1051223 配合新系統改寫，寫死資料
            int port = 9082;           

            IPAddress[] IPs = Dns.GetHostAddresses(host);
            _mySocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _mySocket.Connect(IPs[0], port);

            try
            {
                while (true)
                {
                    //測試
                    Thread.Sleep(500);                    

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

                        //3秒執行一次
                        //Thread.Sleep(3000);
                        //1.儀器所傳送資料不為空
                        string msg = receiveMsgNew();
                        //string msg = "P|||||測試病患";
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
                                oLabComm.FeedbackMsg(bw, msgtype.msgGeneral, string.Format("姓名:{0} 傳送中...。", data[5].ToString()));
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
                                //string chMachineMapping = data[3].ToString().Split('^')[0];
                                //SQL.ExecuteSQL("update opd.labdrep set chRepValue='" + data[5].ToString() + "', chUnit='" + data[6].ToString() + "', chAbNormal='', chValueDttm='" + DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString().PadLeft(2, '0') + DateTime.Now.Day.ToString().PadLeft(2, '0') + DateTime.Now.Hour.ToString().PadLeft(2, '0') + DateTime.Now.Minute.ToString().PadLeft(2, '0') + "' where chlabemrno = '" + chlabemrno + "' and chMachineMapping='" + chMachineMapping + "' and chRepValue = '' ");                                
                                //SQL.ExecuteSQL("insert into opd.labinstrumentlog (type,log,time) values('HCLABReceive','" + chMachineMapping + "','" + DateTime.Now.ToLongDateString() + DateTime.Now.ToLongTimeString() + "')");

                                //1051223 配合新系統改寫，呼叫Node.js API
                                //儀器數值代碼                                
                                string chMachineMapping = data[3].ToString().Split('^')[0]; 

                                //數值
                                string chRepValue = data[5].ToString();

                                dynamic dymSaveData = new JObject();
                                dymSaveData.Add("tubeNo", orderID);
                                dymSaveData.Add("seqNo", "1"); //非對應預設帶1
                                dymSaveData.Add("macPscCode", chMachineMapping);
                                dymSaveData.Add("repValue", chRepValue);

                                string jSaveData = JsonConvert.SerializeObject(dymSaveData);

                                //回傳數值至HIS系統
                                oLabComm.PostSaveResult(jSaveData);

                                dynamic dymTransFinish = new JObject();
                                dymTransFinish.Add("tubeNo", orderID);
                                dymTransFinish.Add("seqNo", "1"); //非對應預設帶1
                                dymTransFinish.Add("macPscCode", chMachineMapping);
                                dymTransFinish.Add("txTime", DateTime.Now.ToString("yyyy-MM-dd HH:mm"));

                                string jTransFinish = JsonConvert.SerializeObject(dymTransFinish);

                                //回傳完成訊息至HIS系統
                                oLabComm.PostTxResult(jTransFinish);

                            }
                        }

                        //前人經驗，先送個ACK
                        _mySocket.Send(Encoding.GetEncoding("Big5").GetBytes("\u0006"));
                    }
                    catch (Exception ex)
                    {
                        oLabComm.FeedbackMsg(bw, msgtype.msgErr, string.Format("Exception 傳送錯誤: 標籤號:{0} 訊息{1}。", orderID, ex.Message));                        
                        oLabComm.insertlog(sFirmType, ex.Message);
                        
                        continue;
                    }
                }
            }
            catch (Exception ex)
            {
                oLabComm.FeedbackMsg(bw, msgtype.msgErr, string.Format("Exception 傳送錯誤: 標籤號:{0} 訊息{1}。", orderID, ex.Message));                        
                oLabComm.insertlog(sFirmType, ex.Message);                
            }
        }


        /// <summary>
        /// 儀器所傳送資料
        /// </summary>
        /// <returns></returns>
        public string receiveMsgNew()
        {
            byte[] returnMsg = new byte[_mySocket.ReceiveBufferSize];

            try
            {
                int count = 0;
                if ((count = _mySocket.Receive(returnMsg)) != 0)
                {
                    //_mySocket.Send(Encoding.GetEncoding("Big5").GetBytes("\u0006"));
                    //回傳值(04:EOT)
                    string result = ASCIITOHex(Encoding.ASCII.GetString(returnMsg, 0, count));
                    if (result == "04")
                    {
                        //obj.AppendText("傳送結束。" + "\r\n");
                        //obj.ScrollToCaret();
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
                        //EzMySQL SQL = new EzMySQL();
                        SQL.ExecuteSQL("insert into opd.labinstrumentlog (type,log,time) values('HCLABReceive','" + finalReturn + "','" + DateTime.Now.ToLongDateString() + DateTime.Now.ToLongTimeString() + "')");
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
            catch (Exception ex)
            {

                throw;
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

            try
            {
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
                        //EzMySQL SQL = new EzMySQL();
                        SQL.ExecuteSQL("insert into opd.labinstrumentlog (type,log,time) values('HCLABReceive','" + finalReturn + "','" + DateTime.Now.ToLongDateString() + DateTime.Now.ToLongTimeString() + "')");
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
            catch (Exception ex)
            {
                
                throw;
            }

            
        }
    }
}
