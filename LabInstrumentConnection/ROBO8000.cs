using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LabInstrumentConnection
{
    public class ROBO8000
    {
        LabComm oLabComm = new LabComm();
        //廠商類別
        string sFirmType = "robo8000";

        Socket _mySocket;
        int _port = 0;
        bool closeSymbol = false;
        /// <summary>
        /// 備管機為Socket Client 設計因此在New時不須指定PORT,而是看資料庫設定，PORT資料存在DB中
        /// </summary>
        /// <param name="port"></param>
        public ROBO8000(int port = 0)
        {
            //_port = port;
        }
        public void Start(System.Windows.Forms.RichTextBox msgText, System.Windows.Forms.RichTextBox errorText)
        {
            closeSymbol = false;
            string fileName = "";
            string cn = "server=192.168.150.53;user id=hisuser;Password=ca@2014;persist security info=True;database=BASEDATA";
            string cmd = "select * from basedata.systemctrl where chparamcode = 'LabTubeIP';";

            MySqlDataAdapter da = new MySqlDataAdapter(cmd, cn);
            DataTable dt = new DataTable();
            da.Fill(dt);
            //備管機IP
            //string host = dt.Rows[0]["chParamText"].ToString();
            string host = "192.168.12.27";
            dt.Clear();
            da.SelectCommand.CommandText = "select * from basedata.systemctrl where chparamcode = 'LabTubePort';";
            da.Fill(dt);
            //備管機port
            //int port = int.Parse(dt.Rows[0]["chParamText"].ToString());
            int port = 19999;


            byte[] STX = Encoding.GetEncoding("Big5").GetBytes("\u0002");
            byte[] ETX = Encoding.GetEncoding("Big5").GetBytes("\u0003");
            byte[] CR = Encoding.GetEncoding("Big5").GetBytes("\r");
            byte[] ACK = Encoding.GetEncoding("Big5").GetBytes("\u0006");
            byte[] NAK = Encoding.GetEncoding("Big5").GetBytes("\u0015");


            //Console.WriteLine("開始連接.");
            IPAddress[] IPs = Dns.GetHostAddresses(host);
            //Console.WriteLine("正在連接至 {0}:{1} ...", host, port);

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
                        //1秒執行一次
                        Thread.Sleep(1000);
                        TubeFtpFileInfo FtpInfo = new TubeFtpFileInfo();
                        List<string> fileList = FtpInfo.FileList();
                        fileName = "";//清空檔名
                        //確定有檔案需要執行 再開socket
                        if (fileList.Count > 0)
                        {
                            EzMySQL SQL = new EzMySQL();
                            _mySocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                            _mySocket.Connect(IPs[0], port);

                            //確定檔案已在處理結束狀態
                            //_mySocket.Send(Encoding.GetEncoding("Big5").GetBytes("\u0002" + "9" + "                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                           " + "\u0003"));


                            //取得檔案清單逐筆執行
                            foreach (string item in fileList)
                            {
                                //廠商說傳太快，擋個200毫秒
                                Thread.Sleep(50);
                                fileName = item;
                                string[] orderList = FtpInfo.FileContent(item);
                                //取得檔案內容後，逐筆執行檔案內容的指令
                                foreach (string order in orderList)
                                {
                                    //SQL INJECTION
                                    //SQL.ExecuteSQL("insert into opd.labinstrumentlog (type,log,time) values('robo8000','" + order + "','" + DateTime.Now.ToLongDateString() + DateTime.Now.ToLongTimeString() + "')");
                                    SQL.ExecuteSQL("insert into opd.labinstrumentlog (type,log,time) values('robo8000','" + " 檔案" + item + "處理成功。" + "','" + DateTime.Now.ToLongDateString() + DateTime.Now.ToLongTimeString() + "')");


                                    int byteCount = _mySocket.Send(Encoding.GetEncoding("Big5").GetBytes(order));

                                    byte[] readBuffer = new byte[_mySocket.ReceiveBufferSize];
                                    int count = 0;
                                    if ((count = _mySocket.Receive(readBuffer)) != 0)
                                    {
                                        //在此決定傳輸失敗時的做法，目前尚未處理，相信廠商的機器「應該」都很穩定
                                        string clientRequest = Encoding.ASCII.GetString(readBuffer, 0, count).Substring(0, 14);
                                        if (clientRequest != "31000000000000")
                                        {
                                            errorText.AppendText("時間:" + DateTime.Now.ToShortTimeString() + " 檔案" + item + "處理失敗。" + "\r\n");
                                            errorText.ScrollToCaret();

                                            _mySocket.Send(Encoding.GetEncoding("Big5").GetBytes("\u0002" + "9" + "                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                           " + "\u0003" + "\r"));
                                            break;
                                        }
                                    }
                                }

                                //處理完畢
                                SQL.ExecuteSQL("insert into opd.labinstrumentlog (type,log,time) values('robo8000','" + "\u0002" + "9" + "                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                           " + "\u0003" + "\r" + "','" + DateTime.Now.ToLongDateString() + DateTime.Now.ToLongTimeString() + "')");
                                _mySocket.Send(Encoding.GetEncoding("Big5").GetBytes("\u0002" + "9" + "                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                           " + "\u0003" + "\r"));

                                byte[] readBuffer2 = new byte[_mySocket.ReceiveBufferSize];
                                int count2 = 0;
                                if ((count2 = _mySocket.Receive(readBuffer2)) != 0)
                                {
                                    //在此決定傳輸失敗時的做法，目前尚未處理，相信廠商的機器「應該」都很穩定
                                    string clientRequest = Encoding.ASCII.GetString(readBuffer2, 0, count2).Substring(0, 14);
                                    if (clientRequest != "31000000000000")
                                    {
                                        errorText.AppendText("時間:" + DateTime.Now.ToShortTimeString() + " 檔案" + item + "處理失敗。" + "\r\n");
                                        errorText.ScrollToCaret();

                                        _mySocket.Send(Encoding.GetEncoding("Big5").GetBytes("\u0002" + "9" + "                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                           " + "\u0003" + "\r"));
                                        break;
                                    }
                                }

                                msgText.AppendText("時間:" + DateTime.Now.ToShortTimeString() + " 檔案" + item + "處理完畢。" + "\r\n");
                                msgText.ScrollToCaret();

                            }
                            //處理完畢後，刪除所有以處理資料
                            FtpInfo.FileDelete(fileList);
                            _mySocket.Close();
                        }
                    }
                    catch (Exception ex)
                    {
                        //EzMySQL SQL = new EzMySQL();
                        //SQL.ExecuteSQL("insert into opd.labinstrumentlog (type,log,time) values('robo8000','" + ex.Message + "','" + DateTime.Now.ToLongDateString() + DateTime.Now.ToLongTimeString() + "')");
                        oLabComm.insertlog(sFirmType, ex.Message);
                        errorText.AppendText("時間:" + DateTime.Now.ToShortTimeString() + " 檔案" + fileName + "處理失敗。" + "訊息" + ex.Message + "\r\n");
                        //break;
                    }
                }
            }
            catch (Exception ex)
            {
                //EzMySQL SQL = new EzMySQL();
                //SQL.ExecuteSQL("insert into opd.labinstrumentlog (type,log,time) values('robo8000','" + ex.Message + "','" + DateTime.Now.ToLongDateString() + DateTime.Now.ToLongTimeString() + "')");
                oLabComm.insertlog(sFirmType, ex.Message);
                errorText.AppendText("時間:" + DateTime.Now.ToShortTimeString() + " 檔案" + fileName + "處理失敗。" + "訊息" + ex.Message + "\r\n");
            }
        }

        public void Close()
        {
            closeSymbol = true;

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
    }


    public class TubeFtpFileInfo
    {
        string _cn = "server=192.168.150.53;user id=hisuser;Password=ca@2014;persist security info=True;database=BASEDATA";
        string _cmd = "";

        /// <summary>
        /// 取得Ftp上的所有檔案名稱
        /// </summary>
        public List<string> FileList()
        {
            _cmd = "select * from basedata.systemctrl where chparamcode = 'LabFtpIP'";
            MySqlDataAdapter da = new MySqlDataAdapter(_cmd, _cn);
            DataTable dt = new DataTable();
            da.Fill(dt);
            //ftp ip
            string ftpIP = dt.Rows[0]["chParamText"].ToString();
            dt.Clear();
            da.SelectCommand.CommandText = "select * from basedata.systemctrl where chparamcode = 'LabTubeFolder';";
            da.Fill(dt);
            //ftp 路徑
            string ftpDir = dt.Rows[0]["chParamText"].ToString();
            dt.Clear();
            da.SelectCommand.CommandText = "select * from basedata.systemctrl where chparamcode = 'LabFtpUser';";
            da.Fill(dt);
            //帳號
            string ftpUName = dt.Rows[0]["chParamText"].ToString();
            dt.Clear();
            da.SelectCommand.CommandText = "select * from basedata.systemctrl where chparamcode = 'LabFtpPWD';";
            da.Fill(dt);
            //密碼
            string ftpUPWD = dt.Rows[0]["chParamText"].ToString();

            List<string> al = new List<string>();
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(new Uri("ftp://" + ftpIP + "/" + ftpDir));
            request.Credentials = new NetworkCredential(ftpUName, ftpUPWD);
            request.Method = WebRequestMethods.Ftp.ListDirectory;
            FtpWebResponse response = (FtpWebResponse)request.GetResponse();

            using (Stream responsestream = response.GetResponseStream())
            {
                using (StreamReader reader = new StreamReader(responsestream))
                {
                    string line = reader.ReadLine();
                    while (line != null && line != String.Empty)
                    {
                        al.Add(line.Replace(ftpDir + "/", ""));
                        //Console.WriteLine(line);
                        line = reader.ReadLine();
                    }
                }
            }
            return al;
        }
        /// <summary>
        /// 回傳檔案上的內容
        /// 檔二內容相當於要用socket傳出去的指令
        /// </summary>
        /// <returns></returns>
        public string[] FileContent(string fileName)
        {
            MySqlDataAdapter da = new MySqlDataAdapter(_cmd, _cn);
            DataTable dt = new DataTable();
            da.Fill(dt);
            //ftp ip
            string ftpIP = dt.Rows[0]["chParamText"].ToString();
            dt.Clear();
            da.SelectCommand.CommandText = "select * from basedata.systemctrl where chparamcode = 'LabTubeFolder';";
            da.Fill(dt);
            //ftp 路徑
            string ftpDir = dt.Rows[0]["chParamText"].ToString();
            dt.Clear();
            da.SelectCommand.CommandText = "select * from basedata.systemctrl where chparamcode = 'LabFtpUser';";
            da.Fill(dt);
            //帳號
            string ftpUName = dt.Rows[0]["chParamText"].ToString();
            dt.Clear();
            da.SelectCommand.CommandText = "select * from basedata.systemctrl where chparamcode = 'LabFtpPWD';";
            da.Fill(dt);
            //密碼
            string ftpUPWD = dt.Rows[0]["chParamText"].ToString();

            WebClient request = new WebClient();
            string url = "ftp://" + ftpIP + "/" + fileName;
            request.Credentials = new NetworkCredential(ftpUName, ftpUPWD);
            string content = "";

            byte[] newFileData = request.DownloadData(url);
            content = Encoding.GetEncoding(950).GetString(newFileData);
            request.Dispose();

            string[] result = content.Split(new string[] { "###" }, StringSplitOptions.RemoveEmptyEntries);
            return result;
        }
        /// <summary>
        /// 刪除所有指定的檔案
        /// </summary>
        /// <param name="fileList"></param>
        public void FileDelete(List<string> fileList)
        {
            MySqlDataAdapter da = new MySqlDataAdapter(_cmd, _cn);
            DataTable dt = new DataTable();
            da.Fill(dt);
            //ftp ip
            string ftpIP = dt.Rows[0]["chParamText"].ToString();
            dt.Clear();
            da.SelectCommand.CommandText = "select * from basedata.systemctrl where chparamcode = 'LabTubeFolder';";
            da.Fill(dt);
            //ftp 路徑
            string ftpDir = dt.Rows[0]["chParamText"].ToString();
            dt.Clear();
            da.SelectCommand.CommandText = "select * from basedata.systemctrl where chparamcode = 'LabFtpUser';";
            da.Fill(dt);
            //帳號
            string ftpUName = dt.Rows[0]["chParamText"].ToString();
            dt.Clear();
            da.SelectCommand.CommandText = "select * from basedata.systemctrl where chparamcode = 'LabFtpPWD';";
            da.Fill(dt);
            //密碼
            string ftpUPWD = dt.Rows[0]["chParamText"].ToString();

            foreach (string item in fileList)
            {
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(new Uri("ftp://" + ftpIP + "/" + item));
                request.Method = WebRequestMethods.Ftp.DeleteFile;
                request.Credentials = new NetworkCredential(ftpUName, ftpUPWD);

                FtpWebResponse responseFileDelete = (FtpWebResponse)request.GetResponse();
            }
        }
    }


    public class ROBO8000Client
    {
        Socket mySocket;
        public ROBO8000Client(Socket mySocket)
        {
            this.mySocket = mySocket;
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
            //server & client 已經連線完成
            while (mySocket.Connected)
            {
                byte[] readBuffer = new byte[mySocket.ReceiveBufferSize];
                int count = 0;
                if ((count = mySocket.Receive(readBuffer)) != 0)
                {
                    string clientRequest = Encoding.ASCII.GetString(readBuffer, 0, count);
                    Console.WriteLine(clientRequest);
                }
            }
        }
    }
}
