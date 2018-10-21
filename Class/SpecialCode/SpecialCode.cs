using System.Data;
using System.Xml;
using His_Pos.Class.Declare;

namespace His_Pos.Class.SpecialCode
{
    public class SpecialCode : Selection
    {
        public SpecialCode()
        {
        }

        public SpecialCode(DataRow specialCode)
        {
            Id = specialCode["HISSPE_ID"].ToString();
            Name = specialCode["HISSPE_NAME"].ToString();
            FullName = specialCode["HISSPE_FULLNAME"].ToString();
        }
        public SpecialCode(XmlNode xml) {
            Id = xml.SelectSingleNode("d26") == null ? null : xml.SelectSingleNode("d26").InnerText;
        }

        public SpecialCode(DeclareFileDdata d)
        {
            Id = !string.IsNullOrEmpty(d.Dbody.D26) ? d.Dbody.D26 : string.Empty;
        }
    }
}
