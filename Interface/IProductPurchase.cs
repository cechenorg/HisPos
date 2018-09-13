﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using His_Pos.AbstractClass;
using His_Pos.Class;

namespace His_Pos.Interface
{
    public interface IProductPurchase
    {
        bool Status { get; set; }
        string Note { get; set; }
        InStock Stock { get; set; }
        double LastPrice { get; set; }
        double OrderAmount { get; set; }
        double FreeAmount { get; set; }
        string Invoice { get; set; }
        string ValidDate { get; set; }
        string BatchNumber { get; set; }

        bool IsFirstBatch { get; set; }

        void CopyFilledData(Product product);
    }
}
