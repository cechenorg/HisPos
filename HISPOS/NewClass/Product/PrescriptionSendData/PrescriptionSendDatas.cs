using His_Pos.NewClass.Medicine.InventoryMedicineStruct;
using His_Pos.NewClass.Prescription;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Medicines = His_Pos.NewClass.Medicine.Base.Medicines;

namespace His_Pos.NewClass.Product.PrescriptionSendData
{
    public class PrescriptionSendDatas : ObservableCollection<PrescriptionSendData>
    {
        public PrescriptionSendDatas()
        {
        }

        public void ConvertMedToSendData(Prescription.Prescription prescription, bool isAllSend)
        {
            Clear();
            List<MedicineInventoryStruct> tempMeds = new List<MedicineInventoryStruct>();
            List<string> meds = new List<string>();
            Medicines preoriginMeds = new Medicines();
            if (prescription.Type == PrescriptionType.ChronicRegister)
                preoriginMeds.GetDataByPrescriptionId(prescription.ID);
            else if (prescription.Type == PrescriptionType.ChronicReserve)
                preoriginMeds.GetDataByReserveId(int.Parse(prescription.SourceId));

            foreach (var m in preoriginMeds)
            {
                if (!string.IsNullOrEmpty(m.ID) && m.AdjustNoBuckle == false)
                {
                    double selfPrepareAmount;
                    if (prescription.PrescriptionStatus.OrderStatus.Contains("訂單狀態:已收貨") || prescription.PrescriptionStatus.OrderStatus.Contains("備藥狀態:已備藥"))
                    {
                        if (m.SendAmount >= 0)
                            selfPrepareAmount = m.Amount;
                        else
                            selfPrepareAmount = 0;
                    }
                    else
                        selfPrepareAmount = m.SendAmount <= 0 ? 0 : m.Amount - m.SendAmount;


                    if (tempMeds.Count(t => t.ID == m.InventoryID) == 0)
                        tempMeds.Add(new MedicineInventoryStruct(m.InventoryID, selfPrepareAmount));
                    else
                    {
                        for (int i = 0; i < tempMeds.Count; i++)
                        {
                            if (tempMeds[i].ID == m.InventoryID)
                                tempMeds[i].Amount += selfPrepareAmount;
                        }
                    }
                }
            }
            foreach (var m in prescription.Medicines)
            {
                if (!string.IsNullOrEmpty(m.ID) && m.AdjustNoBuckle == false)
                {
                    Add(new PrescriptionSendData(m));
                    meds.Add(m.ID);
                    if (tempMeds.Count(temp => temp.ID == m.InventoryID) == 0)
                        tempMeds.Add(new MedicineInventoryStruct(m.InventoryID, 0));
                }
            }
            Inventorys inventories = Inventorys.GetAllInventoryByProIDs(meds);
            foreach (var inv in inventories)
            {
                for (int i = 0; i < tempMeds.Count; i++)
                {
                    if (tempMeds[i].ID == inv.InvID)
                    {
                        tempMeds[i].Amount += inv.InventoryAmount + inv.OnTheWayAmount - inv.MegBagAmount;
                        tempMeds[i].Amount = tempMeds[i].Amount < 0 ? 0 : tempMeds[i].Amount;
                    }
                }
            }
            for (int i = 0; i < tempMeds.Count; i++)
            {
                for (int j = 0; j < this.Count; j++)
                {
                    if (this[j].InvID == tempMeds[i].ID)
                    {
                        double canUseAmount = tempMeds[i].Amount;
                        this[j].CanUseAmount = canUseAmount;
                    }
                }
            }
            for (int i = 0; i < this.Count; i++)
            {
                for (int j = 0; j < tempMeds.Count; j++)
                {
                    if (tempMeds[j].ID == this[i].InvID)
                    {
                        if (this[i].IsCommon == true)
                        {
                            this[i].PrepareAmount = this[i].TreatAmount;
                            this[i].SendAmount = 0;
                        }
                        else
                        {
                            this[i].PrepareAmount = isAllSend ? 0 : tempMeds[j].Amount - this[i].TreatAmount >= 0 ? this[i].TreatAmount : tempMeds[j].Amount;
                            this[i].SendAmount = isAllSend ? this[i].TreatAmount : this[i].TreatAmount - this[i].PrepareAmount;
                        }
                        tempMeds[j].Amount = tempMeds[j].Amount - this[i].PrepareAmount >= 0 ? tempMeds[j].Amount - this[i].PrepareAmount : 0;
                        if (this[i].SingdeInv < this[i].SendAmount) { this[i].SingdeInvNotEnough = true; }
                    }
                }
            }
        }
    }
}