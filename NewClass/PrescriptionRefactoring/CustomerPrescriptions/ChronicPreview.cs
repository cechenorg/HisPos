using System;
using System.Data;
using His_Pos.NewClass.Prescription;

namespace His_Pos.NewClass.PrescriptionRefactoring.CustomerPrescriptions
{
    public enum ChronicType
    {
        Register = 0,
        Reserve = 1
    }
    public class ChronicPreview : CusPrePreviewBase
    {
        private int ID { get; set; }
        public ChronicType Type { get; }
        public DateTime AdjustDate { get; }
        public int ChronicSeq { get; }
        public int ChronicTotal { get; }
        public ChronicPreview(DataRow r, ChronicType type) : base(r)
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
                case ChronicType.Register:
                    table = PrescriptionDb.GetPrescriptionByID(ID);
                    return table.Rows.Count > 0 ? new Prescription(table.Rows[0],Type) : null;
                case ChronicType.Reserve:
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
                case ChronicType.Register:
                    Medicines.GetDataByPrescriptionId(ID);
                    break;
                case ChronicType.Reserve:
                    Medicines.GetDataByReserveId(ID);
                    break;
            }
        }
    }
}
