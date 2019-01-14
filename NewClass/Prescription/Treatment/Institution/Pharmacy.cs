using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;

namespace His_Pos.NewClass.Prescription.Treatment.Institution {
    public class Pharmacy : ObservableObject
    {
        public Pharmacy() {
        }

        public Pharmacy(DataRow r) {
            Id = r.Field<string>("CurPha_ID");
            Name = r.Field<string>("CurPha_Name");
            Address = r.Field<string>("CurPha_Address");
            Tel = r.Field<string>("CurPha_Telephone");
            ReaderCom = r.Field<byte>("CurPha_ReaderCom");
            VpnIp = r.Field<string>("CurPha_VPN");
            NewReader = r.Field<bool>("CurPha_ReaderIsNew");
        }

        public string Id { get; set; }
        public string Name { get; set; } 
        public string Address { get; set; }
        public string Tel { get; set; }
        public int ReaderCom { get; set; }
        public string VpnIp { get; set; }
        public bool NewReader { get; set; }
        public MedicalPersonnel medicalPersonnel { get; set; }
        public Collection<MedicalPersonnel> MedicalPersonnelCollection { get; set; }

        #region Function
        public static Pharmacy GetCurrentPharmacy() {
            MainWindow.ServerConnection.OpenConnection();

            DataTable tableCurrentPharmacy = PharmacyDb.GetCurrentPharmacy();
            Pharmacy pharmacy = new Pharmacy(tableCurrentPharmacy.Rows[0]);
            pharmacy.medicalPersonnel = new MedicalPersonnel(MainWindow.CurrentUser);
            pharmacy.MedicalPersonnelCollection = new Collection<MedicalPersonnel>();

            MainWindow.ServerConnection.CloseConnection();
         
            return pharmacy;
        }
        #endregion
    }
}
