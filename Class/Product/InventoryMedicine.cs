﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using His_Pos.Interface;

namespace His_Pos.Class.Product
{
    public class InventoryMedicine : AbstractClass.Product, IInventory
    {
        public InventoryMedicine(DataRow dataRow) : base(dataRow)
        {
            Status = dataRow["PRO_STATUS"].ToString().Equals("1");
            TypeIcon = new BitmapImage(new Uri(@"..\Images\HisDot.png", UriKind.Relative));
            StockValue = dataRow["TOTAL"].ToString();
            Location = dataRow["PRO_LOCATION"].ToString();
            Note = dataRow["PRO_DESCRIPTION"].ToString();
            Stock = new InStock()
            {
                Inventory = double.Parse(dataRow["PRO_INVENTORY"].ToString()),
                SafeAmount = dataRow["PRO_SAFEQTY"].ToString(),
                BasicAmount = dataRow["PRO_BASICQTY"].ToString()
            }; 
        Ingredient =  dataRow["HISMED_INGREDIENT"].ToString();
    }
        public InStock Stock { get; set; }
        public string Location { get; set; }
        public bool Status { get; set; }
        public BitmapImage TypeIcon { get; set; }
        public string StockValue { get; set; }
        public string Note { get; set; }
        public string Ingredient { get; set; }
    }
}
