using System.Data;
using System.Xml;

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
    }
}
