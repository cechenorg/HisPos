using System.Data;

namespace His_Pos.NewClass.Product.Medicine.CooperativeAdjustMedicine {
     public class CooperativeAdjustMedicine :Product {
        public CooperativeAdjustMedicine() { }
        public CooperativeAdjustMedicine(DataRow dataRow):base(dataRow) {
            IsControl = dataRow.Field<byte?>("Med_Control"); 
            MedUseAmount = dataRow["MedicineAmount"].ToString();
        }  
        public int? IsControl { get; set; }  
        public string MedUseAmount { get; set; }
    }
}
