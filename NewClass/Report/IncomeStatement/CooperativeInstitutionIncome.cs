using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.NewClass.Report.IncomeStatement
{
    public class CooperativeInstitutionIncome
    {
        public CooperativeInstitutionIncome()
        {

        }

        public CooperativeInstitutionIncome(int medicalService,int medicine,int other)
        {
            MedicalServiceIncome = medicalService;
            MedicineIncome = medicine;
            OtherIncome = other;
            TotalIncome = MedicalServiceIncome + MedicineIncome + OtherIncome;
        }

        public int MedicalServiceIncome { get; set; }
        public int MedicineIncome { get; set; }
        public int OtherIncome { get; set; }
        public int TotalIncome { get; set; }
    }
}
