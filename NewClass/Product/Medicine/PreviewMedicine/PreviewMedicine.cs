using System;
using System.Data;
using His_Pos.NewClass.Cooperative.XmlOfPrescription;
using His_Pos.NewClass.CooperativeInstitution;

namespace His_Pos.NewClass.Product.Medicine.PreviewMedicine
{
    public class PreviewMedicine : Product
    {
        public PreviewMedicine()
        {

        }

        public PreviewMedicine(Item m) : base(m)
        {
            Usage = m.Freq;
            Position = m.Way;
            Amount = Convert.ToDouble(m.Total_dose);
            Dosage = Convert.ToDouble(m.Divided_dose);
            Days = Convert.ToInt32(m.Days);
        }

        public PreviewMedicine(CooperativePrescription.Item m):base(m)
        {
            Usage = m.Freq;
            Position = m.Way;
            Amount = Convert.ToDouble(m.Total_dose);
            Dosage = Convert.ToDouble(m.Divided_dose);
            Days = (int)Convert.ToDouble(m.Days);
        }

        public PreviewMedicine(DataRow r) : base(r)
        {
            Dosage = r.Field<double?>("Dosage");
            Usage = r.Field<string>("Usage");
            Days = r.Field<int?>("MedicineDays");
            Position = r.Field<string>("Position");
            Amount = r.Field<double>("TotalAmount");
        }

        public double? Dosage { get; }
        public string Usage { get;}
        public int? Days { get; }
        public string Position { get; }
        public double Amount { get; }
    }
}
