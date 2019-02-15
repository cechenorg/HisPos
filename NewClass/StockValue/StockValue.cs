﻿using GalaSoft.MvvmLight;
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
            FinalStockValue = Math.Round(r.Field<double>("DaiSto_FinalValue"),2); 
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
