using GalaSoft.MvvmLight;
using System;
using System.Data;

namespace His_Pos.NewClass.Medicine.ControlMedicineEdit
{
    public class ControlMedicineEdit : ObservableObject
    {
        public ControlMedicineEdit(string medID, string warID)
        {
            MedicineID = medID;
            WarID = warID;
        }

        public ControlMedicineEdit(DataRow r)
        {
            MedicineID = r.Field<string>("MedicineID");
            WarID = r.Field<int>("WareHouseID").ToString();
            Date = r.Field<DateTime>("Date");
            Amount = r.Field<int>("Amount");
            ManufactoryID = r.Field<int>("ManufactoryID");
            Type = r.Field<string>("Type");
            BatchNumber = r.Field<string>("BatchNumber");
        }

        public string MedicineID { get; set; }
        public string WarID { get; set; }
        public int ManufactoryID { get; set; }
        private string type;

        public string Type
        {
            get { return type; }
            set
            {
                Set(() => Type, ref type, value);
            }
        }

        private Manufactory.Manufactory manufactory;

        public Manufactory.Manufactory Manufactory
        {
            get { return manufactory; }
            set
            {
                Set(() => Manufactory, ref manufactory, value);
            }
        }

        private int amount = 0;

        public int Amount
        {
            get { return amount; }
            set
            {
                Set(() => Amount, ref amount, value);
            }
        }

        private DateTime date = DateTime.Today;

        public DateTime Date
        {
            get { return date; }
            set
            {
                Set(() => Date, ref date, value);
            }
        }

        private bool isNew;

        public bool IsNew
        {
            get { return isNew; }
            set
            {
                Set(() => IsNew, ref isNew, value);
            }
        }

        private bool isSelect;

        public bool IsSelect
        {
            get { return isSelect; }
            set
            {
                Set(() => IsSelect, ref isSelect, value);
            }
        }

        private string batchNumber;

        public string BatchNumber
        {
            get { return batchNumber; }
            set
            {
                Set(() => BatchNumber, ref batchNumber, value);
            }
        }
    }
}