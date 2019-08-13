﻿using His_Pos.NewClass.Medicine;
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
            foreach (var m in prescription.Medicines) {
                if (!string.IsNullOrEmpty(m.ID) && !(m is MedicineOTC)) {
                    Add(new PrescriptionSendData(m));
                    if(tempMeds.Count(t => t.ID == m.InventoryID) == 0)
                        tempMeds.Add(new MedicineInventoryStruct(m.InventoryID,m.UsableAmount));
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
