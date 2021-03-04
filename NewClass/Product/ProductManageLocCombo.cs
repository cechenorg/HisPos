using System;
using System.Data;

namespace His_Pos.NewClass.Product.ProductManagement
{
    public class ProductManageLocCombo
    {
        public ProductManageLocCombo(DataRow row)
        {
          
            ID = row.Field<int>("ID");
            NAME = row.Field<string>("NAME");
           
        }

        public int ID { get; set; }
        public string NAME { get; set; }


    }
}