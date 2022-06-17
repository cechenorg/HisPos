using GalaSoft.MvvmLight;
using System;
using System.Data;
using DomainModel.Enum;

namespace His_Pos.NewClass.StoreOrder.Report
{
    public class ManufactoryOrderDetail : ObservableObject
    {
        #region ----- Define Variables -----

        private double unTaxPrice;
        private double tax;
        private double taxPrice;
        private string name;

        public OrderTypeEnum Type { get; set; }
        public string ID { get; set; }
        public DateTime DoneTime { get; set; }
        private double InitialPrice { get; set; }
        public double ReturnStockValue { get; set; }
        public string CheckCode { get; set; }
        public string OTCType { get; set; }
        public string Note { get; set; }
        public int IsControl { get; set; }
        public int IsFrozen { get; set; }
        public double UnTaxPrice
        {
            get { return unTaxPrice; }
            set { Set(() => UnTaxPrice, ref unTaxPrice, value); }
        }

        public double Tax
        {
            get { return tax; }
            set { Set(() => Tax, ref tax, value); }
        }

        public double TaxPrice
        {
            get { return taxPrice; }
            set { Set(() => TaxPrice, ref taxPrice, value); }
        }

        public string Name
        {
            get { return name; }
            set { Set(() => Name, ref name, value); }
        }

        public double ReturnDiff { get { return TaxPrice - ReturnStockValue; } }

        #endregion ----- Define Variables -----

        public ManufactoryOrderDetail(DataRow dataRow)
        {
            Type = dataRow.Field<string>("StoOrd_Type").Equals("P") ? OrderTypeEnum.PURCHASE : OrderTypeEnum.RETURN;
            ID = dataRow.Field<string>("StoOrd_ID");
            DoneTime = dataRow.Field<DateTime>("StoOrd_ReceiveTime");
            InitialPrice = (double)dataRow.Field<decimal>("PRICE");
            ReturnStockValue = (double)dataRow.Field<decimal>("RETURN_PRICE");
            Name = dataRow.Field<string>("Man_Name");
            CheckCode = dataRow.Field<string>("StoOrd_CheckCode");
            OTCType = dataRow.Field<string>("StoOrd_IsOTCType");
            Note = dataRow.Field<string>("StoOrd_Note");
            IsFrozen = dataRow.Field<int>("IsFrozen");
            IsControl = dataRow.Field<int>("IsControl");
            CalculateTax();
        }

        #region ----- Define Functions -----

        internal void CalculateTax()
        {
            TaxPrice = Math.Round(InitialPrice);

            UnTaxPrice = Math.Round(TaxPrice * 100 / 105, MidpointRounding.AwayFromZero);

            Tax = TaxPrice - UnTaxPrice;
        }

        #endregion ----- Define Functions -----
    }
}