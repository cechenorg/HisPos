using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Linq;

namespace His_Pos.Class.Manufactory
{
    public class ManageManufactory : Manufactory, ICloneable, INotifyPropertyChanged
    {
        private ManageManufactory() { }

        public ManageManufactory(DataRow row) : base(row)
        {
            NickName = row["MAN_NICKNAME"].ToString();
            UniformNumber = row["MAN_EIN"].ToString();
            Email = row["MAN_EMAIL"].ToString();
            ControlMedicineID = row["MAN_CONTROLMEDID"].ToString();
            MedicalID = row["MAN_MEDICALID"].ToString();
            Note = row["MAN_NOTE"].ToString();
            ManufactoryPrincipals = new ObservableCollection<ManufactoryPrincipal>();
        }

        public string NickName { get; set; }
        public string UniformNumber { get; set; }
        public string Email { get; set; }
        public string ControlMedicineID { get; set; }
        public string MedicalID { get; set; }
        public string Note { get; set; }
        private ObservableCollection<ManufactoryPrincipal> manufactoryPrincipals;
        public ObservableCollection<ManufactoryPrincipal> ManufactoryPrincipals
        {
            get { return manufactoryPrincipals; }
            set
            {
                manufactoryPrincipals = value;
                NotifyPropertyChanged("ManufactoryPrincipals");
            }
        }

        public int ManufactoryPrincipalsCount { get { return ManufactoryPrincipals.Count(m => m.IsEnable); } }

        public object Clone()
        {
            ManageManufactory newManageManufactory = new ManageManufactory
            {
                Id = Id,
                Name = Name,
                Address = Address,
                Telphone = Telphone,
                Fax = Fax,
                NickName = NickName,
                UniformNumber = UniformNumber,
                Email = Email,
                ControlMedicineID = ControlMedicineID,
                MedicalID = MedicalID,
                Note = Note
            };

            newManageManufactory.ManufactoryPrincipals = new ObservableCollection<ManufactoryPrincipal>();

            foreach (ManufactoryPrincipal manufactoryPrincipal in ManufactoryPrincipals)
            {
                newManageManufactory.ManufactoryPrincipals.Add(manufactoryPrincipal.Clone() as ManufactoryPrincipal);
            }

            return newManageManufactory;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }
    }
}
