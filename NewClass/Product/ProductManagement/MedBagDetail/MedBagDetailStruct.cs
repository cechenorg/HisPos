using System;
using System.Data;

namespace His_Pos.NewClass.Product.ProductManagement.MedBagDetail
{
    public struct MedBagDetailStruct
    {
        #region ----- Define Variables -----

        public string Type { get; set; }
        public string Name { get; set; }
        public DateTime? AdjustDate { get; set; }
        public double SelfAmount { get; set; }
        public double SendAmount { get; set; }
        public string SelfAmountHeader { get; set; }
        public string SendAmountHeader { get; set; }

        #endregion ----- Define Variables -----

        public MedBagDetailStruct(DataRow row)
        {
            Type = row.Field<string>("TYPE");
            Name = row.Field<string>("Cus_Name");
            AdjustDate = row.Field<DateTime?>("ResMas_AdjustDate");
            SelfAmount = row.Field<double>("MEDBAG_AMOUNT");
            SendAmount = row.Field<double>("SEND_AMOUNT");

            SelfAmountHeader = "自備:";
            SendAmountHeader = "傳送:";
        }
    }
}