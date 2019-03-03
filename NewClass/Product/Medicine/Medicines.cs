using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;

namespace His_Pos.NewClass.Product.Medicine
{
    public class Medicines:ObservableCollection<Medicine>
    {
        public Medicines() { }

        public int CountMedicinePoint()
        {
            var medicinePoint = this.Where(m => m is MedicineNHI && !m.PaySelf)
                .Sum(m => m.NHIPrice * m.Amount) + this.Where(m => m is MedicineSpecialMaterial && !m.PaySelf)
                                    .Sum(m => m.NHIPrice * m.Amount * 1.05);
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
                med.Dosage = r.Field<double?>("Dosage");
                med.UsageName = r.Field<string>("Usage");
                med.PositionName = r.Field<string>("Position");
                med.Days = r.Field<short?>("MedicineDays");
                med.PaySelf = r.Field<bool>("PaySelf");
                med.IsBuckle = r.Field<bool>("IsBuckle");
                med.Amount = r.Field<double>("TotalAmount");
                med.BuckleAmount = r.Field<double?>("BuckleAmount");
                med.Price = (double)r.Field<decimal>("PaySelfValue");
                Add(med);
            }
        }
        public void GetDataByReserveId(string resId)
        {
            DataTable table = MedicineDb.GetDataByReserveId(resId);
            foreach (DataRow r in table.Rows)
            {
                Medicine med = new Medicine(r);
                med.Dosage = r.Field<double>("Dosage");
                med.UsageName = r.Field<string>("Usage");
                med.PositionName = r.Field<string>("Position");
                med.Days = r.Field<short>("MedicineDays");
                med.PaySelf = r.Field<bool>("PaySelf");
                med.IsBuckle = r.Field<bool>("IsBuckle");
                med.Amount = r.Field<double>("TotalAmount");
                Add(med);
            }
        }
        public string CreateMedicalData(string dateTime)
        {
            var medList = this.Where(m => (m is MedicineNHI || m is MedicineSpecialMaterial) && !m.PaySelf).ToList();
            var result = string.Empty;
            foreach (var med in medList)
            {
                result += dateTime;
                if (med is MedicineNHI)
                {
                    result += "1";
                    result += med.ID.PadLeft(12, ' ');
                    result += med.PositionName.PadLeft(6, ' ');
                    result += med.UsageName.PadLeft(18, ' ');
                    result += med.Days.ToString().PadLeft(2, ' ');
                    result += med.Amount.ToString().PadLeft(7, ' ');
                    result += "01";
                }
                else
                {
                    result += "4";
                    result += med.ID.PadLeft(12, ' ');
                    result += string.Empty.PadLeft(6, ' ');
                    result += string.Empty.PadLeft(18, ' ');
                    if(med.Days != null)
                        result += med.Days.ToString().PadLeft(2, ' ');
                    else
                        result += string.Empty.PadLeft(2, ' ');
                    result += med.Amount.ToString().PadLeft(7, ' ');
                    result += "03";
                }
            }
            return result;
        }

        public void SetBuckle(bool b)
        {
            foreach (var m in Items)
            {
                if (m is MedicineNHI || m is MedicineOTC || m is MedicineSpecialMaterial)
                    m.IsBuckle = b;
            }
        }
    }
}
