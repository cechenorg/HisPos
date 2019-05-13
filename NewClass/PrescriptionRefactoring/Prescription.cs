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
    public enum PrescriptionType
    {
        Normal = 0,
        Chronic = 1,
        Reserve = 2,
        Cooperative = 3,
        Orthopedics = 4,
        Prescribe = 5
    }
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
        public IPrescriptionService PrescriptionService { get; set; }
        public PrescriptionType Type { get;private set; }
        #endregion

        public void CheckTypeByInstitution()
        {
            if (Treatment.Institution.CheckIsOrthopedics())
            {
                Type = PrescriptionType.Orthopedics;
                PrescriptionStatus.IsBuckle = false;
                PrescriptionService = new OrthopedicsPrescriptionService();
                return;
            }

            var clinic = Treatment.Institution.IsCooperativeClinic();
            if (clinic != null)
            {
                PrescriptionStatus.IsBuckle = clinic.IsBuckle;
                PrescriptionService = new CooperativePrescriptionService();
                return;
            }

            Type = PrescriptionType.Normal;
            PrescriptionStatus.IsBuckle = true;
            PrescriptionService = new NormalPrescriptionService();
        }

        public void CheckTypeByAdjustCase()
        {
            switch (Treatment.AdjustCase.ID)
            {
                case "0":
                    Type = PrescriptionType.Prescribe;
                    PrescriptionService = new PrescribePrescriptionService();
                    break;
                case "2":
                    if (Type == PrescriptionType.Normal)
                        PrescriptionService = new ChronicPrescriptionService();
                    break;
            }
        }
    }
}
