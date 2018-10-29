namespace His_Pos.Class.HisApiData
{
    //get by 
    public struct MedicalInformation
    {
        //每組69byte
        public string TreatmentCategory { get; set; }//就醫類別 (85-86)
        public string NewBornTreatmentMark { get; set; }//新生兒就醫註記 (89)
        public string TreatmentDate { get; set; }//就診日期時間(88-100)
        public string MakeUpMark { get; set; }//補卡註記(101)
        public string MedicalNumber { get; set; }//就醫序號(102-105)
        public string MedicalIntitutionId { get; set; }//醫療院所ID (106-115)
    }


}
