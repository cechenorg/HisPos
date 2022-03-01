using System.Data;

namespace His_Pos.NewClass.Manufactory
{
    public class ControlManufactory : Manufactory
    {
        public ControlManufactory(DataRow r) : base(r)
        {
            ControlmedicineID = r.Field<string>("Man_ControlMedicineID");
        }

        private string controlmedicineID;

        public string ControlmedicineID
        {
            get { return controlmedicineID; }
            set { Set(() => ControlmedicineID, ref controlmedicineID, value); }
        }
    }
}