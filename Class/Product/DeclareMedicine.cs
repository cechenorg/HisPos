﻿using System;
using System.Data;
using System.Text.RegularExpressions;
using His_Pos.H1_DECLARE.PrescriptionDec2;
using His_Pos.Interface;
using His_Pos.Service;

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
        }

        public DeclareMedicine(DataRow dataRow,string type) : base(dataRow)
        {
            MedicalCategory = new Medicate();
            Usage = new Usage();
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
                    Dosage = string.Empty,
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
                    Dosage = string.IsNullOrEmpty(dataRow["HISDECDET_AMOUNT"].ToString())? string.Empty : dataRow["HISDECDET_AMOUNT"].ToString();
                    UsageName = string.IsNullOrEmpty(dataRow["HISFEQ_ID"].ToString())? string.Empty : dataRow["HISFEQ_ID"].ToString();
                    Days = string.IsNullOrEmpty(dataRow["HISDECDET_DRUGDAY"].ToString())? string.Empty : dataRow["HISDECDET_DRUGDAY"].ToString();
                    Position = string.IsNullOrEmpty(dataRow["HISWAY_ID"].ToString())? string.Empty : dataRow["HISWAY_ID"].ToString();
                    Amount = string.IsNullOrEmpty(dataRow["HISDECDET_QTY"].ToString())? 0 : Convert.ToDouble(dataRow["HISDECDET_QTY"].ToString());
                }
                Ingredient = dataRow["HISMED_INGREDIENT"].ToString();
                Price = double.Parse(dataRow["PRO_LASTPRICE"].ToString());
                Note = dataRow["HISMED_NOTE"].ToString().Equals(string.Empty) ? string.Empty : dataRow["HISMED_NOTE"].ToString();
            }
            ControlLevel = dataRow["HISMED_CONTROL"].ToString();
            IsFrozMed = bool.Parse(dataRow["HISMED_FROZ"].ToString().Equals(string.Empty) ? "False" : dataRow["HISMED_FROZ"].ToString());
            HcPrice = double.TryParse(dataRow["HISMED_PRICE"].ToString(), out var hcPrice) ? hcPrice : 0.0000;
            IsBuckle = true;
        }

        public bool IsControl => !string.IsNullOrEmpty(ControlLevel);

        private string _controlLevel;
        public string ControlLevel
        {
            get => _controlLevel;
            set
            {
                _controlLevel = value.Equals("0") ? string.Empty : value;
                NotifyPropertyChanged(nameof(ControlLevel));
            }
        }

        public bool IsFrozMed { get; set; }
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

        public string UsageName
        {
            get => Usage != null ? Usage.Name : string.Empty;
            set
            {
                Usage.Name = value;
                if(double.TryParse(Dosage, out _) && (Id.EndsWith("00") || Id.EndsWith("G0")) && !string.IsNullOrEmpty(UsageName) && int.TryParse(Days, out _))
                    CalculateAmount();
                NotifyPropertyChanged(nameof(UsageName));
            }
        }

        public string Dosage
        {
            get => MedicalCategory != null ? MedicalCategory.Dosage : string.Empty;
            set
            {
                MedicalCategory.Dosage = value;
                if (double.TryParse(value, out _) && (Id.EndsWith("00") || Id.EndsWith("G0")) && !string.IsNullOrEmpty(UsageName) && int.TryParse(Days, out _))
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
                if (int.TryParse(value, out _) && (Id.EndsWith("00") || Id.EndsWith("G0")) && !string.IsNullOrEmpty(UsageName) && double.TryParse(Dosage, out _))
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

        private double _inventory;
        public double Inventory
        {
            get => _inventory;
            set
            {
                _inventory = value;
                NotifyPropertyChanged(nameof(Inventory));
            }
        }

        string IProductDeclare.ProductId { get => Id; set => Id = value; }
        string IProductDeclare.ProductName { get => Name; set => Name=value; }
        string IProductDeclare.Dosage { get => Dosage; set => Dosage = value; }
        string IProductDeclare.Usage { get => Usage.Name; set => Usage.Name = value; }
        string IProductDeclare.Position { get => Position; set => Position = value; }
        string IProductDeclare.Days { get => Days; set => Days = value; }
        double IProductDeclare.Amount { get => Amount; set => Amount = value; }
        double IProductDeclare.Inventory { get => Inventory; set => Inventory = value; }
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
            Amount = find ? double.Parse(Dosage)*UsagesFunction.CheckUsage(int.Parse(_days), tmpUsage) : double.Parse(Dosage)*UsagesFunction.CheckUsage(int.Parse(_days));
        }

        public object Clone()
        {
            var declareMedicine = new DeclareMedicine()
            {
                Id = Id,
                Name = Name,
                ChiName = ChiName,
                EngName = EngName,
                IsFrozMed = IsFrozMed,
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
                ControlLevel = ControlLevel
            };
            return declareMedicine;
        }
    }
}