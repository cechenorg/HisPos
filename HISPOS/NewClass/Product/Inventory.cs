using GalaSoft.MvvmLight;
using System;
using System.Data;

namespace His_Pos.NewClass.Product
{
    public class Inventory : ObservableObject
    {
        public Inventory(DataRow r)
        {
            InvID = r.Table.Columns.Contains("Inv_ID") ? r.Field<int>("Inv_ID") : 0;
            InventoryAmount = r.Table.Columns.Contains("Inv_Inventory") ? r.Field<double>("Inv_Inventory") : 0;
            OnTheWayAmount = r.Table.Columns.Contains("Inv_OnTheWay") ? r.Field<double>("Inv_OnTheWay") : 0;
            MegBagAmount = r.Table.Columns.Contains("Inv_OnTheWay") ? r.Field<double>("MegBagAmount") : 0;
            OnTheFrame = r.Table.Columns.Contains("OnTheFrame") ? r.Field<double>("OnTheFrame") : 0;
            CanUseAmount = r.Table.Columns.Contains("CanUseAmount") ? r.Field<double>("CanUseAmount") : 0;
            Med_Price = r.Table.Columns.Contains("Med_Price") ? Convert.ToDouble(r["Med_Price"]) : 0;
        }

        public int InvID { get; set; }
        public double InventoryAmount { get; set; }
        public double OnTheWayAmount { get; set; }
        public double MegBagAmount { get; set; }
        public double OnTheFrame { get; set; }
        public double CanUseAmount { get; set; }
        public double Med_Price { get; set; }
    }
}