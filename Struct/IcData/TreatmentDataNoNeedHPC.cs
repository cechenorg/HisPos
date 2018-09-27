namespace His_Pos.Struct.IcData
{
    public struct TreatmentDataNoNeedHpc
    {
        public string TreatmentCategory;
        public string NewbornTreatmentMark;
        public string TreatmentDateTime;
        public string MakeUpMark;
        public string MedicalNumber;
        public string HospitalId;
        public string TreatmentFee;
        public string TreatmentCopaymentFee;
        public string HospitalizationFee;
        public string HospitalCopayFeeUnderDaysLimit;
        public string HospitalCopayFeeOverDaysLimit;
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
        public TreatmentDataNoNeedHpc(byte[] pBuffer, int startIndex)
        {
            var f = new Function();
            TreatmentCategory = f.ByteArrayToString(2, pBuffer, startIndex);
            NewbornTreatmentMark = f.ByteArrayToString(1, pBuffer, startIndex + 2);
            TreatmentDateTime = f.ByteArrayToString(13, pBuffer, startIndex + 3);
            MakeUpMark = f.ByteArrayToString(1, pBuffer, startIndex + 16);
            MedicalNumber = f.ByteArrayToString(4, pBuffer, startIndex + 17);
            HospitalId = f.ByteArrayToString(10, pBuffer, startIndex + 21);
            TreatmentFee = f.ByteArrayToString(8, pBuffer, startIndex + 31);
            TreatmentCopaymentFee = f.ByteArrayToString(8, pBuffer, startIndex + 39);
            HospitalizationFee = f.ByteArrayToString(8, pBuffer, startIndex + 47);
            HospitalCopayFeeUnderDaysLimit = f.ByteArrayToString(8, pBuffer, startIndex + 55);
            HospitalCopayFeeOverDaysLimit = f.ByteArrayToString(8, pBuffer, startIndex + 63);
        }
    }
}
