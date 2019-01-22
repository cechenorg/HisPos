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
        public double Inventory { get; set; }
        public double SaveAmount { get; set; }
        public double BasicAmount { get; set; }
        public double LastPrice { get; set; }
        public double OrderAmount { get; set; }
        public double FreeAmount { get; set; }
        public double Price { get; set; }
        public string Invoice { get; set; }
        public string ValidDate { get; set; }
        public string BatchNumber { get; set; }
        public string Note { get; set; }

        public bool IsFirstBatch { get; set; }
        public double SingdePackageAmount { get; } 
        public double SingdePackagePrice { get; }
        public double SingdePrice { get; }
        public bool IsManufactorySingde { get; set; }
        #endregion

        public PurchaseProduct() : base() {}

        public PurchaseProduct(DataRow dataRow) : base(dataRow)
        {
        }
    }
}
