using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabMachiConnAdvia
{
    class AdivaClass
    {
    }


    class AdivaData
    {
        public List<AdivaRecordData> RecordList = new List<AdivaRecordData>();
        //機器對應碼清單
        public List<string> OrderList = new List<string>();

        //internal List<AdivaRecordData> RecordList
        //{
        //    get { return _RecordList; }
        //    set { _RecordList = value; }
        //}

        //管號(檢體號)
        public string SampleID = "";
        //架號
        public string RackNum = "";
        //位置
        public string SamplePo = "";
        //機器對應碼
        //public string MachineMapping = "";

        public void AddRecord(AdivaRecordData pAdivaRecordData)
        {
            RecordList.Add(pAdivaRecordData);
        }

    }

    class AdivaRecordData
    {
        //項目代碼(R3.4)
        public string LIScode;

        //結果狀態(R3.8)
        //ALGC - Allergen code
        //CLSS - Allergy class
        //COFF - Cut off (in Master Curve units)
        //CSN - Country specific name (allergen name)
        //DOSE - Result concentration value (in Master
        //Curve units)
        //INDX - Result index value
        //INTR - Result interpretation
        //RLU - RLU value used in result calculation
        public string ResultAspects;

        //結果數值(R4)
        public string DataValue;

        //結果單位(R5)
        public string DataUnits;

        //結果狀態(R9)
        public string DataStatus;
    }

    class LabMachineData
    {
        //開單號
        public string chLabEmrNo = "";
        //序號
        public string intSeq = "";
        //管號
        public string chTubeNo = "";
        //
        public string chCombineCode = "";
        //機器分類代碼
        public string chMachineNo = "";
        //HIS對應機器碼
        public string chMachineMapping = "";
        //數值類別
        public string chValuetype = "";
        //數值
        public string chValue = "";
        //單位
        public string chUnit = "";
        //HIS接收時間
        public string chTransmitTime = "";
        //機器報告時間
        public string chOnBoardTime = "";

    }
}
