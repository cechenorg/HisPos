using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using His_Pos.Interface;

namespace His_Pos.Class.Product
{
    public class StockTakingMedicine : AbstractClass.Product, IStockTaking
    {
        public StockTakingMedicine(DataRow dataRow) : base(dataRow)
        {

        }

        public string Category { get; set; }
        public double Inventory { get; set; }
        public string ValidDate { get; set; }
        public string LastCheckDate { get; set; }
        public string BatchNumber { get; set; }
        public string Location { get; set; }
    }
}
