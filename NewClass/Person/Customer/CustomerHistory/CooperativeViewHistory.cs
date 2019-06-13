using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using GalaSoft.MvvmLight;
using His_Pos.NewClass.Product;
using His_Pos.NewClass.Product.Medicine;

namespace His_Pos.NewClass.Person.Customer.CustomerHistory
{
    public class CooperativeViewHistory:ObservableObject
    {
        public CooperativeViewHistory()
        {
            Products = new Products();
        }
        public CooperativeViewHistory(Prescription.Prescription p) {
            PrescriptionID = p.Id;
            Institution = p.Treatment.Institution.Name;
            Division = p.Treatment.Division.Name;
            AdjustDate = (DateTime)p.Treatment.AdjustDate;
            TotalPoint = p.PrescriptionPoint.TotalPoint;
            Products = new Products();
            var idList = new List<string>();
            foreach (var m in p.Medicines)
            {
                if (!idList.Contains(m.ID))
                    idList.Add(m.ID);
            }
            var table = MedicineDb.GetMedicinesBySearchIds(idList, "0");
            var tempList = new List<Medicine>();
            for (var i = 0; i < table.Rows.Count; i++)
            {
                var tempMedList = p.Medicines.Where(m => m.ID.Equals(table.Rows[i].Field<string>("Pro_ID")));
                foreach (var setItem in tempMedList)
                {
                    var medicine = new Medicine();
                    switch (table.Rows[i].Field<int>("DataType"))
                    {
                        case 1:
                            medicine = new MedicineNHI(table.Rows[i]);
                            break;
                        case 2:
                            medicine = new MedicineOTC(table.Rows[i]);
                            break;
                        case 3:
                            medicine = new MedicineSpecialMaterial(table.Rows[i]);
                            break;
                    }
                    medicine.Dosage = setItem.Dosage;
                    medicine.UsageName = setItem.UsageName;
                    medicine.Amount = setItem.Amount;
                    tempList.Add(medicine);
                }
            }
            foreach (var setItem in p.Medicines)
            {
                if (Products.Count(m => m.ID.Equals(setItem.ID)) > 0) continue;
                var medList = tempList.Where(m => m.ID.Equals(setItem.ID));
                foreach (var item in medList)
                {
                    Products.Add(item);
                }
            }
        }
        public CooperativeViewHistory(DataRow r) { }
        public int PrescriptionID { get; set; }
        public string Institution { get; set; }
        public string Division { get; set; }
        public DateTime AdjustDate { get; set; }
        public int TotalPoint { get; set; }
        public Products Products { get; set; }
    }
}
