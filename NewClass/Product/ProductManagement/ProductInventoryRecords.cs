using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;

namespace His_Pos.NewClass.Product.ProductManagement
{
    public class ProductInventoryRecords : Collection<ProductInventoryRecord>
    {
        public double RangePurchaseReturnAmount { get { return Items.Where(r => r.Type == ProductInventoryRecordType.PurchaseReturn).Sum(r => r.Amount); } }

        public double RangePrescriptionAmount { get { return Items.Where(r => r.Type == ProductInventoryRecordType.Prescription || r.Type == ProductInventoryRecordType.Transaction).Sum(r => r.Amount); } }

        public double RangeStockTakingAmount { get { return Items.Where(r => r.Type == ProductInventoryRecordType.StockTaking).Sum(r => r.Amount); } }

        public ProductInventoryRecords(DataTable dataTable)
        {
            foreach (DataRow row in dataTable.Rows)
            {
                Add(new ProductInventoryRecord(row));
            }
        }

        internal static ProductInventoryRecords GetInventoryRecordsByID(string proID, string wareID, DateTime startDate, DateTime endDate)
        {
            return new ProductInventoryRecords(ProductDetailDB.GetInventoryRecordsByID(proID, wareID, startDate, endDate));
        }
    }
}