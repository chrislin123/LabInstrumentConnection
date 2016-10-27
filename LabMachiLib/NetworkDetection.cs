using System;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;

namespace LabMachiLib
{
    public class NetworkDetection
    {
        /// <summary>利用IPAddress屬性配合Ping進行遠端Server的確認。
        /// 
        /// </summary>
        /// <returns>true：存在；false：不存在</returns>
        public bool ByPing(string IPv4Address)
        {
            IPAddress tIP = IPAddress.Parse(IPv4Address);
            
            try
            {
                using (Ping tPingControl = new Ping())
                {   
                    PingReply tReply = tPingControl.Send(tIP);
                    tPingControl.Dispose();
                    if (tReply.Status != IPStatus.Success)
                        return false;
                    else
                        return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }


        /// <summary>利用TcpClient類別進行遠端主機指定的IP與Port是否開通：
        /// 利用IPAddress與Port屬性配合TCPClient進行遠端Server的確認。
        /// </summary>
        /// <returns>true：存在；false：不存在</returns>
        public bool ByTcpIp(string IPv4Address, int Port,int iTimeOut)
        {   
            //暫時停用-因為可能會造成 mysql - 1129, “Host ‘xxx.xxx.xxx.xxx’ is blocked 
            //because of many connection errors; unblock with ‘mysqladmin flush-hosts'”


            IPEndPoint tIPEndPoint = new IPEndPoint(IPAddress.Parse(IPv4Address), Port);
            bool tResult = false;
            try
            {
                tResult = IsServerConnectable(IPv4Address, Port, iTimeOut);

                //using (TcpClient tClient = new TcpClient())
                //{
                    
                //    tClient.Connect(tIPEndPoint);
                //    tResult = tClient.Connected;
                //    tClient.Close();
                //}

                return tResult;
            }
            catch (Exception)
            {
                return false;
            }
        }


        /// <summary>
        /// 偵測指定的伺服器的特定 port 是否可以連接。
        /// </summary>
        /// <param name="host">伺服器名稱或 IP 位址。</param>
        /// <param name="port">Port 號。</param>
        /// <param name="timeOut">連線逾時時間，單位：秒。</param>
        /// <returns></returns>
        private bool IsServerConnectable(string host, int port, int timeOut)
        {   
            DateTime t = DateTime.Now;

            try
            {
                using (TcpClient tClient = new TcpClient())
                {
                    //tClient.Client.Poll(0, SelectMode.SelectRead);
                    IAsyncResult ar = tClient.BeginConnect(host, port, null, null);
                   
                    while (!ar.IsCompleted)
                    {
                        if (DateTime.Now > t.AddSeconds(timeOut))
                        {
                            throw new Exception("Connection timeout!");
                        }
                        System.Threading.Thread.Sleep(100);
                    }

                    tClient.EndConnect(ar);

                    tClient.GetStream().Close();
                    
                    tClient.Close();
                }
                
                return true;
            }
            catch
            {
                return false;
            }
        }


    }
}
