using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.NewClass.Product.ProductManagement.ProductManageDetail
{
    class ProductNHISpecialDetail : ProductManageDetail
    {
        public ProductNHISpecialDetail(DataRow row) : base(row)
        {
            NHIPrice = (double)row.Field<decimal>("Med_Price");
            BigCategory = row.Field<string>("Med_Manufactory");
            SmallCategory = row.Field<string>("Med_Manufactory");
            Manufactory = row.Field<string>("Med_Manufactory");
        }
        
        public double NHIPrice { get; }
        public string BigCategory { get; }
        public string SmallCategory { get; }

        public string Manufactory { get; }
        
    }
}
