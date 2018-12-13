using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using His_Pos.Class.Product;
using His_Pos.RDLC;
using His_Pos.Service;
using Microsoft.VisualBasic;

namespace His_Pos.Class.Declare
{
    public class DeclareData 
    {
        public DeclareData() { }
        public DeclareData(Prescription prescription)
        {
            Prescription = new Prescription();
            Prescription = prescription;
            D31SpecailMaterialPoint = 0;
            D32DiagnosisPoint = 0;
            D33DrugsPoint = 0;
            SetDeclareDetail();
            if (prescription.Treatment.AdjustCase.Id.Equals("0")) return;
            SetCopaymentPoint();
            var cusAge = DateTimeExtensions.CalculateAge(Prescription.Customer.Birthday);
            var medFormCount = CountOralLiquidAgent();
            var dayPay = CountDayPayAmount(cusAge, medFormCount);
            CountDeclareDeatailPoint(dayPay);
        }
     
        public DeclareData(DataRow row)
        {
            DecMasId = row["HISDECMAS_ID"].ToString();
            Prescription = new Prescription(row);
            D16DeclarePoint = Convert.ToInt32(row["HISDECMAS_POINT"].ToString());
            D17CopaymentPoint = Convert.ToInt32(row["HISDECMAS_COPAYMENTPOINT"].ToString());
            D18TotalPoint = Convert.ToInt32(row["HISDECMAS_TOTALPOINT"].ToString());
            DeclareDetails = PrescriptionDB.GetDeclareDetailByMasId(row["HISDECMAS_ID"].ToString());
        }
        public DeclareData(XmlNode xml) { //匯入處方申報檔用
            Prescription = new Prescription(xml);
            D4DeclareMakeUp = xml.SelectSingleNode("d4") == null ? null : xml.SelectSingleNode("d4").InnerText;
            D16DeclarePoint = xml.SelectSingleNode("d16") == null ? 0 : Convert.ToInt32(xml.SelectSingleNode("d16").InnerText);
            D17CopaymentPoint = xml.SelectSingleNode("d17") == null ? 0 : Convert.ToInt32(xml.SelectSingleNode("d17").InnerText);
            D18TotalPoint = xml.SelectSingleNode("d18") == null ? 0 : Convert.ToInt32(xml.SelectSingleNode("d18").InnerText);
            D31SpecailMaterialPoint = xml.SelectSingleNode("d31") == null ? 0 : Convert.ToInt32(xml.SelectSingleNode("d31").InnerText);
            D32DiagnosisPoint = xml.SelectSingleNode("d32") == null ? 0 : Convert.ToInt32(xml.SelectSingleNode("d32").InnerText);
            D33DrugsPoint = xml.SelectSingleNode("d33") == null ? 0 : Convert.ToInt32(xml.SelectSingleNode("d33").InnerText);
            D37MedicalServiceCode = xml.SelectSingleNode("d37") == null ? null : xml.SelectSingleNode("d37").InnerText;
            D38MedicalServicePoint = xml.SelectSingleNode("d38") == null ? 0 : Convert.ToInt32(xml.SelectSingleNode("d38").InnerText);
            XmlDocument tempxml = new XmlDocument();
            tempxml.LoadXml("<temp>" + xml.InnerXml + "</temp>");
            XmlNodeList pdatas = tempxml.GetElementsByTagName("pdata");
            foreach (XmlNode pdata in pdatas)
            {
                DeclareDetails.Add(new DeclareDetail(pdata));
            }
        }
        
        public string DecMasId { get; set; }
        public Prescription Prescription { get; set; }
        public ObservableCollection<DeclareDetail> DeclareDetails { get; set; } = new ObservableCollection<DeclareDetail>();
        public string D4DeclareMakeUp { get; set; }//D4補報註記
        public int D16DeclarePoint { get; set; }//D16申請點數
        public int D17CopaymentPoint { get; set; }//D17部分負擔點數
        public int D18TotalPoint { get; set; }//D18合計點數
        public int D31SpecailMaterialPoint { get; set; }//D31特殊材料明細點數小計
        public int D32DiagnosisPoint { get; set; }//D32診療明細點數小計
        public int D33DrugsPoint { get; set; }//D33用藥明細點數小計
        public string D37MedicalServiceCode { get; set; }//D37藥事服務費項目代號
        public string P2DayPayCode { get; set; }//P2 日劑藥費代號
        public int D38MedicalServicePoint { get; set; }//D38藥事服務費點數
        public Ddata DeclareXml { get; set; } = new Ddata();
        public string Id { get; set; }
        

        private void SetCopaymentPoint()
        {
            var copaymentId = Prescription.Treatment.Copayment.Id;
            D17CopaymentPoint = NewFunction.CheckCopaymentFreeProject(copaymentId) ? 0 : Prescription.Treatment.Copayment.Point;
        }

        private void CountDeclareDeatailPoint(int dayPay)
        {
            SetMedicalServiceCode(dayPay);//判斷藥事服務費項目代碼
            SetCopaymentPoint();//計算部分負擔點數
            D18TotalPoint = D31SpecailMaterialPoint + D32DiagnosisPoint + D33DrugsPoint + D38MedicalServicePoint;//計算總申報點數
            D16DeclarePoint = D18TotalPoint - D17CopaymentPoint;//申請點數 = 總申報點數 - 部分負擔點數
        }

        private int CountDayPayAmount(double cusAge, int medFormCount)
        {
            const int ma1 = 22, ma2 = 31, ma3 = 37, ma4 = 41;
            if (cusAge <= 12 && medFormCount == 1) return ma2;
            if (cusAge <= 12 && medFormCount == 2) return ma3;
            if (cusAge <= 12 && medFormCount >= 3) return ma4;
            return ma1;
        }

        private int CountOralLiquidAgent()
        {
            var medFormCount = 0;
            const string oralLiquidAgent = "口服液劑(原瓶包裝)";
            foreach (var med in Prescription.Medicines)
            {
                if (!(med is DeclareMedicine declare)) continue;
                if (declare.HcNote == null) continue;
                if (declare.HcNote.Contains(oralLiquidAgent) && !declare.PaySelf)
                    medFormCount++;
            }
            return medFormCount;
        }

        private void CheckDayPay(int dayPay)
        {
            D33DrugsPoint = dayPay * Convert.ToInt32(Prescription.Treatment.MedicineDays);
            AdjustCase.AdjustCase tmpAdjustCase = Prescription.Treatment.AdjustCase.ShallowCopy();
            tmpAdjustCase.Id = "3";
            Prescription.Treatment.AdjustCase = tmpAdjustCase;//將調劑案件轉換為日劑藥費
            Prescription.Treatment.MedicalInfo.TreatmentCase =
                MainWindow.TreatmentCase.SingleOrDefault(t => t.Id.Equals("01")).DeepCloneViaJson();
            switch (dayPay)
            {
                case 22:
                    P2DayPayCode = "MA1";
                    break;
                case 31:
                    P2DayPayCode = "MA2";
                    break;
                case 37:
                    P2DayPayCode = "MA3";
                    break;
                case 41:
                    P2DayPayCode = "MA4";
                    break;
            }
        }

        private void SetMedicalServiceCode(int dayPay)
        {
            /*
             * 申報案件不為「01：西醫一般案件」，藥費申請皆以健保署每月公告核定藥價實報實銷，
             * 另，案件分類「01：一般案件」案件，經採交付調劑，需應改以「09:西醫其他專案」申報。
             */
            if (Prescription.Treatment.MedicalInfo.TreatmentCase.Id.Equals("01"))
                Prescription.Treatment.MedicalInfo.TreatmentCase =
                    MainWindow.TreatmentCase.SingleOrDefault(t => t.Id.Equals("09"));
            var adjustCaseId = Prescription.Treatment.AdjustCase.Id;
            var treatmentCaseId = Prescription.Treatment.MedicalInfo.TreatmentCase.Id;
            const string westMedNormal = "01"; //原處方案件:西醫一般
            const string westMedOther = "09"; //原處方案件:西醫一般
            var medicineDays = Convert.ToInt32(Prescription.Treatment.MedicineDays);
            const int daysLimit = 3; //日劑藥費天數限制
            const int normalDaysLimit = 7; //西醫一般案件天數限制
            switch (adjustCaseId)
            {
                case "3" when treatmentCaseId == westMedNormal && medicineDays > daysLimit:
                    //throw new ArgumentException(Resources.MedicineDaysOutOfRange, "original");
                    break;
                case "1" :
                case "3":
                    if (medicineDays <= normalDaysLimit)
                    {
                        if (D33DrugsPoint <= dayPay * medicineDays && medicineDays <= daysLimit)
                            CheckDayPay(dayPay);
                        D37MedicalServiceCode = "05202B";//一般處方給付(7天以內)
                        D38MedicalServicePoint = 48;
                    }
                    break;
                case "2" :
                    SetChronicMedicalServiceCode();
                    break;
                default:
                    break;
            }

        }

        private void SetDeclareDetail()
        {
            var count = 1;
            foreach (var medicine in Prescription.Medicines)
            {
                switch (medicine)
                {
                    case PrescriptionOTC otc:
                        var detailOtc = new DeclareDetail(otc);
                        DeclareDetails.Add(detailOtc);
                        break;
                    case DeclareMedicine declare:
                        if (!declare.PaySelf)
                        {
                            var detail = new DeclareDetail(declare, count);
                            CountDeclarePoint(detail);
                            DeclareDetails.Add(detail);
                            count++;
                        }
                        else
                        {
                            var detail = new DeclareDetail(declare, 0);
                            DeclareDetails.Add(detail);
                        }
                        break;
                }
            }
        }

        private void CountDeclarePoint(DeclareDetail detail)
        {
            if (detail.P1MedicalOrder.Equals("1"))
                D33DrugsPoint += detail.P9Point;
            else if (detail.P1MedicalOrder.Equals("3"))
                D31SpecailMaterialPoint += detail.P9Point;
        }

        private void SetChronicMedicalServiceCode()
        {
            var medDays = int.Parse(Prescription.Treatment.MedicineDays);
            if (medDays >= 28)
            {
                D37MedicalServiceCode = "05210B";//門診藥事服務費－每人每日80件內-慢性病處方給藥28天以上-特約藥局(山地離島地區每人每日100件內)
                D38MedicalServicePoint = 69;
            }
            else if (medDays < 14)
            {
                D37MedicalServiceCode = "05223B";//門診藥事服務費-每人每日80件內-慢性病處方給藥13天以內-特約藥局(山地離島地區每人每日100件內)
                D38MedicalServicePoint = 48;
            }
            else
            {
                D37MedicalServiceCode = "05206B";//門診藥事服務費－每人每日80件內-慢性病處方給藥14-27天-特約藥局(山地離島地區每人每日100件內)
                D38MedicalServicePoint = 59;
            }
        }

        private Function function = new Function();
        public void CreatDeclareDataXmlObject()
        {
            var p = Prescription;
            var c = p.Customer;
            var t = p.Treatment;
            var m = t.MedicalInfo;
            var ic = c.IcCard;
            DeclareXml = new Ddata
            {
                Dhead = new Dhead
                {
                    D1 = Prescription.Treatment.AdjustCase.Id,
                    D3 = c.IcNumber,
                    D5 = t.PaymentCategory.Id,
                    D6 = DateTimeExtensions.ConvertToTaiwanCalender(c.Birthday, false),
                    D7 = CheckXmlEmptyValue(ic.MedicalNumber.PadLeft(4,'0')),
                    D8 = CheckXmlEmptyValue(m.MainDiseaseCode.Id),
                    D9 = CheckXmlEmptyValue(m.SecondDiseaseCode.Id),
                    D13 = CheckXmlEmptyValue(m.Hospital.Division.Id),
                    D14 = DateTimeExtensions.ConvertToTaiwanCalender(t.TreatmentDate, false),
                    D15 = CheckXmlEmptyValue(t.Copayment.Id),
                    D16 = D16DeclarePoint.ToString(),
                    D17 = D17CopaymentPoint.ToString(),
                    D18 = D18TotalPoint.ToString(),
                    D20 = Strings.StrConv(c.Name, VbStrConv.Narrow),
                    D21 = CheckXmlEmptyValue(m.Hospital.Id),
                    D22 = CheckXmlEmptyValue(m.TreatmentCase.Id),
                    D23 = DateTimeExtensions.ConvertToTaiwanCalender(t.AdjustDate, false),
                    D24 = string.IsNullOrEmpty(m.Hospital.Doctor.IcNumber) ? m.Hospital.Id : m.Hospital.Doctor.IcNumber,
                    D25 = p.Pharmacy.MedicalPersonnel.IcNumber
                },
                Dbody = new Dbody
                {
                    D30 = t.MedicineDays,
                    D31 = D31SpecailMaterialPoint.ToString(),
                    D32 = D32DiagnosisPoint.ToString(),
                    D33 = D33DrugsPoint.ToString(),
                    D35 = CheckXmlEmptyValue(p.ChronicSequence),
                    D36 = CheckXmlEmptyValue(p.ChronicTotal),
                    D37 = D37MedicalServiceCode,
                    D38 = D38MedicalServicePoint.ToString(),
                    D44 = t.Copayment.Id.Equals("903")?DateTimeExtensions.ConvertToTaiwanCalender(ic.IcMarks.NewbornsData.Birthday, false):string.Empty
                }
            };
            if (!string.IsNullOrEmpty(D4DeclareMakeUp))
            {
                DeclareXml.Dhead.D4 = D4DeclareMakeUp;
            }

            if (!string.IsNullOrEmpty(t.AdjustCase.Id) && !t.AdjustCase.Id.StartsWith("D") && !t.AdjustCase.Id.StartsWith("5"))
            {
                if (string.IsNullOrEmpty(m.Hospital.Doctor.IcNumber))
                {
                    m.Hospital.Doctor.IcNumber = m.Hospital.Id;
                }
                DeclareXml.Dhead.D24 = m.Hospital.Doctor.IcNumber;
                DeclareXml.Dbody.D26 = t.MedicalInfo.SpecialCode is null? string.Empty: t.MedicalInfo.SpecialCode.Id;
            }

            if (!string.IsNullOrEmpty(p.ChronicSequence))
            {
                if (int.Parse(p.ChronicSequence) >= 2)
                {
                    DeclareXml.Dbody.D43 = p.OriginalMedicalNumber.PadLeft(4,'0');
                }
            }

            if (DeclareDetails.Count <= 0) return;
            DeclareXml.Dbody.Pdata = new List<Pdata>();
            var declareCount = 1;
            var specialCount = 0;
            foreach (var detail in DeclareDetails)
            {
                var pdata = new Pdata
                {
                    P1 = detail.P1MedicalOrder,
                    P2 = detail.P2MedicalId,
                    P3 = function.SetStrFormat(detail.P3Dosage, "{0:0000.00}"),
                    P4 = detail.P4Usage,
                    P5 = detail.P5Position,
                    P6 = detail.P6Percent.ToString(),
                    P7 = function.SetStrFormat(detail.P7Total, "{0:00000.0}"),
                    P8 = function.SetStrFormat(detail.P8Price, "{0:0000000.00}"),
                    P9 = function.SetStrFormatInt(detail.P9Point,"{0:D8}"),
                    P10 = function.SetStrFormatInt(declareCount, "{0:D3}"),
                    P11 = detail.P11Days.ToString(),
                    P12 = detail.P12StartDate,
                    P13 = detail.P13EndDate,
                };
                if (pdata.P1.Equals("D") || pdata.P1.Equals("E") || pdata.P1.Equals("F"))
                {
                    specialCount++;
                    pdata.P15 = specialCount.ToString();
                }

                DeclareXml.Dbody.Pdata.Add(pdata);
                declareCount++;
            }
        }

        public string SerializeObject<T>()
        {
            CreatDeclareDataXmlObject();
            var xmlSerializer = new XmlSerializer(DeclareXml.GetType());
            using (var textWriter = new StringWriter())
            {
                xmlSerializer.Serialize(textWriter, DeclareXml);
                var document = XDocument.Parse(ReportService.PrettyXml(textWriter));
                document.Root?.RemoveAttributes();
                return document.ToString();
            }
        }

        private string CheckXmlEmptyValue(string value)
        {
            if (value != null)
                return value.Length > 0 ? value : string.Empty;
            return null;
        }
    }
}