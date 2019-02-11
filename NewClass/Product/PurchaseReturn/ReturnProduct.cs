using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.NewClass.Product.PurchaseReturn
{
    public class ReturnProduct : Product
    {
        #region ----- Define Variables -----
        public double Inventory { get; }
        public string UnitName { get; set; }
        public double UnitAmount { get; set; }
        public int SafeAmount { get; }
        public string Note { get; set; }
        public string BatchNumber { get; set; }
        public double BatchLimit { get; set; }
        public double ReturnAmount { get; set; }
        public double RealAmount { get; set; }
        public double Price { get; set; }
        public double SubTotal { get; set; }
        #endregion

        public ReturnProduct() : base() {}

        public ReturnProduct(DataRow dataRow) : base(dataRow)
        {
        }

        public void CopyOldProductData(ReturnProduct returnProduct)
        {
            
        }
    }
}
