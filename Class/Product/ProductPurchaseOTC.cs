﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using His_Pos.Interface;

namespace His_Pos.Class.Product
{
    public class ProductPurchaseOtc : AbstractClass.Product, IProductPurchase, IDeletable, ITrade, INotifyPropertyChanged, ICloneable
    {
        public ProductPurchaseOtc(DataRow dataRow, DataSource dataSource) : base(dataRow)
        {
            BatchNumber = "";

            LastPrice = Double.Parse(dataRow["LAST_PRICE"].ToString());

            Stock = new InStock()
            {
                Inventory = double.Parse(dataRow["PRO_INVENTORY"].ToString()),
                SafeAmount = dataRow["PRO_SAFEQTY"].ToString(),
                BasicAmount = dataRow["PRO_BASICQTY"].ToString()
            };

            switch (dataSource)
            {
                case DataSource.PRODUCTBASICORSAFE:
                    Amount = Int16.Parse(dataRow["PRO_BASICQTY"].ToString()) -
                             Int16.Parse(dataRow["PRO_INVENTORY"].ToString());
                    Price = 0;
                    TotalPrice = 0;
                    Note = "";
                    Invoice = "";
                    FreeAmount = 0;
                    ValidDate = "";
                    break;
                case DataSource.GetStoreOrderDetail:
                    Price = Double.Parse(dataRow["STOORDDET_PRICE"].ToString());
                    TotalPrice = Double.Parse(dataRow["STOORDDET_SUBTOTAL"].ToString());
                    Amount = Int32.Parse(dataRow["STOORDDET_QTY"].ToString());
                    Note = dataRow["PRO_DESCRIPTION"].ToString();
                    Invoice = dataRow["STOORDDET_INVOICE"].ToString();
                    FreeAmount = Int32.Parse(dataRow["STOORDDET_FREEQTY"].ToString());
                    ValidDate = dataRow["STOORDDET_VALIDDATE"].ToString();
                    break;
                case DataSource.GetItemDialogProduct:
                    Amount = 0;
                    Price = 0;
                    TotalPrice = 0;
                    Note = "";
                    Invoice = "";
                    FreeAmount = 0;
                    ValidDate = "";
                    break;
            }
        }

        private ProductPurchaseOtc()
        {
        }

        public InStock Stock { get; set; }
        public double LastPrice { get; set; }
        public string source;
        public string Source {
            get {
                return source;
            }
            set {
                source = value;
                NotifyPropertyChanged("Source");
            }
        }
        public double Cost { get; set; }
        public double totalPrice;
        public double TotalPrice
        {
            get { return totalPrice; }
            set
            {
                totalPrice = value;
                NotifyPropertyChanged("TotalPrice");
            }
        }
        public double amount;
        public double Amount
        {
            get { return amount; }
            set
            {
                amount = value;
                CalculateData();
                NotifyPropertyChanged("Amount");
            }
        }
        public double price;
        public double Price
        {
            get { return price; }
            set
            {
                price = value;
                CalculateData();
            }
        }
        public string Note { get; set; }
        public int FreeAmount { get; set; }
        public string Invoice { get; set; }
        public string ValidDate { get; set; }
        public string BatchNumber { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }

        public void CalculateData()
        {
            TotalPrice = amount * price;
        }

        public object Clone()
        {
            ProductPurchaseOtc otc = new ProductPurchaseOtc();

            otc.Id = Id;
            otc.Name = Name;
            otc.Stock=Stock;
            otc.LastPrice=LastPrice;
            otc.Source=Source;
            otc.Cost=Cost;
            otc.TotalPrice=TotalPrice;
            otc.Amount=Amount;
            otc.Price=Price;
            otc.Note=Note;
            otc.FreeAmount=FreeAmount;
            otc.Invoice=Invoice;
            otc.ValidDate=ValidDate;
            otc.BatchNumber=BatchNumber;

            return otc;
        }
    }
}
