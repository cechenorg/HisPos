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
            Prescription = new Prescription(xmlDocument); 
        }
        public string DeclareId { get; set; } 
        public Prescription Prescription { get; set; }
    }
}
