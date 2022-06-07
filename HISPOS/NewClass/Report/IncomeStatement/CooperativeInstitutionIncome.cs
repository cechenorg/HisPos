namespace His_Pos.NewClass.Report.IncomeStatement
{
    public class CooperativeInstitutionIncome
    {
        public CooperativeInstitutionIncome()
        {
        }

        public CooperativeInstitutionIncome(decimal medicalService, decimal medicine, decimal other)
        {
            MedicalServiceIncome = medicalService;
            MedicineIncome = medicine;
            OtherIncome = other;
        }

        public decimal MedicalServiceIncome { get; set; }
        public decimal MedicineIncome { get; set; }
        public decimal OtherIncome { get; set; }
        public decimal TotalIncome => MedicalServiceIncome + MedicineIncome + OtherIncome;
    }
}