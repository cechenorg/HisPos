using GalaSoft.MvvmLight;
using His_Pos.NewClass.Product.ProductTransaction;
using System;

namespace His_Pos.NewClass.Trade
{
    public class Transaction : ObservableObject
    {
        #region ----- Define Variables -----

        private TradeProduct selectedItem;

        public TransactionTypeEnum Type { get; set; }
        public int ID { get; set; }
        public int TotalPrice { get; set; }
        public int DiscountPrice { get; set; }
        public float DiscountPercent { get; set; }
        public int DiscountPercentPrice { get; set; }
        public int DiscountPoint { get; set; }
        public int DiscountPointPrice { get; set; }
        public int Cash { get; set; }
        public int OrderCash { get; set; }
        public int CardPrice { get; set; }
        public string CardNumber { get; set; }
        public string InvoiceNumber { get; set; }
        public string TaxIDNumber { get; set; }

        public int FinalTotalPrice { get; set; }
        public int PayPrice { get; set; }
        public int LeftPrice { get; set; }
        public TradeProducts ProductCollection { get; set; }

        public TradeProduct SelectedItem
        {
            get => selectedItem;
            set { Set(() => SelectedItem, ref selectedItem, value); }
        }

        #endregion ----- Define Variables -----

        public Transaction()
        {
            ProductCollection = new TradeProducts();
        }

        #region ----- Define Functions -----

        internal void Clear()
        {
            throw new NotImplementedException();
        }

        internal void CalculateTotalPrice()
        {
            throw new NotImplementedException();
        }

        internal void DeleteSelectedProduct()
        {
            throw new NotImplementedException();
        }

        internal void AddProductByID(string productID)
        {
            throw new NotImplementedException();
        }

        #endregion ----- Define Functions -----
    }
}