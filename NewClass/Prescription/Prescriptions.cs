using System;
using System.Collections.ObjectModel;
using System.Data;
using His_Pos.ChromeTabViewModel;

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
        
        public void GetCooperaPrescriptionsByCusIDNumber(string cusIDNum) //取得合作診所
        {
            Prescriptions table = PrescriptionDb.GetCooperaPrescriptionsDataByCusIdNumber(ViewModelMainWindow.CurrentPharmacy.ID, cusIDNum);
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
