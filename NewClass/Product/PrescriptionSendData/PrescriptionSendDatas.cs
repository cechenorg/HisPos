using His_Pos.NewClass.Medicine;
using His_Pos.NewClass.Medicine.InventoryMedicineStruct;
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
            List<MedicineInventoryStruct> tempMeds = new List<MedicineInventoryStruct>();
            List<string> meds = new List<string>();
            foreach (var m in prescription.Medicines) {
                if (!string.IsNullOrEmpty(m.ID) && !(m is MedicineOTC)) {
                    Add(new PrescriptionSendData(m));
                    if (tempMeds.Count(t => t.ID == m.InventoryID) == 0) {
                        double selfPrepareAmount;
                        if (prescription.PrescriptionStatus.OrderStatus.Contains("訂單狀態:已收貨") || prescription.PrescriptionStatus.OrderStatus.Contains("備藥狀態:已備藥")) {
                            if (m.SendAmount >= 0)
                                selfPrepareAmount = m.Amount;
                            else
                                selfPrepareAmount = 0;
                        }
                        else
                            selfPrepareAmount = m.SendAmount < 0 ? 0 : m.Amount - m.SendAmount;
                        
                        tempMeds.Add(new MedicineInventoryStruct(m.InventoryID, selfPrepareAmount));
                        meds.Add(m.ID);
                    }
                } 
            }
            Inventorys inventories = Inventorys.GetAllInventoryByProIDs(meds); 
            foreach (var inv in inventories) { 
                for (int i = 0; i < tempMeds.Count; i++)
                {
                    if (tempMeds[i].ID == inv.InvID) {
                        tempMeds[i].Amount += inv.InventoryAmount + inv.OnTheWayAmount - inv.MegBagAmount;
                        tempMeds[i].Amount = tempMeds[i].Amount < 0 ? 0 : tempMeds[i].Amount;
                    }
                        
                }
            }
            for (int i = 0; i < tempMeds.Count; i++) {
                for (int j = 0; j < this.Count; j++) {
                    if (this[j].InvID == tempMeds[i].ID) {
                        double canUseAmount = tempMeds[i].Amount;
                        this[j].CanUseAmount = canUseAmount;
                    } 
                }
            }
            for (int i = 0; i < this.Count; i++) {
                for (int j = 0; j < tempMeds.Count; j++) {
                    if (tempMeds[j].ID == this[i].InvID) {
                        this[i].PrepareAmount = isAllSend ? 0 : tempMeds[j].Amount - this[i].TreatAmount >= 0 ? this[i].TreatAmount : tempMeds[j].Amount;
                        this[i].SendAmount = isAllSend ? this[i].TreatAmount : this[i].TreatAmount - this[i].PrepareAmount;
                        tempMeds[j].Amount = tempMeds[j].Amount - this[i].PrepareAmount >= 0 ? tempMeds[j].Amount - this[i].PrepareAmount : 0; 
                    } 
                } 
            }  
        }
    }
}
