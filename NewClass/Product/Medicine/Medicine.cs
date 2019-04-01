using His_Pos.ChromeTabViewModel;
using His_Pos.NewClass.CooperativeInstitution;
using System;
using System.Data;
using System.Threading;
using His_Pos.Interface;
using His_Pos.Service;

namespace His_Pos.NewClass.Product.Medicine
{
    public class Medicine:Product, IDeletableProduct
    {
        public Medicine() : base()
        {
            Usage = new Usage.Usage();
            Position = new Position.Position();
            IsBuckle = true;
        }
        public Medicine(DataRow r) : base(r)
        {
            NHIPrice = (double)r.Field<decimal>("Med_Price");
            Inventory = r.Field<double>("Inv_Inventory");
            CostPrice = (double)r.Field<decimal>("Pro_LastPrice");
            Vendor = r.Field<string>("Med_Manufactory");
            Frozen = r.Field<bool>("Med_IsFrozen");
            Enable = r.Field<bool>("Pro_IsEnable");
            Usage = new Usage.Usage();
            Position = new Position.Position(); 
        }
        public Medicine(ProductStruct p) : base(p)
        {
            Usage = new Usage.Usage();
            Position = new Position.Position();
            Price = p.SellPrice;
            NHIPrice = p.NHIPrice;
        }

        public Medicine(Item m)
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
            Days = Convert.ToInt32(m.Days);
            PaySelf = !string.IsNullOrEmpty(m.Remark) ;
            IsBuckle = false;
            switch (m.Remark) {
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
    
        private double amount;//總量
        public double Amount
        {
            get => amount;
            set
            {
                Set(() => Amount, ref amount, value);
                BuckleAmount = amount;
                CheckIsPriceReadOnly();
                CountTotalPrice();
            }
        }

        private void CountTotalPrice()
        {
            if(Amount <= 0) return;
            TotalPrice = PaySelf ? Math.Round(Amount * Price, MidpointRounding.AwayFromZero) : Math.Round(Amount * NHIPrice, MidpointRounding.AwayFromZero);
        }

        private double? dosage;//每次用量
        public double? Dosage
        {
            get => dosage;
            set
            {
                Set(() => Dosage, ref dosage, value);
                if (ID is null) return;
                if (ID.EndsWith("00") || ID.EndsWith("G0") && !string.IsNullOrEmpty(Usage.Name) && (Days != null && Days > 0) && (Dosage != null && Dosage > 0))
                    CalculateAmount();
            }
        }
        private string _usageName;
        public string UsageName
        {
            get => _usageName;
            set
            {
                if (value != null)
                {
                    Set(() => UsageName, ref _usageName, value);
                    Usage = ViewModelMainWindow.FindUsageByQuickName(value);
                    if (Usage is null)
                    {
                        Usage = ViewModelMainWindow.GetUsage(value);
                        Usage.Name = UsageName;
                    }
                    else
                    {
                        UsageName = Usage.Name;
                    }
                    if (ID is null) return;
                    if ((ID.EndsWith("00") || ID.EndsWith("G0")) && !string.IsNullOrEmpty(Usage.Name) && (Days != null && Days > 0) && (Dosage != null && Dosage > 0))
                        CalculateAmount();
                }
            }
        }
        private Usage.Usage usage;//用法
        public Usage.Usage Usage
        {
            get => usage;
            set
            {
                if (usage != value)
                {
                    Set(() => Usage, ref usage, value);
                }
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
                if (position != value)
                {
                    Set(() => Position, ref position, value);
                }
            }
        }
        private int? days;//給藥天數
        public int? Days
        {
            get => days;
            set
            {
                if (days != value)
                {
                    Set(() => Days, ref days, value);
                    if (ID is null) return;
                    if ((ID.EndsWith("00") || ID.EndsWith("G0")) && !string.IsNullOrEmpty(Usage.Name) && (Days != null && Days > 0) && (Dosage != null && Dosage > 0))
                        CalculateAmount();
                }
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
        private double nhiPrice;//健保價
        public double NHIPrice
        {
            get => nhiPrice;
            set
            {
                if (nhiPrice != value)
                {
                    Set(() => NHIPrice, ref nhiPrice, value);
                }
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
                if (paySelf != value)
                {
                    Set(() => PaySelf, ref paySelf, value);
                    CheckIsPriceReadOnly();
                    CountTotalPrice();
                }
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

        public bool NotBuckle
        {
            get => !IsBuckle;
        }

        private void CheckIsPriceReadOnly()
        {
            if (Amount <= 0)
                IsPriceReadOnly = true;
            else
            {
                IsPriceReadOnly = !PaySelf;
            }
        }

        private string vendor;//製造商
        public string Vendor
        {
            get => vendor;
            set
            {
                if (vendor != value)
                {
                    Set(() => Vendor, ref vendor, value);
                }
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
        private Item m;

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

        private bool isPriceReadOnly;
        public bool IsPriceReadOnly {
            get => isPriceReadOnly;
            set
            {
                Set(() => IsPriceReadOnly, ref isPriceReadOnly, value);
            }
        }

        private void CalculateAmount()
        {
            if(Days is null || Dosage is null || string.IsNullOrEmpty(UsageName)) return;
            Amount = (double)Dosage * UsagesFunction.CheckUsage((int)Days, Usage) == 0 ? Amount : (double)Dosage * UsagesFunction.CheckUsage((int)Days, Usage);
        }

        public void CheckPaySelf(string adjustCaseId)
        {
            if (string.IsNullOrEmpty(adjustCaseId)) return;
            if (adjustCaseId.Equals("0"))
                PaySelf = true;
        }

        private bool isSelected = false;
        public bool IsSelected
        {
            get => isSelected;
            set { Set(() => IsSelected, ref isSelected, value); }
        }
        private int? controlLevel;

        public int? ControlLevel
        {
            get => controlLevel;
            set
            {
                Set(() => ControlLevel, ref controlLevel, value);
            }
        }
        private double buckleAmount = 0;
        public double BuckleAmount
        {
            get => buckleAmount;
            set
            {
                if (value >= Amount)
                    Set(() => BuckleAmount, ref buckleAmount, Amount);
                else
                    Set(() => BuckleAmount, ref buckleAmount, value);
            }
        }

        public void CopyPrevious(Medicine preMed)
        {
            Dosage = preMed.Dosage;
            UsageName = preMed.UsageName;
            Days = preMed.Days;
        }
    }
}
