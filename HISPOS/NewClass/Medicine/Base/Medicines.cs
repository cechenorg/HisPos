using His_Pos.Class;
using His_Pos.FunctionWindow;
using His_Pos.NewClass.Medicine.InventoryMedicineStruct;
using His_Pos.NewClass.Medicine.MedicineSet;
using His_Pos.NewClass.Medicine.NotEnoughMedicine;
using His_Pos.NewClass.Prescription;
using His_Pos.NewClass.Product.ProductManagement;
using His_Pos.NewClass.Product.ProductManagement.ProductManageDetail;
using His_Pos.Properties;
using His_Pos.SYSTEM_TAB.H1_DECLARE.PrescriptionDeclare.FunctionWindow.NotEnoughMedicinePurchaseWindow;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using CooperativeMedicine = His_Pos.NewClass.Cooperative.XmlOfPrescription.CooperativePrescription.Item;
using OrthopedicsMedicine = His_Pos.NewClass.Cooperative.CooperativeInstitution.Item;

// ReSharper disable TooManyDeclarations

namespace His_Pos.NewClass.Medicine.Base
{
    public class Medicines : ObservableCollection<Medicine>
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
            ReOrder();
        }

        private Medicines CreateTempMedicinesByOrthopedics(List<OrthopedicsMedicine> medicineOrderItem, string wareHouseID, DateTime? adjustDate)
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

        #endregion OrthopedicsFunctions

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
            ReOrder();
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

        private void AddCooperativeIDEmptyMedicine(Medicines tempList, List<CooperativeMedicine> medicineOrderItem)
        {
            foreach (var order in medicineOrderItem.Where(m => string.IsNullOrEmpty(m.Id)))
            {
                var medicine = new MedicineOTC(order);
                tempList.Add(medicine);
            }
        }

        #endregion CooperativeFunctions

        public void GetDataByPrescriptionId(int id)
        {
            var table = MedicineDb.GetDataByPrescriptionId(id);
            CreateMedicines(table);
            ReOrder();
        }

        public void GetDataByReserveId(int id)
        {
            var table = MedicineDb.GetDataByReserveId(id);
            CreateMedicines(table);
            ReOrder();
        }

        private void CreateMedicines(DataTable table)
        {
            foreach (DataRow r in table.Rows)
            {
                Medicine medicine;
                switch (r.Field<int>("DataType"))
                {
                    case 0:
                        medicine = new MedicineVirtual(r);
                        break;

                    case 1:
                        medicine = new MedicineNHI(r);
                        break;

                    case 2:
                        medicine = new MedicineOTC(r);
                        break;

                    case 3:
                        medicine = new MedicineSpecialMaterial(r);
                        break;

                    default:
                        continue;
                }
                medicine.SetValueByDataRow(r);
                AddElseValue(medicine,r);
                Add(medicine);
            }
        }

        private void AddElseValue(Medicine medicine,DataRow dr)
        {
            if(dr != null)
            {
                if(dr["Med_Indication"] != null)
                {
                    medicine.Indication = Convert.ToString(dr["Med_Indication"]);
                }
                if (dr["Med_Ingredient"] != null)
                {
                    medicine.Ingredient = Convert.ToString(dr["Med_Ingredient"]);
                }
            }
        }

        public int CountMedicinePoint()
        {
            var notPaySelfNhiMedicines = GetNotPaySelfNhiMedicines();
            if (!notPaySelfNhiMedicines.Any()) return 0;
            var medicinePoint = notPaySelfNhiMedicines.Sum(m => m.TotalPrice);
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
            var specialMaterial = notPaySelfSpecialMaterials.Sum(m => (int)Math.Round(Convert.ToDouble((m.TotalPrice * 1.05).ToString(CultureInfo.InvariantCulture)), 0, MidpointRounding.AwayFromZero));
            return specialMaterial;
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
                return (int)this.Where(m => m.CanCountMedicineDays()).Max(m => m.Days);//計算最大給藥日份
            return 0;
        }

        public int CountOralLiquidAgent()
        {
            return Items.Count(m => m is MedicineNHI med && !string.IsNullOrEmpty(med.Note) && med.Note.Contains(Resources.口服液劑) && med.PaySelf == false);
        }

        public void AddMedicine(string medicineId, bool paySelf, int? selectedMedicinesIndex, string wareHouseId, DateTime? adjustDate)
        {
            var table = MedicineDb.GetMedicinesBySearchIds(new List<string> { medicineId }, wareHouseId, adjustDate);
            var medicine = AddMedicineByDataType(table);
            Debug.Assert(medicine != null, nameof(medicine) + " != null");
            medicine.PaySelf = paySelf;
            if (medicine.ID.EndsWith("00") || medicine.ID.EndsWith("G0"))
                medicine.PositionID = "PO";
            medicine.IsBuckle = !string.IsNullOrEmpty(wareHouseId);
            medicine.UsableAmount = medicine.OnTheFrameAmount;
            if (selectedMedicinesIndex != null)
            {
                if (selectedMedicinesIndex >= 0)
                    medicine.CopyPrevious(this[(int)selectedMedicinesIndex]);
                medicine.BuckleAmount = medicine.IsBuckle ? medicine.Amount : 0;
                medicine.Order = (int)selectedMedicinesIndex + 1;
                this[(int)selectedMedicinesIndex] = medicine;
            }
            else
            {
                if (Count > 0)
                    medicine.CopyPrevious(this[Count - 1]);
                medicine.BuckleAmount = medicine.IsBuckle ? medicine.Amount : 0;
                medicine.Order = Count + 1;
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
                    case 4:
                        medicine = new MedicineOTC(r);
                        medicine.PaySelf = true;

                        /* Retail Price */
                        var detailtable = ProductDetailDB.GetProductManageOTCMedicineDetailByID(medicine.ID);
                        var detail = new ProductManageDetail(detailtable.Rows[0]);
                        medicine.Price = detail.RetailPrice;

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

        public void Update(bool buckle, int id, PrescriptionType type, DateTime? adjustDate = null, string wareHouseID = null)
        {
            SetBuckleAmount(buckle);
            DataTable table;
            if (type == PrescriptionType.ChronicReserve)
                table = MedicineDb.GetUsableAmountByReserveID(id);
            else
            {
                if (id == 0)
                {
                    var idList = this.Select(m => m.ID).ToList();
                    table = MedicineDb.GetMedicinesBySearchIds(idList, wareHouseID, adjustDate);
                    UpdateInventoryID(table);
                }
                else
                    table = MedicineDb.GetUsableAmountByPrescriptionID(id);
            }
            if (id == 0 && type != PrescriptionType.ChronicReserve)
                ReserveUpdatePriceAndUsableAmount(table);
            else
                NormalUpdatePriceAndUsableAmount(table);
        }

        private void NormalUpdatePriceAndUsableAmount(DataTable table)
        {
            foreach (DataRow r in table.Rows)
            {
                var medList = this.Where(m => m.InventoryID.Equals(r.Field<int>("Inv_ID")));
                foreach (var m in medList)
                {
                    m.NHIPrice = r.Field<decimal?>("Med_Price") is null ? 0 : (double)r.Field<decimal>("Med_Price");
                    m.UsableAmount = r.Field<double?>("CanUseAmount") is null ? 0 : r.Field<double>("CanUseAmount");
                }
            }
        }

        private void ReserveUpdatePriceAndUsableAmount(DataTable table)
        {
            foreach (DataRow r in table.Rows)
            {
                var medList = this.Where(m => m.InventoryID.Equals(r.Field<int>("Inv_ID")));
                foreach (var m in medList)
                {
                    m.NHIPrice = (double)r.Field<decimal>("Med_Price");
                    m.UsableAmount = r.Field<double?>("Inv_OntheFrame") is null ? 0 : r.Field<double>("Inv_OntheFrame");
                }
            }
        }

        private void UpdateInventoryID(DataTable table)
        {
            foreach (DataRow r in table.Rows)
            {
                foreach (var m in this)
                {
                    if (r.Field<string>("Pro_Id").Equals(m.ID))
                        m.InventoryID = r.Field<int>("Inv_ID");
                }
            }
        }

        [SuppressMessage("ReSharper", "FlagArgument")]
        private void SetBuckleAmount(bool buckle)
        {
            foreach (var m in this)
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
                        CreateMedicineNHIData(ref result, med);
                        break;

                    case MedicineVirtual _:
                        CreateMedicineVirtualData(ref result, med);
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

        private void CreateMedicineNHIData(ref string result, Medicine med)
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

        public void GetMedicineBySet(MedicineSet.MedicineSet currentSet, string wareHouseID, DateTime? adjustDate)
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

        private List<Medicine> GetMedicinesByCurrentSetAndTable(MedicineSet.MedicineSet currentSet, DataTable table)
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

        private List<string> CreateIdListByMedicineSet(MedicineSet.MedicineSet currentSet)
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

        public string CheckNegativeStock(string warID, MedicineInventoryStructs usableAmountList, string cusName, string note = null)
        {
            var inventoryIDList = new List<int>();
            foreach (var med in this)
            {
                if (med is MedicineVirtual) continue;
                if (!inventoryIDList.Contains(med.InventoryID))
                    inventoryIDList.Add(med.InventoryID);
            }

            var buckleMedicines =
                (from inv in inventoryIDList
                 let editMed = this.Where(m => m.InventoryID.Equals(inv))
                 let buckleAmount = editMed.Sum(m => m.BuckleAmount)
                 select new MedicineInventoryStruct(inv, buckleAmount)).ToList();

            var medIDs = this.Where(m => !(m is MedicineVirtual)).Select(m => m.InventoryID).ToList();
            MainWindow.ServerConnection.OpenConnection();
            var inventories = MedicineDb.GetInventoryByInvIDs(medIDs);
            MainWindow.ServerConnection.CloseConnection();
            var inventoryList = new List<MedicineInventoryStruct>();
            foreach (DataRow r in inventories.Rows)
            {
                if (usableAmountList.Count(u => u.ID.Equals(r.Field<int>("Inv_ID"))) == 1)
                {
                    var usable = usableAmountList.SingleOrDefault(u => u.ID.Equals(r.Field<int>("Inv_ID")));
                    inventoryList.Add(new MedicineInventoryStruct(usable.ID, usable.Amount));
                }
                else
                {
                    inventoryList.Add(new MedicineInventoryStruct(r.Field<int>("Inv_ID"), r.Field<double>("Inv_OnTheFrame")));
                }
            }
            var negativeStock = string.Empty;
            var notEnoughMedicines = new NotEnoughMedicines();
            foreach (var inv in inventoryList)
            {
                var buckle = buckleMedicines.Single(m => m.ID.Equals(inv.ID));
                if (buckle.Amount == 0) continue;
                if (inv.Amount - buckleMedicines.Single(m => m.ID.Equals(inv.ID)).Amount >= 0) continue;
                foreach (var m in this)
                {
                    if (!m.InventoryID.Equals(inv.ID) || m is MedicineVirtual) continue;
                    if (m.IsCommon) {
                        m.BuckleAmount = m.UsableAmount;
                        continue; //常備藥不可於登錄採購
                    }
                    var controlLevel = m is MedicineNHI nhiMed ? nhiMed.ControlLevel : null;
                    //notEnoughMedicines.Add(new NotEnoughMedicine.NotEnoughMedicine(m.ID, m.FullName, m.Amount - m.UsableAmount, m.IsCommon, m.Frozen, controlLevel, m.AveragePrice, m.Amount - m.UsableAmount));
                    if (m.Amount > m.UsableAmount)  { 
                        notEnoughMedicines.Add(new NotEnoughMedicine.NotEnoughMedicine(m.ID, m.FullName, m.Amount - m.UsableAmount, m.IsCommon, m.Frozen, controlLevel, m.AveragePrice, m.Amount - m.UsableAmount, m.UsableAmount, m.Amount,m.SingdeInv));
                    }                    
                }
                negativeStock = this.Where(med => !(med is MedicineVirtual))
                    .Where(med => (med.InventoryID.Equals(inv.ID) && med.BuckleAmount>med.UsableAmount))
                    .Aggregate(negativeStock, (current, med) => current + ("藥品" + med.ID + "\n"));
            }
            if (notEnoughMedicines.Count > 0 && warID.Equals("0"))
            {
                var purchaseWindow = new NotEnoughMedicinePurchaseWindow(note, cusName, notEnoughMedicines);
                if (purchaseWindow.DialogResult is null || !(bool)purchaseWindow.DialogResult)
                {
                    negativeStock = "欠藥採購取消。";
                }
                else
                {
                    SetBuckleAmountToUsableAmount(notEnoughMedicines);
                    return string.Empty;
                }
            }
            if (string.IsNullOrEmpty(negativeStock)) return negativeStock;
            negativeStock += "如需繼續調劑請將扣庫量調至小於等於庫存或0。";
            MessageWindow.ShowMessage(negativeStock, MessageType.WARNING);
            return negativeStock;
        }

        private void SetBuckleAmountToUsableAmount(NotEnoughMedicines notEnoughMedicines)
        {
            foreach (var m in notEnoughMedicines)
            {
                var originalMed = this.Single(med => med.ID.Equals(m.ID));
                originalMed.BuckleAmount = originalMed.UsableAmount;
            }
        }

        public void ReOrder()
        {
            for (var i = 1; i <= Count; i++)
            {
                this[i - 1].Order = i;
            }
        }

        public void CheckUsableAmount(MedicineInventoryStructs usableMedicines)
        {
            if (usableMedicines.Count <= 0) return;
            foreach (var usableMedicine in usableMedicines)
            {
                foreach (var m in this)
                {
                    if (!m.InventoryID.Equals(usableMedicine.ID)) continue;
                    m.UsableAmount = usableMedicine.Amount;
                }
            }
        }
    }
}