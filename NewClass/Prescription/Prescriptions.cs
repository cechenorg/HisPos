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

        public void GetCooperativePrescriptions(string pharmcyMedicalNum, DateTime sDate, DateTime eDate)
        {
            Prescriptions prescriptions = PrescriptionDb.GetCooperaPrescriptionsDataByDate(pharmcyMedicalNum, sDate, eDate);
            foreach (var p in prescriptions)
            {
                Add(p);
            }
           
        }
        public void GetSearchPrescriptions(DateTime? sDate, DateTime? eDate,AdjustCase adj,Institution ins,MedicalPersonnel pharmacist)
        {
            var table = PrescriptionDb.GetSearchPrescriptionsData(sDate,eDate,adj,ins,pharmacist);
            foreach (DataRow r in table.Rows)
            {
                Add(new Prescription(r,PrescriptionSource.Normal));
            }
        }

        public void GetReservePrescription()
        {
            var table = PrescriptionDb.GetReservePrescriptionsData();
            foreach (DataRow r in table.Rows)
            {
                Add(new Prescription(r, PrescriptionSource.Normal));
            }
        }
        public void GetPrescriptionsByCusId(string CusId) //取得處方
        {
            var table = PrescriptionDb.GetPrescriptionsByCusId(CusId);
            foreach (DataRow r in table.Rows)
            {
                Add(new Prescription(r, PrescriptionSource.Normal));
            }
        }
        public void GetPrescriptionsNoGetCardByCusId(string CusId) //取得未過卡處方
        {
            var table = PrescriptionDb.GetPrescriptionsNoGetCardByCusId(CusId);
            foreach (DataRow r in table.Rows)
            {
                Add(new Prescription(r, PrescriptionSource.Normal));
            }
        } 
        public void GetReservePrescriptionByCusId(string CusId) //取得預約慢箋
        {
            var table = PrescriptionDb.GetReservePrescriptionByCusId(CusId);
            foreach (DataRow r in table.Rows)
            {
                Add(new Prescription(r, PrescriptionSource.ChronicReserve));
            }
        }
        public void GetCooperaPrescriptionsByCusIdNumber(string CusIdNum) //取德合作診所
        {
            Prescriptions table = PrescriptionDb.GetCooperaPrescriptionsDataByCusIdNumber(ViewModelMainWindow.CurrentPharmacy.Id, CusIdNum);
            foreach (var r in table)
            {
                Add(r);
            }
        }
        public void ImportDeclareXml() {
            PrescriptionDb.ImportDeclareXml(this);
        }
    }
}
