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
        public string ManufactoryID { get; set; }
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
            ManufactoryID = dataRow.Field<string>("");
            ManufactoryName = dataRow.Field<string>("");
            PurchaseCount = dataRow.Field<int>("");
            PurchasePrice = (double)dataRow.Field<decimal>("");
            ReturnCount = dataRow.Field<int>("");
            ReturnPrice = (double)dataRow.Field<decimal>("");
            IncludeTax = dataRow.Field<bool>("");
        }

        #region ----- Define Functions -----
        public void GetOrderDetails()
        {
            OrderDetails = ManufactoryOrderDetails.GetOrderDetails(ManufactoryID);
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
