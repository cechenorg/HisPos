using GalaSoft.MvvmLight;
using System;
using System.Data;

namespace His_Pos.NewClass.StockValue {
    public class StockValue : ObservableObject {
        public StockValue() {

        }
        public StockValue(DataRow r) {
            Date =  r.Field<DateTime>("DaiSto_Date");
            InitStockValue = r.Field<double>("DaiSto_InitialValue");
            PurchaseValue = r.Field<double>("DaiSto_PurchaseValue");
            ReturnValue = r.Field<double>("DaiSto_ReturnValue");
            StockCheckValue = r.Field<double>("DaiSto_StockCheckValue");
            MedUseValue = r.Field<double>("DaiSto_MedUseValue");
            FinalStockValue = r.Field<double>("DaiSto_FinalValue"); 
        }
        public DateTime Date { get; set; }
        public double InitStockValue { get; set; }
        public double PurchaseValue { get; set; }
        public double ReturnValue { get; set; }
        public double StockCheckValue { get; set; }
        public double MedUseValue { get; set; }
        public double FinalStockValue { get; set; }
        #region Function
        public static void UpdateDailyStockValue() {
            StockValueDb.UpdateDailyStockValue();
        }
        #endregion
    }
}
