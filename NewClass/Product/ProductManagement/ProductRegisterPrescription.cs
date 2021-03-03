using System;
using System.Data;

namespace His_Pos.NewClass.Product.ProductManagement
{
    public struct ProductRegisterPrescription
    {
        public ProductRegisterPrescriptionTypeEnum Type { get; set; }
        public string PreparedStatus { get; set; }
        public int ID { get; set; }
        public string Name { get; set; }
        public DateTime? Date { get; set; }
        public double Amount { get; set; }

        public ProductRegisterPrescription(DataRow row)
        {
            Type = row.Field<string>("TYPE").Equals("P") ? ProductRegisterPrescriptionTypeEnum.PRESCRIPTION : ProductRegisterPrescriptionTypeEnum.REGISTER;
            ID = row.Field<int>("PreMas_ID");
            PreparedStatus = row.Field<string>("PREPARE_STATUS");
            Name = row.Field<string>("Cus_Name");
            Date = row.Field<DateTime>("PreMas_AdjustDate");
            Amount = row.Field<double>("PreDet_TotalAmount");
        }
    }
}