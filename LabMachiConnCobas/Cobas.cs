using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using LabMachiLib;
using System.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http;

namespace LabMachiConnCobas
{
    public class Cobas
    {
        LabComm oLabComm = new LabComm(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
        EzMySQL SQL = new EzMySQL(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);

        //廠商類別
        string sFirmType = "Cobas";

        // 宣告客戶端清單
        public static List<CobasClient> ClientList;
        public static List<CobasClientNew> ClientListNew;
        int _port = 0;
        Socket _ListenSocket;
        bool closeSymbol = false;
        public Cobas(int port)
        {
            _port = port;
        }
        public void Start(System.Windows.Forms.RichTextBox msgText, System.Windows.Forms.RichTextBox errorText)
        {
            closeSymbol = false;

            // 設定連線接聽阜值
            int ServerPort = _port;

            // 設定半開連接數
            const int BackLog = 1800;

            // 初始化客戶端清單
            ClientList = new List<CobasClient>();

            // 初始化客戶端序列號
            int PlayerNum = 0;
            // 建立接聽Socket
            _ListenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            // 綁定接聽Socket的IP與Port
            _ListenSocket.Bind(new IPEndPoint(IPAddress.Any, ServerPort));

            // 開始接聽
            _ListenSocket.Listen(BackLog);

            // 輸出伺服器端起始訊息
            //Console.WriteLine("開始接聽客戶端連線...");



            // 持續等待客戶端接入
            while (true)
            {
                if (closeSymbol == true) { break; }
                try
                {
                    ClientList.Add(new CobasClient(PlayerNum, _ListenSocket.Accept(), msgText, errorText));
                    PlayerNum++;
                }
                catch (Exception ex)
                {
                    break;
                }
            }


        }

        public void StartNew(System.Windows.Forms.RichTextBox msgText, System.Windows.Forms.RichTextBox errorText)
        {
            closeSymbol = false;

            // 設定連線接聽阜值
            int ServerPort = _port;

            // 設定半開連接數
            const int BackLog = 1800;

            // 初始化客戶端清單
            ClientListNew = new List<CobasClientNew>();

            // 初始化客戶端序列號
            int PlayerNum = 0;
            // 建立接聽Socket
            _ListenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            // 綁定接聽Socket的IP與Port
            _ListenSocket.Bind(new IPEndPoint(IPAddress.Any, ServerPort));

            // 開始接聽
            _ListenSocket.Listen(BackLog);

            // 輸出伺服器端起始訊息
            //Console.WriteLine("開始接聽客戶端連線...");



            // 持續等待客戶端接入
            while (true)
            {
                if (closeSymbol == true) { break; }
                try
                {
                    ClientListNew.Add(new CobasClientNew(PlayerNum, _ListenSocket.Accept(), msgText, errorText));
                    PlayerNum++;
                }
                catch (Exception ex)
                {
                    break;
                }
            }


        }



        public static void BoardCast(string serverResponse)
        {
            foreach (CobasClient myClient in Cobas.ClientList)
            {
                myClient.SendToPlayer(serverResponse);
                //Console.WriteLine("廣播訊息：{0}", serverResponse);
            }
        }

        /// <summary>
        /// 移除CobasClient清單的項目
        /// </summary>
        /// <param name="PlayerNum"></param>
        public static void RemoveClient(int PlayerNum)
        {
            foreach (CobasClient myClient in Cobas.ClientList)
            {
                if (myClient.CurrentPlayerNum.Equals(PlayerNum))
                {
                    Cobas.ClientList.Remove(myClient);
                    break;
                }
            }
        }

        public void Close()
        {
            if (_ListenSocket == null)
            {
                return;
            }
            else
            {
                oLabComm.CloseSocket(_ListenSocket);
                closeSymbol = true;
                //_ListenSocket.Close();
                //_ListenSocket.Dispose();

            }
        }
    }

    public class CobasClient
    {
        LabComm oLabComm = new LabComm(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
        EzMySQL SQL = new EzMySQL(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
        string ssql = string.Empty;
        //廠商類別
        string sFirmType = "Cobas";

        public int CurrentPlayerNum { get; set; }
        public Socket PlayerSocket { get; set; }
        public int CurrentStatus { get; set; }
        public System.Windows.Forms.RichTextBox currenrMsgText;
        public System.Windows.Forms.RichTextBox currentErrorText;
        public CobasClient(int PlayerNum, Socket mySocket, System.Windows.Forms.RichTextBox msgText, System.Windows.Forms.RichTextBox errorText)
        {
            this.CurrentPlayerNum = PlayerNum;
            this.PlayerSocket = mySocket;
            this.CurrentStatus = 1;
            this.currenrMsgText = msgText;
            this.currentErrorText = errorText;
            DoCommunicate();
        }

        public void DoCommunicate()
        {
            //產生 BackgroundWorker 負責處理每一個 socket client 的 reuqest
            BackgroundWorker bgwSocket = new BackgroundWorker();
            bgwSocket.DoWork += new DoWorkEventHandler(bgwSocket_DoWork);
            bgwSocket.RunWorkerAsync();
        }

        private void bgwSocket_DoWork(object sender, DoWorkEventArgs e)
        {
            string orderID = "";//標籤號
            string transmitTime = "";//傳送時間
            try
            {
                while (PlayerSocket.Connected)
                {
                    byte[] readBuffer = new byte[PlayerSocket.ReceiveBufferSize];
                    int count = 0;

                    if ((count = PlayerSocket.Receive(readBuffer)) > 0)
                    {
                        string clientRequest = Encoding.UTF8.GetString(readBuffer, 0, count);
                        //極重要 發報告轉檔=========================================================================
                        //處理換行，"\r", "\rO", "\rR"
                        string[] breakLine = new string[] { "\r", "\rO", "\rR" };
                        string[] breakLineDataString = clientRequest.Split(breakLine, System.StringSplitOptions.None);


                        //每行數值
                        for (int i = 0; i < breakLineDataString.Length; i++)
                        {
                            oLabComm.insertlog(sFirmType, breakLineDataString[i]);
                            //SQL.ExecuteSQL("insert into opd.labinstrumentlog (type,log,time) values('Cobas','" + breakLineDataString + "','" + DateTime.Now.ToLongDateString() + DateTime.Now.ToLongTimeString() + "')");

                            string[] detailData = breakLineDataString[i].Split('|');

                            //處理傳送時參數
                            if (detailData[0] == "H")
                            {
                                //並不是檢驗結果回傳數值  ex:有可能是回傳校正值... 等等
                                if (detailData[10] != "M")
                                {
                                    //將最重要的參數再次設預設值
                                    orderID = "";//標籤號
                                    transmitTime = "";//傳送時間

                                    break;
                                }
                                transmitTime = detailData[13];
                            }
                            //處理病患參數 ==>暫時不處理
                            if (detailData[0] == "P")
                            {

                            }

                            //處理標籤號
                            if (detailData[0] == "O")
                            {
                                orderID = detailData[3].Split('^')[0];
                                oLabComm.FormMsgShow(currenrMsgText, string.Format(" 標籤號:{0} 處理中", orderID));

                            }

                            //處理回傳數值 並且最重要的2個參數不能為空值
                            if (detailData[0] == "R" && transmitTime != "" && orderID != "")
                            {
                                //檢驗科主任要求，不簽收也要能上機
                                ssql = "select * from opd.TubeMapping where chTubeNo='" + orderID + "'";
                                DataTable dt = SQL.Get_DataTable(ssql);

                                //找到對應標籤號單子
                                if (dt.Rows.Count > 0)
                                {
                                    //工作單號   
                                    string chlabemrno = dt.Rows[0]["chlabemrno"].ToString();
                                    //工作單號   
                                    string chintSeq = dt.Rows[0]["intSeq"].ToString();
                                    //儀器數值代碼的斷行符號
                                    string[] chMachineMappingBreak = new string[] { "^^^" };
                                    //儀器數值代碼
                                    string chMachineMapping = detailData[2].Split(chMachineMappingBreak, System.StringSplitOptions.None)[1];
                                    string chAbNormal = "";
                                    //數值
                                    string chRepValue = detailData[3];
                                    float fRepValue = 0;
                                    //單位
                                    string chUnit = detailData[4];

                                    //***找到對應標籤號單子的報告數值資料,並判斷數值是否正常或異常***
                                    ssql = "select * from opd.labdrep where chlabemrno='{0}' and chMachineMapping='{1}' and intSeq='{2}' ";
                                    ssql = string.Format(ssql, chlabemrno, chMachineMapping, chintSeq);


                                    DataTable dt0 = SQL.Get_DataTable(ssql);
                                    if (dt0.Rows.Count > 0)
                                    {
                                        //int.TryParse(chRepValue, out intRepValue) == true
                                        ////如果區間值不為空白 且 回傳的數值為數值資料 =>進行報告數值判斷     **目前暫時不判斷非數值的回傳值**
                                        //if (dt0.Rows[0]["chNormal"].ToString() != "" && float.TryParse(chRepValue, out fRepValue) == true)
                                        //{
                                        //    string[] compare = dt0.Rows[0]["chNormal"].ToString().Split('~');
                                        //    float fNo1 = 0;
                                        //    float fNo2 = 0;
                                        //    if (float.TryParse(compare[0], out fNo1) == true && float.TryParse(compare[1], out fNo2) == true)
                                        //    {
                                        //        if (fRepValue >= fNo1 && fRepValue <= fNo2)
                                        //        {
                                        //            chAbNormal = "N";
                                        //        }
                                        //        else
                                        //        {
                                        //            chAbNormal = "Y";
                                        //        }
                                        //    }
                                        //}


                                        ssql = "update opd.labdrep set chRepValue='" + chRepValue + "', chUnit='" + chUnit + "', chAbNormal='" + chAbNormal + "', chValueDttm='" + transmitTime + "' where chlabemrno = '" + chlabemrno + "' and chMachineMapping='" + chMachineMapping + "' and intSeq='" + chintSeq + "' and chRepValue = '' ";
                                        SQL.ExecuteSQL(ssql);

                                        oLabComm.FormMsgShow(currenrMsgText, string.Format(" 標籤號:{0} 處理完畢", orderID));
                                    }
                                    else
                                    {
                                        //代表機器所吐出的數值，檢驗科不接
                                        continue;
                                    }
                                }
                                else
                                {
                                    //沒對應標籤號，代表沒有簽收或根本找不到管子 
                                    //將最重要的參數再次設預設值
                                    //標籤號
                                    orderID = "";
                                    //傳送時間
                                    transmitTime = "";

                                    oLabComm.FormMsgShow(currenrMsgText, string.Format(" 標籤號:{0} 找不到對應單號。", orderID));

                                    break;
                                }

                            }

                            //遇到結束縛號清掉
                            if (detailData[0] == "L")
                            {
                                transmitTime = "";
                                orderID = "";
                            }
                        }

                        //===========================================================================================
                        //ACK                        
                        PlayerSocket.Send(Encoding.GetEncoding("Big5").GetBytes("\u0006"));
                    }
                }
            }
            catch (Exception ex)
            {
                oLabComm.FormMsgShow(currentErrorText, string.Format(" 標籤號:{0} 處理失敗。訊息:{1}", orderID, ex.Message));

                oLabComm.insertlog(sFirmType, ex.Message);
            }
            finally
            {
                transmitTime = "";
                orderID = "";
                PlayerSocket.Close();
                this.CurrentStatus = 0;
                Cobas.RemoveClient(this.CurrentPlayerNum);
            }
        }

        public int SendToPlayer(string serverResponse)
        {
            if (this.CurrentStatus.Equals(1))
            {
                try
                {
                    byte[] sendBytes = Encoding.UTF8.GetBytes(serverResponse);
                    return PlayerSocket.Send(sendBytes);
                }
                catch
                {
                    return 0;
                }
            }
            else
                return 0;
        }
    }

    public class CobasClientNew
    {
        LabComm oLabComm = new LabComm(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
        EzMySQL SQL = new EzMySQL(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
        string ssql = string.Empty;
        //廠商類別
        string sFirmType = "Cobas";

        public int CurrentPlayerNum { get; set; }
        public Socket PlayerSocket { get; set; }
        public int CurrentStatus { get; set; }
        public System.Windows.Forms.RichTextBox currenrMsgText;
        public System.Windows.Forms.RichTextBox currentErrorText;
        public CobasClientNew(int PlayerNum, Socket mySocket, System.Windows.Forms.RichTextBox msgText, System.Windows.Forms.RichTextBox errorText)
        {
            this.CurrentPlayerNum = PlayerNum;
            this.PlayerSocket = mySocket;
            this.CurrentStatus = 1;
            this.currenrMsgText = msgText;
            this.currentErrorText = errorText;
            DoCommunicate();
        }

        public void DoCommunicate()
        {
            //產生 BackgroundWorker 負責處理每一個 socket client 的 reuqest
            BackgroundWorker bgwSocket = new BackgroundWorker();
            bgwSocket.DoWork += new DoWorkEventHandler(bgwSocket_DoWork);
            bgwSocket.RunWorkerAsync();
        }

        private void bgwSocket_DoWork(object sender, DoWorkEventArgs e)
        {
            string orderID = "";//標籤號
            string transmitTime = "";//傳送時間
            try
            {
                while (PlayerSocket.Connected)
                {
                    byte[] readBuffer = new byte[PlayerSocket.ReceiveBufferSize];
                    int count = 0;

                    if ((count = PlayerSocket.Receive(readBuffer)) > 0)
                    {
                        string clientRequest = Encoding.UTF8.GetString(readBuffer, 0, count);
                        //極重要 發報告轉檔=========================================================================
                        //處理換行，"\r", "\rO", "\rR"
                        string[] breakLine = new string[] { "\r", "\rO", "\rR" };
                        string[] breakLineDataString = clientRequest.Split(breakLine, System.StringSplitOptions.None);


                        //每行數值
                        for (int i = 0; i < breakLineDataString.Length; i++)
                        {
                            oLabComm.insertlog(sFirmType, breakLineDataString[i]);
                            //SQL.ExecuteSQL("insert into opd.labinstrumentlog (type,log,time) values('Cobas','" + breakLineDataString + "','" + DateTime.Now.ToLongDateString() + DateTime.Now.ToLongTimeString() + "')");

                            string[] detailData = breakLineDataString[i].Split('|');

                            //處理傳送時參數
                            if (detailData[0] == "H")
                            {
                                //並不是檢驗結果回傳數值  ex:有可能是回傳校正值... 等等
                                if (detailData[10] != "M")
                                {
                                    //將最重要的參數再次設預設值
                                    orderID = "";//標籤號
                                    transmitTime = "";//傳送時間

                                    break;
                                }
                                transmitTime = detailData[13];
                            }
                            //處理病患參數 ==>暫時不處理
                            if (detailData[0] == "P")
                            {

                            }

                            //處理標籤號
                            if (detailData[0] == "O")
                            {
                                orderID = detailData[3].Split('^')[0];
                                oLabComm.FormMsgShow(currenrMsgText, string.Format(" 標籤號:{0} 處理中", orderID));

                            }

                            //處理回傳數值 並且最重要的2個參數不能為空值
                            if (detailData[0] == "R" && transmitTime != "" && orderID != "")
                            {

                                //儀器數值代碼
                                string[] chMachineMappingBreak = new string[] { "^^^" };
                                string chMachineMapping = detailData[2].Split(chMachineMappingBreak, System.StringSplitOptions.None)[1];

                                //數值
                                string chRepValue = detailData[3];
                                
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

                                //數值非空值寫入記錄檔
                                if (chRepValue != "")
                                {
                                    oLabComm.insertlog(sFirmType, string.Format("檢體號：{0},機器碼：{1},數值：{2}", orderID, chMachineMapping, chRepValue));
                                }
                            }

                            //遇到結束縛號清掉
                            if (detailData[0] == "L")
                            {
                                transmitTime = "";
                                orderID = "";
                            }
                        }

                        //回報畫面處理完畢
                        oLabComm.FormMsgShow(currenrMsgText, string.Format(" 標籤號:{0} 處理完畢", orderID));

                        //===========================================================================================
                        //ACK                        
                        PlayerSocket.Send(Encoding.GetEncoding("Big5").GetBytes("\u0006"));
                    }
                }
            }
            catch (Exception ex)
            {
                oLabComm.FormMsgShow(currentErrorText, string.Format(" 標籤號:{0} 處理失敗。訊息:{1}", orderID, ex.Message));

                oLabComm.insertlog(sFirmType, ex.Message);
            }
            finally
            {
                transmitTime = "";
                orderID = "";
                PlayerSocket.Close();
                this.CurrentStatus = 0;
                Cobas.RemoveClient(this.CurrentPlayerNum);
            }
        }

        public int SendToPlayer(string serverResponse)
        {
            if (this.CurrentStatus.Equals(1))
            {
                try
                {
                    byte[] sendBytes = Encoding.UTF8.GetBytes(serverResponse);
                    return PlayerSocket.Send(sendBytes);
                }
                catch
                {
                    return 0;
                }
            }
            else
                return 0;
        }
    }

}
