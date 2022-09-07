using His_Pos.ChromeTabViewModel;
using His_Pos.FunctionWindow;
using His_Pos.NewClass.Medicine.MedBag;
using His_Pos.Service;
using System;
using System.Data;
using CooperativeMedicine = His_Pos.NewClass.Cooperative.XmlOfPrescription.CooperativePrescription.Item;
using OrthopedicsMedicine = His_Pos.NewClass.Cooperative.CooperativeInstitution.Item;

namespace His_Pos.NewClass.Medicine.Base
{
    public abstract class Medicine : Product.Product, ICloneable
    {
        public Medicine() : base()
        {
        }

        public Medicine(DataRow r) : base(r)
        {
            NHIPrice = (double)r.Field<decimal>("Med_Price");
            OnTheFrameAmount = r.Field<double?>("Inv_OntheFrame") is null ? 0 : r.Field<double>("Inv_OntheFrame");
            if (NewFunction.CheckDataRowContainsColumn(r, "Singde_Inv"))
                SingdeInv = r.Field<int>("Singde_Inv");
            else
                SingdeInv = 0;
            CostPrice = (double)(r.Field<decimal?>("Inv_LastPrice") is null ? 0 : r.Field<decimal>("Inv_LastPrice"));
            AveragePrice = r.Field<double?>("AveragePrice") is null ? 0 : r.Field<double>("AveragePrice");
            Price = r.Field<double>("Pro_SelfPayPrice");
            InventoryID = r.Field<int>("Inv_ID");
            Ingredient = r.Field<string>("Med_Ingredient");
            Indication = r.Field<string>("Med_Indication");
            if (NewFunction.CheckDataRowContainsColumn(r, "MedbagAmountLack"))
                InventoryError = r.Field<int>("MedbagAmountLack") == 1;
            if (NewFunction.CheckDataRowContainsColumn(r, "AdjustNoBuckle") && r.Field<bool?>("AdjustNoBuckle") != null)
                AdjustNoBuckle = r.Field<bool>("AdjustNoBuckle");
            else
                AdjustNoBuckle = false;
            if (NewFunction.CheckDataRowContainsColumn(r, "IsClosed") && r.Field<bool?>("IsClosed") != null)
                IsClosed = r.Field<bool>("IsClosed");
            else
                IsClosed = false;
            SendAmount = -1;
        }

        public Medicine(CooperativeMedicine m)
        {
            Usage = new Usage.Usage();
            Position = new Position.Position();
            ID = m.Id;
            ChineseName = m.Desc;
            UsageName = m.Freq;
            PositionID = m.Way;
            Amount = Convert.ToDouble(m.Total_dose);
            Dosage = Convert.ToDouble(m.Divided_dose);
            Days = Convert.ToInt32(Math.Floor(Convert.ToDouble(m.Days)));
            PaySelf = !string.IsNullOrEmpty(m.Remark);
            IsBuckle = false;
            switch (m.Remark)
            {
                case "":
                    TotalPrice = Amount * Convert.ToDouble(m.Price);
                    break;

                case "-":
                    TotalPrice = 0;
                    break;

                case "*":
                    TotalPrice = Convert.ToDouble(m.Price);
                    break;
            }
            AdjustNoBuckle = false;
            IsClosed = false;
        }

        public Medicine(OrthopedicsMedicine m)
        {
            Usage = new Usage.Usage();
            Position = new Position.Position();
            ID = m.Id;
            ChineseName = m.Desc;
            EnglishName = m.Desc;
            UsageName = m.Freq;
            PositionID = m.Way;
            Amount = Convert.ToDouble(m.Total_dose);
            Dosage = Convert.ToDouble(m.Divided_dose);
            Days = Convert.ToInt32(Math.Floor(Convert.ToDouble(m.Days)));
            PaySelf = m.Remark.Equals("-") || m.Remark.Equals("*");
            IsBuckle = false;
            switch (m.Remark)
            {
                case "":
                    TotalPrice = Amount * Convert.ToDouble(m.Price);
                    break;

                case "-":
                    TotalPrice = 0;
                    break;

                case "*":
                    TotalPrice = Convert.ToDouble(m.Price);
                    break;

                default:
                    TotalPrice = Amount * Convert.ToDouble(m.Price);
                    break;
            }
            AdjustNoBuckle = false;
            IsClosed = false;
        }

        #region Properties

        private double? dosage;//每次用量

        public double? Dosage
        {
            get => dosage;
            set
            {
                Set(() => Dosage, ref dosage, value);
                CalculateAmount();
            }
        }

        private bool canEdit;

        public bool CanEdit
        {
            get => canEdit;
            set
            {
                Set(() => CanEdit, ref canEdit, value);
            }
        }

        private string usageName;

        public string UsageName
        {
            get => usageName;
            set
            {
                Set(() => UsageName, ref usageName, value);
                Usage = ViewModelMainWindow.FindUsageByName(value);
                if (Usage is null)
                {
                    Usage = ViewModelMainWindow.FindUsageByQuickName(value);
                    if (Usage is null)
                    {
                        Usage = ViewModelMainWindow.GetUsage(value);
                        Usage.Name = UsageName;
                    }
                    else
                        usageName = Usage.Name;
                }
                else
                    usageName = Usage.Name;
                CalculateAmount();
            }
        }

        private Usage.Usage usage;//用法

        public Usage.Usage Usage
        {
            get => usage;
            set
            {
                Set(() => Usage, ref usage, value);
            }
        }

        private string positionID;

        public string PositionID
        {
            get => positionID;
            set
            {
                if (value != null)
                {
                    Set(() => PositionID, ref positionID, value);
                    Position = ViewModelMainWindow.GetPosition(positionID);
                    if (Position != null)
                    {
                        Position.ID = positionID;
                    }
                }
            }
        }

        private Position.Position position;//途徑

        public Position.Position Position
        {
            get => position;
            set
            {
                Set(() => Position, ref position, value);
            }
        }

        private int? days;//給藥天數

        public int? Days
        {
            get => days;
            set
            {
                Set(() => Days, ref days, value);
                CalculateAmount();
            }
        }

        private double price;//售價

        public double Price
        {
            get => price;
            set
            {
                Set(() => Price, ref price, value);
                CountTotalPrice();
            }
        }

        private double totalPrice;//總價

        public double TotalPrice
        {
            get => totalPrice;
            set
            {
                Set(() => TotalPrice, ref totalPrice, value);
            }
        }

        private double onTheFrameAmount;//庫存

        public double OnTheFrameAmount
        {
            get => onTheFrameAmount;
            set
            {
                Set(() => OnTheFrameAmount, ref onTheFrameAmount, value);
            }
        }

        private int singdeInv; //杏德庫存
        public int SingdeInv
        {
            get => singdeInv;
            set
            {
                Set(() => SingdeInv, ref singdeInv, value);
            }
        }

        private int inventoryID;//庫存編號

        public int InventoryID
        {
            get => inventoryID;
            set
            {
                if (inventoryID != value)
                {
                    Set(() => InventoryID, ref inventoryID, value);
                }
            }
        }

        private double costPrice;//成本

        public double CostPrice
        {
            get => costPrice;
            set
            {
                Set(() => CostPrice, ref costPrice, value);
            }
        }

        private double averagePrice;//均價

        public double AveragePrice
        {
            get => averagePrice;
            set
            {
                Set(() => AveragePrice, ref averagePrice, value);
            }
        }

        private bool paySelf;//是否自費

        public bool PaySelf
        {
            get => paySelf;
            set
            {
                if (value != null && !value)
                {
                    if (this is MedicineOTC) return;
                }
                Set(() => PaySelf, ref paySelf, value);
                CountTotalPrice();
            }
        }

        private bool isBuckle = true;

        public bool IsBuckle
        {
            get => isBuckle;
            set
            {
                Set(() => IsBuckle, ref isBuckle, value);
            }
        }

        private bool frozen;//是否為冷藏藥品

        public bool Frozen
        {
            get => frozen;
            set
            {
                if (frozen != value)
                {
                    Set(() => Frozen, ref frozen, value);
                }
            }
        }

        private bool enable;//是否停用

        public bool Enable
        {
            get => enable;
            set
            {
                if (enable != value)
                {
                    Set(() => Enable, ref enable, value);
                }
            }
        }

        private string ingredient;//成分

        public string Ingredient
        {
            get => ingredient;
            set
            {
                if (ingredient != value)
                {
                    Set(() => Ingredient, ref ingredient, value);
                }
            }
        }

        private string sideEffect;//副作用

        public string SideEffect
        {
            get => sideEffect;
            set
            {
                if (sideEffect != value)
                {
                    Set(() => SideEffect, ref sideEffect, value);
                }
            }
        }

        private string indication;//適應症

        public string Indication
        {
            get => indication;
            set
            {
                if (indication != value)
                {
                    Set(() => Indication, ref indication, value);
                }
            }
        }

        private double amount;//總量

        public double Amount
        {
            get => amount;
            set
            {
                Set(() => Amount, ref amount, value);
                CountTotalPrice();
            }
        }

        private double buckleAmount;

        public double BuckleAmount
        {
            get => buckleAmount;
            set
            {
                Set(() => BuckleAmount, ref buckleAmount, value);
                NotEnough = BuckleAmount < Amount;
            }
        }

        private bool notEnough;

        public bool NotEnough
        {
            get => notEnough;
            set
            {
                Set(() => NotEnough, ref notEnough, value);
            }
        }

        private double nhiPrice;//健保價

        public double NHIPrice
        {
            get => nhiPrice;
            set
            {
                Set(() => NHIPrice, ref nhiPrice, value);
            }
        }

        private bool isSelected;

        public bool IsSelected
        {
            get => isSelected;
            set { Set(() => IsSelected, ref isSelected, value); }
        }

        private int order;

        public int Order
        {
            get => order;
            set { Set(() => Order, ref order, value); }
        }

        private double sendAmount;

        public double SendAmount
        {
            get => sendAmount;
            set { Set(() => SendAmount, ref sendAmount, value); }
        }

        private double usableAmount;

        public double UsableAmount
        {
            get => usableAmount;
            set
            {
                Set(() => UsableAmount, ref usableAmount, value);
            }
        }

        private bool inventoryError;

        public bool InventoryError
        {
            get => inventoryError;
            set { Set(() => InventoryError, ref inventoryError, value); }
        }

        private bool adjustNoBuckle;//欠藥/調劑不扣庫

        public bool AdjustNoBuckle
        {
            get => adjustNoBuckle;
            set
            {
                if (adjustNoBuckle != value)
                {
                    Set(() => AdjustNoBuckle, ref adjustNoBuckle, value);
                }
            }
        }

        private bool isClosed; //結案

        public bool IsClosed
        {
            get => isClosed;
            set
            {
                if (IsClosed != value)
                {
                    Set(() => IsClosed, ref isClosed, value);
                }
            }
        }

        #endregion Properties

        public bool CheckIsBloodGlucoseTestStrip()
        {
            return this is MedicineSpecialMaterial && ID.StartsWith("TSS01");
        }

        public abstract MedBagMedicine CreateMedBagMedicine(bool isSingle);

        public void CopyPrevious(Medicine previous)
        {
            Dosage = previous.Dosage;
            UsageName = previous.UsageName;
            Days = previous.Days;
        }

        private void CalculateAmount()
        {
            if (string.IsNullOrEmpty(ID)) return;
            if (Dosage is null || Days is null) return;
            if (Usage is null || string.IsNullOrEmpty(Usage.Name)) return;
            if (ID.EndsWith("00") || ID.EndsWith("G0"))
            {
                var countByUsage = (double)Dosage * UsagesFunction.CheckUsage((int)Days, Usage);
                Amount = countByUsage == 0 ? Amount : countByUsage;
            }
        }

        private void CountTotalPrice()
        {
            if (Amount < 0) return;
            var tempPrice = (PaySelf ? Price : NHIPrice) * Amount;
            TotalPrice = Math.Round(Convert.ToDouble(tempPrice.ToString()), 0, MidpointRounding.AwayFromZero);
        }

        public void SetValueByOrthopedicsMedicine(OrthopedicsMedicine setItem)
        {
            Dosage = Convert.ToDouble(setItem.Divided_dose);
            UsageName = setItem.Freq;
            PositionID = setItem.Way;
            Days = Convert.ToInt32(setItem.Days);
            Amount = Convert.ToDouble(setItem.Total_dose);
            PaySelf = !string.IsNullOrEmpty(setItem.Remark) && (setItem.Remark.Equals("*") || setItem.Remark.Equals("-"));
            SetPriceByRemark(setItem.Remark, setItem.Price);
        }

        public void SetValueByCooperativeMedicine(CooperativeMedicine setItem)
        {
            Dosage = Convert.ToDouble(setItem.Divided_dose);
            UsageName = setItem.Freq.ToUpper();
            PositionID = setItem.Way.ToUpper();
            Days = Convert.ToInt32(setItem.Days.Contains(".") ? setItem.Days.Substring(0, setItem.Days.IndexOf('.')) : setItem.Days);
            Amount = Convert.ToDouble(setItem.Total_dose);
            PaySelf = !string.IsNullOrEmpty(setItem.Remark) && (setItem.Remark.Equals("*") || setItem.Remark.Equals("-"));
            if (LoginWindowViewModel.ReadSettingFilePharmacyName() == "宏昌藥局")
            {
                SetPriceByRemarkHONGCHANG(setItem.Remark, setItem.Price);
            }
            else
            {
                SetPriceByRemark(setItem.Remark, setItem.Price);
            }
        }

        private void SetPriceByRemarkHONGCHANG(string remark, string itemPrice)
        {
            var priceValue = Convert.ToDouble(itemPrice);
            if (priceValue < 0) priceValue *= -1;
            switch (remark)
            {
                case "":
                    TotalPrice = Amount * priceValue;
                    break;

                case "-":
                    TotalPrice = 0;
                    break;

                case "*":
                    TotalPrice = Amount * Price;
                    break;
            }
        }

        private void SetPriceByRemark(string remark, string itemPrice)
        {
            var priceValue = 0.0;
            if (itemPrice == "")
            {
                priceValue = 0;
            }
            else
            {
                priceValue = Convert.ToDouble(itemPrice);
            }
            if (priceValue < 0) priceValue *= -1;
            switch (remark)
            {
                case "":
                    TotalPrice = Amount * priceValue;
                    break;

                case "-":
                    TotalPrice = 0;
                    break;

                case "*":
                    TotalPrice = priceValue;
                    break;
            }
        }

        public bool CanCountMedicineDays()
        {
            return (this is MedicineNHI || this is MedicineSpecialMaterial) && !PaySelf && CheckDaysNotNull();
        }

        private bool CheckDaysNotNull()
        {
            return Days != null && Days > 0;
        }

        public object Clone()
        {
            if (this is MedicineNHI)
            {
                var clonedMed = this as MedicineNHI;
                var medNHI = new MedicineNHI
                {
                    ID = clonedMed.ID,
                    ChineseName = clonedMed.ChineseName,
                    EnglishName = clonedMed.EnglishName,
                    UsageName = clonedMed.UsageName,
                    PositionID = clonedMed.PositionID,
                    Dosage = clonedMed.Dosage,
                    Days = clonedMed.Days,
                    Amount = clonedMed.Amount,
                    PaySelf = clonedMed.PaySelf,
                    IsBuckle = clonedMed.IsBuckle,
                    BuckleAmount = clonedMed.BuckleAmount,
                    ATCCode = clonedMed.ATCCode,
                    ControlLevel = clonedMed.ControlLevel,
                    Form = clonedMed.Form,
                    Note = clonedMed.Note,
                    SingleCompound = clonedMed.SingleCompound,
                    Warning = clonedMed.Warning,
                    CostPrice = clonedMed.CostPrice,
                    Indication = clonedMed.Indication,
                    Ingredient = clonedMed.Ingredient,
                    Price = clonedMed.Price,
                    NHIPrice = clonedMed.NHIPrice,
                    OnTheFrameAmount = clonedMed.OnTheFrameAmount,
                    IsCommon = clonedMed.IsCommon,
                    SideEffect = clonedMed.SideEffect,
                    TotalPrice = clonedMed.TotalPrice,
                    Enable = clonedMed.Enable,
                    Frozen = clonedMed.Frozen,
                    CanEdit = true,
                    SendAmount = -1
                };
                return medNHI;
            }

            if (this is MedicineSpecialMaterial)
            {
                var clonedMed = this as MedicineSpecialMaterial;
                var medSpecialMaterial = new MedicineSpecialMaterial
                {
                    ID = clonedMed.ID,
                    ChineseName = clonedMed.ChineseName,
                    EnglishName = clonedMed.EnglishName,
                    UsageName = clonedMed.UsageName,
                    PositionID = clonedMed.PositionID,
                    Dosage = clonedMed.Dosage,
                    Days = clonedMed.Days,
                    Amount = clonedMed.Amount,
                    PaySelf = clonedMed.PaySelf,
                    IsBuckle = clonedMed.IsBuckle,
                    BuckleAmount = clonedMed.BuckleAmount,
                    CostPrice = clonedMed.CostPrice,
                    Indication = clonedMed.Indication,
                    Ingredient = clonedMed.Ingredient,
                    Price = clonedMed.Price,
                    NHIPrice = clonedMed.NHIPrice,
                    OnTheFrameAmount = clonedMed.OnTheFrameAmount,
                    IsCommon = clonedMed.IsCommon,
                    SideEffect = clonedMed.SideEffect,
                    TotalPrice = clonedMed.TotalPrice,
                    Enable = clonedMed.Enable,
                    Frozen = clonedMed.Frozen,
                    CanEdit = true,
                    SendAmount = -1
                };
                return medSpecialMaterial;
            }

            if (this is MedicineOTC)
            {
                var clonedMed = this as MedicineOTC;
                var medOtc = new MedicineOTC
                {
                    ID = clonedMed.ID,
                    ChineseName = clonedMed.ChineseName,
                    EnglishName = clonedMed.EnglishName,
                    UsageName = clonedMed.UsageName,
                    PositionID = clonedMed.PositionID,
                    Dosage = clonedMed.Dosage,
                    Days = clonedMed.Days,
                    Amount = clonedMed.Amount,
                    PaySelf = clonedMed.PaySelf,
                    IsBuckle = clonedMed.IsBuckle,
                    BuckleAmount = clonedMed.BuckleAmount,
                    CostPrice = clonedMed.CostPrice,
                    Indication = clonedMed.Indication,
                    Ingredient = clonedMed.Ingredient,
                    Price = clonedMed.Price,
                    NHIPrice = 0,
                    OnTheFrameAmount = clonedMed.OnTheFrameAmount,
                    IsCommon = clonedMed.IsCommon,
                    SideEffect = clonedMed.SideEffect,
                    TotalPrice = clonedMed.TotalPrice,
                    Enable = clonedMed.Enable,
                    Frozen = clonedMed.Frozen,
                    CanEdit = true,
                    SendAmount = -1
                };
                return medOtc;
            }

            if (this is MedicineVirtual)
            {
                var medVirtual = new MedicineVirtual(ID);
                return medVirtual;
            }
            return null;
        }

        public void SetValueByDataRow(DataRow r)
        {
            Usage = new Usage.Usage();
            Position = new Position.Position();
            Dosage = r.Field<double?>("Dosage");
            UsageName = r.Field<string>("Usage");
            PositionID = r.Field<string>("Position");
            Days = r.Field<int?>("MedicineDays");
            PaySelf = r.Field<bool>("PaySelf");
            IsBuckle = r.Field<bool>("IsBuckle");
            Amount = r.Field<double>("TotalAmount");
            BuckleAmount = NewFunction.CheckDataRowContainsColumn(r, "BuckleAmount") ? r.Field<double>("BuckleAmount") : Amount;
            Price = NewFunction.CheckDataRowContainsColumn(r, "PaySelfValue") ? (double)r.Field<decimal>("PaySelfValue") : 0;
            if (PaySelf)
            {
                TotalPrice = r.Field<int>("Point");
            }
            Order = r.Field<int>("OrderNumber");
            SendAmount = r.Field<double>("SendAmount");
        }

        public void ResetBuckleAmount()
        {
            BuckleAmount = Amount;
        }
        public void ClearBuckleAmount()
        {
            BuckleAmount = 0;
        }
    }
}