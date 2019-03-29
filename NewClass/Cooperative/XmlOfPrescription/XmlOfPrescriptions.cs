using His_Pos.NewClass.Cooperative.CooperativeClinicSetting;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace His_Pos.NewClass.Cooperative.XmlOfPrescription
{
   public class XmlOfPrescriptions : ObservableCollection<XmlOfPrescription>
    {
        public XmlOfPrescriptions() {
        }
        public static void GetFile() {
            CooperativeClinicSettings cooperativeClinicSettings = new CooperativeClinicSettings();
            cooperativeClinicSettings.Init();
            List<XDocument> xmls = new List<XDocument>();
            List<string> cusIdNumbers = new List<string>();
            foreach (var c in cooperativeClinicSettings) {
                string path = c.FilePath;
                string[] fileEntries = Directory.GetFiles(path);
                foreach (string s in fileEntries) { 
                    XDocument xDocument = XDocument.Load(s);
                    string cusIdNumber = xDocument.Element("case").Element("profile").Element("person").Attribute("id").Value;
                    xmls.Add(xDocument);
                    cusIdNumbers.Add(cusIdNumber); 
                }
                XmlOfPrescriptionDb.Insert(cusIdNumbers, xmls, c.TypeName);
            }
        }
         
       
    }
}
