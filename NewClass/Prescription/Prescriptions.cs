using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using His_Pos.NewClass.Person.Customer;

namespace His_Pos.NewClass.Prescription
{
    public class Prescriptions:ObservableCollection<Prescription>
    {
        public Prescriptions()
        {
        }

        public void GetCooperativePrescriptions(string pharmcyMedicalNum, DateTime sDate, DateTime eDate)
        {
            Prescriptions prescriptions = PrescriptionDb.GetCooperaPrescriptionsDataByDate(pharmcyMedicalNum, sDate, eDate);
            foreach (var p in prescriptions)
            {
                Add(p);
            }
        }
    }
}
