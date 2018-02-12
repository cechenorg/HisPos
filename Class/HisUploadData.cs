using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace His_Pos.Class
{
    class HisUploadData
    {
        //    public List<DeclareDetail> DeclareDetails { get; set; }
        //    public Prescription Prescription { get; set; }
        //    public string SafeCode { get; set; } //安全模組代碼
        //    public string DeclareMakeUp { get; set; } //補報註記
        //    public string RsaNum { get; set; } //安全簽章
        //    private XmlDocument CreateToXml()
        //    {
        //        string msh, mb1, mb2;
        //        XmlDocument xml = new XmlDocument();

        //        //msh資料段
        //        msh = "<MSH>";
        //        msh += "<A00>" + "1" + "</A00>";
        //        msh += "<A01>" + A01 + "</A01>";
        //        msh += "<A02>" + A02 + "</A02>";
        //        msh += "</MSH>";

        //        //mb1 健保資料段

        //        mb1 = "<MB1>";
        //        mb1 += "<A11>" + Prescription.IcCard.ICNumber + "</A11>";
        //        mb1 += "<A12>" + Prescription.IcCard.Customer.IcNumber + "</A12>";
        //        mb1 += "<A13>" + Prescription.IcCard.Customer.Birthday + "</A13>";
        //        mb1 += "<A14>" + "30" + "</A14>";
        //        mb1 += "<A15>" + Prescription.Treatment.MedicalPersonId + "</A15>";
        //        mb1 += "<A16>" + SafeCode + "</A16>";
        //        mb1 += "<A17>" + Prescription.Treatment.TreatmentDate + "</A17>";
        //        mb1 += "<A18>" + Prescription.OriginalMedicalNumber + "</A18>";
        //        if (DeclareMakeUp != string.Empty) mb1 += "<A19>" + DeclareMakeUp + "</A19>";
        //        if (Prescription.IcCard.IcMarks.NewbornsData.Birthday != string.Empty) mb1 += "<A20>" + Prescription.IcCard.IcMarks.NewbornsData.Birthday + "</A20>";
        //        if (Prescription.IcCard.IcMarks.NewbornsData.NewbornMark != string.Empty) mb1 += "<A21>" + Prescription.IcCard.IcMarks.NewbornsData.NewbornMark + "</A21>";
        //        mb1 += "<A22>" + RsaNum + "</A22>";
        //        mb1 += "<A23>" + Prescription.Treatment.MedicalInfo.Hospital.Division + "</A23>";
        //        if (Prescription.IcCard.IcMarks.NewbornsData.TreatMark != string.Empty) mb1 += "<A24>" + Prescription.IcCard.IcMarks.NewbornsData.TreatMark + "</A24>";
        //        int diseasecodecount = 25;
        //        foreach (var diseasecode in Prescription.Treatment.MedicalInfo.DiseaseCodes)
        //        {
        //            mb1 += "<A" + diseasecodecount + ">" + diseasecode.Id + "</A" + diseasecodecount + ">";
        //            diseasecodecount++;
        //        }


        //        if (A31 != string.Empty) mb1 += "<A31>" + A31 + "</A31>";
        //        if (A32 != string.Empty) mb1 += "<A32>" + A32 + "</A32>";
        //        if (A33 != string.Empty) mb1 += "<A33>" + A33 + "</A33>";
        //        if (A34 != string.Empty) mb1 += "<A34>" + A34 + "</A34>";
        //        if (A35 != string.Empty) mb1 += "<A35>" + A35 + "</A35>";
        //        //預防保健
        //        if (A41 != string.Empty) mb1 += "<A41>" + A41 + "</A41>";
        //        if (A42 != string.Empty) mb1 += "<A42>" + A42 + "</A42>";
        //        if (A43 != string.Empty) mb1 += "<A43>" + A43 + "</A43>";
        //        if (A44 != string.Empty) mb1 += "<A44>" + A44 + "</A44>";
        //        //孕婦
        //        if (A51 != string.Empty) mb1 += "<A51>" + A51 + "</A51>";
        //        if (A52 != string.Empty) mb1 += "<A52>" + A52 + "</A52>";
        //        if (A53 != string.Empty) mb1 += "<A53>" + A53 + "</A53>";
        //        if (A54 != string.Empty) mb1 += "<A54>" + A54 + "</A54>";
        //        //預防接種
        //        if (A61 != string.Empty) mb1 += "<A61>" + A61 + "</A61>";
        //        if (A62 != string.Empty) mb1 += "<A62>" + A62 + "</A62>";
        //        if (A63 != string.Empty) mb1 += "<A63>" + A63 + "</A63>";
        //        if (A64 != string.Empty) mb1 += "<A64>" + A64 + "</A64>";
        //        mb1 += "</MB1>";

        //        //醫療專區
        //        mb2 = "";
        //        foreach (DeclareDetail data in DeclareDetails)
        //        {
        //            mb2 += "<MB2>";
        //            mb2 += "<A71>" + Prescription.Treatment.TreatmentDate + "</A71>";
        //            mb2 += "<A72>" + data.MedicalOrder + "</A72>";
        //            mb2 += "<A73>" + data.MedicalId + "</A73>";
        //            mb2 += "<A74>" + data.Position + "</A74>";
        //            mb2 += "<A75>" + data.Usage + "</A75>";
        //            mb2 += "<A76>" + data.Dosage + "</A76>";
        //            mb2 += "<A77>" + data.Total + "</A77>";
        //            mb2 += "</MB2>";
        //        }
        //        xml.LoadXml("<REC>" + msh + "<MB>" + mb1 + mb2 + "</MB></REC>");
        //        return xml;
        //    }
    }
}
