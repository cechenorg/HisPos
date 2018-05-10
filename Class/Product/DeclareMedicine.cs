using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using His_Pos.Interface;

namespace His_Pos.Class.Product
{
    class DeclareMedicine : Medicine, ITrade
    {
        public DeclareMedicine(DataRow dataRow, DataSource dataSource): base(dataRow)
        {
            Stock = new InStock()
            {
                BasicAmount = dataRow["PRO_BASICQTY"].ToString(),
                SafeAmount = dataRow["PRO_SAFEQTY"].ToString(),
                Inventory = Double.Parse((dataRow["PRO_INVENTORY"].ToString() == "")
                    ? "0" : dataRow["PRO_INVENTORY"].ToString())
            };

            Price = Double.Parse(dataRow["STOORDDET_PRICE"].ToString());
            TotalPrice = Double.Parse(dataRow["STOORDDET_SUBTOTAL"].ToString());
        }

        public InStock Stock { get; set; }
        public double Cost { get; set; }
        public double TotalPrice { get; set; }
        public double Amount { get; set; }
        public double Price { get; set; }

        public void CalculateData(string inputSource)
        {
            throw new NotImplementedException();
        }
    }
}
