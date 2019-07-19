using System;
using System.Data;

namespace His_Pos.NewClass.Prescription.CustomerPrescriptions
{
    public class ChronicPreview : CusPrePreviewBase
    {
        private int ID { get; set; }
        public PrescriptionType Type { get; }
        public int ChronicSeq { get; }
        public int ChronicTotal { get; }
        public string IsSend { get; }
        public ChronicPreview(DataRow r, PrescriptionType type) : base(r)
        {
            Type = type;
            ID = r.Field<int>("ID");
            AdjustDate = r.Field<DateTime>("Adj_Date");
            ChronicSeq = r.Field<byte>("ChronicSequence");
            ChronicTotal = r.Field<byte>("ChronicTotal");
            if(type.Equals(PrescriptionType.ChronicReserve))
            {
                switch (r.Field<string>("MedPrepareStatus"))
                {
                    case "N":
                        IsSend = "未處理";
                        break;
                    case "D":
                        IsSend = "已備藥";
                        break;
                    default:
                        IsSend = "不備藥";
                        break;
                }
            }
        }

        public override void Print()
        {
            throw new NotImplementedException();
        }

        public override NewClass.Prescription.Prescription CreatePrescription()
        {
            DataTable table;
            switch (Type)
            {
                case PrescriptionType.ChronicRegister:
                    table = PrescriptionDb.GetPrescriptionByID(ID);
                    return table.Rows.Count > 0 ? new NewClass.Prescription.Prescription(table.Rows[0],Type) : null;
                case PrescriptionType.ChronicReserve:
                    var reserveSend = IsSend.Equals("已備藥");
                    table = PrescriptionDb.GetReservePrescriptionByID(ID);
                    return table.Rows.Count > 0 ? new NewClass.Prescription.Prescription(table.Rows[0],Type, reserveSend) : null;
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
