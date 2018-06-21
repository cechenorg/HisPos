using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.Class.Manufactory
{
    public class ManageManufactory : Manufactory
    {
        public ManageManufactory(DataRow row) : base(row, DataSource.MANUFACTORY)
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
        public ObservableCollection<ManufactoryPrincipal> ManufactoryPrincipals { get; set; }
    }
}
