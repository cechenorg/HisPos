namespace His_Pos.NewClass.Prescription.ICCard
{
    public struct IcPrescriptData
    {
        public string TreatmentCategory;
        public string MedicineId;
        public string Position;
        public string Usage;
        public string Days;
        public string Total;
        public string PrescripSign;
        public string DataStr;
        /*
         * 醫令類別(14)
         * 診療項目代號(15-26)
         * 診療部位(27-32)
         * 用法(33-50)
         * 天數(51-52)
         * 總量(53-59)
         * 交付處方註記(60-61)
         */
    }
}