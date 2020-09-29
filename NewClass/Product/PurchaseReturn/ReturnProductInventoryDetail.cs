﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using GalaSoft.MvvmLight;

namespace His_Pos.NewClass.Product.PurchaseReturn
{
    public class ReturnProductInventoryDetail : ObservableObject
    {
        #region ----- Define Variables -----
        private double returnAmount;
        private double returnStockValue;

        public int ID { get; set; }
        public string BatchNumber { get; set; }
        public DateTime? ValidDate { get; set; }
        public double Price { get; set; }
        public double Inventory { get; set; }

        public int TypeOTC { get; set; }
        public double ReturnStockValue
        {
            get { return returnStockValue; }
            set { Set(() => ReturnStockValue, ref returnStockValue, value); }
        }
        public double ReturnAmount
        {
            get { return returnAmount; }
            set
            {
                if(value > Inventory)
                    Set(() => ReturnAmount, ref returnAmount, Inventory);
                else
                    Set(() => ReturnAmount, ref returnAmount, value);

                CalculateStockValue();
            }
        }
        #endregion

        public ReturnProductInventoryDetail(DataRow row)
        {
            TypeOTC = row.Field<int>("Pro_TypeID");
            ID = row.Field<int>("InvDet_ID");
            BatchNumber = row.Field<string>("InvDet_BatchNumber");
            ValidDate = row.Field<DateTime?>("InvDet_ValidDate");
            Price = (double)row.Field<decimal>("InvDet_Price");
            Inventory = row.Field<double>("InvDet_Amount");
            ReturnStockValue = 0;
            ReturnAmount = 0;
        }

        #region ----- Define Functions -----
        private void CalculateStockValue()
        {
            ReturnStockValue = Price * ReturnAmount;
        }
        #endregion
    }
}
