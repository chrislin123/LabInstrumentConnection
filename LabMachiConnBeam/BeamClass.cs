using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabMachiConnBeam
{
    class BeamClass
    {
    }

    class LabMachineResult
    {
        //報告明細檔 檢驗單號
        public string chLabEmrNo = "";
        //報告明細檔 流水號
        public string intSeq = "";
        //報告明細檔 檢驗項目
        public string chDiaCode = "";
        //檢驗項目 上機代號
        public string chDiaCodeOfMachineNo = "";
        //檢體編號
        public string chSampleID = "";
        //急件
        public string chEmg = "";
        //報告值
        public string chRepValue = "";
        //報告狀態
        public string chRepValueStatus = "";
        //檢體類別代碼
        public string chSampleType = "";
        //檢體類別
        public string chDataType = "";
        //單位
        public string chUnit = "";
        //產生時間
        public string chCreateDttm = "";
        //上機儀器代號
        public string chOnMachineId = "";
        //上機儀器名稱
        public string chOnMachineName = "";
        //上機時間
        public string chOnMachineDttm = "";
        //上機完成時間
        public string chMachineReturnDttm = "";
        //完成時間
        public string chFinshDttm = "";

    }



}
