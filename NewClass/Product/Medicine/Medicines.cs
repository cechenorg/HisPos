using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Windows.Documents;

namespace His_Pos.NewClass.Product.Medicine
{
    public class Medicines:ObservableCollection<Medicine>
    {
        public Medicines() { }

        public int CountMedicinePoint()
        {
            var medicinePoint = this.Where(m => m is MedicineNHI && !m.PaySelf).Sum(m => m.NHIPrice * m.Amount);
            return (int)Math.Round(medicinePoint, 0, MidpointRounding.AwayFromZero);
        }

        public int CountSpecialMedicinePoint()
        {
            var specialMaterial = this.Where(m => m is MedicineSpecialMaterial && !m.PaySelf).Sum(m => m.NHIPrice * m.Amount * 1.05);
            return (int)Math.Round(specialMaterial, 0, MidpointRounding.AwayFromZero);
        }

        public int CountSelfPay()
        {
            var selfPay = this.Where(m => m.PaySelf).Sum(m=>m.TotalPrice);
            return (int)Math.Ceiling(selfPay);
        }
        public void GetDataByPrescriptionId(int preId) {
            DataTable table = MedicineDb.GetDataByPrescriptionId(preId);
            foreach (DataRow r in table.Rows) {
                var med = new Medicine(r);
                med.Dosage = r.Field<double?>("Dosage");
                med.UsageName = r.Field<string>("Usage");
                med.PositionID = r.Field<string>("Position");
                med.Days = r.Field<int?>("MedicineDays");
                med.PaySelf = r.Field<bool>("PaySelf");
                med.IsBuckle = r.Field<bool>("IsBuckle");
                med.Amount = r.Field<double>("TotalAmount");
                med.BuckleAmount = r.Field<double>("BuckleAmount");
                med.Price = (double)r.Field<decimal>("PaySelfValue");
                if (med.PaySelf)
                {
                    med.TotalPrice = r.Field<int>("Point");
                }
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
                med.PositionID = r.Field<string>("Position");
                med.Days = r.Field<int>("MedicineDays");
                med.PaySelf = r.Field<bool>("PaySelf");
                med.IsBuckle = r.Field<bool>("IsBuckle");
                med.Amount = r.Field<double>("TotalAmount");
                if (med.PaySelf)
                {
                    med.TotalPrice = r.Field<int>("Point");
                }
                Add(med);
            }
        }
        public string CreateMedicalData(string dateTime)
        {
            foreach (var m in Items)
            {
                if (!(m is MedicineNHI) || m.PaySelf) continue;
                if (string.IsNullOrEmpty(m.PositionID))
                    m.PositionID = "XX";
                if (string.IsNullOrEmpty(m.UsageName))
                    m.UsageName = "ASORDER";
            }
            var medList = this.Where(m => (m is MedicineNHI || m is MedicineSpecialMaterial || m is MedicineVirtual) && !m.PaySelf).ToList();
            var result = string.Empty;
            foreach (var med in medList)
            {
                result += dateTime;
                if (med is MedicineNHI)
                {
                    result += "1";
                    result += med.ID.PadLeft(12, ' ');
                    result += med.PositionID.PadLeft(6, ' ');
                    result += med.UsageName.PadLeft(18, ' ');
                    result += med.Days.ToString().PadLeft(2, ' ');
                    result += med.Amount.ToString().PadLeft(7, ' ');
                    result += "01";
                }
                else if (med is MedicineVirtual)
                {
                    result += "G";
                    result += med.ID.PadLeft(12, ' ');
                    result += string.Empty.PadLeft(6, ' ');
                    result += string.Empty.PadLeft(18, ' ');
                    result += string.Empty.PadLeft(2, ' ');
                    result += med.Amount.ToString().PadLeft(7, ' ');
                    result += string.Empty.PadLeft(2, ' ');
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
                switch (m)
                {
                    case MedicineNHI _:
                    case MedicineOTC _:
                    case MedicineSpecialMaterial _:
                    {
                        if (!b)
                            m.BuckleAmount = 0;
                        else
                        {
                            m.BuckleAmount = m.Amount;
                        }
                        m.IsBuckle = b;
                        break;
                    }
                    case MedicineVirtual _:
                        m.BuckleAmount = 0;
                        m.IsBuckle = false;
                        break;
                }
            }
        }

        public void GetMedicineBySet(MedicineSet.MedicineSet currentSet)
        {
            var medicineIDList = new List<string>();
            foreach (var item in currentSet.MedicineSetItems)
            {
                medicineIDList.Add(item.ID);
            }
            var table = MedicineDb.GetMedicinesBySearchIds(medicineIDList);
            for (var i = 0; i < table.Rows.Count; i++)
            {
                foreach (DataRow r in table.Rows)
                {
                    var medicine = new Medicine();
                    switch (r.Field<int>("DataType"))
                    {
                        case 1:
                            medicine = new MedicineNHI(r);
                            break;
                        case 2:
                            medicine = new MedicineOTC(r);
                            break;
                        case 3:
                            medicine = new MedicineSpecialMaterial(r);
                            break;
                    }
                    medicine.Dosage = currentSet.MedicineSetItems[i].Dosage;
                    medicine.UsageName = currentSet.MedicineSetItems[i].UsageName;
                    medicine.PositionID = currentSet.MedicineSetItems[i].PositionID;
                    medicine.Days = currentSet.MedicineSetItems[i].Days;
                    medicine.Amount = currentSet.MedicineSetItems[i].Amount;
                    medicine.PaySelf = currentSet.MedicineSetItems[i].PaySelf;
                    if (medicine.PaySelf)
                        medicine.Price = currentSet.MedicineSetItems[i].Price;
                    Add(medicine);
                }
            }
        }
    }
}
