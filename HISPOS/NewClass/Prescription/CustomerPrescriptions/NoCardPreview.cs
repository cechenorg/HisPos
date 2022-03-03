using System;
using System.Data;

namespace His_Pos.NewClass.Prescription.CustomerPrescriptions
{
    public class NoCardPreview : CusPrePreviewBase
    {
        private int ID { get; set; }
        public PrescriptionType Type { get; }

        public NoCardPreview(DataRow r) : base(r)
        {
            Type = PrescriptionType.Normal;
            ID = r.Field<int>("ID");
            AdjustDate = r.Field<DateTime>("Adj_Date");
        }

        public override void Print()
        {
            throw new NotImplementedException();
        }

        public override NewClass.Prescription.Prescription CreatePrescription()
        {
            var table = PrescriptionDb.GetPrescriptionByID(ID);
            return table.Rows.Count > 0 ? new NewClass.Prescription.Prescription(table.Rows[0], Type) : null;
        }

        public override void GetMedicines()
        {
            Medicines.Clear();
            Medicines.GetDataByPrescriptionId(ID);
        }

        public void MakeUp()
        {
        }

        public override void PrintDir()
        {
            throw new NotImplementedException();
        }
    }
}