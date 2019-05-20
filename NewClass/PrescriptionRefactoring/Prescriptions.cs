using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using His_Pos.NewClass.Cooperative.CooperativeClinicSetting;
using His_Pos.NewClass.Cooperative.XmlOfPrescription;
using His_Pos.Service;
using VM = His_Pos.ChromeTabViewModel.ViewModelMainWindow;

namespace His_Pos.NewClass.PrescriptionRefactoring
{
    public class Prescriptions : ObservableCollection<Prescription>
    {
        public Prescriptions() { }


        public void GetOrthopedics(DateTime addDays, DateTime today)
        {
            //VM.CurrentPharmacy.ID
        }

        public void GetCooperative(DateTime addDays, DateTime today)
        {
            GetOrthopedics(addDays, today);

        }

        public static void GetXmlFiles()
        {
            CooperativeClinicSettings cooperativeClinicSettings = new CooperativeClinicSettings();
            cooperativeClinicSettings.Init();
            List<XDocument> xmls = new List<XDocument>();
            List<string> cusIdNumbers = new List<string>();
            List<string> filepaths = new List<string>();
            foreach (var c in cooperativeClinicSettings)
            {
                var path = c.FilePath;
                if (!string.IsNullOrEmpty(path))
                {
                    try
                    {
                        var fileEntries = Directory.GetFiles(path);
                        foreach (var s in fileEntries)
                        {
                            try
                            {
                                XDocument xDocument = XDocument.Load(s);
                                string cusIdNumber = xDocument.Element("case").Element("profile").Element("person").Attribute("id").Value;
                                xmls.Add(xDocument);
                                cusIdNumbers.Add(cusIdNumber);
                                filepaths.Add(s);
                            }
                            catch (Exception ex)
                            {
                                NewFunction.ExceptionLog(ex.Message);
                            }
                        }
                        XmlOfPrescriptionDb.Insert(cusIdNumbers, filepaths, xmls, c.TypeName);
                    }
                    catch (Exception ex)
                    {
                    }
                }
            }
        }
    }
}
