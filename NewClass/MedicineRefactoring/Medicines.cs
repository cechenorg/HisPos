using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using His_Pos.Class;
using His_Pos.FunctionWindow;
using His_Pos.NewClass.Product.Medicine;
using His_Pos.NewClass.Product.Medicine.MedicineSet;
using His_Pos.NewClass.Product.Medicine.Position;
using His_Pos.NewClass.Product.Medicine.Usage;
using His_Pos.Properties;
using His_Pos.Service;
using CooperativeMedicine = His_Pos.NewClass.Cooperative.XmlOfPrescription.CooperativePrescription.Item;
using OrthopedicsMedicine = His_Pos.NewClass.CooperativeInstitution.Item;
// ReSharper disable TooManyDeclarations

namespace His_Pos.NewClass.MedicineRefactoring
{
    public class Medicines:ObservableCollection<Medicine>
    {
        public Medicines()
        {

        }

        #region OrthopedicsFunctions
        [SuppressMessage("ReSharper", "TooManyArguments")]
        public void GetDataByOrthopedicsPrescription(List<OrthopedicsMedicine> medicineOrderItem, string wareHouseID, bool isBuckle, DateTime? adjustDate)
        {
            Clear();
            var tempList = CreateTempMedicinesByOrthopedics(medicineOrderItem, wareHouseID, adjustDate);
            foreach (var order in medicineOrderItem)
            {
                if (Items.Count(m => m.ID.Equals(order.Id)) > 0) continue;
                foreach (var item in tempList.Where(m => m.ID.Equals(order.Id)))
                    Items.Add(item);
            }
            SetBuckleAmount(isBuckle);
        }
        private Medicines CreateTempMedicinesByOrthopedics(List<OrthopedicsMedicine> medicineOrderItem, string wareHouseID,DateTime? adjustDate)
        {
            var idList = medicineOrderItem.Select(m => m.Id).ToList();
            var table = MedicineDb.GetMedicinesBySearchIds(idList, wareHouseID, adjustDate);
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
                if (string.IsNullOrEmpty(order.Id)) continue;
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
        [SuppressMessage("ReSharper", "TooManyArguments")]
        public void GetDataByCooperativePrescription(List<CooperativeMedicine> medicineOrderItem, string wareHouseID, bool isBuckle, DateTime? adjustDate)
        {
            Clear();
            var tempList = CreateTempMedicinesByCooperative(medicineOrderItem, wareHouseID, adjustDate);
            foreach (var order in medicineOrderItem)
            {
                if (Items.Count(m => m.ID.Equals(order.Id)) > 0) continue;
                foreach (var item in tempList.Where(m => m.ID.Equals(order.Id)))
                    Items.Add(item);
            }
            SetBuckleAmount(isBuckle);
        }
        private Medicines CreateTempMedicinesByCooperative(List<CooperativeMedicine> medicineOrderItem, string wareHouseID, DateTime? adjustDate)
        {
            var idList = medicineOrderItem.Select(m => m.Id).ToList();
            var table = MedicineDb.GetMedicinesBySearchIds(idList, wareHouseID, adjustDate);
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
                if(string.IsNullOrEmpty(order.Id)) continue;
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

        public void GetDataByPrescriptionId(int id,string wareHouseID, DateTime? adjustDate)
        {
            var table = MedicineDb.GetDataByPrescriptionId(id);
            CreateMedicines(table, wareHouseID, adjustDate);
        }

        public void GetDataByReserveId(int id,string wareHouseID, DateTime? adjustDate)
        {
            var table = MedicineDb.GetDataByReserveId(id);
            CreateMedicines(table, wareHouseID, adjustDate);
        }

        private void CreateMedicines(DataTable table,string wareHouseID,DateTime? adjustDate)
        {
            var idList = new List<string>();
            foreach (DataRow r in table.Rows)
            {
                if(!idList.Contains(r.Field<string>("Pro_ID")))
                    idList.Add(r.Field<string>("Pro_ID"));
            }
            var medicinesTable = MedicineDb.GetMedicinesBySearchIds(idList, wareHouseID, adjustDate);
            var medicineRows = new List<DataRow>();
            foreach (DataRow r in medicinesTable.Rows)
            {
                medicineRows.Add(r);
            }
            foreach (DataRow r in table.Rows)
            {
                Medicine medicine;
                var medicineRow = medicineRows.SingleOrDefault(row => row.Field<string>("Pro_ID").Equals(r.Field<string>("Pro_ID")));
                if(medicineRow is null)
                {
                    switch (r.Field<string>("Pro_ID"))
                    {
                        case "R001":
                        case "R002":
                        case "R003":
                        case "R004":
                            medicine = new MedicineVirtual(r.Field<string>("Pro_ID"));
                            Add(medicine);
                            continue;
                    }
                }
                switch (medicineRow.Field<int>("DataType"))
                {
                    case 1:
                        medicine = new MedicineNHI(medicineRow);
                        break;
                    case 2:
                        medicine = new MedicineOTC(medicineRow);
                        break;
                    case 3:
                        medicine = new MedicineSpecialMaterial(medicineRow);
                        break;
                    default:
                        continue;
                }
                medicine.SetValueByDataRow(r);
                Add(medicine);
            }
        }

        public int CountMedicinePoint()
        {
            var notPaySelfNhiMedicines = GetNotPaySelfNhiMedicines();
            if (!notPaySelfNhiMedicines.Any()) return 0;
            var medicinePoint = notPaySelfNhiMedicines.Sum(m => m.NHIPrice * m.Amount);
            return (int)Math.Round(Convert.ToDouble(medicinePoint.ToString(CultureInfo.InvariantCulture)), 0, MidpointRounding.AwayFromZero);
        }

        private List<Medicine> GetNotPaySelfNhiMedicines()
        {
            return Items.Where(m => m is MedicineNHI && !m.PaySelf).ToList();
        }

        public int CountSpecialMedicinePoint()
        {
            var notPaySelfSpecialMaterials = GetNotPaySelfSpecialMaterials();
            if (!notPaySelfSpecialMaterials.Any()) return 0;
            var specialMaterial = notPaySelfSpecialMaterials.Sum(m => m.NHIPrice * m.Amount) * 1.05;
            return (int)Math.Round(Convert.ToDouble(specialMaterial.ToString(CultureInfo.InvariantCulture)), 0, MidpointRounding.AwayFromZero);
        }

        private List<Medicine> GetNotPaySelfSpecialMaterials()
        {
            return Items.Where(m => !m.PaySelf && m is MedicineSpecialMaterial).ToList();
        }

        public int CountSelfPay()
        {
            var selfPay = Items.Where(m => m.PaySelf).Sum(m => m.TotalPrice);
            return (int)Math.Ceiling(selfPay);
        }

        public IEnumerable<Medicine> GetDeclare()
        {
            return Items.Where(m => (m is MedicineNHI || m is MedicineSpecialMaterial || m is MedicineVirtual) && !m.PaySelf);
        }

        public int CountMedicineDays()
        {
            if (Items.Count(m => m.CanCountMedicineDays()) > 0)
                return (int)Items.Where(m => m.CanCountMedicineDays()).Max(m => m.Days);//計算最大給藥日份
            return 0;
        }

        public int CountOralLiquidAgent()
        {
            return Items.Count(m => m is MedicineNHI med && !string.IsNullOrEmpty(med.Note) && med.Note.Contains(Resources.口服液劑));
        }

        public void AddMedicine(string medicineId,bool paySelf,int? selectedMedicinesIndex,string wareHouseId,DateTime? adjustDate)
        {
            var table = MedicineDb.GetMedicinesBySearchId(medicineId, wareHouseId, adjustDate);
            var medicine = AddMedicineByDataType(table);
            Debug.Assert(medicine != null, nameof(medicine) + " != null");
            medicine.PaySelf = paySelf;
            if (medicine.ID.EndsWith("00") || medicine.ID.EndsWith("G0"))
                medicine.PositionID = "PO";
            medicine.IsBuckle = !string.IsNullOrEmpty(wareHouseId);
            if (selectedMedicinesIndex != null)
            {
                if (selectedMedicinesIndex > 0)
                    medicine.CopyPrevious(this[(int)selectedMedicinesIndex]);
                medicine.BuckleAmount = medicine.IsBuckle ? medicine.Amount : 0;
                this[(int)selectedMedicinesIndex] = medicine;
            }
            else
            {
                if (Count > 0)
                    medicine.CopyPrevious(this[Count - 1]);
                medicine.BuckleAmount = medicine.IsBuckle ? medicine.Amount : 0;
                Add(medicine);
            }
        }

        private Medicine AddMedicineByDataType(DataTable table)
        {
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
            return medicine;
        }

        public string Check()
        {
            var errorMsg = string.Empty;
            errorMsg += CheckIDEmpty();
            errorMsg += CheckAmountZero();
            return errorMsg;
        }

        [SuppressMessage("ReSharper", "ComplexConditionExpression")]
        private string CheckIDEmpty()
        {
            var emptyMedicine = string.Empty;
            var emptyList = (from m in Items where string.IsNullOrEmpty(m.ID) select "藥品:" + m.FullName + "代碼不得為空。\r\n").ToList();
            return emptyList.Count <= 0 ? emptyMedicine : emptyList.Aggregate(emptyMedicine, (current, s) => current + s);
        }

        [SuppressMessage("ReSharper", "CompareOfFloatsByEqualityOperator")]
        private string CheckAmountZero()
        {
            if (!Items.Any())
                return Resources.MedicineEmpty;
            return Items.Count(m => m.Amount == 0) == 0 ? string.Empty : 
                Items.Where(m => !(m is MedicineVirtual) && m.Amount == 0).Aggregate(string.Empty, (current, m) => current + ("藥品:" + m.FullName + "總量不可為0\r\n"));
        }

        public void Update(bool buckle, string wareHouseId,DateTime? adjustDate)
        {
            var idList = CreateIdList();
            SetBuckleAmount(buckle);
            MainWindow.ServerConnection.OpenConnection();
            var table = MedicineDb.GetMedicinesBySearchIds(idList, wareHouseId, adjustDate);
            MainWindow.ServerConnection.CloseConnection();
            foreach (DataRow r in table.Rows)
            {
                var medList = Items.Where(m => m.ID.Equals(r.Field<string>("Pro_ID")));
                foreach (var m in medList)
                {
                    m.NHIPrice = (double)r.Field<decimal>("Med_Price");
                    m.Inventory = r.Field<double?>("Inv_Inventory") is null ? 0 : r.Field<double>("Inv_Inventory");
                }
            }
        }

        private List<string> CreateIdList()
        {
            var idList = new List<string>();
            foreach (var m in Items)
            {
                switch (m)
                {
                    case MedicineNHI _:
                    case MedicineOTC _:
                    case MedicineSpecialMaterial _:
                    {
                        if (!idList.Contains(m.ID))
                            idList.Add(m.ID);
                        break;
                    }
                }
            }
            return idList;
        }

        [SuppressMessage("ReSharper", "FlagArgument")]
        private void SetBuckleAmount(bool buckle)
        {
            foreach (var m in Items)
            {
                switch (m)
                {
                    case MedicineNHI _:
                    case MedicineOTC _:
                    case MedicineSpecialMaterial _:
                    {
                        if (!buckle)
                            m.BuckleAmount = 0;
                        else
                            m.BuckleAmount = m.Amount;
                        m.IsBuckle = buckle;
                        break;
                    }
                    case MedicineVirtual _:
                        m.BuckleAmount = 0;
                        m.IsBuckle = false;
                        break;
                }
            }
        }

        public string CreateMedicalData(string dateTime)
        {
            SetPosition();
            var medList = Items.Where(CheckDeclareMedicine).ToList();
            var result = string.Empty;
            foreach (var med in medList)
            {
                result += dateTime;
                switch (med)
                {
                    case MedicineNHI _:
                        CreateMedicineNHIData(ref result,med);
                        break;
                    case MedicineVirtual _:
                        CreateMedicineVirtualData(ref result,med);
                        break;
                    default:
                        CreateMedicineSpecialMaterialData(ref result, med);
                        break;
                }
            }
            return result;
        }

        private bool CheckDeclareMedicine(Medicine medicine)
        {
            return (medicine is MedicineNHI || medicine is MedicineSpecialMaterial || medicine is MedicineVirtual) && !medicine.PaySelf;
        }

        private void CreateMedicineNHIData(ref string result,Medicine med)
        {
            result += "1";
            result += med.ID.PadLeft(12, ' ');
            result += med.PositionID.PadLeft(6, ' ');
            result += med.UsageName.PadLeft(18, ' ');
            result += med.Days.ToString().PadLeft(2, ' ');
            result += med.Amount.ToString(CultureInfo.InvariantCulture).PadLeft(7, ' ');
            result += "01";
        }

        private void CreateMedicineVirtualData(ref string result, Medicine med)
        {
            result += "G";
            result += med.ID.PadLeft(12, ' ');
            result += string.Empty.PadLeft(6, ' ');
            result += string.Empty.PadLeft(18, ' ');
            result += string.Empty.PadLeft(2, ' ');
            result += med.Amount.ToString(CultureInfo.InvariantCulture).PadLeft(7, ' ');
            result += string.Empty.PadLeft(2, ' ');
        }

        private void CreateMedicineSpecialMaterialData(ref string result, Medicine med)
        {
            result += "4";
            result += med.ID.Substring(0, 12).PadLeft(12, ' ');
            if (med.Days != null)
                result += med.Days.ToString().PadLeft(2, ' ');
            else
                result += string.Empty.PadLeft(2, ' ');
            result += string.Empty.PadLeft(6, ' ');
            result += string.Empty.PadLeft(18, ' ');
            if (med.Days != null)
                result += med.Days.ToString().PadLeft(2, ' ');
            else
                result += string.Empty.PadLeft(2, ' ');
            result += med.Amount.ToString(CultureInfo.InvariantCulture).PadLeft(7, ' ');
            result += "03";
        }

        private void SetPosition()
        {
            foreach (var m in Items)
            {
                if (!(m is MedicineNHI) || m.PaySelf) continue;
                if (string.IsNullOrEmpty(m.PositionID))
                    m.PositionID = "XX";
                if (string.IsNullOrEmpty(m.UsageName))
                    m.UsageName = "ASORDER";
            }
        }

        public void GetMedicineBySet(MedicineSet currentSet, string wareHouseID, DateTime? adjustDate)
        {
            Clear();
            var medicineIDList = CreateIdListByMedicineSet(currentSet);
            var table = MedicineDb.GetMedicinesBySearchIds(medicineIDList, wareHouseID, adjustDate);
            var tempList = GetMedicinesByCurrentSetAndTable(currentSet, table);
            foreach (var setItem in currentSet.MedicineSetItems)
            {
                if (Items.Count(m => m.ID.Equals(setItem.ID)) > 0) continue;
                var medList = tempList.Where(m => m.ID.Equals(setItem.ID));
                foreach (var item in medList)
                {
                    Add(item);
                }
            }
        }

        private List<Medicine> GetMedicinesByCurrentSetAndTable(MedicineSet currentSet, DataTable table)
        {
            var tempList = new List<Medicine>();
            for (var i = 0; i < table.Rows.Count; i++)
            {
                var current = i;
                var tempMedList = currentSet.MedicineSetItems.Where(m => m.ID.Equals(table.Rows[current].Field<string>("Pro_ID")));
                tempList.AddRange(tempMedList.Select(setItem => AddMedicineByDataTypeAndMedicineSetItem(table.Rows[i], setItem)));
            }
            return tempList;
        }

        private Medicine AddMedicineByDataTypeAndMedicineSetItem(DataRow r, MedicineSetItem setItem)
        {
            Medicine medicine;
            switch (r.Field<int>("DataType"))
            {
                case 1:
                    medicine = new MedicineNHI(r);
                    SetValue(medicine, setItem);
                    return medicine;
                case 2:
                    medicine = new MedicineOTC(r);
                    SetValue(medicine, setItem);
                    return medicine;
                case 3:
                    medicine = new MedicineSpecialMaterial(r);
                    SetValue(medicine, setItem);
                    return medicine;
                default:
                    return null;
            }
        }

        private List<string> CreateIdListByMedicineSet(MedicineSet currentSet)
        {
            var medicineIDList = new List<string>();
            foreach (var item in currentSet.MedicineSetItems)
            {
                if (!medicineIDList.Contains(item.ID))
                    medicineIDList.Add(item.ID);
            }
            return medicineIDList;
        }

        private void SetValue(Medicine medicine, MedicineSetItem setItem)
        {
            medicine.Dosage = setItem.Dosage;
            medicine.UsageName = setItem.UsageName;
            medicine.PositionID = setItem.PositionID;
            medicine.Days = setItem.Days;
            medicine.Amount = setItem.Amount;
            medicine.PaySelf = setItem.PaySelf;
            if (medicine.PaySelf)
                medicine.Price = setItem.Price;
        }

        public void SetToPaySelf()
        {
            foreach (var m in Items)
            {
                if (m is MedicineVirtual) continue;
                if (!m.PaySelf)
                    m.PaySelf = true;
            }
        }

        public string CheckNegativeStock()
        {
            var result = Items.Where(m => !(m is MedicineVirtual)).Where(m => m.Inventory - m.BuckleAmount < 0 && m.BuckleAmount > 0).Aggregate(string.Empty, (current, m) => current + ("藥品" + m.ID + "扣庫量大於庫存\n"));
            
            if (!string.IsNullOrEmpty(result))
            {
                result += "如需繼續調劑請將扣庫量調至小於等於庫存或0。";
                MessageWindow.ShowMessage(result,MessageType.WARNING);
            }
            return result;
        }
    }
}
