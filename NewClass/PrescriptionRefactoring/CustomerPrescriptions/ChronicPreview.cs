using System;
using System.Data;
using His_Pos.NewClass.Prescription;

namespace His_Pos.NewClass.PrescriptionRefactoring.CustomerPrescriptions
{
    public class ChronicPreview : CusPrePreviewBase
    {
        private int ID { get; set; }
        public PrescriptionType Type { get; }
        public int ChronicSeq { get; }
        public int ChronicTotal { get; }
        public ChronicPreview(DataRow r, PrescriptionType type) : base(r)
        {
            Type = type;
            ID = r.Field<int>("ID");
            AdjustDate = r.Field<DateTime>("Adj_Date");
            ChronicSeq = r.Field<byte>("ChronicSequence");
            ChronicTotal = r.Field<byte>("ChronicTotal");
        }

        public override void Print()
        {
            throw new NotImplementedException();
        }

        public override Prescription CreatePrescription()
        {
            DataTable table;
            switch (Type)
            {
                case PrescriptionType.ChronicRegister:
                    table = PrescriptionDb.GetPrescriptionByID(ID);
                    return table.Rows.Count > 0 ? new Prescription(table.Rows[0],Type) : null;
                case PrescriptionType.ChronicReserve:
                    table = PrescriptionDb.GetReservePrescriptionByID(ID);
                    return table.Rows.Count > 0 ? new Prescription(table.Rows[0],Type) : null;
                default:
                    return null;
            }
        }

        public override void GetMedicines()
        {
            Medicines.Clear();
            switch (Type)
            {
                case PrescriptionType.ChronicReserve:
                    Medicines.GetDataByReserveId(ID);
                    break;
                default:
                    Medicines.GetDataByPrescriptionId(ID);
                    break;
            }
        }
    }
}
