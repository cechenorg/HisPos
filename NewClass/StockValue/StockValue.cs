using GalaSoft.MvvmLight;
using System;
using System.Data;

namespace His_Pos.NewClass.StockValue
{
    public class StockValue : ObservableObject
    {
        public StockValue()
        {
        }

        public StockValue(DataRow r, ref double stock)
        {
            Date = r.Field<DateTime>("InvRec_Time");
            InitStockValue = stock;
            PurchaseValue = r.Field<double>("進貨");
            ReturnValue = r.Field<double>("退貨");
            StockCheckValue = r.Field<double>("盤點");
            MedUseValue = r.Field<double>("調劑耗用");
            MinusStockAdjustValue = r.Field<double>("進貨負庫調整");
            TrashValue = r.Field<double>("報廢");
            AdjustValue = r.Field<double>("調整");
            stock += PurchaseValue + ReturnValue + StockCheckValue + MedUseValue + MinusStockAdjustValue + TrashValue;
            FinalStockValue = stock;
        }

        public DateTime Date { get; set; }
        private double initStockValue;

        public double InitStockValue
        {
            get { return initStockValue; }
            set { Set(() => InitStockValue, ref initStockValue, value); }
        }

        private double purchaseValue;

        public double PurchaseValue
        {
            get { return purchaseValue; }
            set { Set(() => PurchaseValue, ref purchaseValue, value); }
        }

        private double returnValue;

        public double ReturnValue
        {
            get { return returnValue; }
            set { Set(() => ReturnValue, ref returnValue, value); }
        }

        private double stockCheckValue;

        public double StockCheckValue
        {
            get { return stockCheckValue; }
            set { Set(() => StockCheckValue, ref stockCheckValue, value); }
        }

        private double medUseValue;

        public double MedUseValue
        {
            get { return medUseValue; }
            set { Set(() => MedUseValue, ref medUseValue, value); }
        }

        private double minusStockAdjustValue;

        public double MinusStockAdjustValue
        {
            get { return minusStockAdjustValue; }
            set { Set(() => MinusStockAdjustValue, ref minusStockAdjustValue, value); }
        }

        private double trashValue;

        public double TrashValue
        {
            get { return trashValue; }
            set { Set(() => TrashValue, ref trashValue, value); }
        }

        private double finalStockValue;

        public double FinalStockValue
        {
            get { return finalStockValue; }
            set { Set(() => FinalStockValue, ref finalStockValue, value); }
        }

        private double adjustValue;

        public double AdjustValue
        {
            get { return adjustValue; }
            set { Set(() => AdjustValue, ref adjustValue, value); }
        }
    }
}