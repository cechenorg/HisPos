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
using CooperativeMedicine = His_Pos.NewClass.Cooperative.XmlOfPrescription.CooperativePrescription.Item;
using OrthopedicsMedicine = His_Pos.NewClass.CooperativeInstitution.Item;

namespace His_Pos.NewClass.MedicineRefactoring
{
    public class Medicines:ObservableCollection<Medicine>
    {
        public Medicines()
        {

        }

        #region OrthopedicsFunctions
        public void GetDataByOrthopedicsPrescription(List<OrthopedicsMedicine> medicineOrderItem, string wareHouseID, bool isBuckle)
        {
            Clear();
            var tempList = CreateTempMedicinesByOrthopedics(medicineOrderItem, wareHouseID);
            foreach (var order in medicineOrderItem)
            {
                if (Items.Count(m => m.ID.Equals(order.Id)) > 0) continue;
                foreach (var item in tempList.Where(m => m.ID.Equals(order.Id)))
                    Items.Add(item);
            }
            SetBuckle(isBuckle);
        }
        private Medicines CreateTempMedicinesByOrthopedics(List<OrthopedicsMedicine> medicineOrderItem, string wareHouseID)
        {
            var idList = medicineOrderItem.Select(m => m.Id).Distinct().ToList();
            var table = MedicineDb.GetMedicinesBySearchIds(idList, wareHouseID);
            var tempList = new Medicines();
            AddOrthopedicsMedicineByDataTable(tempList, table, medicineOrderItem);
            AddOrthopedicsMedicineNotFound(tempList, medicineOrderItem);
            AddOrthopedicsIDEmptyMedicine(tempList, medicineOrderItem);
            return tempList;
        }
        private void AddOrthopedicsMedicineByDataTable(Medicines tempList, DataTable table, List<OrthopedicsMedicine> medicineOrderItem)
        {
            for (var i = 0; i < table.Rows.Count; i++)
            {
                foreach (var item in medicineOrderItem.Where(m => m.Id.Equals(table.Rows[i].Field<string>("Pro_ID"))))
                {
                    Medicine medicine;
                    switch (table.Rows[i].Field<int>("DataType"))
                    {
                        case 1:
                            medicine = new MedicineNHI(table.Rows[i]);
                            break;
                        case 3:
                            medicine = new MedicineSpecialMaterial(table.Rows[i]);
                            break;
                        default:
                            medicine = new MedicineOTC(table.Rows[i]);
                            break;
                    }
                    medicine.SetValueByOrthopedicsMedicine(item);
                    tempList.Add(medicine);
                }
            }
        }
        private void AddOrthopedicsMedicineNotFound(Medicines tempList, List<OrthopedicsMedicine> medicineOrderItem)
        {
            foreach (var order in medicineOrderItem)
            {
                if (tempList.Count(m => m.ID.Equals(order.Id)) > 0) continue;
                var medicine = new MedicineOTC
                {
                    ID = order.Id,
                    ChineseName = order.Desc
                };
                MedicineDb.InsertCooperativeMedicineOTC(medicine.ID, medicine.ChineseName);//新增合作診所MedicineOtc
                tempList.Add(medicine);
            }
        }
        private void AddOrthopedicsIDEmptyMedicine(Medicines tempList, List<OrthopedicsMedicine> medicineOrderItem)
        {
            foreach (var order in medicineOrderItem.Where(m => string.IsNullOrEmpty(m.Id)))
            {
                var medicine = new MedicineOTC(order);
                tempList.Add(medicine);
            }
        }
        #endregion

        #region CooperativeFunctions
        public void GetDataByCooperativePrescription(List<CooperativeMedicine> medicineOrderItem, string wareHouseID, bool isBuckle)
        {
            Clear();
            var tempList = CreateTempMedicinesByCooperative(medicineOrderItem, wareHouseID);
            foreach (var order in medicineOrderItem)
            {
                if (Items.Count(m => m.ID.Equals(order.Id)) > 0) continue;
                foreach (var item in tempList.Where(m => m.ID.Equals(order.Id)))
                    Items.Add(item);
            }
            SetBuckle(isBuckle);
        }
        private Medicines CreateTempMedicinesByCooperative(List<CooperativeMedicine> medicineOrderItem, string wareHouseID)
        {
            var idList = medicineOrderItem.Select(m => m.Id).Distinct().ToList();
            var table = MedicineDb.GetMedicinesBySearchIds(idList, wareHouseID);
            var tempList = new Medicines();
            AddCooperativeMedicineByDataTable(tempList, table, medicineOrderItem);
            AddCooperativeMedicineNotFound(tempList, medicineOrderItem);
            AddCooperativeIDEmptyMedicine(tempList, medicineOrderItem);
            return tempList;
        }
        private void AddCooperativeMedicineByDataTable(Medicines tempList, DataTable table, List<CooperativeMedicine> medicineOrderItem)
        {
            for (var i = 0; i < table.Rows.Count; i++)
            {
                foreach (var item in medicineOrderItem.Where(m => m.Id.Equals(table.Rows[i].Field<string>("Pro_ID"))))
                {
                    Medicine medicine;
                    switch (table.Rows[i].Field<int>("DataType"))
                    {
                        case 1:
                            medicine = new MedicineNHI(table.Rows[i]);
                            break;
                        case 3:
                            medicine = new MedicineSpecialMaterial(table.Rows[i]);
                            break;
                        default:
                            medicine = new MedicineOTC(table.Rows[i]);
                            break;
                    }
                    medicine.SetValueByCooperativeMedicine(item);
                    tempList.Add(medicine);
                }
            }
        }
        private void AddCooperativeMedicineNotFound(Medicines tempList, List<CooperativeMedicine> medicineOrderItem)
        {
            foreach (var order in medicineOrderItem)
            {
                if (tempList.Count(m => m.ID.Equals(order.Id)) > 0) continue;
                var medicine = new MedicineOTC
                {
                    ID = order.Id,
                    ChineseName = order.Desc
                };
                MedicineDb.InsertCooperativeMedicineOTC(medicine.ID, medicine.ChineseName);//新增合作診所MedicineOtc
                tempList.Add(medicine);
            }
        }
        private void AddCooperativeIDEmptyMedicine(Medicines tempList, List<CooperativeMedicine> medicineOrderItem)
        {
            foreach (var order in medicineOrderItem.Where(m => string.IsNullOrEmpty(m.Id)))
            {
                var medicine = new MedicineOTC(order);
                tempList.Add(medicine);
            }
        }

        #endregion

        private void SetBuckle(bool isBuckle)
        {
            foreach (var m in Items)
            {
                m.IsBuckle = isBuckle;
                m.BuckleAmount = isBuckle ? m.Amount : 0;
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

        public string Check()
        {
            var errorMsg = string.Empty;
            errorMsg += CheckIDEmpty();
            errorMsg += CheckAmountZero();
            return errorMsg;
        }
        private string CheckIDEmpty()
        {
            var emptyMedicine = string.Empty;
            var emptyList = (from m in Items where string.IsNullOrEmpty(m.ID) select "藥品:" + m.FullName + "代碼不得為空。\r\n").ToList();
            //var sameList = (from m in Items where string.IsNullOrEmpty(m.ID) select "藥品:" + m.FullName + "代碼不得為空。\n").ToList();
            return emptyList.Count <= 0 ? emptyMedicine : emptyList.Aggregate(emptyMedicine, (current, s) => current + s);
        }

        private string CheckAmountZero()
        {
            if (!Items.Any())
                return Resources.MedicineEmpty;
            return Items.Count(m => m.Amount == 0) == 0 ? string.Empty : 
                Items.Where(m => !(m is MedicineVirtual) && m.Amount == 0).Aggregate(string.Empty, (current, m) => current + ("藥品:" + m.FullName + "總量不可為0\r\n"));
        }
    }
}
