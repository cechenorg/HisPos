using System.Data;
using System.Xml;

namespace His_Pos.Class.AdjustCase
{
    public class AdjustCase : Selection
    {
        public AdjustCase()
        {
        }
        public AdjustCase(XmlNode xml) {
           Id = xml.SelectSingleNode("d1") == null ? null : xml.SelectSingleNode("d1").InnerText;
        }
        public AdjustCase(DataRow dataRow)
        {
            Id = dataRow["ADJUSTCASE_ID"].ToString();
            Name = dataRow["ADJUSTCASE_NAME"].ToString();
            FullName = dataRow["ADJUSTCASE_FULLNAME"].ToString();
        }
    }
}