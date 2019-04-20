using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.NewClass.StoreOrder.Report
{
    public class ManufactoryOrder
    {
        #region ----- Define Variables -----
        public int ManufactoryID { get; set; }
        public string ManufactoryName { get; set; }
        public int PurchaseCount { get; set; }
        public double PurchasePrice { get; set; }
        public int ReturnCount { get; set; }
        public double ReturnPrice { get; set; }
        public bool IncludeTax { get; set; }
        public ManufactoryOrderDetails OrderDetails { get; set; }
        #endregion

        public ManufactoryOrder(DataRow dataRow)
        {
            ManufactoryID = dataRow.Field<int>("Man_ID");
            ManufactoryName = dataRow.Field<string>("Man_Name");
            PurchaseCount = dataRow.Field<int>("PURCHASE_COUNT");
            PurchasePrice = (double)dataRow.Field<decimal>("PURCHASE_PRICE");
            ReturnCount = dataRow.Field<int>("RETURN_COUNT");
            ReturnPrice = (double)dataRow.Field<decimal>("RETURN_PRICE");
            IncludeTax = dataRow.Field<bool>("Man_IncludeTax");
        }

        #region ----- Define Functions -----
        public void GetOrderDetails(DateTime searchStartDate, DateTime searchEndDate)
        {
            OrderDetails = ManufactoryOrderDetails.GetOrderDetails(ManufactoryID, searchStartDate, searchEndDate);
        }
        public void ChangeIncludeTaxFlag(bool taxFlag)
        {
            IncludeTax = taxFlag;

            OrderDetails.ReCalculateTax(IncludeTax);
            SaveTaxFlag();
        }
        public void ExportToCSV()
        {

        }
        private void SaveTaxFlag()
        {

        }
        #endregion
    }
}
