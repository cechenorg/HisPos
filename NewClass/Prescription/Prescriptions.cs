using System;
using System.Collections.ObjectModel;
using System.Data;
using His_Pos.ChromeTabViewModel;
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

        public void GetCooperativePrescriptions(string pharmacyID, DateTime sDate, DateTime eDate)
        {
            Prescriptions prescriptions = PrescriptionDb.GetCooperaPrescriptionsDataByDate(pharmacyID, sDate, eDate);
            foreach (var p in prescriptions)
            {
                Add(p);
            }
           
        }
        
        public  void GetPrescriptionsByCusIdNumber(string cusIDNumber) //取得處方
        {
            var table = PrescriptionDb.GetPrescriptionsByCusIdNumber(cusIDNumber);
            foreach (DataRow r in table.Rows)
            {
                Add(new Prescription(r, PrescriptionSource.Normal));
            }
        }
        public void GetPrescriptionsNoGetCardByCusId(int cusID) //取得未過卡處方
        {
            var table = PrescriptionDb.GetPrescriptionsNoGetCardByCusId(cusID);
            foreach (DataRow r in table.Rows)
            {
                Add(new Prescription(r, PrescriptionSource.Normal));
            }
        } 
        public void GetReservePrescriptionByCusId(int cusID) //取得預約慢箋
        {
            var table = PrescriptionDb.GetReservePrescriptionByCusId(cusID);
            foreach (DataRow r in table.Rows)
            {
                Add(new Prescription(r, PrescriptionSource.ChronicReserve));
            }
        }
        public void GetRegisterPrescriptionByCusId(int cusID) //取得登錄慢箋
        {
            var table = PrescriptionDb.GetRegisterPrescriptionByCusId(cusID);
            foreach (DataRow r in table.Rows)
            {
                Add(new Prescription(r, PrescriptionSource.Normal));
            }
        }
        public void GetCooperaPrescriptionsByCusIDNumber(string cusIDNum) //取得合作診所
        {
            Prescriptions table = PrescriptionDb.GetCooperaPrescriptionsDataByCusIdNumber(ViewModelMainWindow.CurrentPharmacy.Id, cusIDNum);
            foreach (var r in table)
            {
                Add(r);
            }
        }
        public static void PredictThreeMonthPrescription() {
            PrescriptionDb.PredictThreeMonthPrescription();
        }
         
    }
}
