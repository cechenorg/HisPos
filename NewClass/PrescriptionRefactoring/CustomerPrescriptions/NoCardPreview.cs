using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using His_Pos.NewClass.Prescription;

namespace His_Pos.NewClass.PrescriptionRefactoring.CustomerPrescriptions
{
    public class NoCardPreview : CusPrePreviewBase
    {
        private int ID { get; set; }
        public PrescriptionType Type { get; }
        public NoCardPreview(DataRow r):base(r)
        {
            Type = PrescriptionType.Normal;
            ID = r.Field<int>("ID");
            AdjustDate = r.Field<DateTime>("Adj_Date");
        }

        public override void Print()
        {
            throw new NotImplementedException();
        }

        public override Prescription CreatePrescription()
        {
            var table = PrescriptionDb.GetPrescriptionByID(ID);
            return table.Rows.Count > 0 ? new Prescription(table.Rows[0], Type) : null;
        }

        public override void GetMedicines()
        {
            Medicines.Clear();
            Medicines.GetDataByPrescriptionId(ID);
        }

        public void MakeUp()
        {

        }
    }
}
