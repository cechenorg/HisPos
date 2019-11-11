﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using His_Pos.NewClass.Product.TypeManage;

namespace His_Pos.NewClass.ProductType
{
    public class ProductTypeManageDetail : ProductType
    {
        #region ----- Define Variables -----
        private TypeManageProducts productCollection;
        
        public double StockValue { get; set; }
        public double Sales { get; set; }
        public int ItemCount { get; set; }
        public TypeManageProducts ProductCollection
        {
            get => productCollection;
            set { Set(() => ProductCollection, ref productCollection, value); }
        }
        #endregion
        
        public ProductTypeManageDetail(DataRow row) : base(row)
        {
            ItemCount = row.Field<int>("PRO_COUNT");
        }

        internal void GetTypeDetailProducts()
        {
            ProductCollection = TypeManageProducts.GetTypeProducts(ID);
        }
    }
}