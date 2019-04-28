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
            NHIPrice = (double)row.Field<decimal>("SpeMed_NHIPrice");
            BigCategory = row.Field<string>("SpeMed_BigCategory");
            SmallCategory = row.Field<string>("SpeMed_SmallCategory");
            Manufactory = row.Field<string>("SpeMed_Manufactory");
        }
        
        public double NHIPrice { get; }
        public string BigCategory { get; }
        public string SmallCategory { get; }

        public string Manufactory { get; }
        
    }
}
