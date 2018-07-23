using His_Pos.AbstractClass;
using His_Pos.Class.Person;
using System.Data;
using System.Xml;

namespace His_Pos.Class
{
    public class Pharmacy : Selection
    {
        public Pharmacy()
        {
            MedicalPersonnel = new MedicalPersonnel();
        }
        public Pharmacy(DataRow row) {
            MedicalPersonnel = new MedicalPersonnel(row);
        }
        public Pharmacy(string id, string name)
        {
            Id = id;
            Name = name;
            FullName = id + " " + name;
            MedicalPersonnel = new MedicalPersonnel();
        }
        public Pharmacy(XmlNode xml) {
            MedicalPersonnel = new MedicalPersonnel();
            MedicalPersonnel.IcNumber = xml.SelectSingleNode("d25") == null ? null : xml.SelectSingleNode("d25").InnerText;
        }
        public MedicalPersonnel MedicalPersonnel { get; set; }
    }
}
