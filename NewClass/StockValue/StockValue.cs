using GalaSoft.MvvmLight;
using System;
using System.Data;

namespace His_Pos.NewClass.StockValue {
    public class StockValue : ObservableObject {
        public StockValue() {

        }
        public StockValue(DataRow r) {
            Date =  r.Field<DateTime>("DaiSto_Date");
            InitStockValue = Math.Round(r.Field<double>("DaiSto_InitialValue"),2);
            PurchaseValue = Math.Round(r.Field<double>("DaiSto_PurchaseValue"), 2);   
            ReturnValue = Math.Round(r.Field<double>("DaiSto_ReturnValue"),2);
            StockCheckValue = Math.Round(r.Field<double>("DaiSto_StockCheckValue"),2);
            MedUseValue = Math.Round(r.Field<double>("DaiSto_MedUseValue"),2);
            PurchaseAdjustValue = Math.Round(r.Field<double>("DaiSto_PurchaseAdjustValue"), 2);
            FinalStockValue = Math.Round(r.Field<double>("DaiSto_FinalValue"),2); 
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
        public double ReturnValue {
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
        private double purchaseAdjustValue;
        public double PurchaseAdjustValue
        {
            get { return purchaseAdjustValue; }
            set { Set(() => PurchaseAdjustValue, ref purchaseAdjustValue, value); }
        }
        private double finalStockValue;
        public double FinalStockValue
        {
            get { return finalStockValue; }
            set { Set(() => FinalStockValue, ref finalStockValue, value); }
        }
        #region Function
        public static void UpdateDailyStockValue() {
            StockValueDb.UpdateDailyStockValue();
        }
        #endregion
    }
}
