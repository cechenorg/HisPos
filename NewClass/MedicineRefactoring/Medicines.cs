using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using His_Pos.NewClass.CooperativeInstitution;
using His_Pos.NewClass.Product.Medicine;
using His_Pos.NewClass.Product.Medicine.Position;
using His_Pos.NewClass.Product.Medicine.Usage;
using His_Pos.Properties;
using His_Pos.Service;
using VM = His_Pos.ChromeTabViewModel.ViewModelMainWindow;

namespace His_Pos.NewClass.MedicineRefactoring
{
    public class Medicines:ObservableCollection<Medicine>
    {
        public Medicines()
        {

        }

        public void GetDataByOrthopedicsPrescription(IEnumerable<Item> medicineOrderItem,string wareHouseID,bool isBuckle)
        {
            Clear();
            var idList = medicineOrderItem.Select(m => m.Id).ToList();
            var table = MedicineDb.GetMedicinesBySearchIds(idList, wareHouseID);
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
                if (string.IsNullOrEmpty(item.Id))
                {
                    var med = new MedicineOTC(item);
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
                else
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
        }
        public void GetDataByCooperativePrescription(List<Cooperative.XmlOfPrescription.CooperativePrescription.Item> medicineOrderItem,bool isBuckle,string wareHouseID)
        {
            var idList = medicineOrderItem.Select(m => m.Id).ToList();
            var table = MedicineDb.GetMedicinesBySearchIds(idList, wareHouseID);
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
                if (string.IsNullOrEmpty(item.Id))
                {
                    var med = new MedicineOTC(item);
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
                            var price = Convert.ToDouble(item.Price);
                            med.TotalPrice = med.Amount * (price < 0 ? price * -1 : price);
                            break;
                        case "-":
                            med.TotalPrice = 0;
                            break;
                        case "*":
                            price = Convert.ToDouble(item.Price);
                            med.TotalPrice = price < 0 ? price * -1 : price;
                            break;
                    }
                    Add(med);
                }
                else
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
        }

        public void GetDataByPrescriptionId(int id,string wareHouseID)
        {
            var table = MedicineDb.GetDataByPrescriptionId(id);
            CreateMedicines(table,wareHouseID);
        }

        public void GetDataByReserveId(int id,string wareHouseID)
        {
            var table = MedicineDb.GetDataByReserveId(id);
            CreateMedicines(table,wareHouseID);
        }

        private void CreateMedicines(DataTable table,string wareHouseID)
        {
            var idList = new List<string>();
            foreach (DataRow r in table.Rows)
            {
                idList.Add(r.Field<string>("Pro_ID"));
            }
            var medicinesTable = MedicineDb.GetMedicinesBySearchIds(idList, wareHouseID);
            var medicines = new Medicines();
            for (var i = 0; i < medicinesTable.Rows.Count; i++)
            {
                Medicine medicine;
                switch (medicinesTable.Rows[i].Field<int>("DataType"))
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
            foreach (DataRow r in table.Rows)
            {
                var med = medicines.Single(m => m.ID.Equals(r.Field<string>("Pro_ID")));
                med.Usage = new Usage();
                med.Position = new Position();
                med.Dosage = r.Field<double?>("Dosage");
                med.UsageName = r.Field<string>("Usage");
                med.PositionID = r.Field<string>("Position");
                med.Days = r.Field<int?>("MedicineDays");
                med.PaySelf = r.Field<bool>("PaySelf");
                med.IsBuckle = r.Field<bool>("IsBuckle");
                med.Amount = r.Field<double>("TotalAmount");
                med.BuckleAmount = NewFunction.CheckDataRowContainsColumn(r, "BuckleAmount") ? r.Field<double>("BuckleAmount") : med.Amount;
                med.Price = NewFunction.CheckDataRowContainsColumn(r, "PaySelfValue") ? (double)r.Field<decimal>("PaySelfValue") : 0;
                if (med.PaySelf)
                {
                    med.TotalPrice = r.Field<int>("Point");
                }
                Add(med);
            }
        }

        public int CountMedicinePoint()
        {
            var notPaySelfNhiMedicines = GetNotPaySelfNhiMedicines();
            if (!notPaySelfNhiMedicines.Any()) return 0;
            var medicinePoint = notPaySelfNhiMedicines.Sum(m => m.NHIPrice * m.Amount);
            return (int)Math.Round(Convert.ToDouble(medicinePoint.ToString()), 0, MidpointRounding.AwayFromZero);
        }

        private List<Medicine> GetNotPaySelfNhiMedicines()
        {
            return this.Where(m => m is MedicineNHI && !m.PaySelf).ToList();
        }

        public int CountSpecialMedicinePoint()
        {
            var notPaySelfSpecialMaterials = GetNotPaySelfSpecialMaterials();
            if (!notPaySelfSpecialMaterials.Any()) return 0;
            var specialMaterial = notPaySelfSpecialMaterials.Sum(m => m.NHIPrice * m.Amount) * 1.05;
            return (int)Math.Round(Convert.ToDouble(specialMaterial.ToString()), 0, MidpointRounding.AwayFromZero);
        }

        private List<Medicine> GetNotPaySelfSpecialMaterials()
        {
            return this.Where(m => !m.PaySelf && m is MedicineSpecialMaterial).ToList();
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

        public void AddMedicine(string medicineId,bool paySelf,int? selectedMedicinesIndex,string wareHouseId)
        {
            var table = MedicineDb.GetMedicinesBySearchId(medicineId, wareHouseId);
            Medicine medicine = null;
            foreach (DataRow r in table.Rows)
            {
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
            }
            Debug.Assert(medicine != null, nameof(medicine) + " != null");
            medicine.PaySelf = paySelf;
            if (medicine.ID.EndsWith("00") ||
                medicine.ID.EndsWith("G0"))
                medicine.PositionID = "PO";
            if (selectedMedicinesIndex != null)
            {
                if (selectedMedicinesIndex > 0)
                    medicine.CopyPrevious(Items[(int)selectedMedicinesIndex - 1]);
                Items[(int)selectedMedicinesIndex] = medicine;
            }
            else
            {
                if (Count > 0)
                    medicine.CopyPrevious(Items[Count - 1]);
                Add(medicine);
            }
        }
    }
}
