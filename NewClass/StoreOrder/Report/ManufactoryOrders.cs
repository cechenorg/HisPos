﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.NewClass.StoreOrder.Report
{
    public class ManufactoryOrders : Collection<ManufactoryOrder>
    {
        internal static ManufactoryOrders GetManufactoryOrdersBySearchCondition(DateTime startDate, DateTime endDate, string manufactoryName)
        {
            throw new NotImplementedException();
        }
    }
}
