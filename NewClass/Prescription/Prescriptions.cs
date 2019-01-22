using System;
using System.Collections.ObjectModel;
using System.Data;
using His_Pos.NewClass.Person.MedicalPerson;
using His_Pos.NewClass.Prescription.Treatment.AdjustCase;
using His_Pos.NewClass.Prescription.Treatment.Institution;

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
        public void GetSearchPrescriptions(DateTime? sDate, DateTime? eDate,string patient,AdjustCase adj,Institution ins,MedicalPersonnel pharmacist)
        {
            var table = PrescriptionDb.GetSearchPrescriptionsData(sDate,eDate,patient,adj,ins,pharmacist);
            foreach (DataRow r in table.Rows)
            {
                Add(new Prescription(r));
            }
        }

        public void GetReservePrescription()
        {
            var table = PrescriptionDb.GetReservePrescriptionsData();
            foreach (DataRow r in table.Rows)
            {
                Add(new Prescription(r));
            }
        }
    }
}
