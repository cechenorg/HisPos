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

        }

        public string Id { get; set; }
        public string Name { get; set; }
        public string FullName { get; set; }
        public string Address { get; set; }
        public string Tel { get; set; }
        public int ReaderCom { get; set; }
        public string VpnIp { get; set; }
        public bool NewReader { get; set; }
        public MedicalPersonnel medicalPersonnel { get; set; }
        public Collection<MedicalPersonnel> MedicalPersonnelCollection { get; set; }

        #region Function
        public Pharmacy GetCurrentPharmacy() {
            return null;
        }
        #endregion
    }
}
