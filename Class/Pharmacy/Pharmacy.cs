using System.Collections.ObjectModel;
using His_Pos.AbstractClass;
using His_Pos.Class;
using His_Pos.Class.Person;
using System.Data;
using System.IO;
using System.Xml;
using His_Pos.Class.Declare;

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
        public Pharmacy(string id, string name,string address,string tel)
        {
            Id = id;
            Name = name;
            FullName = id + " " + name;
            Address = address;
            Tel = tel;
            MedicalPersonnel = new MedicalPersonnel();
            MedicalPersonnelCollection = PharmacyDb.GetPharmacyMedicalPersonData();
        }
        public Pharmacy(XmlNode xml) {
            MedicalPersonnel = new MedicalPersonnel();
            MedicalPersonnel.IcNumber = xml.SelectSingleNode("d25") == null ? null : xml.SelectSingleNode("d25").InnerText;
        }

        public Pharmacy(DeclareFileDdata d)
        {
            MedicalPersonnel = new MedicalPersonnel();
            MedicalPersonnel.IcNumber = !string.IsNullOrEmpty(d.Dbody.D25) ? d.Dbody.D25 : string.Empty;
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
    }
}
