using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.NewClass.Prescription {
    public class PrescriptionPoint {
        public int ApplyPoint { get; set; }//申請點數 
        public int TotalPoint { get; set; } //總點數
        public int CopaymentPoint { get; set; } //部分負擔點數
        public int SpecialMaterialPoint { get; set; } //特殊材料費用
        public int TreatmentPoint { get; set; } //診療點數
        public int MedicinePoint { get; set; } //藥品點數
        public int MedicalServicePoint { get; set; } //藥事服務點數
        
    }
    
}
