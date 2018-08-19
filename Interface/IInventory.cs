﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using His_Pos.Class;

namespace His_Pos.Interface
{
    interface IInventory
    {
        InStock Stock { get; set; }
        string Location { get; set; }
        bool Status { get; set; }
        BitmapImage TypeIcon { get; set; }
        string StockValue { get; set; }
        string Note { get; set; }
        string WareHouse { get; set; }
    }
}
