using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DialysisMachiConn
{
    class DialysysClass
    {

    }

    class DialysysData
    {
        /// <summary>
        /// 儀器ip
        /// </summary>
        public string machinip = "";
        /// <summary>
        /// 轉檔日期
        /// </summary>
        public string chdate = "";
        /// <summary>
        /// 轉檔日期時間(14碼)
        /// </summary>
        public string chdt = "";
        /// <summary>
        /// 目標脫水量
        /// </summary>
        public string A = "";
        /// <summary>
        /// 總脫水量
        /// </summary>
        public string B = "";
        /// <summary>
        /// 脫水速度
        /// </summary>
        public string C = "";
        /// <summary>
        /// 血液流量
        /// </summary>
        public string D = "";        
        /// <summary>
        /// Syringe流量
        /// </summary>
        public string E = "";        
        /// <summary>
        /// 透析液濕度
        /// </summary>
        public string F = "";
        /// <summary>
        /// 透析液濃度
        /// </summary>
        public string G = "";
        /// <summary>
        /// 靜脈壓
        /// </summary>
        public string H = "";
        /// <summary>
        /// 透析液壓
        /// </summary>
        public string I = "";
        /// <summary>
        /// TMP
        /// </summary>
        public string J = "";
        /// <summary>
        /// 透析時間
        /// </summary>
        public string K = "";
        /// <summary>
        /// 透析液流量
        /// </summary>
        public string L = "";
        /// <summary>
        /// 液溫警報
        /// </summary>
        public string a = "";
        /// <summary>
        /// 濃度警報
        /// </summary>
        public string b = "";
        /// <summary>
        /// 靜脈壓警報
        /// </summary>
        public string c = "";
        /// <summary>
        /// 液壓警報
        /// </summary>
        public string d = "";
        /// <summary>
        /// TMP警報
        /// </summary>
        public string e = "";
        /// <summary>
        /// 氣泡檢測警報
        /// </summary>
        public string f = "";
        /// <summary>
        /// 漏血警報
        /// </summary>
        public string g = "";
        /// <summary>
        /// 其他警報
        /// </summary>
        public string h = "";
        /// <summary>
        /// 治療中標誌
        /// </summary>
        public string M = "";
        /// <summary>
        /// 治療模式
        /// </summary>
        public string N = "";
        /// <summary>
        /// 補液目標值
        /// </summary>
        public string O = "";
        /// <summary>
        /// 補液經過值
        /// </summary>
        public string P = "";
        /// <summary>
        /// 補液速度
        /// </summary>
        public string Q = "";
        /// <summary>
        /// 補液濕度
        /// </summary>
        public string R = "";
        /// <summary>
        /// 血壓測定時間
        /// </summary>
        public string S = "";
        /// <summary>
        /// 最高血壓
        /// </summary>
        public string T = "";
        /// <summary>
        /// 最低血壓
        /// </summary>
        public string U = "";
        /// <summary>
        /// 脈搏
        /// </summary>
        public string V = "";
        /// <summary>
        /// 血壓警報
        /// </summary>
        public string i = "";
    }

    class DsRecord
    {
        /// <summary>
        /// 時間
        /// </summary>
        public string chtime = "";
        /// <summary>
        /// 血壓
        /// </summary>
        public string Blood = "";
        /// <summary>
        /// 脈搏
        /// </summary>
        public string Pulse = "";
        /// <summary>
        /// 血流速
        /// </summary>
        public string flow = "";
        /// <summary>
        /// 靜脈壓
        /// </summary>
        public string Venous = "";
        /// <summary>
        /// TMP
        /// </summary>
        public string TMP = "";
        /// <summary>
        /// UFR
        /// </summary>
        public string UFR = "";
        /// <summary>
        /// 脫水總量
        /// </summary>
        public string totaldehy = "";
        /// <summary>
        /// 透析濃度
        /// </summary>
        public string concentration = "";
        /// <summary>
        /// 溫度
        /// </summary>
        public string temperature = "";
        /// <summary>
        /// Heparin
        /// </summary>
        public string Heparin = "";
        /// <summary>
        /// N/S沖水
        /// </summary>
        public string flush = "";
        /// <summary>
        /// AK
        /// </summary>
        public string AKCLOT = "";
        /// <summary>
        /// 備註
        /// </summary>
        public string Remark = "";
        
    }
}
