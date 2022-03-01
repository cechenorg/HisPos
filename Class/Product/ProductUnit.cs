using His_Pos.Interface;
using System;
using System.ComponentModel;
using System.Data;

namespace His_Pos.Class.Product
{
    public class ProductUnit : IDeletable, INotifyPropertyChanged
    {
        public ProductUnit()
        {
        }

        public ProductUnit(string unit)
        {
            Id = "";
            Unit = unit;
            Amount = 0;
            Price = 0;
            VIPPrice = 0;
            EmpPrice = 0;
            BaseType = false;
        }

        public ProductUnit(DataRow row)
        {
            Id = row["PROUNI_ID"].ToString();
            Unit = row["PROUNI_TYPE"].ToString();
            Amount = Double.Parse(row["PROUNI_QTY"].ToString());
            Price = Double.Parse(row["PRO_SELL_PRICE"].ToString());
            VIPPrice = Double.Parse(row["PRO_VIP_PRICE"].ToString());
            EmpPrice = Double.Parse(row["PRO_EMP_PRICE"].ToString());
            BaseType = Boolean.Parse(row["PRO_BASETYPE_STATUS"].ToString());
        }

        public ProductUnit(string id, string unit, double amount, double price, double vIPPrice, double empPrice)
        {
            Id = id;
            Unit = unit;
            Amount = amount;
            Price = price;
            VIPPrice = vIPPrice;
            EmpPrice = empPrice;
            BaseType = false;
        }

        public string Id { get; set; }
        public string Unit { get; set; }
        public double Amount { get; set; }
        public double Price { get; set; }
        public double VIPPrice { get; set; }
        public double EmpPrice { get; set; }
        public bool BaseType { get; set; }
        private string source = "";

        public string Source
        {
            get { return source; }
            set
            {
                source = value;
                NotifyPropertyChanged("Source");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }
    }
}