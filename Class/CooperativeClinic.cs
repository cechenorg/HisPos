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
        public CooperativeClinic(XmlDocument xmlDocument,string type) { 
            DeclareId = xmlDocument.SelectSingleNode("DeclareXml/DeclareId").InnerText;
            Remark = xmlDocument.SelectSingleNode("DeclareXml/DeclareXmlDocument/case/profile/person/remark").InnerText.Trim(); 
            Prescription = new Prescription(xmlDocument);
            Xml = xmlDocument;
            if (type == "ByDate") {
                InsertDate = Convert.ToDateTime(xmlDocument.SelectSingleNode("DeclareXml/InsertDate").InnerText).ToString("yyyy-MM-dd");
                DeclareStatus = xmlDocument.SelectSingleNode("DeclareXml/Status").InnerText == "N" ? "未調劑" : "已調劑";
            }
        }
        public string DeclareId { get; set; } 
        public string Remark { get; set; }
        public string InsertDate { get; set; }
        public string DeclareStatus { get; set; }
        public Prescription Prescription { get; set; }
        public XmlDocument Xml { get; set; }
    }
}
