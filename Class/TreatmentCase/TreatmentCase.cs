using System.Data;
using System.Xml;
using His_Pos.Class.Declare;

namespace His_Pos.Class.TreatmentCase
{
    public class TreatmentCase : Selection
    {
        public TreatmentCase(DataRow dataRow)
        {
            Id = dataRow["HISMEDCAS_ID"].ToString();
            Name = dataRow["HISMEDCAS_NAME"].ToString();
            FullName = dataRow["HISMEDCAS_FULLNAME"].ToString();
        }

        public TreatmentCase()
        {
            
        }
        public TreatmentCase(XmlNode xml)
        {
            Id = xml.SelectSingleNode("d22") == null ? null : xml.SelectSingleNode("d22").InnerText;
        }
        public TreatmentCase(XmlDocument xml)
        {
            Id = xml.SelectSingleNode("DeclareXml/DeclareXmlDocument/case/insurance").Attributes["case_type"].Value;
        }
        public TreatmentCase(DeclareFileDdata d)
        {
            Id = !string.IsNullOrEmpty(d.Dhead.D22) ? d.Dhead.D22 : string.Empty;
        }
    }
}
