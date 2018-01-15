using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.Class
{
    public class Treatment
    {
        public Hospital Hospital { get; set; }//d21 原處方服務機構代號
        public Division Division { get; set; }//d13 就醫科別
        public MedicalPersonnel Doctor { get; set; }//d24 診治醫師代號
        public SpecialCode SpecialCode { get; set; }//d26 原處方服務機構之特定治療項目代號
        public PaymentCategory PaymentCategory { get; set; }//d5 給付類別
        public Copayment Copayment { get; set; }//d15 部分負擔代碼
        public TreatmentCase TreatmentCase { get; set; }//d22 原處方服務機構之案件分類
        public AdjustCase AdjustCase { get; set; }//d1 案件分類
        public List<DiseaseCode> DiseaseCodes { get; set; } = new List<DiseaseCode>();//d8.d9 國際疾病分類碼
        public string TreatmentDate { get; set; }//d14 就醫(處方)日期
        public string AdjustDate { get; set; }//d23 調劑日期
        public string MedicineDays { get; set; }//d30  給藥日份
    }
}
