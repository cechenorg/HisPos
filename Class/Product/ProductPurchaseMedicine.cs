﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using His_Pos.Interface;

namespace His_Pos.Class.Product
{
    class ProductPurchaseMedicine : AbstractClass.Product, IProductPurchase, IDeletable, ITrade, INotifyPropertyChanged, ICloneable
    {
        public ProductPurchaseMedicine(DataRow dataRow, DataSource dataSource) : base(dataRow)
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
                    BatchNumber = "";
                    break;
                case DataSource.GetStoreOrderDetail:
                    Price = Double.Parse(dataRow["STOORDDET_PRICE"].ToString());
                    TotalPrice = Double.Parse(dataRow["STOORDDET_SUBTOTAL"].ToString());
                    Amount = Int32.Parse(dataRow["STOORDDET_QTY"].ToString());
                    Note = dataRow["PRO_DESCRIPTION"].ToString();
                    Invoice = dataRow["STOORDDET_INVOICE"].ToString();
                    FreeAmount = Int32.Parse(dataRow["STOORDDET_FREEQTY"].ToString());
                    ValidDate = (dataRow["STOORDDET_VALIDDATE"].ToString().Equals("1900/01/01"))? "" : dataRow["STOORDDET_VALIDDATE"].ToString();
                    BatchNumber = dataRow["STOORDDET_BATCHNUMBER"].ToString();
                    break;
                case DataSource.GetItemDialogProduct:
                    Amount = 0;
                    Price = 0;
                    TotalPrice = 0;
                    Note = "";
                    Invoice = "";
                    FreeAmount = 0;
                    ValidDate = "";
                    BatchNumber = "";
                    break;
            }
        }

        private ProductPurchaseMedicine()
        {
        }

        public InStock Stock { get; set; }
        private string note;
        public string Note
        {
            get { return note; }
            set
            {
                note = value;
                NotifyPropertyChanged("Note");
            }
        }
        public double LastPrice { get; set; }
        private string source;
        public string Source
        {
            get
            {
                return source;
            }
            set
            {
                source = value;
                NotifyPropertyChanged("Source");
            }
        }
        public double Cost { get; set; }
        private double totalPrice;
        public double TotalPrice
        {
            get { return totalPrice; }
            set
            {
                totalPrice = value;
                NotifyPropertyChanged("TotalPrice");
            }
        }
        private double amount;
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
        private double price;
        public double Price
        {
            get { return price; }
            set
            {
                price = value;
                CalculateData();
            }
        }

        private int freeAmount;

        public int FreeAmount
        {
            get { return freeAmount; }
            set
            {
                freeAmount = value;
                NotifyPropertyChanged("FreeAmount");
            }
        }

        private string invoice;

        public string Invoice
        {
            get { return invoice; }
            set
            {
                invoice = value;
                NotifyPropertyChanged("Invoice");
            }
        }

        private string validDate;

        public string ValidDate
        {
            get { return validDate; }
            set
            {
                validDate = value;
                NotifyPropertyChanged("ValidDate");
            }
        }

        private string batchNumber;

        public string BatchNumber
        {
            get { return batchNumber; }
            set
            {
                batchNumber = value;
                NotifyPropertyChanged("BatchNumber");
            }
        }

        public void CalculateData()
        {
            TotalPrice = amount * price;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }

        public object Clone()
        {
            ProductPurchaseMedicine med = new ProductPurchaseMedicine();

            med.Id = Id;
            med.Name = Name;
            med.Stock = Stock;
            med.LastPrice = LastPrice;
            med.Source = Source;
            med.Cost = Cost;
            med.TotalPrice = TotalPrice;
            med.Amount = Amount;
            med.Price = Price;
            med.Note = Note;
            med.FreeAmount = FreeAmount;
            med.Invoice = Invoice;
            med.ValidDate = ValidDate;
            med.BatchNumber = BatchNumber;

            return med;
        }
    }
}
