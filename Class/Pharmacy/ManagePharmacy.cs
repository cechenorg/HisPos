using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.Class.Pharmacy
{
    public class ManagePharmacy : Manufactory.Manufactory, ICloneable, INotifyPropertyChanged
    {
        private ManagePharmacy() { }

        public ManagePharmacy(DataRow row) : base(row)
        {
            NickName = row["MAN_NICKNAME"].ToString();
            UniformNumber = row["MAN_EIN"].ToString();
            Email = row["MAN_EMAIL"].ToString();
            MedicalID = row["MAN_MEDICALID"].ToString();
            Note = row["MAN_NOTE"].ToString();
            PharmacyPrincipals = new ObservableCollection<PharmacyPrincipal>();
        }

        public string NickName { get; set; }
        public string UniformNumber { get; set; }
        public string Email { get; set; }
        public string MedicalID { get; set; }
        public string Note { get; set; }
        private ObservableCollection<PharmacyPrincipal> pharmacyPrincipals;
        public ObservableCollection<PharmacyPrincipal> PharmacyPrincipals
        {
            get { return pharmacyPrincipals; }
            set
            {
                pharmacyPrincipals = value;
                NotifyPropertyChanged("PharmacyPrincipals");
            }
        }

        public object Clone()
        {
            ManagePharmacy newManagePharmacy = new ManagePharmacy
            {
                Id = Id,
                Name = Name,
                Address = Address,
                Telphone = Telphone,
                Fax = Fax,
                NickName = NickName,
                UniformNumber = UniformNumber,
                Email = Email,
                MedicalID = MedicalID,
                Note = Note
            };

            newManagePharmacy.PharmacyPrincipals = new ObservableCollection<PharmacyPrincipal>();

            foreach (PharmacyPrincipal pharmacyPrincipal in PharmacyPrincipals)
            {
                newManagePharmacy.PharmacyPrincipals.Add(pharmacyPrincipal.Clone() as PharmacyPrincipal);
            }

            return newManagePharmacy;
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
