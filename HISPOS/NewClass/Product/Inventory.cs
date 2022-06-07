using GalaSoft.MvvmLight;
using System.Data;

namespace His_Pos.NewClass.Product
{
    public class Inventory : ObservableObject
    {
        public Inventory(DataRow r)
        {
            InvID = r.Field<int>("Inv_ID");
            InventoryAmount = r.Field<double>("Inv_Inventory");
            OnTheWayAmount = r.Field<double>("Inv_OnTheWay");
            MegBagAmount = r.Field<double>("MegBagAmount");
            OnTheFrame = r.Field<double>("OnTheFrame");
        }

        public int InvID { get; set; }
        public double InventoryAmount { get; set; }
        public double OnTheWayAmount { get; set; }
        public double MegBagAmount { get; set; }
        public double OnTheFrame { get; set; }
    }
}