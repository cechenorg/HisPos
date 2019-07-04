using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.NewClass.Product.ProductManagement
{
    public struct ProductRegisterPrescription
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public DateTime? Date { get; set; }
        public double Amount { get; set; }

        public ProductRegisterPrescription(DataRow row)
        {
            ID = row.Field<int>("PreMas_ID");
            Name = row.Field<string>("Cus_Name");
            Date = row.Field<DateTime>("PreMas_AdjustDate");
            Amount = row.Field<double>("PreDet_TotalAmount");
        }
    }
}
