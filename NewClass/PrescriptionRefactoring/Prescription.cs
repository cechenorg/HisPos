using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using His_Pos.ChromeTabViewModel;
using His_Pos.Interface;
using His_Pos.NewClass.Cooperative.CooperativeClinicSetting;
using His_Pos.NewClass.MedicineRefactoring;
using His_Pos.NewClass.Prescription;
using His_Pos.NewClass.Prescription.Treatment;

namespace His_Pos.NewClass.PrescriptionRefactoring
{
    public class Prescription
    {
        public Prescription()
        {

        }
        #region Properties
        public IcCard Card { get; set; }
        public Treatment Treatment { get; set; }//處方資料
        public int MedicineDays { get; set; } //給藥日份
        public string MedicalServiceCode { get; set; } //藥事服務代碼 
        public XDocument DeclareContent { get; set; } = new XDocument(); //申報檔內容
        public int? DeclareFileID { get; set; } //申報檔ID
        public PrescriptionPoint PrescriptionPoint { get; set; } = new PrescriptionPoint(); //處方點數區
        public PrescriptionStatus PrescriptionStatus { get; set; } = new PrescriptionStatus(); //處方狀態區
        public List<string> PrescriptionSign { get; set; }
        public Medicines Medicines { get; set; }
        
        #endregion
    }
}
