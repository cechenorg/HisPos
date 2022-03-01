using GalaSoft.MvvmLight;
using System;
using System.Data;

namespace His_Pos.NewClass.Report
{
    public class InventoryDifference : ObservableObject
    {
        public InventoryDifference()
        {
        }

        public InventoryDifference(DataRow r)
        {
            InventoryOverage = r.Field<decimal>("OVERAGE");
            InventoryShortage = r.Field<decimal>("SHORTAGE");
            InventoryScrap = r.Field<decimal>("SCRAP");
        }

        private decimal inventoryOverage;

        public decimal InventoryOverage
        {
            get => inventoryOverage;
            set
            {
                Set(() => InventoryOverage, ref inventoryOverage, value);
            }
        }

        private decimal inventoryShortage;

        public decimal InventoryShortage
        {
            get => inventoryShortage;
            set
            {
                Set(() => InventoryShortage, ref inventoryShortage, value);
            }
        }

        private decimal inventoryScrap;

        public decimal InventoryScrap
        {
            get => inventoryScrap;
            set
            {
                Set(() => InventoryScrap, ref inventoryScrap, value);
            }
        }

        private double inventoryTotal;

        public double InventoryTotal
        {
            get => inventoryTotal;
            set
            {
                Set(() => InventoryTotal, ref inventoryTotal, value);
            }
        }

        public void GetInventoryDifference(DateTime startDate, DateTime endDate)
        {
            throw new NotImplementedException();
        }
    }
}