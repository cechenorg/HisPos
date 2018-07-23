using System.Xml;

namespace His_Pos.Class
{
    public class SpecialCode : Selection
    {
        public SpecialCode() { }
        public SpecialCode(XmlNode xml) {
            Id = xml.SelectSingleNode("d21") == null ? null : xml.SelectSingleNode("d21").InnerText;
        }
    }
}
