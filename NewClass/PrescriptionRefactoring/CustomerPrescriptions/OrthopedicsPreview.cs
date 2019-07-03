using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using His_Pos.NewClass.CooperativeInstitution;
using His_Pos.NewClass.Prescription;
using His_Pos.NewClass.Product.Medicine;
using His_Pos.NewClass.Product.Medicine.PreviewMedicine;

namespace His_Pos.NewClass.PrescriptionRefactoring.CustomerPrescriptions
{
    public class OrthopedicsPreview : CusPrePreviewBase
    {
        public OrthopedicsPreview(OrthopedicsPrescription c) :base(c)
        {
            Content = c;
            DoctorName = "醫師 "+c.DeclareXmlDocument.Prescription.Study.Doctor_Name;
        }
        public OrthopedicsPrescription Content { get;}
        public string DoctorName { get; }
        public override void Print()
        {
            var printPre = CreatePrescription();
            printPre.PrintMedBagAndReceipt();
        }
        public override Prescription CreatePrescription()
        {
            var pre = new Prescription(Content);
            PrescriptionDb.UpdateCooperativePrescriptionIsRead(pre.SourceId);
            pre.CountPrescriptionPoint();
            pre.CountSelfPay();
            pre.PrescriptionPoint.CountAmountsPay();
            return pre;
        }

        public override void GetMedicines()
        {
            var medicines = new PreviewMedicines();
            var tempList = CreateTempMedicines(null);
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

        private PreviewMedicines CreateTempMedicines(DateTime? adjustDate)
        {
            var idList = CreateMedicineIDList();
            var table = MedicineDb.GetMedicinesBySearchIds(idList, "0", adjustDate);
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
            var xmlDoc = Content.DeclareXmlDocument;
            foreach (var med in xmlDoc.Prescription.MedicineOrder.Item)
            {
                if (!idList.Contains(med.Id))
                    idList.Add(med.Id);
            }
            return idList;
        }
    }
}
