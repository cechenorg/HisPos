using His_Pos.Class.Declare;
using His_Pos.Class.Person;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace His_Pos.Class
{
    public class CooperativeClinic
    {
        public CooperativeClinic() { }
        public CooperativeClinic(XmlDocument xmlDocument) { 
            DeclareId = xmlDocument.SelectSingleNode("DeclareXml/DeclareId").InnerText;
            Remark = xmlDocument.SelectSingleNode("DeclareXml/DeclareXmlDocument/case/profile/person/remark").InnerText.Trim();
            Prescription = new Prescription(xmlDocument);
            Xml = xmlDocument;
        }
        public string DeclareId { get; set; } 
        public string Remark { get; set; }
        public Prescription Prescription { get; set; }
        public XmlDocument Xml { get; set; }
    }
}
