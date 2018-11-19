using System;
using System.Data;
using System.Linq;
using His_Pos.Interface;
using His_Pos.Service;

namespace His_Pos.Class.Product
{
    public class PrescriptionOTC : AbstractClass.Product, ITrade, IDeletable, ICloneable, IProductDeclare
    {
        public PrescriptionOTC()
        {
            PaySelf = true;
            HcPrice = 0.0000;
            Cost = 0.00;
            TotalPrice = 0.00;
            Amount = 0.00;
            Price = 0.00;
            Source = string.Empty;
            CountStatus = string.Empty;
            FocusColumn = string.Empty;
            Dosage = "0";
            Usage = new Usage();
            Position = string.Empty;
            Days = string.Empty;
            Stock = new InStock();
            ControlLevel = string.Empty;
        }

        public PrescriptionOTC(DataRow dataRow) : base(dataRow)
        {
            PaySelf = true;
            HcPrice = 0.0000;
            Cost = 0.00;
            TotalPrice = 0.00;
            Amount = 0.00;
            Price = 0.00;
            Source = string.Empty;
            CountStatus = string.Empty;
            FocusColumn = string.Empty;
            Dosage = "0";
            Usage = new Usage();
            Position = string.Empty;
            Days = string.Empty;
            ControlLevel = "0";
            Stock = new InStock(dataRow);
        }

        private bool _paySelf;
        public bool PaySelf
        {
            get => _paySelf;
            set
            {
                _paySelf = true;
                NotifyPropertyChanged(nameof(PaySelf));
            }
        }
        private double _hcPrice;
        private double HcPrice { get => _hcPrice;
            set
            {
                _hcPrice = 0.0000;
                NotifyPropertyChanged(nameof(HcPrice));
            }
        }
        public double Cost { get; set; }
        private double _totalPrice;

        public double TotalPrice
        {
            get => _totalPrice;
            set
            {
                _totalPrice = value;
                NotifyPropertyChanged(nameof(TotalPrice));
            }
        }

        private double _amount;

        public double Amount
        {
            get => _amount;
            set
            {
                _amount = value;
                NotifyPropertyChanged(nameof(Amount));
            }
        }

        private double _price;
        public double Price
        {
            get => _price;
            set
            {
                _price = value;
                NotifyPropertyChanged(nameof(Price));
            }
        }

        private string _source;
        public string Source
        {
            get => _source;
            set
            {
                _source = value;
                NotifyPropertyChanged(nameof(Source));
            }
        }
        public string CountStatus { get; set; }
        public string FocusColumn { get; set; }
        private string _dosage;
        public string Dosage
        {
            get => _dosage;
            set
            {
                _dosage = value;
                NotifyPropertyChanged(nameof(Dosage));
            }
        }

        private Usage _usage;

        public Usage Usage
        {
            get => _usage;
            set
            {
                _usage = value;
                NotifyPropertyChanged(nameof(Usage));
            }
        }

        public string UsageName
        {
            get => Usage != null ? Usage.Name : string.Empty;
            set
            {
                Usage.PrintName = value.Any(char.IsDigit) ? NewFunction.FindUsageName(value) : NewFunction.GetUsagePrintName(value);
                NotifyPropertyChanged(nameof(UsageName));
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

        private string _positionId;
        public string PositionId
        {
            get => _positionId;
            set
            {
                _positionId = value;
                NotifyPropertyChanged(nameof(PositionId));
            }
        }
        private string _days;
        public string Days
        {
            get => _days;
            set
            {
                _days = value;
                NotifyPropertyChanged(nameof(Days));
            }
        }

        private InStock _stock;

        public InStock Stock
        {
            get => _stock;
            set
            {
                _stock = value;
                NotifyPropertyChanged(nameof(Stock));
            }
        }

        public string ControlLevel { get; set; }
        string IDeletable.Source { get => Source; set => Source =value; }
        double ITrade.Cost { get => Cost; set => Cost = value; }
        double ITrade.TotalPrice { get => TotalPrice; set => TotalPrice = value; }
        double IProductDeclare.TotalPrice { get => TotalPrice; set => TotalPrice = value; }
        double ITrade.Amount { get => Amount; set => Amount = value; }
        double IProductDeclare.Amount { get => Amount; set => Amount = value; }
        double ITrade.Price { get => Price; set => Price = value; }
        string ITrade.CountStatus { get => CountStatus; set => CountStatus = value; }
        string ITrade.FocusColumn { get => FocusColumn; set => FocusColumn = value; }
        string IProductDeclare.ProductId { get => Id; set => Id = value; }
        string IProductDeclare.ProductName { get => Name; set => Name = value; }
        string IProductDeclare.Dosage { get => Dosage; set => Dosage = value; }
        string IProductDeclare.Usage { get => UsageName; set => UsageName = value; }
        string IProductDeclare.Position { get => PositionId; set => PositionId = value; }
        string IProductDeclare.Days { get => Days; set => Days = value; }
        double IProductDeclare.Inventory { get => Stock.Inventory; set => Stock.Inventory = value; }
        double IProductDeclare.HcPrice { get => 0.0000;set => throw new NotImplementedException(); }
        bool IProductDeclare.PaySelf { get => PaySelf; set => PaySelf = true; }
        string IProductDeclare.ControlLevel { get => ControlLevel; set => ControlLevel = string.Empty; }
        string IProductDeclare.Forms { get => string.Empty; set => throw new NotImplementedException(); }
        string IProductDeclare.Ingredient { get => string.Empty; set => throw new NotImplementedException(); }
        string IProductDeclare.SideEffect { get => string.Empty; set => throw new NotImplementedException(); }
        string IProductDeclare.Indication { get => string.Empty; set => throw new NotImplementedException(); }

        public object Clone()
        {
            var prescriptionOtc = new PrescriptionOTC()
            {
                Id = Id,
                Name = Name,
                ChiName = ChiName,
                EngName = EngName,
                PaySelf = PaySelf,
                HcPrice = HcPrice,
                Cost = Cost,
                Price = Price,
                TotalPrice = TotalPrice,
                Amount = Amount,
                CountStatus = CountStatus,
                FocusColumn = FocusColumn,
                Usage = Usage,
                Dosage = Dosage,
                Days = Days,
                Position = Position,
                Source = Source,
                Stock = Stock,
                ControlLevel = ControlLevel
            };
            return prescriptionOtc;
        }

        void ITrade.CalculateData(string inputSource)
        {
            throw new NotImplementedException();
        }
    }
}
