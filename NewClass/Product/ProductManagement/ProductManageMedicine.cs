using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.NewClass.Product.ProductManagement
{
    public class ProductManageMedicine : Product, ICloneable
    {
        private ProductManageMedicine() { }

        public ProductManageMedicine(DataRow dataRow) : base(dataRow)
        {
            Status = dataRow["PRO_STATUS"].ToString().Equals("1");
            StockValue = dataRow["TOTAL"].ToString();
            Location = dataRow["PRO_LOCATION"].ToString();
            Note = dataRow["PRO_DESCRIPTION"].ToString();
            Ingredient = dataRow["HISMED_INGREDIENT"].ToString();
            Frozen = dataRow["HISMED_FROZ"].ToString().Equals("True");
            Control = dataRow["HISMED_CONTROL"].ToString();
            Common = dataRow["HISMED_COMMON"].ToString().Equals("True");
            WareHouse = dataRow["PROWAR_NAME"].ToString();
            WareHouseId = dataRow["PROWAR_ID"].ToString();
            Indication = dataRow["HISMED_INDICATION"].ToString();
            SideEffect = dataRow["HISMED_SIDEFFECT"].ToString();
            BarCode = dataRow["PRO_BARCODE"].ToString();
            Warnings = dataRow["HISMED_NOTE"].ToString();
        }
        public string Location { get; set; }
        public bool Status { get; set; }
        public bool Frozen { get; set; }
        public string Control { get; set; }
        public bool Common { get; set; }
        public string Note { get; set; }
        public string Ingredient { get; set; }
        public string Indication { get; set; }
        public string Warnings { get; set; }
        public string SideEffect { get; set; }
        public string WareHouseId { get; set; }
        public string WareHouse { get; set; }
        public string BarCode { get; set; }
        public string StockValue { get; set; }

        public object Clone()
        {
            throw new NotImplementedException();
        }
    }
}
