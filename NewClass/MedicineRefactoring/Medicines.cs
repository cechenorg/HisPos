using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using His_Pos.NewClass.CooperativeInstitution;
using His_Pos.NewClass.Product.Medicine;
using His_Pos.NewClass.Product.Medicine.Position;
using His_Pos.NewClass.Product.Medicine.Usage;
using His_Pos.Properties;

namespace His_Pos.NewClass.MedicineRefactoring
{
    public class Medicines:ObservableCollection<Medicine>
    {
        public Medicines()
        {

        }

        public void GetDataByOrthopedicsPrescription(IEnumerable<Item> medicineOrderItem)
        {
            var idList = medicineOrderItem.Select(m => m.Id).ToList();
            var table = MedicineDb.GetMedicinesBySearchIds(idList);
            var medicines = new Medicines();
            for (var i = 0; i < table.Rows.Count; i++)
            {
                Medicine medicine;
                switch (table.Rows[i].Field<int>("DataType"))
                {
                    case 1:
                        medicine = new MedicineNHI(table.Rows[i]);
                        medicines.Add(medicine);
                        break;
                    case 2:
                        medicine = new MedicineOTC(table.Rows[i]);
                        medicines.Add(medicine);
                        break;
                    case 3:
                        medicine = new MedicineSpecialMaterial(table.Rows[i]);
                        medicines.Add(medicine);
                        break;
                }
            }

            foreach (var item in medicineOrderItem)
            {
                var med = medicines.Single(m => m.ID.Equals(item.Id));
                med.Usage = new Usage();
                med.Position = new Position();
                med.UsageName = item.Freq;
                med.PositionID = item.Way;
                med.Amount = Convert.ToDouble(item.Total_dose);
                med.Dosage = Convert.ToDouble(item.Divided_dose);
                med.Days = Convert.ToInt32(item.Days);
                med.PaySelf = item.Remark == "-" || item.Remark == "*";
                med.IsBuckle = false;
                switch (item.Remark)
                {
                    case "":
                        med.TotalPrice = med.Amount * Convert.ToDouble(item.Price);
                        break;
                    case "-":
                        med.TotalPrice = 0;
                        break;
                    case "*":
                        med.TotalPrice = Convert.ToDouble(item.Price);
                        break;
                    default:
                        med.TotalPrice = med.Amount * Convert.ToDouble(item.Price);
                        break;
                }
                Add(med);
            }
        }
        public void GetDataByCooperativePrescription(List<Cooperative.XmlOfPrescription.CooperativePrescription.Item> medicineOrderItem,bool isBuckle)
        {
            var idList = medicineOrderItem.Select(m => m.Id).ToList();
            var table = MedicineDb.GetMedicinesBySearchIds(idList);
            var medicines = new Medicines();
            for (var i = 0; i < table.Rows.Count; i++)
            {
                Medicine medicine;
                switch (table.Rows[i].Field<int>("DataType"))
                {
                    case 1:
                        medicine = new MedicineNHI(table.Rows[i]);
                        medicines.Add(medicine);
                        break;
                    case 2:
                        medicine = new MedicineOTC(table.Rows[i]);
                        medicines.Add(medicine);
                        break;
                    case 3:
                        medicine = new MedicineSpecialMaterial(table.Rows[i]);
                        medicines.Add(medicine);
                        break;
                }
            }
            foreach (var item in medicineOrderItem)
            {
                var med = medicines.Single(m => m.ID.Equals(item.Id));
                med.Usage = new Usage();
                med.Position = new Position();
                med.UsageName = item.Freq;
                med.PositionID = item.Way;
                med.Amount = Convert.ToDouble(item.Total_dose);
                med.Dosage = Convert.ToDouble(item.Divided_dose);
                med.Days = Convert.ToInt32(item.Days);
                med.PaySelf = !string.IsNullOrEmpty(item.Remark);
                med.IsBuckle = isBuckle;
                switch (item.Remark)
                {
                    case "":
                        med.TotalPrice = med.Amount * Convert.ToDouble(item.Price);
                        break;
                    case "-":
                        med.TotalPrice = 0;
                        break;
                    case "*":
                        med.TotalPrice = Convert.ToDouble(item.Price);
                        break;
                }
                Add(med);
            }
        }

        public int CountMedicinePoint()
        {
            var medicinePoint = this.Where(m => m is MedicineNHI && !m.PaySelf).Sum(m => m.NHIPrice * m.Amount);
            return (int)Math.Round(Convert.ToDouble(medicinePoint.ToString()), 0, MidpointRounding.AwayFromZero);
        }

        public int CountSpecialMedicinePoint()
        {
            var specialMaterial = this.Where(m => m is MedicineSpecialMaterial && !m.PaySelf).Sum(m => m.NHIPrice * m.Amount * 1.05);
            return (int)Math.Round(Convert.ToDouble(specialMaterial.ToString()), 0, MidpointRounding.AwayFromZero);
        }

        public int CountSelfPay()
        {
            var selfPay = this.Where(m => m.PaySelf).Sum(m => m.TotalPrice);
            return (int)Math.Ceiling(selfPay);
        }

        public IEnumerable<Medicine> GetDeclare()
        {
            return this.Where(m => (m is MedicineNHI || m is MedicineSpecialMaterial || m is MedicineVirtual) && !m.PaySelf);
        }

        public int CountMedicineDays()
        {
            if (this.Count(m => (m is MedicineNHI || m is MedicineSpecialMaterial) && !m.PaySelf && m.Days != null) > 0)
                return (int)this.Where(m => (m is MedicineNHI || m is MedicineSpecialMaterial) && !m.PaySelf).Max(m => m.Days);//計算最大給藥日份
            return 0;
        }

        public int CountOralLiquidAgent()
        {
            return this.Count(m => m is MedicineNHI med && !string.IsNullOrEmpty(med.Note) && med.Note.Contains(Resources.口服液劑));
        }
    }
}
