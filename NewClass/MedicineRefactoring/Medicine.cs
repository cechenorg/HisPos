using System;
using System.Data;
using His_Pos.Interface;
using His_Pos.NewClass.Product.Medicine.MedBag;
using CooperativeMedicine = His_Pos.NewClass.Cooperative.XmlOfPrescription.CooperativePrescription.Item;
using OrthopedicsMedicine = His_Pos.NewClass.CooperativeInstitution.Item;
using His_Pos.NewClass.Product.Medicine.Position;
using His_Pos.NewClass.Product.Medicine.Usage;

namespace His_Pos.NewClass.MedicineRefactoring
{
    public abstract class Medicine:Product.Product,IDeletableProduct
    {
        public Medicine() : base()
        {

        }

        public Medicine(DataRow r) : base(r)
        {
            NHIPrice = (double)r.Field<decimal>("Med_Price");
        }

        public Medicine(CooperativeMedicine m)
        {
            Usage = new Usage();
            Position = new Position();
            ID = m.Id;
            ChineseName = m.Desc;
            UsageName = m.Freq;
            PositionID = m.Way;
            Amount = Convert.ToDouble(m.Total_dose);
            Dosage = Convert.ToDouble(m.Divided_dose);
            Days = Convert.ToInt32(m.Days);
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
        }

        public Medicine(OrthopedicsMedicine m)
        {
            Usage = new Usage();
            Position = new Position();
            ID = m.Id;
            ChineseName = m.Desc;
            EnglishName = m.Desc;
            UsageName = m.Freq;
            PositionID = m.Way;
            Amount = Convert.ToDouble(m.Total_dose);
            Dosage = Convert.ToDouble(m.Divided_dose);
            Days = Convert.ToInt32(m.Days);
            PaySelf = m.Remark == "-" || m.Remark == "*";
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
        }

        #region Properties
        private double? dosage;//每次用量
        public double? Dosage
        {
            get => dosage;
            set
            {
                Set(() => Dosage, ref dosage, value);
            }
        }

        private string usageName;
        public string UsageName
        {
            get => usageName;
            set
            {
                Set(() => UsageName, ref usageName, value);
            }
        }

        private Usage usage;//用法
        public Usage Usage
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
                Set(() => PositionID, ref positionID, value);
            }
        }

        private Position position;//途徑
        public Position Position
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
            }
        }

        private double price;//售價
        public double Price
        {
            get => price;
            set
            {
                Set(() => Price, ref price, value);
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

        private double inventory;//庫存
        public double Inventory
        {
            get => inventory;
            set
            {
                if (inventory != value)
                {
                    Set(() => Inventory, ref inventory, value);
                }
            }
        }

        private double costPrice;//成本
        public double CostPrice
        {
            get => costPrice;
            set
            {
                if (costPrice != value)
                {
                    Set(() => CostPrice, ref costPrice, value);
                }
            }
        }

        private bool paySelf;//是否自費
        public bool PaySelf
        {
            get => paySelf;
            set
            {
                Set(() => PaySelf, ref paySelf, value);
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
            }
        }

        private double buckleAmount;

        public double BuckleAmount
        {
            get => buckleAmount;
            set
            {
                Set(() => BuckleAmount, ref buckleAmount, value);
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

        private bool isSelected = false;
        public bool IsSelected
        {
            get => isSelected;
            set { Set(() => IsSelected, ref isSelected, value); }
        }
        #endregion

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
    }
}
