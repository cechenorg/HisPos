﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.NewClass.Product.CommonProduct
{
    public class CommonProduct : Product
    {
        public CommonProduct(DataRow r):base(r) {
            OnTheFramAmount = r.Field<double>("OnTheFrame");
            BasicAmount = r.Field<double>("BasicAmount");
            PurchaseAmount = r.Field<double>("PurchaseAmount");
        }
        public double OnTheFramAmount { get; set; }
        public double BasicAmount { get; set; }
        public double PurchaseAmount { get; set; }
    }
}