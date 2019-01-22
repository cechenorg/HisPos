using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.NewClass.Product.Medicine
{
    public class Medicines:ObservableCollection<Medicine>
    {
        public Medicines() { }

        public int CountMedicinePoint()
        {
            var medicinePoint = this.Where(m => m is MedicineNHI med && !med.PaySelf)
                .Sum(med => med.NHIPrice * med.Amount);
            return (int)Math.Round(medicinePoint, 0, MidpointRounding.AwayFromZero);
        }

        public int CountSelfPay()
        {
            var selfPay = this.Where(m => m.PaySelf).Sum(m=>m.TotalPrice);
            return (int)Math.Ceiling(selfPay);
        }
        public void GetDataByPrescriptionId(int preId) {
            DataTable table = MedicineDb.GetDataByPrescriptionId(preId);
            foreach (DataRow r in table.Rows) {
                Medicine med = new Medicine(r);
                med.Usage.Name = r.Field<string>("Usage");
                med.Position.Name = r.Field<string>("Position");
                med.Days = r.Field<short>("MedicineDays");
                med.PaySelf = r.Field<bool>("PaySelf");
                med.Amount = r.Field<double>("TotalAmount");
                Add(med);
            }
        }
    }
}
