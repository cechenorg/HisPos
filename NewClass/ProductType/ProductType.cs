﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;

namespace His_Pos.NewClass.ProductType
{
    public class ProductType : ObservableObject
    {
        #region ----- Define Variables -----
        private string name;

        public int ID { get; set; }
        public int ParentID { get; set; }
        public string Name
        {
            get => name;
            set { Set(() => Name, ref name, value); }
        }
        #endregion

        public ProductType(DataRow row)
        {
            ID = row.Field<int>("Type_ID");
            ParentID = row.Field<int>("Type_Parent");
            Name = row.Field<string>("Type_Name");
        }
    }
}
