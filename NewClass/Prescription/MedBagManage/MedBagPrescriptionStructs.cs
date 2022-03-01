using System.Collections.ObjectModel;
using System.Data;
using System.Linq;

namespace His_Pos.NewClass.Prescription.MedBagManage
{
    public class MedBagPrescriptionStructs : Collection<MedBagPrescriptionStruct>
    {
        #region ----- Define Variables -----

        public string Type { get; set; }
        public double TotalStockValue => this.Sum(p => p.StockValue);

        #endregion ----- Define Variables -----

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

        internal static MedBagPrescriptionStructs GetPastReserveMedBagPrescriptions()
        {
            return GetMedBagPrescriptionStructs("PastRes");
        }

        private static MedBagPrescriptionStructs GetMedBagPrescriptionStructs(string type)
        {
            var temp = new MedBagPrescriptionStructs(PrescriptionDb.GetMedBagPrescriptionStructsByType(type));

            temp.Type = type;

            return temp;
        }

        #endregion ----- Define Functions -----
    }
}