using His_Pos.NewClass.Product;
using His_Pos.Service;
using System.Data;
using VM = His_Pos.ChromeTabViewModel.ViewModelMainWindow;

namespace His_Pos.NewClass.Medicine.MedicineSet
{
    public class MedicineSetItem : Product.Product
    {
        public MedicineSetItem()
        {
            Usage = new Usage.Usage();
            Position = new Position.Position();
        }

        public MedicineSetItem(DataRow r) : base(r)
        {
            NHIPrice = (double)r.Field<decimal>("Med_Price");
            Frozen = r.Field<bool>("Med_IsFrozen");
            Usage = new Usage.Usage();
            Position = new Position.Position();
            Days = r.Field<short?>("MedSetDet_MedicineDays");
            Amount = r.Field<double>("MedSetDet_TotalAmount");
            Dosage = r.Field<double>("MedSetDet_Dosage");
            PaySelf = r.Field<bool>("MedSetDet_IsPaySelf");
            Price = r.Field<double>("MedSetDet_PaySelfPrice");
        }

        public MedicineSetItem(ProductStruct p) : base(p)
        {
            Usage = new Usage.Usage();
            Position = new Position.Position();
            Price = p.SellPrice;
            NHIPrice = p.NHIPrice;
        }

        private bool isSelected = false;

        public bool IsSelected
        {
            get => isSelected;
            set { Set(() => IsSelected, ref isSelected, value); }
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
                    Usage = VM.FindUsageByQuickName(value);
                    if (Usage is null)
                    {
                        Usage = VM.GetUsage(value);
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
                    Position = VM.GetPosition(positionID);
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

        private double amount;//總量

        public double Amount
        {
            get => amount;
            set
            {
                Set(() => Amount, ref amount, value);
                CheckIsPriceReadOnly();
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

        private double price;//售價

        public double Price
        {
            get => price;
            set
            {
                Set(() => Price, ref price, value);
            }
        }

        private bool isPriceReadOnly;

        public bool IsPriceReadOnly
        {
            get => isPriceReadOnly;
            set
            {
                Set(() => IsPriceReadOnly, ref isPriceReadOnly, value);
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
                }
            }
        }

        #region Functions

        private void CalculateAmount()
        {
            if (Days is null || Dosage is null || string.IsNullOrEmpty(UsageName)) return;
            Amount = (double)Dosage * UsagesFunction.CheckUsage((int)Days, Usage) == 0 ? Amount : (double)Dosage * UsagesFunction.CheckUsage((int)Days, Usage);
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

        #endregion Functions

        public void CopyPrevious(MedicineSetItem previousItem)
        {
            Dosage = previousItem.Dosage;
            UsageName = previousItem.UsageName;
            Days = previousItem.Days;
        }
    }
}