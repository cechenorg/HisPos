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
        public Division(DataRow dataRow)
        {
            Id = dataRow["HISDIV_ID"].ToString();
            Name = dataRow["HISDIV_NAME"].ToString();
            FullName = dataRow["HISDIV_FULLNAME"].ToString();
        }

        public Division(DeclareFileDdata d)
        {
            Id = !string.IsNullOrEmpty(d.Dhead.D13) ? d.Dhead.D13 : string.Empty;
        }
    }
}
