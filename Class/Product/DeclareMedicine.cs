using System;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;
using His_Pos.Interface;
using His_Pos.NewClass.Usage;
using His_Pos.Service;
using His_Pos.SYSTEM_TAB.H1_DECLARE.PrescriptionDec2;

namespace His_Pos.Class.Product
{
    public class DeclareMedicine : AbstractClass.Product, ITrade, IDeletable, ICloneable,IProductDeclare
    {
        public DeclareMedicine()
        {
            HcPrice = 0.0000;
            Ingredient = string.Empty;
            MedicalCategory = new Medicate();
            Cost = 0;
            Price = 0;
            TotalPrice = 0;
            Amount = 0;
            CountStatus = string.Empty;
            FocusColumn = string.Empty;
            Usage = new Usage();
            _days = string.Empty;
            Position = string.Empty;
            IsBuckle = true;
            source = string.Empty;
            SideEffect = string.Empty;
            Indication = string.Empty;
            IsControl = false;
            IsCommon = false;
            IsFrozen = false;
        }
        public DeclareMedicine(XmlNode xml)
        {
            Id = xml.Attributes["id"].Value;
            Name = xml.Attributes["desc"].Value;
            ChiName = xml.Attributes["desc"].Value;
            EngName = string.Empty;
            Ingredient = string.Empty;
            Usage = new Usage();
            UsageName = xml.Attributes["freq"].Value;
            Position = xml.Attributes["way"].Value;
            Amount = Convert.ToDouble(xml.Attributes["total_dose"].Value);  
            MedicalCategory = new Medicate();
            MedicalCategory.Dosage = double.Parse(xml.Attributes["daily_dose"].Value);
            Dosage = Convert.ToDouble(xml.Attributes["divided_dose"].Value);
            Days = xml.Attributes["days"].Value;
            PaySelf = xml.Attributes["remark"].Value == "*" ? true : false; 
            Cost = 0;
            TotalPrice = PaySelf ? Convert.ToDouble(xml.Attributes["price"].Value) : 0;
            Price = TotalPrice / Amount;
            CountStatus = string.Empty;
            FocusColumn = string.Empty;
            IsBuckle = false;
            source = string.Empty;
            SideEffect = string.Empty;
            Indication = string.Empty;
        }
        public DeclareMedicine(DataRow dataRow,string type) : base(dataRow)
        {
            MedicalCategory = new Medicate();
            Usage = new Usage();
            IsBuckle = true;
            if (type.Equals("DeclareFile"))
            {
                Cost = 0;
                Price = 0;
                TotalPrice = 0;
                Amount = 0;
                CountStatus = string.Empty;
                FocusColumn = string.Empty;
                Usage = new Usage();
                _days = string.Empty;
                Position = string.Empty;
                source = string.Empty;
                MedicalCategory = new Medicate
                {
                    Dosage = 0,
                    Unit = string.Empty,
                    Form = dataRow["HISMED_FORM"].ToString()
                };
            }
            else
            {
                MedicalCategory = new Medicate(dataRow);
                PaySelf = false;
                Stock = new InStock(dataRow);
                Cost = 1;
                Usage = new Usage();
                if (dataRow.Table.Columns.Contains("PRO_SELL_PRICE"))
                    Price = Double.Parse(dataRow["PRO_SELL_PRICE"].ToString());
                TotalPrice = 0;
                CountStatus = string.Empty;
                FocusColumn = string.Empty;
                source = string.Empty;

                if (type.Equals("Init"))
                {
                    Amount = 0;
                    _days = string.Empty;
                }
                else
                {
                    Dosage = string.IsNullOrEmpty(dataRow["HISDECDET_AMOUNT"].ToString())? 0 : double.Parse(dataRow["HISDECDET_AMOUNT"].ToString());
                    if (!string.IsNullOrEmpty(dataRow["HISFEQ_ID"].ToString()))
                    {
                        UsageName = MainWindow.Usages.SingleOrDefault(usg => usg.Id.Equals(dataRow["HISFEQ_ID"].ToString())).Name;
                    }
                    Days = string.IsNullOrEmpty(dataRow["HISDECDET_DRUGDAY"].ToString())? string.Empty : dataRow["HISDECDET_DRUGDAY"].ToString();
                    Position = string.IsNullOrEmpty(dataRow["HISWAY_ID"].ToString())? string.Empty : dataRow["HISWAY_ID"].ToString();
                    Amount = string.IsNullOrEmpty(dataRow["HISDECDET_QTY"].ToString())? 0 : Convert.ToDouble(dataRow["HISDECDET_QTY"].ToString());
                }
                Ingredient = dataRow["HISMED_INGREDIENT"].ToString();
                Price = double.Parse(dataRow["PRO_SELL_PRICE"].ToString());
                Cost = double.Parse(dataRow["PRO_LASTPRICE"].ToString());
                Note = dataRow["HISMED_NOTE"].ToString().Equals(string.Empty) ? string.Empty : dataRow["HISMED_NOTE"].ToString();
                HcNote = dataRow["HISMED_HCNOTE"].ToString().Equals(string.Empty) ? string.Empty : dataRow["HISMED_HCNOTE"].ToString();
                if(type.Equals("Get"))
                IsBuckle = Convert.ToBoolean(dataRow["IS_BUCKLE"].ToString());
                Indication = dataRow["HISMED_INDICATION"].ToString();
                SideEffect = dataRow["HISMED_SIDEFFECT"].ToString();
            }
            ControlLevel = dataRow["HISMED_CONTROL"].ToString();
            IsFrozen = dataRow["HISMED_FROZ"].ToString().Equals("True");
            IsCommon = dataRow["HISMED_COMMON"].ToString().Equals("True");
            HcPrice = double.TryParse(dataRow["HISMED_PRICE"].ToString(), out var hcPrice) ? hcPrice : 0.0000;
        }
        private bool _isControl;

        public bool IsControl
        {
            get => _isControl;
            set
            {
                _isControl = value;
                NotifyPropertyChanged(nameof(IsControl));
            }
        }
        private bool _isCommon;

        public bool IsCommon
        {
            get => _isCommon;
            set
            {
                _isCommon = value;
                NotifyPropertyChanged(nameof(IsCommon));
            }
        }
        private string _controlLevel;
        public string ControlLevel
        {
            get => _controlLevel;
            set
            {
                if (!string.IsNullOrEmpty(value) && value.Equals("0") || string.IsNullOrEmpty(value))
                {
                    _controlLevel = string.Empty;
                    IsControl = false;
                }
                else
                {
                    _controlLevel = value;
                    IsControl = true;
                }
                NotifyPropertyChanged(nameof(ControlLevel));
            }
        }

        private bool _isFrozen;

        public bool IsFrozen
        {
            get => _isFrozen;
            set
            {
                _isFrozen = value;
                NotifyPropertyChanged(nameof(IsFrozen));
            }
        }

        private bool payself;

        public bool PaySelf
        {
            get => payself;
            set
            {
                payself = value;
                CalculateTotalPrice();
                NotifyPropertyChanged(nameof(PaySelf));
            }
        }

        public double HcPrice { get; set; }
        public string Ingredient { get; set; }
        public Medicate MedicalCategory { get; set; }
        public InStock Stock { get; set; }
        public double Cost { get; set; }
        private double price;

        public double Price
        {
            get => price;
            set
            {
                price = value;
                CalculateTotalPrice();
            }
        }

        private double totalPrice;

        public double TotalPrice
        {
            get => totalPrice;
            set
            {
                totalPrice = value;
                if(PrescriptionDec2View.Instance != null)
                    PrescriptionDec2View.Instance.CountMedicinesCost();
                NotifyPropertyChanged(nameof(TotalPrice));
            }
        }

        private double amount;

        public double Amount
        {
            get => amount;
            set
            {
                amount = value;
                CalculateTotalPrice();
                NotifyPropertyChanged(nameof(Amount));
            }
        }

        public string CountStatus { get; set; }
        public string FocusColumn { get; set; }
        public Usage Usage { get; set; }
        private string _usageName;
        public string UsageName
        {
            get => _usageName;
            set
            {
                if (value != null)
                {
                    _usageName = value;
                    Usage = MainWindow.Usages.SingleOrDefault(u => u.Reg.IsMatch(_usageName.ToString().Replace(" ", ""))).DeepCloneViaJson();
                    if (Usage != null)
                    {
                        Usage.Name = _usageName;
                        if ((Id.EndsWith("00") || Id.EndsWith("G0")) && !string.IsNullOrEmpty(UsageName) && int.TryParse(Days, out _))
                            CalculateAmount();
                    }
                    NotifyPropertyChanged(nameof(UsageName));
                }
            }
        }

        public double Dosage
        {
            get => MedicalCategory.Dosage;
            set
            {
                MedicalCategory.Dosage = value;
                if (Id != null && (Id.EndsWith("00") || Id.EndsWith("G0")) && !string.IsNullOrEmpty(UsageName) && int.TryParse(Days, out _))
                    CalculateAmount();
                NotifyPropertyChanged(nameof(Dosage));
            }
        }

        private string _days;

        public string Days
        {
            get => _days;
            set
            {
                _days = value;
                if (int.TryParse(value, out _) && (Id.EndsWith("00") || Id.EndsWith("G0")) && !string.IsNullOrEmpty(UsageName))
                    CalculateAmount();
                NotifyPropertyChanged(nameof(Days));
            }
        }

        private string _position;

        public string Position
        {
            get => _position;
            set
            {
                _position = value;
                NotifyPropertyChanged(nameof(Position));
            }
        }

        private string sideEffect;

        public string SideEffect
        {
            get => sideEffect;
            set
            {
                sideEffect = value;
                NotifyPropertyChanged(nameof(SideEffect));
            }
        }

        private string indication;

        public string Indication
        {
            get => indication;
            set
            {
                indication = value;
                NotifyPropertyChanged(nameof(Indication));
            }
        }

        public void CalculateData(string inputSource)
        {
            throw new NotImplementedException();
        }

        private string source;

        public string Source
        {
            get => source;
            set
            {
                source = value;
                NotifyPropertyChanged("Source");
            }
        }

        public string Note { get; set; }

        public string HcNote { get; set; }

        private bool isBuckle;
        public bool IsBuckle
        {
            get { return isBuckle; }
            set
            {
                isBuckle = value;
                NotifyPropertyChanged("IsBuckle");
                NotifyPropertyChanged("BuckleIcon");
            }
        }
        public string BuckleIcon {

            get {
                if (IsBuckle)
                    return ""; 
                else
                    return "../../Images/icons8-delete-32.png";
            }
        }

        string IProductDeclare.ProductId { get => Id; set => Id = value; }
        string IProductDeclare.ProductName { get => Name; set => Name=value; }
        double IProductDeclare.Dosage { get => Dosage; set => Dosage = value; }
        string IProductDeclare.Usage { get => Usage.Name; set => Usage.Name = value; }
        string IProductDeclare.Position { get => Position; set => Position = value; }
        string IProductDeclare.Days { get => Days; set => Days = value; }
        double IProductDeclare.Amount { get => Amount; set => Amount = value; }
        double IProductDeclare.Inventory { get => Stock.Inventory; set => Stock.Inventory = value; }
        double IProductDeclare.HcPrice { get => HcPrice; set => HcPrice = value; }
        bool IProductDeclare.PaySelf { get => PaySelf; set => PaySelf = value; }
        double IProductDeclare.TotalPrice { get => TotalPrice; set => TotalPrice = value; }
        string IProductDeclare.ControlLevel { get => ControlLevel; set => ControlLevel = value; }
        string IProductDeclare.Forms { get => MedicalCategory.Form; set => MedicalCategory.Form = value; }

        private void CalculateTotalPrice()
        {
            if (PaySelf)
                TotalPrice = Amount * Price;
            else
                TotalPrice = Amount * HcPrice;
        }

        private void CalculateAmount()
        {
            var tmpUsage = new Usage();
            var find = false;
            foreach (var u in MainWindow.Usages)
            {
                if (!UsageName.Equals(u.Name)) continue;
                tmpUsage = u;
                find = true;
            }
            Amount = find ? Dosage * UsagesFunction.CheckUsage(int.Parse(_days), tmpUsage) : Dosage * UsagesFunction.CheckUsage(int.Parse(_days));
        }

        public object Clone()
        {
            var declareMedicine = new DeclareMedicine()
            {
                Id = Id,
                Name = Name,
                ChiName = ChiName,
                EngName = EngName,
                IsFrozen = IsFrozen,
                PaySelf = PaySelf,
                HcPrice = HcPrice,
                Ingredient = Ingredient,
                MedicalCategory = MedicalCategory,
                Stock = Stock,
                Cost = Cost,
                Price = Price,
                TotalPrice = TotalPrice,
                Amount = Amount,
                CountStatus = CountStatus,
                FocusColumn = FocusColumn,
                Usage = Usage,
                UsageName = UsageName,
                Dosage = Dosage,
                Days = Days,
                Position = Position,
                Source = Source,
                IsBuckle = IsBuckle,
                ControlLevel = ControlLevel,
                Indication = Indication,
                SideEffect = SideEffect,
                IsControl = IsControl,
                IsCommon = IsCommon
            };
            return declareMedicine;
        }
    }
}