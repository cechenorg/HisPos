using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
