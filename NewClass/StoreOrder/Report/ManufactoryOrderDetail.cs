﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;

namespace His_Pos.NewClass.StoreOrder.Report
{
    public class ManufactoryOrderDetail : ObservableObject
    {
        #region ----- Define Variables -----
        private double unTaxPrice;
        private double tax;
        private double taxPrice;

        public OrderTypeEnum Type { get; set; }
        public string ID { get; set; }
        public DateTime DoneTime { get; set; }
        private double InitialPrice { get; set; }
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
        #endregion

        public ManufactoryOrderDetail(DataRow dataRow)
        {
            Type = dataRow.Field<string>("StoOrd_Type").Equals("P")? OrderTypeEnum.PURCHASE : OrderTypeEnum.RETURN;
            ID = dataRow.Field<string>("StoOrd_ID");
            DoneTime = dataRow.Field<DateTime>("StoOrd_ReceiveTime");
            InitialPrice = (double)dataRow.Field<decimal>("PRICE");

            CalculateTax(dataRow.Field<bool>("Man_IncludeTax"));
        }

        #region ----- Define Functions -----
        internal void CalculateTax(bool includeTax)
        {
            if (includeTax)
            {
                TaxPrice = InitialPrice;

                UnTaxPrice = TaxPrice * 100 / 105;

                Tax = UnTaxPrice * 5 / 100;
            }
            else
            {
                UnTaxPrice = InitialPrice;

                Tax = InitialPrice * 5 / 100;

                TaxPrice = UnTaxPrice + Tax;
            }
        }
        #endregion
    }
}