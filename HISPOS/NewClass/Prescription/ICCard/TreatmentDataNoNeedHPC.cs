using His_Pos.Service;
using System;
using System.Collections.Generic;

namespace His_Pos.NewClass.Prescription.ICCard
{
    public struct TreatmentDataNoNeedHpc
    {
        public string TreatmentCategory;
        public string NewbornTreatmentMark;
        public DateTime TreatmentDateTime;
        public string MakeUpMark;
        public string MedicalNumber;
        public string HospitalId;
        public string TreatmentFee;
        public string TreatmentCopaymentFee;
        public string HospitalizationFee;
        public string HospitalCopayFeeUnderDaysLimit;
        public string HospitalCopayFeeOverDaysLimit;
        private Dictionary<string, string> treCatDictionary;
        /*
         * 就醫類別(85-86)
         * 新生兒就醫註記(87)
         * 就診日期時間(88-100)
         * 補卡註記(101)
         * 就醫序號(102-105)
         * 醫療院所代碼(106-115)
         * 門診醫療費用【當次】(116-123)
         * 門診部分負擔費用【當次】(124-131)
         * 住院醫療費用【當次】(132-139)
         * 住院部分負擔費用【當次急性30天， 慢性180天以下】(140-146)
         * 住院部分負擔費用【當次急性31天， 慢性181天以上】(147-153)
         */

        public TreatmentDataNoNeedHpc(byte[] pBuffer, int startIndex, bool isMakeUp)
        {
            treCatDictionary = new Dictionary<string, string>();
            var treCat = Function.ByteArrayToString(2, pBuffer, startIndex);
            TreatmentCategory = string.Empty;
            NewbornTreatmentMark = Function.ByteArrayToString(1, pBuffer, startIndex + 2);
            var treatDateTime = Function.ByteArrayToString(13, pBuffer, startIndex + 3);
            TreatmentDateTime = DateTimeExtensions.TWDateStringToDateTime(treatDateTime);
            MakeUpMark = isMakeUp ? "2" : Function.ByteArrayToString(1, pBuffer, startIndex + 16);
            MedicalNumber = Function.ByteArrayToString(4, pBuffer, startIndex + 17);
            HospitalId = Function.ByteArrayToString(10, pBuffer, startIndex + 21);
            TreatmentFee = Function.ByteArrayToString(8, pBuffer, startIndex + 31);
            TreatmentCopaymentFee = Function.ByteArrayToString(8, pBuffer, startIndex + 39);
            HospitalizationFee = Function.ByteArrayToString(8, pBuffer, startIndex + 47);
            HospitalCopayFeeUnderDaysLimit = Function.ByteArrayToString(7, pBuffer, startIndex + 55);
            HospitalCopayFeeOverDaysLimit = Function.ByteArrayToString(7, pBuffer, startIndex + 62);
            CreateTreatmentCategoryDictionary();
            TreatmentCategory = treCatDictionary[treCat];
        }

        private void CreateTreatmentCategoryDictionary()
        {
            treCatDictionary.Add("01", "西醫門診");
            treCatDictionary.Add("02", "牙醫門診");
            treCatDictionary.Add("03", "中醫門診");
            treCatDictionary.Add("04", "急診");
            treCatDictionary.Add("05", "住院");
            treCatDictionary.Add("06", "門診轉診就醫");
            treCatDictionary.Add("07", "門診手術回診");
            treCatDictionary.Add("08", "住院患者出院回診");
            treCatDictionary.Add("AA", "同療程(6次以內治療)");
            treCatDictionary.Add("AB", "同療程(非6次以內治療)");
            treCatDictionary.Add("AC", "預防保健");
            treCatDictionary.Add("AD", "職業傷害/職業病門(急)診");
            treCatDictionary.Add("AE", "慢性病連續處方箋領藥");
            treCatDictionary.Add("AF", "藥局調劑");
            treCatDictionary.Add("AG", "排程檢劑");
            treCatDictionary.Add("AH", "居家照護(第二次以後)");
            treCatDictionary.Add("AI", "同日同醫師看診(第二次以後)");
            treCatDictionary.Add("BA", "門(急)診當次轉住院入院");
            treCatDictionary.Add("BB", "出院");
            treCatDictionary.Add("BC", "急診/住院中執行項目");
            treCatDictionary.Add("BD", "急診第二日(含)以後離院");
            treCatDictionary.Add("BE", "職業傷害/職業病住院");
            treCatDictionary.Add("BF", "繼續住院依規定分段結清，切帳申報");
            treCatDictionary.Add("CA", "其他規定不須累計就醫序號急不扣除就醫次數");
            treCatDictionary.Add("DA", "門診轉出");
            treCatDictionary.Add("DB", "門診日術後須於七日內之一次回診");
            treCatDictionary.Add("DC", "住院患者出院後七日內之一次回診");
            treCatDictionary.Add("ZA", "取消24小時內所有就醫類別");
            treCatDictionary.Add("ZB", "取消24小時內部份就醫類別");
        }
    }
}