﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.NewClass.ProductType
{
    public class ProductTypeManageMaster : ProductType
    {
        #region ----- Define Variables -----

        private ProductTypeManageDetail currentDetailType;
        private ProductTypeManageDetails productTypeDetails;

        public ProductTypeManageDetail CurrentDetailType
        {
            get { return currentDetailType; }
            set
            {
                MainWindow.ServerConnection.OpenConnection();
                value?.GetTypeDetailProducts();
                MainWindow.ServerConnection.CloseConnection();
                Set(() => CurrentDetailType, ref currentDetailType, value);
            }
        }
        public ProductTypeManageDetails ProductTypeDetails
        {
            get => productTypeDetails;
            set
            {
                Set(() => ProductTypeDetails, ref productTypeDetails, value); 
                RaisePropertyChanged(nameof(TotalStockValue));
            }
        }
        public double TotalStockValue { get { return ProductTypeDetails.Sum(d => d.StockValue); } }
        public double TotalSales { get { return ProductTypeDetails.Sum(d => d.Sales); } }
        #endregion

        public ProductTypeManageMaster(DataRow row) : base(row)
        {
        }

        internal void GetTypeDetails()
        {
            throw new NotImplementedException();
        }
    }
}
