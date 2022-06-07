using System;
using System.Collections.Generic;
using System.Data;

namespace His_Pos.NewClass.Cooperative.CooperativeClinicJson
{
    public class CooperativeClinicJsonClass
    {
        public CooperativeClinicJsonClass(DataRow r)
        {
            Id = r.Field<string>("CooPreMas_PrescriptionID");
            AdjustTime = r.Field<DateTime>("PreMas_AdjustDate");
            Remark = r.Field<string>("CooPreMas_Remark");
        }

        public string Id { get; set; }
        public DateTime AdjustTime { get; set; }
        public string Remark { get; set; }
        public List<Medicines> MedicineCollection { get; set; } = new List<Medicines>();

        public class Medicines
        {
            public Medicines(DataRow r)
            {
                Id = r.Field<string>("CooPreDet_MedicineID");
                Amount = r.Field<double>("CooPreDet_MedicineAmount");
            }

            public string Id { get; set; }
            public double Amount { get; set; }
        }
         
    }
}