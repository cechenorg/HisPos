using System;
using System.Data;
using His_Pos.Interface;

namespace His_Pos.NewClass.Product.Medicine
{
    public class MedicineSpecialMaterial : Medicine, IDeletable
    {
        public MedicineSpecialMaterial() { }

        public MedicineSpecialMaterial(DataRow r) : base(r)
        {

        }
        public string BigCate { get; set; }//大類碼
        public string BigSmallCate { get; set; }//大小類
        public string BigSmallChiCate { get; set; }//大小類名稱
        public string ProductID { get; set; }//品名碼
        public string FunctionCate { get; set; }//功能類別
        public string FunctionChiCate { get; set; }//功能類別名稱
        public string SpecialID { get; set; }//特材代碼
        public string Type { get; set; }//產品型號或規格
        public string Unit { get; set; }//單位
        public DateTime StartDate { get; set; }//生效日期
        public DateTime ReviewDate { get; set; }//事前審查生效日期
        public string IsReview { get; set; }//事前審查
        public string RuleID { get; set; }//給付規定代碼
        public DateTime EndDate { get; set; }//生效迄日
        public string Manufactory { get; set; }//申請者簡稱
        public string LicenseID { get; set; }//許可證字號
        public string SelfPayItemName { get; set; }//自付差額品名
        public string LinkID { get; set; }//整組組件特材關聯代碼表
        private string source;
        public string Source
        {
            get => source;
            set
            {
                Set(() => Source, ref source, value);
            }
        }
    }
}
