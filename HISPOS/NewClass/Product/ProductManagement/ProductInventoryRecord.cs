using System;
using System.Data;

namespace His_Pos.NewClass.Product.ProductManagement
{
    public struct ProductInventoryRecord
    {
        #region ----- Define Variables -----

        public string ID { get; }
        public string Name { get; }
        public ProductInventoryRecordType Type { get; }
        public DateTime Time { get; }
        public double Amount { get; }
        public double Stock { get; set; }
        public string Note { get; }

        public decimal Price { get; }
        #endregion ----- Define Variables -----

        public ProductInventoryRecord(DataRow row)
        {
            string typeName = row.Field<string>("ICON");

            switch (typeName)
            {
                case "進貨":
                    Type = ProductInventoryRecordType.PurchaseReturn;
                    break;

                case "處方調劑":
                    Type = ProductInventoryRecordType.Prescription;
                    break;

                case "庫存管理":
                    Type = ProductInventoryRecordType.StockTaking;
                    break;

                case "合併拆分":
                    Type = ProductInventoryRecordType.MergeSplit;
                    break;

                case "銷售":
                    Type = ProductInventoryRecordType.Transaction;
                    break;

                case "銷售刪單":
                    Type = ProductInventoryRecordType.Transaction;
                    break;

                default:
                    Type = ProductInventoryRecordType.Error;
                    break;
            }

            ID = row.Field<string>("InvRecSourceID");
            Name = row.Field<string>("NAME");
            Time = row.Field<DateTime>("InvRec_Time");
            Amount = row.Field<double>("STOCK_DIFF");
            Stock = row.Field<double>("STOCK");
            Note = row.Field<string>("InvRec_Type");
            Price = row.Field<decimal>("STOCK_Price");
        }
    }
}