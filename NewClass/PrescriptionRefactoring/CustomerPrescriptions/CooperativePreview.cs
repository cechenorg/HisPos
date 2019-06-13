using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using His_Pos.NewClass.Cooperative.XmlOfPrescription;
using His_Pos.NewClass.Product.Medicine;
using His_Pos.NewClass.Product.Medicine.PreviewMedicine;

namespace His_Pos.NewClass.PrescriptionRefactoring.CustomerPrescriptions
{
    public class CooperativePreview : CusPrePreviewBase
    {
        public CooperativePreview(CooperativePrescription.Prescription c, DateTime treatDate, string sourceId, bool isRead) : base(c, treatDate, isRead)
        {
            Content = c;
            SourceID = sourceId;
        }
        public CooperativePrescription.Prescription Content { get; }
        public string SourceID { get; }
        public override void Print()
        {
            var printPre = CreatePrescription();
            printPre.PrintMedBagAndReceipt();
        }

        public override Prescription CreatePrescription()
        {
            var pre = new Prescription(Content, TreatDate, SourceID, IsRead);
            pre.UpdateCooperativePrescriptionIsRead();
            pre.CountPrescriptionPoint();
            return pre;
        }

        public override void GetMedicines()
        {
            var medicines = new PreviewMedicines();
            var tempList = CreateTempMedicines();
            foreach (var setItem in Medicines)
            {
                if (medicines.Count(m => m.ID.Equals(setItem.ID)) > 0) continue;
                var medList = tempList.Where(m => m.ID.Equals(setItem.ID));
                foreach (var item in medList)
                {
                    medicines.Add(item);
                }
            }
            Medicines.Clear();
            Medicines = new PreviewMedicines(medicines);
        }

        private PreviewMedicines CreateTempMedicines()
        {
            var idList = CreateMedicineIDList();
            var table = MedicineDb.GetMedicinesBySearchIds(idList, "0");
            var tempList = new PreviewMedicines();
            for (var i = 0; i < table.Rows.Count; i++)
            {
                var r = table.Rows[i];
                var addedList = Medicines.Where(m => m.ID.Equals(r.Field<string>("Pro_ID")));
                foreach (var item in addedList)
                    tempList.Add(new PreviewMedicine(r, item));
            }
            return tempList;
        }

        private List<string> CreateMedicineIDList()
        {
            var idList = new List<string>();
            foreach (var med in Content.MedicineOrder.Item)
            {
                if (!idList.Contains(med.Id))
                    idList.Add(med.Id);
            }
            return idList;
        }
    }
}
