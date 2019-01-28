using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.NewClass.Product.PurchaseReturn
{
    public class PurchaseProduct : Product
    {
        #region ----- Define Variables -----
        public double Inventory { get; }
        public int SafeAmount { get; }
        public int BasicAmount { get; }
        public double OnTheWayAmount { get; }
        public double LastPrice { get; }
        public double OrderAmount { get; set; }
        public double RealAmount { get; set; }
        public double FreeAmount { get; set; }
        public double Price { get; set; }
        public double SubTotal { get; set; }
        public string Invoice { get; set; }
        public DateTime? ValidDate { get; set; }
        public string BatchNumber { get; set; }
        public string Note { get; set; }

        public bool IsFirstBatch { get; set; } = true;
        public int SingdePackageAmount { get; } 
        public double SingdePackagePrice { get; }
        public double SingdePrice { get; }
        #endregion

        public PurchaseProduct() : base() {}

        public PurchaseProduct(DataRow dataRow) : base(dataRow)
        {
            Inventory = dataRow.Field<double>("Inv_Inventory");
            SafeAmount = dataRow.Field<int>("Inv_SafeAmount");
            BasicAmount = dataRow.Field<int>("Inv_BasicAmount");
            OnTheWayAmount = dataRow.Field<double>("Inv_OnTheWay");
            LastPrice = (double)dataRow.Field<decimal>("Pro_LastPrice");
            OrderAmount = (double)dataRow.Field<decimal> ("StoOrdDet_OrderAmount");
            RealAmount = (double)dataRow.Field<decimal>("StoOrdDet_RealAmount");
            FreeAmount = (double)dataRow.Field<decimal>("StoOrdDet_FreeAmount");
            Price = (double)dataRow.Field<decimal>("StoOrdDet_Price");
            SubTotal = (double)dataRow.Field<decimal>("StoOrdDet_SubTotal");
            Invoice = dataRow.Field<string>("StoOrdDet_Invoice");
            ValidDate = dataRow.Field<DateTime?>("StoOrdDet_ValidDate");
            BatchNumber = dataRow.Field<string>("StoOrdDet_BatchNumber");
            Note = dataRow.Field<string>("StoOrdDet_Note");

            SingdePackageAmount = dataRow.Field<int>("SinData_PackageAmount");
            SingdePackagePrice = (double)dataRow.Field<decimal>("SinData_PackagePrice");
            SingdePrice = (double)dataRow.Field<decimal>("SinData_SinglePrice");
        }
    }
}
