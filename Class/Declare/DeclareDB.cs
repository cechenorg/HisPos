using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace His_Pos.Class.Declare
{
    class DeclareDB
    {
        public XmlDocument CreateToXml(DeclareData declareData)
        {
            var xml = new XmlDocument();
            var diseasecodecount = 8;
            var dData = "<ddata><dhead>";
            dData += "<d1>" + declareData.Prescription.Treatment.AdjustCase.Id + "</d1>";
            dData += "<d2></d2>";
            dData += "<d3>" + declareData.Prescription.Treatment.Customer.IcNumber + "</d3>";
            //D4 : 補報原因 非補報免填
            if (declareData.DeclareMakeUp != DBNull.Value.ToString(CultureInfo.InvariantCulture))
                dData += "<d4>" + declareData.DeclareMakeUp + "</d4>";
            if (declareData.Prescription.Treatment.AdjustCase.Id != DBNull.Value.ToString(CultureInfo.InvariantCulture) && (declareData.Prescription.Treatment.AdjustCase.Id.Equals("2") || declareData.Prescription.Treatment.AdjustCase.Id.Equals("D")))
                dData += "<d5>" + declareData.Prescription.Treatment.PaymentCategory.Id + "</d5>";
            dData += "<d6>" + declareData.Prescription.Treatment.Customer.Birthday + "</d6>";
            dData += "<d7>" + declareData.Prescription.IcCard.MedicalNumber + "</d7>";
             //D8 ~ D12 國際疾病代碼
            foreach (var diseasecode in declareData.Prescription.Treatment.MedicalInfo.DiseaseCodes)
            {
                dData += "<d" + diseasecodecount + ">" + diseasecode.Id + "</d" + diseasecodecount + ">";
                diseasecodecount++;
            }
            if (declareData.Prescription.Treatment.MedicalInfo.Hospital.Division.Id != DBNull.Value.ToString(CultureInfo.InvariantCulture))
                dData += "<d13>" + declareData.Prescription.Treatment.MedicalInfo.Hospital.Division.Id + "</d13>";
            if (declareData.Prescription.Treatment.TreatmentDate != DBNull.Value.ToString(CultureInfo.InvariantCulture))
                dData += "<d14>" + declareData.Prescription.Treatment.TreatmentDate + "</d14>";
            dData += "<d15>" + declareData.Prescription.Treatment.Copayment.Id + "</d15>";
            dData += "<d16>" + declareData.DeclarePoint + "</d16>";
            dData += "<d17>" + declareData.Prescription.Treatment.Copayment.Point + "</d17>";
            dData += "<d18>" + declareData.TotalPoint + "</d18>";
            if (declareData.AssistProjectCopaymentPoint.ToString(CultureInfo.InvariantCulture) != DBNull.Value.ToString(CultureInfo.InvariantCulture))
                dData += "<d19>" + declareData.AssistProjectCopaymentPoint + "</d19>";
            dData += "<d20>" + declareData.Prescription.Treatment.Customer.Name + "</d20>";
            dData += "<d21>" + declareData.Prescription.Treatment.MedicalInfo.Hospital.Id + "</d21>";
            dData += "<d22>" + declareData.Prescription.Treatment.MedicalInfo.TreatmentCase.Id + "</d22>";
            dData += "<d23>" + declareData.Prescription.Treatment.AdjustDate + "</d23>";
            dData += "<d24>" + declareData.Prescription.Treatment.MedicalInfo.Hospital.Doctor.Id + "</d24>";
            dData += "<d25>" + declareData.Prescription.Treatment.MedicalPersonId + "</d25>";
            if (declareData.Prescription.Treatment.MedicalInfo.SpecialCode.Id != DBNull.Value.ToString(CultureInfo.InvariantCulture))
                dData += "<d26>" + declareData.Prescription.Treatment.MedicalInfo.SpecialCode.Id + "</d26>";           
            if (declareData.Prescription.Treatment.MedicineDays != DBNull.Value.ToString(CultureInfo.InvariantCulture))
                dData += "<d30>" + declareData.Prescription.Treatment.MedicineDays + "</d30>";
            if (declareData.SpecailMaterialPoint.ToString() != DBNull.Value.ToString(CultureInfo.InvariantCulture))
                dData += "<d31>" + declareData.SpecailMaterialPoint + "</d31>";
            if (declareData.DiagnosisPoint.ToString() != DBNull.Value.ToString(CultureInfo.InvariantCulture))
                dData += "<d32>" + declareData.DiagnosisPoint + "</d32>";
            if (declareData.DrugsPoint.ToString() != DBNull.Value.ToString(CultureInfo.InvariantCulture))
                dData += "<d33>" + declareData.DrugsPoint + "</d33>";
            //免填 dData += "<d34>" + "" + "</d34>";
            if (declareData.Prescription.Treatment.AdjustCase.Id.Equals("2"))
            {
                dData += "<d35>" + declareData.ChronicSequence + "</d35>";
                dData += "<d36>" + declareData.ChronicTotal + "</d36>";
            }
            //待確認
            /*if (d37.ToString() != DBNull.Value.ToString())*/
            dData += "<d37>" + declareData.MedicalServiceCode + "</d37>";
            /*if (d38.ToString() != DBNull.Value.ToString())*/
            dData += "<d38>" + declareData.MedicalServicePoint + "</d38>";
            //D39~D42免填
            /*if (d44.ToString() != DBNull.Value.ToString())*/
            if (declareData.Prescription.Treatment.AdjustCase.Id.Equals("2") && Convert.ToDecimal(declareData.ChronicSequence) >= 2)
                dData += "<d43>" + declareData.Prescription.OriginalMedicalNumber + "</d43>";
            //待確認 新生兒註記就醫
            dData += "<d44>" + "" + "</d44>";
            //待確認 矯正機關代號
            dData += "<d45>" + "" + "</d45>";
            //特定地區醫療服務 免填
            dData += "</dhead>";
            foreach (var detail in declareData.DeclareDetail)
            {
                var pData = "<pdata>";
                pData += "<p1>" + detail.MedicalOrder + "</p1>";
                pData += "<p2>" + detail.MedicalId + "</p2>";
                pData += "<p7>" + detail.Total + "</p7>";
                pData += "<p8>" + detail.Price + "</p8>";
                pData += "<p9>" + detail.Point + "</p9>";
                if (detail.Dosage.ToString(CultureInfo.InvariantCulture) != DBNull.Value.ToString(CultureInfo.InvariantCulture)) pData += "<p3>" + detail.Dosage + "</p3>";
                if (detail.Usage != DBNull.Value.ToString(CultureInfo.InvariantCulture)) pData += "<p4>" + detail.Usage + "</p4>";
                if (detail.Position != DBNull.Value.ToString(CultureInfo.InvariantCulture)) pData += "<p5>" + detail.Position + "</p5>";
                if (detail.Percent.ToString(CultureInfo.InvariantCulture) != DBNull.Value.ToString(CultureInfo.InvariantCulture)) pData += "<p6>" + detail.Percent + "</p6>";
                pData += "<p10>" + detail.Sequence + "</p10>";
                if (detail.Days.ToString() != DBNull.Value.ToString(CultureInfo.InvariantCulture))
                {
                    pData += "<p11>" + detail.Days + "</p11>";
                    if (Convert.ToInt32(declareData.Prescription.Treatment.MedicineDays) < detail.Days)
                        declareData.Prescription.Treatment.MedicineDays = detail.Days.ToString();
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
