using His_Pos.Properties;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using His_Pos.Class.Product;

namespace His_Pos.Class
{
    public class Prescription
    {
        public Prescription()
        {
        }
        private Pharmacy Pharmacy { get; set; }
        private Treatment Treatment { get; set; }
        private List<Medicine> Medicines { get; set; }
        public void SetTotalPoint(Customer cus)
        {
            int medFormCount = 0;
            short copaymentPoint = 0;
            var dayPay = 22; //日劑藥費最低費用
            var twc = new TaiwanCalendar();
            var year = twc.GetYear(DateTime.Now).ToString();
            var cusAge = Convert.ToInt32(year) - Convert.ToInt32(cus.Birthday.Substring(0, 3));
            foreach (Medicine med in Medicines) {
                if (med.MedicalCategory.Form == "內服液劑")
                    medFormCount++;
            }
            if (cusAge <= 12 && medFormCount == 1) dayPay = 31;
            if (cusAge <= 12 && medFormCount == 2) dayPay = 37;
            if (cusAge <= 12 && medFormCount == 3) dayPay = 41;

            switch (Treatment.AdjustCase.Id)
            {
                case "3" when _d22 == "01" && _d30 > 3:
                    throw new ArgumentException(Resources.MedicineDaysOutOfRange, "original");
                case "1" when _d22 == "01" && _d30 <= 3 && _d33 <= dayPay * _d30:
                    _d33 = dayPay * _d30;
                    _d1 = "3";
                    switch (dayPay)
                    {
                        case 22:
                            _d37 = "MA1";
                            break;
                        case 31:
                            _d37 = "MA2";
                            break;
                        case 37:
                            _d37 = "MA3";
                            break;
                        case 41:
                            _d37 = "MA4";
                            break;
                    }
                    break;
            }

            _d18 = _d31 + _d32 + _d33 + _d38;
            _d16 = _d18 - _d17;

            switch (_d15)
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
                    if (_d33 > 100 && _d33 <= 200)
                        copaymentPoint = 20;
                    else if (_d33 > 201 && _d33 <= 300)
                        copaymentPoint = 40;
                    else if (_d33 > 301 && _d33 <= 400)
                        copaymentPoint = 60;
                    else if (_d33 > 401 && _d33 <= 500)
                        copaymentPoint = 80;
                    else if (_d33 > 501 && _d33 <= 600)
                        copaymentPoint = 100;
                    else if (_d33 > 601 && _d33 <= 700)
                        copaymentPoint = 120;
                    else if (_d33 > 701 && _d33 <= 800)
                        copaymentPoint = 140;
                    else if (_d33 > 801 && _d33 <= 900)
                        copaymentPoint = 160;
                    else if (_d33 > 901 && _d33 <= 1000)
                        copaymentPoint = 180;
                    else if (_d33 > 1001)
                        copaymentPoint = 200;
                    break;
                case "I21"://藥費小於100免收
                    copaymentPoint = 0;
                    break;
                /*慢性病連續處方箋案件、牙醫案件、門診論病例計酬案件免收*/
                case "I22":
                    if (_d22.Equals("08") || _d22.Equals("11") || _d22.Equals("12") || _d22.Equals("13") || _d22.Equals("14") || _d22.Equals("15") || _d22.Equals("16") || _d22.Equals("19"))
                        copaymentPoint = 0;
                    break;
                case "Z00":
                    if (_d33 > 100 && _d33 <= 200)
                        copaymentPoint = 20;
                    else if (_d33 > 201 && _d33 <= 300)
                        copaymentPoint = 40;
                    else if (_d33 > 301 && _d33 <= 400)
                        copaymentPoint = 60;
                    else if (_d33 > 401 && _d33 <= 500)
                        copaymentPoint = 80;
                    else if (_d33 > 501 && _d33 <= 600)
                        copaymentPoint = 100;
                    else if (_d33 > 601 && _d33 <= 700)
                        copaymentPoint = 120;
                    else if (_d33 > 701 && _d33 <= 800)
                        copaymentPoint = 140;
                    else if (_d33 > 801 && _d33 <= 900)
                        copaymentPoint = 160;
                    else if (_d33 > 901 && _d33 <= 1000)
                        copaymentPoint = 180;
                    else if (_d33 > 1001)
                        copaymentPoint = 200;
                    break;
            }
            //部分負擔點數(個人/行政)
            if (_d15.Equals("003") || _d15.Equals("004") || _d15.Equals("005") || _d15.Equals("006") || _d15.Equals("901") || _d15.Equals("902") ||
                _d15.Equals("903") || _d15.Equals("904") || _d15.Equals("906"))
            {
                _d17 = 0;
                _d19 = copaymentPoint;
            }
            else
            {
                _d17 = copaymentPoint;
                _d19 = 0;
            }
            //申請點數
            _d16 = _d18 - _d17;
        }
        public XmlDocument CreateToXml(List<DeclareDetail> de)
        {
            string pData, dData;
            var xml = new XmlDocument();
            int diseasecodecount = 8;
            dData = "<ddata><dhead>";
            dData += "<d1>" + Treatment.AdjustCase.Id + "</d1>";
            dData += "<d2></d2>";
            dData += "<d3>" + Treatment.Customer.IcNumber + "</d3>";
            //D4 : 補報原因 非補報免填
            if (D4 != DBNull.Value.ToString(CultureInfo.CurrentCulture))
                dData += "<d4>" + D4 + "</d4>";
            if (Treatment.AdjustCase.Id != DBNull.Value.ToString(CultureInfo.CurrentCulture) && (Treatment.AdjustCase.Id.Equals("2") || Treatment.AdjustCase.Id.Equals("D")))
                dData += "<d5>" + Treatment.PaymentCategory.Id + "</d5>";
            dData += "<d6>" + Treatment.Customer.Birthday + "</d6>";
            dData += "<d7>" + D7 + "</d7>";
            /*
             D8 ~ D12 國際疾病代碼
             */
            foreach (DiseaseCode diseasecode in Treatment.MedicalInfo.DiseaseCodes)
            {
                dData += "<d" + diseasecodecount + ">" + diseasecode.Id + "</d" + diseasecodecount + ">";
                diseasecodecount++;

            }
            if (Treatment.MedicalInfo.Hospital.Division.Id != DBNull.Value.ToString(CultureInfo.CurrentCulture))
                dData += "<d13>" + Treatment.MedicalInfo.Hospital.Division.Id + "</d13>";
            if (Treatment.TreatmentDate != DBNull.Value.ToString(CultureInfo.CurrentCulture))
                dData += "<d14>" + Treatment.TreatmentDate + "</d14>";
            dData += "<d15>" + Treatment.Copayment.Id + "</d15>";
            dData += "<d16>" + D16 + "</d16>";
            dData += "<d17>" + Treatment.Copayment.Point + "</d17>";
            dData += "<d18>" + D18 + "</d18>";
            if (D19.ToString() != DBNull.Value.ToString(CultureInfo.CurrentCulture))
                dData += "<d19>" + D19 + "</d19>";
            dData += "<d20>" + Treatment.Customer.Name + "</d20>";
            dData += "<d21>" + Treatment.MedicalInfo.Hospital.Id + "</d21>";
            dData += "<d22>" + Treatment.MedicalInfo.TreatmentCase.Id + "</d22>";
            dData += "<d23>" + Treatment.AdjustDate + "</d23>";
            dData += "<d24>" + Treatment.MedicalInfo.Hospital.Doctor.Id + "</d24>";
            dData += "<d25>" + Treatment.MedicalPersonId + "</d25>";
            if (Treatment.MedicalInfo.SpecialCode.Id != DBNull.Value.ToString(CultureInfo.CurrentCulture))
                dData += "<d26>" + Treatment.MedicalInfo.SpecialCode.Id + "</d26>";




            if (D30.ToString() != DBNull.Value.ToString(CultureInfo.CurrentCulture))
                dData += "<d30>" + D30 + "</d30>";
            if (D31.ToString() != DBNull.Value.ToString(CultureInfo.CurrentCulture))
                dData += "<d31>" + D31 + "</d31>";
            if (D32.ToString() != DBNull.Value.ToString(CultureInfo.CurrentCulture))
                dData += "<d32>" + D32 + "</d32>";
            if (D33.ToString() != DBNull.Value.ToString(CultureInfo.CurrentCulture))
                dData += "<d33>" + D33 + "</d33>";
            //免填 dData += "<d34>" + "" + "</d34>";
            if (Treatment.AdjustCase.Id.Equals("2"))
            {
                dData += "<d35>" + D35 + "</d35>";
                dData += "<d36>" + D36 + "</d36>";
            }
            //待確認
            /*if (d37.ToString() != DBNull.Value.ToString())*/
            dData += "<d37>" + D37 + "</d37>";
            /*if (d38.ToString() != DBNull.Value.ToString())*/
            dData += "<d38>" + D38 + "</d38>";
            //D39~D42免填
            //dData += "<d39>" + "" + "</d39>";      
            //dData += "<d40>" + "" + "</d40>";         
            //dData += "<d43>" + "" + "</d43>";
            //dData += "<d41>" + "" + "</d41>";
            //dData += "<d42>" + "" + "</d42>";
            /*if (d44.ToString() != DBNull.Value.ToString())*/
            if (Treatment.AdjustCase.Id.Equals("2") && Convert.ToDecimal(D35) >= 2)
                dData += "<d43>" + D43 + "</d43>";
            //待確認 新生兒註記就醫
            dData += "<d44>" + "" + "</d44>";
            //待確認 矯正機關代號
            dData += "<d45>" + "" + "</d45>";
            //特定地區醫療服務 免填
            //dData += "<d46>" + "" + "</d46>";
            dData += "</dhead>";
            foreach (Medicine detail in Medicines)
            {
                pData = "<pdata>";
                pData += "<p1>" + detail.Id + "</p1>";
                pData += "<p2>" + detail.P2 + "</p2>";
                pData += "<p7>" + detail.P7 + "</p7>";
                pData += "<p8>" + detail.P8 + "</p8>";
                pData += "<p9>" + detail.P9 + "</p9>";
                if (detail.P3 != DBNull.Value.ToString(CultureInfo.CurrentCulture)) pData += "<p3>" + detail.P3 + "</p3>";
                if (detail.P4 != DBNull.Value.ToString(CultureInfo.CurrentCulture)) pData += "<p4>" + detail.P4 + "</p4>";
                if (detail.P5 != DBNull.Value.ToString(CultureInfo.CurrentCulture)) pData += "<p5>" + detail.P5 + "</p5>";
                if (detail.P6 != DBNull.Value.ToString(CultureInfo.CurrentCulture)) pData += "<p6>" + detail.P6 + "</p6>";
                pData += "<p10>" + detail.P10 + "</p10>";
                if (detail.P11.ToString() != DBNull.Value.ToString(CultureInfo.CurrentCulture))
                {
                    pData += "<p11>" + detail.P11 + "</p11>";
                    if (_d30 < detail.P11)
                        _d30 = detail.P11;
                }
                pData += "</pdata>";
                dData += pData;
            }

            dData += "</ddata>";
            xml.LoadXml(dData);
            return xml;
        }
    }
}
