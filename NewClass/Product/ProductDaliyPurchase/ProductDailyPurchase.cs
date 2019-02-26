using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.NewClass.Product.ProductDaliyPurchase {
   public class ProductDailyPurchase : ObservableObject {

        public ProductDailyPurchase() {
        }
        public ProductDailyPurchase(DataRow r) {
            ProId = r.Field<string>("Pro_ID");
            ProName = r.Field<string>("Pro_ChineseName");
            StockValue = r.Field<double>("Inv_Inventory");
            NecessaryValue = r.Field<double>("NecessaryValue");
            PurchaseValue = r.Field<double>("PurchaseValue");
            ReturnValue = r.Field<double>("ReturnValue");
            OntheWayValue = r.Field<double>("OntheWayValue");
        }
        private string proId;
        public string ProId {
            get { return proId; }
            set { Set(()=> ProId,ref proId,value); }
        }
        private string proName;
        public string ProName
        {
            get { return proName; }
            set { Set(() => ProName, ref proName, value); }
        }
        private double stockValue;
        public double StockValue
        {
            get { return stockValue; }
            set { Set(() => StockValue, ref stockValue, value); }
        }
        private double necessaryValue;
        public double NecessaryValue
        {
            get { return necessaryValue; }
            set { Set(() => NecessaryValue, ref necessaryValue, value); }
        }
        private double purchaseValue;
        public double PurchaseValue
        {
            get { return purchaseValue; }
            set { Set(() => PurchaseValue, ref purchaseValue, value); }
        }
        private double ontheWayValue;
        public double OntheWayValue
        {
            get { return ontheWayValue; }
            set { Set(() => OntheWayValue, ref ontheWayValue, value); }
        } 
        private double returnValue;
        public double ReturnValue
        {
            get { return returnValue; }
            set { Set(() => ReturnValue, ref returnValue, value); }
        }
    }
}
