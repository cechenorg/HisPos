using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.NewClass.Product.ProductManagement.ProductStockDetail
{
    public class MedicineStockDetail : ProductStockDetail
    {
        #region ----- Define Variables -----
        private string stockDetail = "";

        public double MedBagOnTheWayAmount { get; set; }
        public double TotalOnTheWayAmount
        {
            get { return OnTheWayAmount + MedBagOnTheWayAmount; }
        }
        public double ShelfInventory { get; set; }
        public double MedBagInventory { get; set; }
        public double ConsumeIn90Days { get; set; }
        public string StockDetail
        {
            get { return stockDetail; }
            set { Set(() => StockDetail, ref stockDetail, value); }
        }
        #endregion

        public MedicineStockDetail(DataRow row) : base(row)
        {
            ShelfInventory = row.Field<double>("SHELF_INV");
            MedBagInventory = row.Field<double>("MEDBAG_INV");
            MedBagOnTheWayAmount = row.Field<double>("Inv_MedBagOnTheWay");
            ConsumeIn90Days = row.Field<double>("CONSUME_AMOUNT");
        }

        #region ----- Define Variables -----
        public void GetStockDetailByID(string proID, string wareID)
        {
            DataTable dataTable = ProductDetailDB.GetStockDetailByID(proID, wareID);
            
            string tempStockDetail = "";

            for (int x = 0; x < dataTable.Rows.Count; x++)
            {
                string amount = dataTable.Rows[x].Field<double>("InvDet_Amount").ToString("0.##").PadLeft(10);
                string price = ((double)dataTable.Rows[x].Field<decimal>("InvDet_Price")).ToString("0.00").PadLeft(10);
                string subtotal = dataTable.Rows[x].Field<double>("SUBTOTAL").ToString("0.00").PadLeft(10);
                string batchNum = dataTable.Rows[x].Field<string>("InvDet_BatchNumber").PadRight(10);

                tempStockDetail += $"數量:{amount}  單價:{price}  小計:{subtotal}  批號: {batchNum}";

                if (x < dataTable.Rows.Count - 1)
                    tempStockDetail += "\n";
            }
            
            StockDetail = tempStockDetail;
        }
        #endregion
    }
}
