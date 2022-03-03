using System;
using System.Data;

namespace His_Pos.NewClass.Prescription.MedBagManage
{
    public struct MedBagPrescriptionStruct
    {
        #region ----- Define Variables -----

        public int ID { get; set; }
        public string Status { get; set; }
        public string CustomerName { get; set; }
        public string Institution { get; set; }
        public string Division { get; set; }
        public DateTime AdjustDate { get; set; }
        public double StockValue { get; set; }

        #endregion ----- Define Variables -----

        public MedBagPrescriptionStruct(DataRow row)
        {
            ID = row.Field<int>("ID");
            Status = row.Field<string>("PREPARE_STATUS");
            CustomerName = row.Field<string>("Cus_Name");
            Institution = row.Field<string>("Ins_Name");
            Division = row.Field<string>("Div_Name");
            AdjustDate = row.Field<DateTime>("ADJUST_DATE");
            StockValue = row.Field<double>("STOCK_VALUE");
        }
    }
}