using His_Pos.Class;
using His_Pos.FunctionWindow;
using His_Pos.NewClass.StoreOrder;
using System;
using System.Data;

namespace His_Pos.NewClass.Prescription.CustomerPrescriptions
{
    public class ChronicPreview : CusPrePreviewBase
    {
        public int ID { get; }
        public PrescriptionType Type { get; }
        public int ChronicSeq { get; }
        public int ChronicTotal { get; }

        private string _isSend;
        public string IsSend { 
            get => _isSend;
            set
            {
                Set(IsSend, ref _isSend, value);
            }
        }
        public string OrderID { get; }

        public ChronicPreview(DataRow r, PrescriptionType type) : base(r)
        {
            Type = type;
            ID = r.Field<int>("ID");
            AdjustDate = r.Field<DateTime>("Adj_Date");
            ChronicSeq = r.Field<byte>("ChronicSequence");
            ChronicTotal = r.Field<byte>("ChronicTotal");
            if (!string.IsNullOrEmpty(r.Field<string>("StoOrdID")))
                OrderID = r.Field<string>("StoOrdID");
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

        public void SwichPrepareMed()
        {
            if (IsSend == "未處理")
                IsSend = "不備藥";
            else if (IsSend == "不備藥")
                IsSend = "未處理";
        }
       
        public override void Print(bool manualPrint = false)
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
                    return table.Rows.Count > 0 ? new Prescription(table.Rows[0], Type) : null;

                case PrescriptionType.ChronicReserve:
                    table = PrescriptionDb.GetReservePrescriptionByID(ID);
                    return table.Rows.Count > 0 ? new Prescription(table.Rows[0], Type) : null;

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

        public void DeleteOrder()
        {
            if (!string.IsNullOrEmpty(OrderID))
            {
                var removeSingdeOrder = StoreOrderDB.RemoveSingdeStoreOrderByID(OrderID).Rows[0].Field<string>("RESULT").Equals("SUCCESS");
                if (!removeSingdeOrder)
                    MessageWindow.ShowMessage("處方訂單已出貨或網路異常，訂單刪除失敗", MessageType.ERROR);
                else
                {
                    var dataTable = StoreOrderDB.RemoveStoreOrderToSingdeByID(OrderID);
                    var removeLocalOrder = dataTable.Rows[0].Field<string>("RESULT").Equals("SUCCESS");
                    if (!removeLocalOrder)
                        MessageWindow.ShowMessage("處方訂單刪除失敗，請至進退貨管理確認。", MessageType.ERROR);
                }
            }
        }

        public override void PrintDir()
        {
            throw new NotImplementedException();
        }
    }
}