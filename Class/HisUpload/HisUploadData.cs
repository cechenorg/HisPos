using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using His_Pos.Class.Declare;

namespace His_Pos.Class
{
    class HisUploadData
    {
        public List<DeclareDetail> DeclareDetails { get; set; }
        public Prescription Prescription { get; set; }
        public string SafeCode { get; set; } //安全模組代碼
        public string DeclareMakeUp { get; set; } //補報註記
        public string RsaNum { get; set; } //安全簽章
        public string DataFormat { get; set; } //資料格式 1:正常上傳 2:異常上傳 3.補正上傳(正常資料) 4.補正上傳(異常資料)
        public XmlDocument CreateToXml()
        {
            string msh, mb1, mb2;
            XmlDocument xml = new XmlDocument();

            //msh資料段
            msh = "<MSH>";
            msh += "<A00>" + "1" + "</A00>";
            msh += "<A01>" + DataFormat + "</A01>";
            msh += "<A02>" + "1.0" + "</A02>";
            msh += "</MSH>";

            //mb1 健保資料段

            mb1 = "<MB1>";
            mb1 += "<A11>" + Prescription.IcCard.ICNumber + "</A11>";
            mb1 += "<A12>" + Prescription.IcCard.Customer.IcNumber + "</A12>";
            mb1 += "<A13>" + Prescription.IcCard.Customer.Birthday + "</A13>";
            mb1 += "<A14>" + "30" + "</A14>";
            mb1 += "<A15>" + Prescription.Treatment.MedicalPersonId + "</A15>";
            mb1 += "<A16>" + SafeCode + "</A16>";
            mb1 += "<A17>" + Prescription.Treatment.TreatmentDate.ToShortDateString() + "</A17>";
            mb1 += "<A18>" + Prescription.OriginalMedicalNumber + "</A18>";
            if (DeclareMakeUp != string.Empty) mb1 += "<A19>" + DeclareMakeUp + "</A19>";
            if (Prescription.IcCard.IcMarks.NewbornsData.Birthday != string.Empty) mb1 += "<A20>" + Prescription.IcCard.IcMarks.NewbornsData.Birthday + "</A20>";
            if (Prescription.IcCard.IcMarks.NewbornsData.NewbornMark != string.Empty) mb1 += "<A21>" + Prescription.IcCard.IcMarks.NewbornsData.NewbornMark + "</A21>";
            mb1 += "<A22>" + RsaNum + "</A22>";
            mb1 += "<A23>" + Prescription.Treatment.MedicalInfo.Hospital.Division + "</A23>";
            if (Prescription.IcCard.IcMarks.NewbornsData.TreatMark != string.Empty) mb1 += "<A24>" + Prescription.IcCard.IcMarks.NewbornsData.TreatMark + "</A24>";
            int diseasecodecount = 25;
            foreach (var diseasecode in Prescription.Treatment.MedicalInfo.DiseaseCodes)
            {
                mb1 += "<A" + diseasecodecount + ">" + diseasecode.Id + "</A" + diseasecodecount + ">";
                diseasecodecount++;
            }

                
            if (Prescription.IcCard.IcCardPay.MedicalPay != string.Empty) mb1 += "<A31>" + Prescription.IcCard.IcCardPay.MedicalPay + "</A31>";
            if (Prescription.IcCard.IcCardPay.MedicalCopay != string.Empty) mb1 += "<A32>" + Prescription.IcCard.IcCardPay.MedicalCopay + "</A32>";
            if (Prescription.IcCard.IcCardPay.HospitalPay != string.Empty) mb1 += "<A33>" + Prescription.IcCard.IcCardPay.HospitalPay + "</A33>";
            if (Prescription.IcCard.IcCardPay.HospitalCopay1 != string.Empty) mb1 += "<A34>" + Prescription.IcCard.IcCardPay.HospitalCopay1 + "</A34>";
            if (Prescription.IcCard.IcCardPay.HospitalCopay2 != string.Empty) mb1 += "<A35>" + Prescription.IcCard.IcCardPay.HospitalCopay2 + "</A35>";
            //預防保健
            if (Prescription.IcCard.IcCardPrediction.HisServiceMark != string.Empty) mb1 += "<A41>" + Prescription.IcCard.IcCardPrediction.HisServiceMark + "</A41>";
            if (Prescription.IcCard.IcCardPrediction.PredictionDate != string.Empty) mb1 += "<A42>" + Prescription.IcCard.IcCardPrediction.PredictionDate + "</A42>";
            if (Prescription.IcCard.IcCardPrediction.PredictionMedicalCode != string.Empty) mb1 += "<A43>" + Prescription.IcCard.IcCardPrediction.PredictionMedicalCode + "</A43>";
            if (Prescription.IcCard.IcCardPrediction.PredictionCheckCode != string.Empty) mb1 += "<A44>" + Prescription.IcCard.IcCardPrediction.PredictionCheckCode + "</A44>";
            //孕婦
            if (Prescription.IcCard.Pregnant.PregnantCheckDate != string.Empty) mb1 += "<A51>" + Prescription.IcCard.Pregnant.PregnantCheckDate + "</A51>";
            if (Prescription.IcCard.Pregnant.PregnantMedicalCode != string.Empty) mb1 += "<A52>" + Prescription.IcCard.Pregnant.PregnantMedicalCode + "</A52>";
            if (Prescription.IcCard.Pregnant.PregnantCheckCode != string.Empty) mb1 += "<A53>" + Prescription.IcCard.Pregnant.PregnantCheckCode + "</A53>";
            if (Prescription.IcCard.Pregnant.PregnantActualTreatDate != string.Empty) mb1 += "<A54>" + Prescription.IcCard.Pregnant.PregnantActualTreatDate + "</A54>";
            //預防接種
            if (Prescription.IcCard.Vaccination.VaccinationCategory != string.Empty) mb1 += "<A61>" + Prescription.IcCard.Vaccination.VaccinationCategory + "</A61>";
            if (Prescription.IcCard.Vaccination.VaccinationDate != string.Empty) mb1 += "<A62>" + Prescription.IcCard.Vaccination.VaccinationDate + "</A62>";
            if (Prescription.IcCard.Vaccination.VaccinationMedicalCode != string.Empty) mb1 += "<A63>" + Prescription.IcCard.Vaccination.VaccinationMedicalCode + "</A63>";
            if (Prescription.IcCard.Vaccination.VaccinationNum != string.Empty) mb1 += "<A64>" + Prescription.IcCard.Vaccination.VaccinationNum + "</A64>";
            mb1 += "</MB1>";

            //醫療專區
            mb2 = "";
            foreach (DeclareDetail data in DeclareDetails)
            {
                mb2 += "<MB2>";
                mb2 += "<A71>" + Prescription.Treatment.TreatmentDate.ToShortDateString() + "</A71>";
                mb2 += "<A72>" + data.MedicalOrder + "</A72>";
                mb2 += "<A73>" + data.MedicalId + "</A73>";
                mb2 += "<A74>" + data.Position + "</A74>";
                mb2 += "<A75>" + data.Usage + "</A75>";
                mb2 += "<A76>" + data.Dosage + "</A76>";
                mb2 += "<A77>" + data.Total + "</A77>";
                mb2 += "</MB2>";
            }
            xml.LoadXml("<REC>" + msh + "<MB>" + mb1 + mb2 + "</MB></REC>");
            return xml;
        }
        
    }
}
