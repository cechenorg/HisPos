using System.Data;
using System.Xml;

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
    }
}
