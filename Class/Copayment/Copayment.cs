using System.Data;
using System.Xml;
using His_Pos.Class.Declare;

namespace His_Pos.Class.Copayment
{
    public class Copayment : Selection
    {
        public Copayment()
        {
        }
        public Copayment(XmlNode xml) {
            Id = xml.SelectSingleNode("d15") == null ? null : xml.SelectSingleNode("d15").InnerText;
        }
        public Copayment(DataRow dataRow)
        {
            Id = dataRow["HISCOP_ID"].ToString();
            Name = dataRow["HISCOP_NAME"].ToString();
            FullName = dataRow["HISCOP_FULLNAME"].ToString();
        }

        public Copayment(DeclareFileDdata d)
        {
            Id = !string.IsNullOrEmpty(d.Dbody.D15) ? d.Dbody.D15 : string.Empty;
        }

        public int Point { get; set; }

        public Copayment ShallowCopy()
        {
            return (Copayment)MemberwiseClone();
        }
    }
}