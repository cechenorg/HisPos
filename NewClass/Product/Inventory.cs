using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.NewClass.Product
{
    public class Inventory : ObservableObject
    {
        public Inventory(DataRow r) {
            InvID = r.Field<int>("Inv_ID");
            InventoryAmount = r.Field<int>("Inv_Inventory");
            OnTheWayAmount = r.Field<int>("Inv_OnTheWay");
            MegBagAmount = r.Field<int>("MegBagAmount");
            OnTheFrame = r.Field<int>("OnTheFrame");
        }
        public int InvID { get; set; }
        public int InventoryAmount { get; set; }
        public int OnTheWayAmount { get; set; }
        public int MegBagAmount { get; set; }
        public int OnTheFrame { get; set; }
    }
}
