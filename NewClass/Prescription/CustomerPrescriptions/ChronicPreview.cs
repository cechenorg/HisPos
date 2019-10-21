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
        public string OrderID { get; }
        public ChronicPreview(DataRow r, PrescriptionType type) : base(r)
        {
            Type = type;
            ID = r.Field<int>("ID");
            AdjustDate = r.Field<DateTime>("Adj_Date");
            ChronicSeq = r.Field<byte>("ChronicSequence");
            ChronicTotal = r.Field<byte>("ChronicTotal");
            if(!string.IsNullOrEmpty(r.Field<string>("StoOrdID")))
                OrderID = "單號:" + r.Field<string>("StoOrdID");
            switch (type)
            {
                case PrescriptionType.ChronicReserve:
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
                    break;
                default:
                    switch (r.Field<string>("StoOrd_Status"))
                    {
                        case "W":
                            IsSend = "等待確認";
                            break;
                        case "P":
                            IsSend = "等待收貨";
                            break;
                        case "D":
                            IsSend = "已收貨";
                            break;
                        case "S":
                            IsSend = "訂單作廢";
                            break;
                        default:
                            IsSend = "無訂單";
                            break;
                    }
                    break;
            }
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
