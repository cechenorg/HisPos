using System.Collections.ObjectModel;
using His_Pos.Class.Person;
using System.Data;
using System.Linq;
using System.Xml;
using His_Pos.Class.Declare;
using His_Pos.Service;

namespace His_Pos.Class.Pharmacy
{
    public class Pharmacy : Selection
    {
        public Pharmacy()
        {
            MedicalPersonnel = new MedicalPersonnel();
        }
        public Pharmacy(DataRow row) {
            MedicalPersonnel = new MedicalPersonnel(row,false);
        }
        public Pharmacy(string id, string name,string address,string tel,string readerCom,string vpnIp,string newReader)
        {
            Id = id;
            Name = name;
            FullName = id + " " + name;
            Address = address;
            Tel = tel;
            ReaderCom = int.Parse(readerCom);
            VpnIp = vpnIp;
            NewReader = !newReader.Equals("0");
            MedicalPersonnel = new MedicalPersonnel();
            MedicalPersonnelCollection = new ObservableCollection<MedicalPersonnel>();
        }
        public Pharmacy(XmlNode xml) {
            MedicalPersonnel = new MedicalPersonnel();
            MedicalPersonnel.IcNumber = xml.SelectSingleNode("d25") == null ? null : xml.SelectSingleNode("d25").InnerText;
        }
        public Pharmacy(XmlDocument xml)
        {
            MedicalPersonnel = new MedicalPersonnel();
            MedicalPersonnel.IcNumber = xml.SelectSingleNode("DeclareXml/DeclareXmlDocument/case/study").Attributes["doctor_id"].Value;
        }
        public Pharmacy(DeclareFileDdata d)
        {
            MedicalPersonnel = new MedicalPersonnel();
          ///  MedicalPersonnel = MainWindow.CurrentPharmacy.MedicalPersonnelCollection.SingleOrDefault(p =>
          ///      p.IdNumber.Equals(d.Dhead.D25));
        }

        private MedicalPersonnel _medicalPersonnel;

        public MedicalPersonnel MedicalPersonnel
        {
            get => _medicalPersonnel;
            set
            {
                _medicalPersonnel = value;
                NotifyPropertyChanged(nameof(MedicalPersonnel));
            }
        }

        private ObservableCollection<MedicalPersonnel> _medicalPersonnelCollection;

        public ObservableCollection<MedicalPersonnel> MedicalPersonnelCollection
        {
            get => _medicalPersonnelCollection;
            set
            {
                _medicalPersonnelCollection = value;
                NotifyPropertyChanged(nameof(MedicalPersonnelCollection));
            }
        } 
        public string Address { get; set; }
        public string Tel { get; set; }

        public int ReaderCom { get; set; }

        public string VpnIp { get; set; }

        public bool NewReader { get; set; }
    }
}
