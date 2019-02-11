﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.NewClass.Product.ProductManagement
{
    public class ProductManageStruct
    {
        public ProductManageStruct(DataRow row)
        {
            ProductType = row.Field<string>("TYPE").Equals("M")? ProductTypeEnum.Medicine : ProductTypeEnum.OTC;
            ID = row.Field<string>("Pro_ID");
            ChineseName = row.Field<string>("Pro_ChineseName");
            EnglishName = row.Field<string>("Pro_EnglishName");
            Inventory = row.Field<double>("Inv_Inventory");
            SafeAmount = row.Field<int>("Inv_SafeAmount");
            BasicAmount = row.Field<int>("Inv_BasicAmount");
            OnTheWayAmount = row.Field<double>("Inv_OnTheWay");
            IsCommon = row.Field<bool>("Med_IsCommon");
            IsFrozen = row.Field<bool>("Med_IsFrozen");
            ControlLevel = row.Field<byte?>("Med_Control");
            StockValue = row.Field<double>("STOCK_VALUE");
            IsEnable = row.Field<bool>("Pro_IsEnable");
        }

        public ProductTypeEnum ProductType { get; set; }
        public string ID { get; set; }
        public string ChineseName { get; set; }
        public string EnglishName { get; set; }
        public string FullName
        {
            get
            {
                if (EnglishName.Contains(" "))
                    return EnglishName.Substring(0, EnglishName.IndexOf(" ")) + ChineseName;
                return EnglishName + ChineseName;
            }
        }
        public double Inventory { get; set; }
        public int SafeAmount { get; set; }
        public int BasicAmount { get; set; }
        public double OnTheWayAmount { get; set; }
        public double StockValue { get; set; }
        public int? ControlLevel { get; set; }
        public bool IsCommon { get; set; }
        public bool IsFrozen { get; set; }
        public bool IsEnable { get; set; }
    }
}