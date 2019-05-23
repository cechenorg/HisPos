using System;
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

        public double Dosage { get; }
        public string Usage { get;}
        public int Days { get; }
        public string Position { get; }
        public double Amount { get; }
    }
}
