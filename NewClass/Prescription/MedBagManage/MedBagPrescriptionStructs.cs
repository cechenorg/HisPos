using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.NewClass.Prescription.MedBagManage
{
    public class MedBagPrescriptionStructs : Collection<MedBagPrescriptionStruct>
    {
        #region ----- Define Variables -----
        public double TotalStockValue => this.Sum(p => p.StockValue);
        #endregion

        private MedBagPrescriptionStructs(DataTable dataTable)
        {
            foreach (DataRow row in dataTable.Rows)
            {
                Add(new MedBagPrescriptionStruct(row));
            }
        }

        #region ----- Define Functions -----
        public static MedBagPrescriptionStructs GetReserveMedBagPrescriptions()
        {
            return GetMedBagPrescriptionStructs("Res");
        }
        public static MedBagPrescriptionStructs GetRegisterMedBagPrescriptions()
        {
            return GetMedBagPrescriptionStructs("Reg");
        }
        private static MedBagPrescriptionStructs GetMedBagPrescriptionStructs(string type)
        {
            return new MedBagPrescriptionStructs(PrescriptionDb.GetMedBagPrescriptionStructsByType(type));
        }
        #endregion
    }
}
