using System;
using System.Data;

namespace His_Pos.NewClass.PrescriptionRefactoring.CustomerPrescriptions
{
    public enum ChronicType
    {
        Register = 0,
        Reserve = 1
    }
    public class ChronicPreview : CusPrePreviewBase
    {
        public ChronicType Type { get; }
        public DateTime AdjustDate { get; }
        public int ChronicSeq { get; }
        public int ChronicTotal { get; }
        public string OriginalMedicalNumber { get; }
        public ChronicPreview(DataRow r, ChronicType reserve) : base(r)
        {
            Type = reserve;
            AdjustDate = r.Field<DateTime>("");
            ChronicSeq = r.Field<int>("");
            ChronicTotal = r.Field<int>("");
            OriginalMedicalNumber = r.Field<string>("");
        }

        public override void Print()
        {
            throw new NotImplementedException();
        }

        public override Prescription CreatePrescription()
        {
            throw new NotImplementedException();
        }
    }
}
