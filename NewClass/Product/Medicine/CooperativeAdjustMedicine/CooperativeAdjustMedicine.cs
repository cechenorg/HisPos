using System.Data;

namespace His_Pos.NewClass.Product.Medicine.CooperativeAdjustMedicine {
     public class CooperativeAdjustMedicine {
        public CooperativeAdjustMedicine() { }
        public CooperativeAdjustMedicine(DataRow dataRow) {
            MedId = dataRow["CooPreDet_MedicineID"].ToString();
            MedName = dataRow["Pro_ChineseName"].ToString();
            MedUseAmount = dataRow["MedicineAmount"].ToString();
        }  
        public string MedId { get; set; }
        public string MedName { get; set; }
        public string MedUseAmount { get; set; }
    }
}
