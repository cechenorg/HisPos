using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.NewClass.Product.ProductGroupSetting.SplitProduct {
    public class SplitProduct : ObservableObject {
        public SplitProduct(DataRow r) {
            WarID = r.Field<int>("War_ID");
            WarName = r.Field<string>("War_Name");
            Amount = r.Field<double>("Inventory");
        }
        public int WarID { get; set; }
        public string WarName { get; set; }
        public double Amount { get; set; }
        public double SplitAmount { get; set; } = 0;
    }
}
