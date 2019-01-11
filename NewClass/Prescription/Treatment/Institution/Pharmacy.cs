using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.NewClass.Prescription.Treatment.Institution {
    public class Pharmacy {
        public Pharmacy() {
        }

        public Pharmacy(DataRow r) {
            Id = r["CurPha_ID"].ToString();
            Name = r["CurPha_Name"].ToString(); 
            Address = r["CurPha_Address"].ToString();
            Tel = r["CurPha_Telephone"].ToString();
            ReaderCom = Convert.ToInt32(r["CurPha_ReaderCom"].ToString());
            VpnIp = r["CurPha_VPN"].ToString();
            NewReader = Convert.ToBoolean(r["CurPha_ReaderIsNew"].ToString()); 
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
