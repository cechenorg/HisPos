using His_Pos.NewClass.Medicine;
using His_Pos.NewClass.Prescription;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using MedicineOTC = His_Pos.NewClass.Medicine.Base.MedicineOTC;
using Medicines = His_Pos.NewClass.Medicine.Base.Medicines;

namespace His_Pos.NewClass.Product.PrescriptionSendData
{
    public class PrescriptionSendDatas : ObservableCollection<PrescriptionSendData>
    {
        public PrescriptionSendDatas() { }
        public void ConvertMedToSendData(Prescription.Prescription prescription, bool isAllSend) {
            Clear();
            DataTable table;
            switch (prescription.Type)
            {
                case PrescriptionType.ChronicReserve:
                    table = MedicineDb.GetUsableAmountByReserveID(int.Parse(prescription.SourceId));
                    break;
                default:
                    table = MedicineDb.GetUsableAmountByPrescriptionID(prescription.ID);
                    break;
            }
            
            foreach (var m in prescription.Medicines) {
                if (!string.IsNullOrEmpty(m.ID) && !(m is MedicineOTC)) {
                    var prescriptionSendData = new PrescriptionSendData(m);

                }
            }
            foreach (var m in ms) {
                if (!string.IsNullOrEmpty(m.ID) && !(m is MedicineOTC)) {
                    var prescriptionSendData = new PrescriptionSendData(m);
                    if (this.Count(M => M.MedId == prescriptionSendData.MedId) == 1) {
                        prescriptionSendData.TreatAmount += this.Single(M => M.MedId == prescriptionSendData.MedId).TreatAmount;
                        prescriptionSendData.OldSendAmount += this.Single(M => M.MedId == prescriptionSendData.MedId).OldSendAmount;
                        this.Remove(this.Single(M => M.MedId == prescriptionSendData.MedId));
                    }
                    if (InventoryCollection.Count(inv => inv.InvID == m.InventoryID) == 1) {
                        var temp = InventoryCollection.Single(inv => inv.InvID == m.InventoryID);
                        if (isAllSend)
                            prescriptionSendData.SendAmount = prescriptionSendData.TreatAmount;
                        else
                        prescriptionSendData.SendAmount = prescriptionSendData.TreatAmount - temp.OnTheFrame - temp.OnTheWayAmount > 0
                            ? prescriptionSendData.TreatAmount - temp.OnTheFrame - temp.OnTheWayAmount : 0;
                        prescriptionSendData.OntheFrame = temp.OnTheFrame;
                        prescriptionSendData.OntheWay = temp.OnTheWayAmount;
                        prescriptionSendData.PrepareAmount = prescriptionSendData.TreatAmount - prescriptionSendData.SendAmount;
                    }
                    Add(prescriptionSendData);
                }
            } 
        }
    }
}
