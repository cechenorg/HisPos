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
        public void ConvertMedToSendData(Medicines ms,int preID) {
            List<string> MedicineIds = new List<string>();
            foreach (var med in ms)
            {
                if (MedicineIds.Count(M => M == med.ID) == 0)
                    MedicineIds.Add(med.ID);
            }
            Inventorys InventoryCollection = Inventorys.GetAllInventoryByProIDs(MedicineIds);

            Medicines medicines = new Medicines();
            medicines.GetDataByPrescriptionId(preID);
            foreach (var m in medicines) {
                var temp = InventoryCollection.Single(inv => inv.InvID == m.InventoryID);
                if(m.SendAmount >= 0)
                temp.OnTheFrame += m.Amount - m.SendAmount;
            } 
            foreach (var m in ms) { 
            if (!string.IsNullOrEmpty(m.ID) && !(m is MedicineOTC)) { 
                PrescriptionSendData prescriptionSendData = new PrescriptionSendData(m);
                if (this.Count(M => M.MedId == prescriptionSendData.MedId) == 1) {
                    prescriptionSendData.TreatAmount += this.Single(M => M.MedId == prescriptionSendData.MedId).TreatAmount;
                    prescriptionSendData.OldSendAmount += this.Single(M => M.MedId == prescriptionSendData.MedId).OldSendAmount;
                    this.Remove(this.Single(M => M.MedId == prescriptionSendData.MedId));
                }
                if (InventoryCollection.Count(inv => inv.InvID == m.InventoryID) == 1) {
                    var temp = InventoryCollection.Single(inv => inv.InvID == m.InventoryID);
                    
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
