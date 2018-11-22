using System.Data;
using System.Xml;
using His_Pos.Class.Declare;

namespace His_Pos.Class.Division
{
    public class Division : Selection
    {
        public Division()
        {
        }
        public Division(XmlNode xml) {
            Id = xml.SelectSingleNode("d13") == null ? null : xml.SelectSingleNode("d13").InnerText;
        }
        public Division(XmlDocument xml)
        {
            Id = xml.SelectSingleNode("DeclareXml/DeclareXmlDocument/case/insurance").Attributes["release_type"].Value;
        }
        public Division(DataRow dataRow)
        {
            Id = dataRow["HISDIV_ID"].ToString();
            Name = dataRow["HISDIV_NAME"].ToString();
            FullName = dataRow["HISDIV_FULLNAME"].ToString();
        }

        public Division(DeclareFileDdata d)
        {
            Id = !string.IsNullOrEmpty(d.Dbody.D13) ? d.Dbody.D13 : string.Empty;
        }
    }
}
