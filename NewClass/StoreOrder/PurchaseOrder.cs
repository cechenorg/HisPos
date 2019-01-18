﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using His_Pos.NewClass.Product.PurchaseReturn;

namespace His_Pos.NewClass.StoreOrder
{
    public class PurchaseOrder: StoreOrder
    {
        #region ----- Define Variables -----
        private PurchaseProducts orderProducts;

        public string PatientName { get; set; }
        public PurchaseProducts OrderProducts
        {
            get { return orderProducts; }
            set { Set(() => OrderProducts, ref orderProducts, value); }
        }
        public int ProductCount
        {
            get
            {
                if (OrderProducts is null) return initProductCount;
                else return OrderProducts.Count;
            }
        }
        #endregion

        public PurchaseOrder(DataRow row) : base(row)
        {
            OrderType = OrderTypeEnum.PURCHASE;
            PatientName = row.Field<string>("Cus_Name");
        }

        public override void GetOrderProducts()
        {
            OrderProducts = PurchaseProducts.GetProductsByStoreOrderID(ID);
        }

        public override void SaveOrder()
        {
        }

        public override void AddProductByID(string iD)
        {
            
        }
    }
}