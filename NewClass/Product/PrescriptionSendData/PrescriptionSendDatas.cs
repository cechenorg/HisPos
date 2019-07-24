using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using MedicineOTC = His_Pos.NewClass.Medicine.Base.MedicineOTC;
using Medicines = His_Pos.NewClass.Medicine.Base.Medicines;

namespace His_Pos.NewClass.Product.PrescriptionSendData
{
    public class PrescriptionSendDatas : ObservableCollection<PrescriptionSendData>
    {
        public PrescriptionSendDatas() { }
        public void ConvertMedToSendData(Medicines ms) {
            List<string> MedicineIds = new List<string>();
            foreach (var med in ms)
            {
                MedicineIds.Add(med.ID);
            }
            Inventorys InventoryCollection = Inventorys.GetAllInventoryByProIDs(MedicineIds);
           
            foreach (var m in ms) {
                if (!string.IsNullOrEmpty(m.ID) && !(m is MedicineOTC)) { 
                    PrescriptionSendData prescriptionSendData = new PrescriptionSendData(m);
                    if (InventoryCollection.Count(inv => inv.InvID == m.InventoryID) == 1) {
                        var temp = InventoryCollection.Single(inv => inv.InvID == m.InventoryID);
                        prescriptionSendData.SendAmount = prescriptionSendData.TreatAmount - temp.OnTheFrame - temp.OnTheWayAmount > 0
                            ? prescriptionSendData.TreatAmount - temp.OnTheFrame - temp.OnTheWayAmount : 0;
                    }
                    Add(prescriptionSendData);
                }
            } 
        }
        

    }
}
