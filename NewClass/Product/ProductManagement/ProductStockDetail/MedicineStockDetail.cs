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
        public double MedBagOnTheWayAmount { get; set; }
        public double TotalOnTheWayAmount
        {
            get { return OnTheWayAmount + MedBagOnTheWayAmount; }
        }
        public double ShelfInventory { get; set; }
        public double MedBagInventory { get; set; }
        #endregion

        public MedicineStockDetail(DataRow row) : base(row)
        {
            ShelfInventory = row.Field<double>("SHELF_INV");
            MedBagInventory = row.Field<double>("MEDBAG_INV");
            MedBagOnTheWayAmount = row.Field<double>("Inv_MedBagOnTheWay");
        }
    }
}
