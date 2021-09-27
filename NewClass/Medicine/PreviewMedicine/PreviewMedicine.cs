using His_Pos.NewClass.Cooperative.XmlOfPrescription;
using System;
using System.Data;
using OrthopedicsMedicine = His_Pos.NewClass.Cooperative.CooperativeInstitution.Item;

namespace His_Pos.NewClass.Medicine.PreviewMedicine
{
    public class PreviewMedicine : Product.Product
    {
        public PreviewMedicine()
        {
        }

        public PreviewMedicine(OrthopedicsMedicine m) : base(m)
        {
            Usage = m.Freq;
            Position = m.Way;
            Amount = Convert.ToDouble(m.Total_dose);
            Dosage = Convert.ToDouble(m.Divided_dose);
            Days = Convert.ToInt32(m.Days);
        }

        public PreviewMedicine(CooperativePrescription.Item m) : base(m)
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

        public PreviewMedicine(DataRow r, PreviewMedicine item)
        {
            ID = r.Field<string>("Pro_ID");
            ChineseName = r.Field<string>("Pro_ChineseName");
            EnglishName = r.Field<string>("Pro_EnglishName");
            Dosage = item.Dosage;
            Usage = item.Usage;
            Amount = item.Amount;
        }

        public double? Dosage { get; set; }
        public string Usage { get; set; }
        public int? Days { get; set; }
        public string Position { get; set; }
        public double Amount { get; set; }
    }
}