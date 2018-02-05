using His_Pos.Properties;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.Class
{
    internal class DeclareData
    {
        public DeclareData(Prescription prescription)
        {
            Prescription = new Prescription();
            Prescription = prescription;
        }
        public DeclareData(ChronicPrescription chronicPrescription)
        {
            ChronicPrescription = new ChronicPrescription();
            ChronicPrescription = chronicPrescription;
        }

        public Prescription Prescription {get;set;}
        public ChronicPrescription ChronicPrescription {get; set;}
        public List<DeclareDetail> DeclareDetail { get; set; }
        public string D4 { get; set; } //補報註記
        public int D16 { get; set; } //申請點數
        public int D17 { get; set; }//部分負擔點數
        public int D18 { get; set; }//合計點數
        public int D19 { get; set; }//行政協助項目部分負擔點數
        public int D31 { get; set; }//特殊材料明細點數小計
        public int D32 { get; set; }//診療明細點數小計
        public int D33 { get; set; }//用藥明細點數小計
        public string D35 { get; set; }//連續處方箋調劑序號
        public string D36 { get; set; }//連續處方可調劑次數
        public string D37 { get; set; }//藥事服務費項目代號
        public int D38 { get; set; }//藥事服務費點數

        public void CountDeclareDeatailPoint() {
            int medFormCount = 0;
            short copaymentPoint = 0;
            var dayPay = 22; //日劑藥費最低費用
            var twc = new TaiwanCalendar();
            var year = twc.GetYear(DateTime.Now).ToString();
            var cusAge = Convert.ToInt32(year) - Convert.ToInt32(Prescription.Treatment.Customer.Birthday.Substring(0, 3));
            foreach (var med in Prescription.Medicines)
            {
                if (med.MedicalCategory.Form == "內服液劑")
                    medFormCount++;
            }
            if (cusAge <= 12 && medFormCount == 1) dayPay = 31;
            if (cusAge <= 12 && medFormCount == 2) dayPay = 37;
            if (cusAge <= 12 && medFormCount == 3) dayPay = 41;

            switch (Prescription.Treatment.AdjustCase.Id)
            {
                case "3" when Prescription.Treatment.MedicalInfo.TreatmentCase.Id == "01" && Convert.ToInt32(Prescription.Treatment.MedicineDays) > 3:
                    throw new ArgumentException(Resources.MedicineDaysOutOfRange, "original");
                case "1" when Prescription.Treatment.MedicalInfo.TreatmentCase.Id == "01" && Convert.ToInt32(Prescription.Treatment.MedicineDays) <= 3 && D33 <= dayPay * Convert.ToInt32(Prescription.Treatment.MedicineDays):
                    D33 = dayPay * Convert.ToInt32(Prescription.Treatment.MedicineDays);
                    Prescription.Treatment.AdjustCase.Id = "3";
                    switch (dayPay)
                    {
                        case 22:
                            D37 = "MA1";
                            break;
                        case 31:
                            D37 = "MA2";
                            break;
                        case 37:
                            D37 = "MA3";
                            break;
                        case 41:
                            D37 = "MA4";
                            break;
                    }
                    break;
            }

            D18 = D31 + D32 + D33 + D38;
            D16 = D18 - D17;

            switch (Prescription.Treatment.Copayment.ToString())
            {
                case "001":
                    copaymentPoint = 0;
                    break;
                case "002":
                    copaymentPoint = 0;
                    break;
                case "007":
                    copaymentPoint = 0;
                    break;
                case "008":
                    copaymentPoint = 0;
                    break;
                case "009":
                    copaymentPoint = 0;
                    break;
                case "801":
                    copaymentPoint = 0;
                    break;
                case "802":
                    copaymentPoint = 0;
                    break;
                case "905":
                    copaymentPoint = 0;
                    break;
                case "I20": //藥費大於100須收部分負擔
                    if (D33 > 100 && D33 <= 200)
                        copaymentPoint = 20;
                    else if (D33 > 201 && D33 <= 300)
                        copaymentPoint = 40;
                    else if (D33 > 301 && D33 <= 400)
                        copaymentPoint = 60;
                    else if (D33 > 401 && D33 <= 500)
                        copaymentPoint = 80;
                    else if (D33 > 501 && D33 <= 600)
                        copaymentPoint = 100;
                    else if (D33 > 601 && D33 <= 700)
                        copaymentPoint = 120;
                    else if (D33 > 701 && D33 <= 800)
                        copaymentPoint = 140;
                    else if (D33 > 801 && D33 <= 900)
                        copaymentPoint = 160;
                    else if (D33 > 901 && D33 <= 1000)
                        copaymentPoint = 180;
                    else if (D33 > 1001)
                        copaymentPoint = 200;
                    break;
                case "I21"://藥費小於100免收
                    copaymentPoint = 0;
                    break;
                /*慢性病連續處方箋案件、牙醫案件、門診論病例計酬案件免收*/
                case "I22":
                    if (Prescription.Treatment.MedicalInfo.TreatmentCase.Equals("08") 
                        || Prescription.Treatment.MedicalInfo.TreatmentCase.Equals("11") 
                        || Prescription.Treatment.MedicalInfo.TreatmentCase.Equals("12") 
                        || Prescription.Treatment.MedicalInfo.TreatmentCase.Equals("13") 
                        || Prescription.Treatment.MedicalInfo.TreatmentCase.Equals("14") 
                        || Prescription.Treatment.MedicalInfo.TreatmentCase.Equals("15") 
                        || Prescription.Treatment.MedicalInfo.TreatmentCase.Equals("16") 
                        || Prescription.Treatment.MedicalInfo.TreatmentCase.Equals("19"))
                        copaymentPoint = 0;
                    break;
                case "Z00":
                    if (D33 > 100 && D33 <= 200)
                        copaymentPoint = 20;
                    else if (D33 > 201 && D33 <= 300)
                        copaymentPoint = 40;
                    else if (D33 > 301 && D33 <= 400)
                        copaymentPoint = 60;
                    else if (D33 > 401 && D33 <= 500)
                        copaymentPoint = 80;
                    else if (D33 > 501 && D33 <= 600)
                        copaymentPoint = 100;
                    else if (D33 > 601 && D33 <= 700)
                        copaymentPoint = 120;
                    else if (D33 > 701 && D33 <= 800)
                        copaymentPoint = 140;
                    else if (D33 > 801 && D33 <= 900)
                        copaymentPoint = 160;
                    else if (D33 > 901 && D33 <= 1000)
                        copaymentPoint = 180;
                    else if (D33 > 1001)
                        copaymentPoint = 200;
                    break;
            }
            //部分負擔點數(個人/行政)
            if (Prescription.Treatment.Copayment.Equals("003") 
                || Prescription.Treatment.Copayment.Equals("004") 
                || Prescription.Treatment.Copayment.Equals("005") 
                || Prescription.Treatment.Copayment.Equals("006") 
                || Prescription.Treatment.Copayment.Equals("901") 
                || Prescription.Treatment.Copayment.Equals("902") 
                || Prescription.Treatment.Copayment.Equals("903") 
                || Prescription.Treatment.Copayment.Equals("904") 
                || Prescription.Treatment.Copayment.Equals("906"))
            {
                D17 = 0;
                D19 = copaymentPoint;
            }
            else
            {
                D17 = copaymentPoint;
                D19 = 0;
            }
            //申請點數
            D16 = D18 - D17;
        }
    }
}
