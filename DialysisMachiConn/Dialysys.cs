using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LabMachiLib;
using System.Configuration;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.ComponentModel;
using System.Data;


namespace DialysisMachiConn
{
    
    class Dialysys
    {
        EzMySQL SQL = new EzMySQL(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
        LabComm oLabComm = new LabComm(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
        NetworkDetection nd = new NetworkDetection();
        string ssql = string.Empty;
        //廠商類別
        string sFirmType = "Dialysis";

        bool closeSymbol = false;

        //狀態設定
        byte[] STX = Encoding.GetEncoding("Big5").GetBytes("\u0002");
        byte[] ETX = Encoding.GetEncoding("Big5").GetBytes("\u0003");
        byte[] CR = Encoding.GetEncoding("Big5").GetBytes("\r");
        byte[] ACK = Encoding.GetEncoding("Big5").GetBytes("\u0006");
        byte[] NAK = Encoding.GetEncoding("Big5").GetBytes("\u0015");

        public void Start(string host, int port,BackgroundWorker bw,Dictionary<string,string>pPar)
        {   
            Socket _mySocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);            
            _mySocket.ReceiveTimeout = 1000;
            _mySocket.SendTimeout = 1000;

            
            //取得IP
            //host = "192.168.10.191";
            //取得Port
            //int port = 1401;

            //try
            //{
            //    //模擬資料
            //    string clientRequest1 = "K2143A00.30B00.00C00.00D00210E001.0F031.6G013.7H 0039I 0209J 0023K00085L00000a0b0c0d0e0f0g0h0M1N1O00.00P00.00Q00.00R000.0S102030T00157U00078V00081i027";

            //    //格式化儀器資料
            //    DialysysData oDialysysData1 = formatDialysysData(clientRequest1);

            //    //1051005 排除==沒有血壓時間的資料
            //    if (oDialysysData1.S == "000/00") return;

            //    //補上資料
            //    oDialysysData1.machinip = host;
            //    oDialysysData1.chdate = oLabComm.GetNowDate();
            //    oDialysysData1.chdt = oLabComm.GetNowFullDateTime();


            //    //寫入資料庫
            //    InsDialysysdata(oDialysysData1);

            //    //寫入HIS系統
            //    InsDialysysdataToHis(oDialysysData1);
            //}
            //catch (Exception ex)
            //{
            //    //紀錄錯誤
            //    oLabComm.insertErrlog(sFirmType, ex.ToString());

            //    return;
            //}

            //return;

            try
            {
                IPAddress[] IPs = Dns.GetHostAddresses(host);

                //
                //int iiii = int.Parse("測試錯誤");

                //測試連線是否有通
                if (nd.ByTcpIp(IPs[0].ToString(), port, 1) == false)
                {
                    string msg = string.Format("機器：{0} 儀器連線關閉無法取得資料。", host);

                    bw.ReportProgress(999, msg);

                    return;
                }

                //Socket連線
                _mySocket.Connect(IPs[0], port);

                byte[] readBuffer = new byte[_mySocket.ReceiveBufferSize];
                int count = 0;

                if (_mySocket.Connected == true)
                {
                    int byteCount = _mySocket.Send(Encoding.GetEncoding("Big5").GetBytes("K\r\n"));
                    
                    count = _mySocket.Receive(readBuffer);

                    //bw.ReportProgress(999, host + "  " + count.ToString());

                    if (count == 152)
                    {
                        string clientRequest = Encoding.ASCII.GetString(readBuffer, 0, count);

                        //模擬資料
                        //clientRequest = "K2143A00.30B00.00C00.00D00210E001.0F031.6G013.7H 0039I 0209J-0100K00085L00000a0b0c0d0e0f0g0h0M1N1O00.00P00.00Q00.00R000.0S102030T0000/U0000/V0000/i027";

                        //格式化儀器資料
                        DialysysData oDialysysData = formatDialysysData(clientRequest);

                        //1051005 排除==沒有血壓時間的資料
                        if (oDialysysData.S == "000/00") return;

                        //1051007 排除==未治療中資料
                        if (oDialysysData.M == "0") return;
                        
                        //補上資料
                        oDialysysData.machinip = host;
                        oDialysysData.chdate = oLabComm.GetNowDate();
                        oDialysysData.chdt = oLabComm.GetNowFullDateTime();

                        //原始資料寫入資料庫
                        if (pPar["bMachiOriLog"] == "Y")
                        {
                            oLabComm.insertErrlog(sFirmType, host + "：" + clientRequest);
                            bw.ReportProgress(999, "原始資料寫入資料庫");
                        }

                        //寫入資料庫
                        InsDialysysdata(oDialysysData);
                        bw.ReportProgress(999, "寫入資料庫");

                        //寫入HIS系統
                        InsDialysysdataToHis(oDialysysData,bw);
                        bw.ReportProgress(999, "寫入HIS系統完成");
                    }
                }

            }
            catch (Exception ex)
            {
                //紀錄錯誤
                oLabComm.insertErrlog(sFirmType, ex.ToString());                               
                
                return;
            }
            finally
            {
                //關閉Socket                
                oLabComm.CloseSocket(_mySocket);
            }
        }

        public void InsDialysysdataToHis(DialysysData oDialysysData, BackgroundWorker bw)
        {
            string sMrno = "";
            bw.ReportProgress(999, "寫入HIS系統1");
            //血壓測定時間變更後，才進行紀錄
            ssql = " select * from opd.diamachineset where machinip = '" + oDialysysData.machinip + "' ";
            DataTable dt = SQL.Get_DataTable(ssql);
            foreach (DataRow dr in dt.Rows)
            {
                //定時連線，測定時間沒改變，則跳出
                if (dr["dmaStatus"].ToString() == "R" && dr["dmabptm"].ToString() == oDialysysData.S)
                {
                    return;
                }
                //狀態D則跳出不新增
                if (dr["dmaStatus"].ToString() == "D")
                {
                    return;
                }

                sMrno = dr["dmaMrNo"].ToString();
            }
            bw.ReportProgress(999, "寫入HIS系統2");
            //取得析中記錄序號
            int iRecordNo = getTransNoCtrlData("DsRecordNo");
            DsRecord oDR = formatDsRecord(oDialysysData);
            bw.ReportProgress(999, "寫入HIS系統3");
            //寫入析中記錄主檔
            insRcmas(oDialysysData, iRecordNo.ToString(), sMrno);

            //寫入析中記錄明細檔
            insRcdtl(iRecordNo.ToString(), 1, "chtime", oDR.chtime);
            insRcdtl(iRecordNo.ToString(), 2, "Blood", oDR.Blood);
            insRcdtl(iRecordNo.ToString(), 3, "Pulse", oDR.Pulse);
            insRcdtl(iRecordNo.ToString(), 4, "flow", oDR.flow);
            insRcdtl(iRecordNo.ToString(), 5, "Venous", oDR.Venous);
            insRcdtl(iRecordNo.ToString(), 6, "TMP", oDR.TMP);
            insRcdtl(iRecordNo.ToString(), 7, "UFR", oDR.UFR);
            insRcdtl(iRecordNo.ToString(), 8, "totaldehy", oDR.totaldehy);
            insRcdtl(iRecordNo.ToString(), 9, "concentration", oDR.concentration);
            insRcdtl(iRecordNo.ToString(), 10, "temperature", oDR.temperature);
            insRcdtl(iRecordNo.ToString(), 11, "Heparin", oDR.Heparin);
            insRcdtl(iRecordNo.ToString(), 12, "flush", oDR.flush);
            insRcdtl(iRecordNo.ToString(), 13, "AKCLOT", oDR.AKCLOT);
            insRcdtl(iRecordNo.ToString(), 14, "Remark", oDR.Remark);

            bw.ReportProgress(999, "寫入HIS系統4");
            //紀錄血壓測定時間
            ssql = " UPDATE opd.diamachineset  SET ";
            ssql = ssql + " dmabptm = '" + oDialysysData.S + "' ";
            ssql = ssql + " ,dmaStatus = 'R' "; //恢復為Route狀態
            ssql = ssql + " WHERE machinip = '" + oDialysysData.machinip + "' ";
            SQL.ExecuteSQL(ssql);


            bw.ReportProgress(999, "寫入HIS系統5");

        }

        private void insRcmas(DialysysData oDialysysData,string pRcNo, string pMrno)
        {
            string sNowDate = oLabComm.GetNowFullDateTime("2");         

            //寫入紀錄主檔
            ssql = " INSERT INTO opd.dshdrecordmas ";
            ssql += " VALUES ";
            ssql += " ( ";
            ssql += "  '" + pRcNo + "' ";
            ssql += " ,'" + pMrno + "' ";
            ssql += " ,'" + oDialysysData.chdate + "' ";
            ssql += " ,0 ";
            ssql += " ,'' ";
            ssql += " ,'' ";
            ssql += " ,'' ";
            ssql += " ,'N' ";
            ssql += " ,'CONV' ";
            ssql += " ,'" + sNowDate + "' ";
            ssql += " ) ";
            SQL.ExecuteSQL(ssql);
        }

        private void insRcdtl(string pRcNo,int pSeqNo,string pItemKey,string pContent)
        {
            string sNowDate = oLabComm.GetNowFullDateTime("2");

            ssql = " INSERT INTO opd.Dshdrecorddtl ";
            ssql += " VALUES ";
            ssql += " ( ";
            ssql += "  '" + pRcNo + "' ";
            ssql += " ," + pSeqNo + " ";
            ssql += " ,'" + pItemKey + "' ";
            ssql += " ,'" + pContent + "' ";
            ssql += " ,'' ";
            ssql += " ,'N' ";
            ssql += " ,'CONV' ";
            ssql += " ,'" + sNowDate + "' ";
            ssql += " ) ";
            SQL.ExecuteSQL(ssql);
        }


        public int getTransNoCtrlData(string pCtrlCode)
        {
            int iNo = 0;
            int iNext = 0;
            ssql = " select * from Transnoctrl where chctrlcode = '" + pCtrlCode + "' ";
            DataTable dt = SQL.Get_DataTable(ssql);

            foreach (DataRow dr in dt.Rows)
            {
                iNo = Convert.ToInt32(dr["intnextno"].ToString());
            }

            iNext = iNo + 1;

            ssql = " UPDATE Transnoctrl  SET intnextno = " + iNext.ToString() + "  where chctrlcode = '" + pCtrlCode + "'";
            SQL.ExecuteSQL(ssql);

            return iNo;
        }


        private DsRecord formatDsRecord(DialysysData oDialysysData)
        {
            DsRecord odr = new DsRecord();            

            // 時間
            odr.chtime = string.Format("{0}:{1}", oDialysysData.S.Substring(0, 2), oDialysysData.S.Substring(2, 2));        
            // 血壓            
            int iT = Convert.ToInt32(oDialysysData.T);
            int iU = Convert.ToInt32(oDialysysData.U);
            odr.Blood = string.Format("{0}/{1}", iT.ToString(), iU.ToString());
            // 脈搏          
            int iV = Convert.ToInt32(oDialysysData.V);
            odr.Pulse = iV.ToString();
            // 血流速            
            int iD = Convert.ToInt32(oDialysysData.D);
            odr.flow = iD.ToString();
            // 靜脈壓            
            int iH = Convert.ToInt32(oDialysysData.H);
            odr.Venous = iH.ToString();
            // TMP           
            int iJ = Convert.ToInt32(oDialysysData.J);
            odr.TMP = iJ.ToString();
            // UFR            
            double iC = Convert.ToDouble(oDialysysData.C);
            odr.UFR = iC.ToString();            
            // 脫水總量            
            double iB = Convert.ToDouble(oDialysysData.B);
            odr.totaldehy = iB.ToString(); 
            // 透析濃度            
            double iG = Convert.ToDouble(oDialysysData.G);
            odr.concentration = iG.ToString();
            // 溫度            
            double iF = Convert.ToDouble(oDialysysData.F);
            odr.temperature = iF.ToString();
            // Heparin            
            odr.Heparin = "";            
            // N/S沖水            
            odr.flush = "";            
            // AK            
            odr.AKCLOT = "";            
            // 備註
            odr.Remark = "";


            return odr;
        }

        public void InsDialysysdata(DialysysData oDialysysData)
        {

            ssql = " INSERT INTO opd.diamachinedata ";
            ssql += " ( ";
            ssql += @" machinip,
                        chdate,
                        chdt,
                        targetdehy,
                        totaldehy,
                        ratedehy,
                        bloodflow,
                        syringeflow,
                        diahum,
                        diaconcen,
                        venouspre,
                        diapre,
                        diatmp,
                        diatime,
                        diaflow,
                        liqtempalarm,
                        concenalarm,
                        venousprealarm,
                        liqprealarm,
                        diatmpalarm,
                        bubblealarm,
                        louxuealarm,
                        otheralarm,
                        treatflag,
                        treatmode,
                        rehytarget,
                        rehyaftervalue,
                        rehyrate,
                        rehyhum,
                        bpmeatime,
                        sysbloodpre,
                        diabloodpre,
                        diapulse,
                        bpalarm ";
            ssql += " ) ";
            ssql += " VALUES ";
            ssql += " ( ";
            ssql += " '" + oDialysysData.machinip + "' ";
            ssql += " ,'" + oDialysysData.chdate + "' ";
            ssql += " ,'" + oDialysysData.chdt + "' ";
            ssql += " ,'" + oDialysysData.A + "' ";
            ssql += " ,'" + oDialysysData.B + "' ";
            ssql += " ,'" + oDialysysData.C + "' ";
            ssql += " ,'" + oDialysysData.D + "' ";
            ssql += " ,'" + oDialysysData.E + "' ";
            ssql += " ,'" + oDialysysData.F + "' ";
            ssql += " ,'" + oDialysysData.G + "' ";
            ssql += " ,'" + oDialysysData.H + "' ";
            ssql += " ,'" + oDialysysData.I + "' ";
            ssql += " ,'" + oDialysysData.J + "' ";
            ssql += " ,'" + oDialysysData.K + "' ";
            ssql += " ,'" + oDialysysData.L + "' ";
            ssql += " ,'" + oDialysysData.a + "' ";
            ssql += " ,'" + oDialysysData.b + "' ";
            ssql += " ,'" + oDialysysData.c + "' ";
            ssql += " ,'" + oDialysysData.d + "' ";
            ssql += " ,'" + oDialysysData.e + "' ";
            ssql += " ,'" + oDialysysData.f + "' ";
            ssql += " ,'" + oDialysysData.g + "' ";
            ssql += " ,'" + oDialysysData.h + "' ";
            ssql += " ,'" + oDialysysData.M + "' ";
            ssql += " ,'" + oDialysysData.N + "' ";
            ssql += " ,'" + oDialysysData.O + "' ";
            ssql += " ,'" + oDialysysData.P + "' ";
            ssql += " ,'" + oDialysysData.Q + "' ";
            ssql += " ,'" + oDialysysData.R + "' ";
            ssql += " ,'" + oDialysysData.S + "' ";
            ssql += " ,'" + oDialysysData.T + "' ";
            ssql += " ,'" + oDialysysData.U + "' ";
            ssql += " ,'" + oDialysysData.V + "' ";
            ssql += " ,'" + oDialysysData.i + "' ";           

            ssql += " ) ";

            SQL.ExecuteSQL(ssql);

        }

        public DialysysData formatDialysysData(string sData)
        {
            DialysysData oDialysysData = new DialysysData();

            //if (sData.Length != 152)
            //{
            //    return oDialysysData;
            //}


            //前五碼為註記非資料

            //"K2143A00.30B00.00C00.00D00210E001.0F031.6G013.7H 0039I 0209J-0100K00085L00000a0b0c0d0e0f0g0h0M1N1O00.00P00.00Q00.00R000.0S000/00T0000/U0000/V0000/i027";
            

            // 目標脫水量
            oDialysysData.A = sData.Substring(6, 5);
            // 總脫水量
            oDialysysData.B = sData.Substring(12, 5);
            // 脫水速度
            oDialysysData.C = sData.Substring(18, 5);
            // 血液流量
            oDialysysData.D = sData.Substring(24, 5);
            // Syringe流量
            oDialysysData.E = sData.Substring(30, 5);
            // 透析液濕度
            oDialysysData.F = sData.Substring(36, 5);
            // 透析液濃度
            oDialysysData.G = sData.Substring(42, 5);
            // 靜脈壓
            oDialysysData.H = sData.Substring(48, 5);
            // 透析液壓
            oDialysysData.I = sData.Substring(54, 5);
            // TMP
            oDialysysData.J = sData.Substring(60, 5);
            // 透析時間
            oDialysysData.K = sData.Substring(66, 5);
            // 透析液流量
            oDialysysData.L = sData.Substring(72, 5);
            // 液溫警報
            oDialysysData.a = sData.Substring(78, 1);
            // 濃度警報
            oDialysysData.b = sData.Substring(80, 1);
            // 靜脈壓警報
            oDialysysData.c = sData.Substring(82, 1);
            // 液壓警報
            oDialysysData.d = sData.Substring(84, 1);
            // TMP警報
            oDialysysData.e = sData.Substring(86, 1);
            // 氣泡檢測警報
            oDialysysData.f = sData.Substring(88, 1);
            // 漏血警報
            oDialysysData.g = sData.Substring(90, 1);
            // 其他警報
            oDialysysData.h = sData.Substring(92, 1);
            // 治療中標誌
            oDialysysData.M = sData.Substring(94, 1);
            // 治療模式
            oDialysysData.N = sData.Substring(96, 1);
            // 補液目標值
            oDialysysData.O = sData.Substring(98, 5);
            // 補液經過值
            oDialysysData.P = sData.Substring(104, 5);
            // 補液速度
            oDialysysData.Q = sData.Substring(110, 5);
            // 補液濕度
            oDialysysData.R = sData.Substring(116, 5);
            // 血壓測定時間(文件寫5Byte，實際為6Byte，格式HHMMSS)
            oDialysysData.S = sData.Substring(122, 6);
            // 最高血壓
            oDialysysData.T = sData.Substring(129, 5);
            // 最低血壓
            oDialysysData.U = sData.Substring(135, 5);
            // 脈搏
            oDialysysData.V = sData.Substring(141, 5);
            // 血壓警報
            oDialysysData.i = sData.Substring(147, 1);

            //***格式化資料***
            // 最高血壓
            if (oDialysysData.T == "0000/") oDialysysData.T = "0";
            // 最低血壓
            if (oDialysysData.U == "0000/") oDialysysData.U = "0";
            // 脈搏
            if (oDialysysData.V == "0000/") oDialysysData.V = "0";

            return oDialysysData;
        }
    }

    
}
