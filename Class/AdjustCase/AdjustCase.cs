using System;
using System.Data;
using System.Xml;
using His_Pos.Class.Declare;

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
        public AdjustCase(XmlDocument xml)
        {
            Id = xml.SelectSingleNode("DeclareXml/DeclareXmlDocument/case/insurance").Attributes["case_type"].Value;
            
        }
        public AdjustCase(DataRow dataRow)
        {
            Id = dataRow["ADJUSTCASE_ID"].ToString();
            Name = dataRow["ADJUSTCASE_NAME"].ToString();
            FullName = dataRow["ADJUSTCASE_FULLNAME"].ToString();
        }

        public AdjustCase(DeclareFileDdata d)
        {
            Id = !string.IsNullOrEmpty(d.Dhead.D1) ? d.Dhead.D1 : string.Empty;
        }
    }
}