using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using His_Pos.Interface;
using His_Pos.Service;

namespace His_Pos.Class.Product
{
    public class DeclareMedicine : AbstractClass.Product, ITrade, IDeletable, ICloneable
    {
        public DeclareMedicine()
        {
        }

        public DeclareMedicine(DataRow dataRow) : base(dataRow)
        {
            IsControlMed = Boolean.Parse((dataRow["HISMED_CONTROL"].ToString() == "") ? "False" : dataRow["HISMED_CONTROL"].ToString());
            IsFrozMed = Boolean.Parse((dataRow["HISMED_FROZ"].ToString() == "") ? "False" : dataRow["HISMED_FROZ"].ToString());
            PaySelf = false;
            HcPrice = dataRow["HISMED_PRICE"].ToString();
            Ingredient = dataRow["HISMED_INGREDIENT"].ToString();
            MedicalCategory = new Medicate(dataRow);
            Stock = new InStock(dataRow);
            Cost = 1;
            Price = dataRow["PRO_SELL_PRICE"].ToString();
            TotalPrice = 0;
            Amount = 0;
            CountStatus = "";
            FocusColumn = "";
            Usage = new Usage();
            days = "";
            Position = "";
            source = "";
        }
        
        public bool IsControlMed { get; set; }
        public bool IsFrozMed { get; set; }
        private bool payself;

        public bool PaySelf
        {
            get { return payself; }
            set
            {
                payself = value;
                CalculateTotalPrice();
                NotifyPropertyChanged("PaySelf");
            }
        }

        public string HcPrice { get; set; }
        public string Ingredient { get; set; }
        public Medicate MedicalCategory { get; set; }
        public InStock Stock { get; set; }
        public double Cost { get; set; }
        private string price;

        public string Price
        {
            get { return price; }
            set
            {
                price = value;
                CalculateTotalPrice();
            }
        }

        private double totalPrice;

        public double TotalPrice
        {
            get { return totalPrice; }
            set
            {
                totalPrice = value;
                NotifyPropertyChanged("TotalPrice");
            }
        }

        public double amount;

        public double Amount
        {
            get { return amount; }
            set
            {
                amount = value;
                CalculateTotalPrice();
                NotifyPropertyChanged("Amount");
            }
        }

        public string CountStatus { get; set; }
        public string FocusColumn { get; set; }
        public Usage Usage { get; set; }

        public string UsageName
        {
            get
            {
                if (Usage != null)
                    return Usage.Name;
                return "";
            }
            set
            {
                Usage.Name = value;
                CalculateAmount();
                NotifyPropertyChanged("UsageName");
            }
        }

        public string Dosage
        {
            get
            {
                if (MedicalCategory != null)
                    return MedicalCategory.Dosage;
                return "";
            }
            set
            {
                MedicalCategory.Dosage = value;
                if (double.TryParse(value, out _))
                    CalculateAmount();
                NotifyPropertyChanged("Dosage");
            }
        }

        private string days;

        public string Days
        {
            get { return days; }
            set
            {
                days = value;
                if (int.TryParse(value, out _))
                    CalculateAmount();
                NotifyPropertyChanged("Days");
            }
        }

        private string position;

        public string Position
        {
            get { return position; }
            set
            {
                position = value;
                NotifyPropertyChanged("Position");
            }
        }

        public void CalculateData(string inputSource)
        {
            throw new NotImplementedException();
        }

        private string source;

        public string Source
        {
            get { return source; }
            set
            {
                source = value;
                NotifyPropertyChanged("Source");
            }
        }

        private void CalculateTotalPrice()
        {
            if (PaySelf)
            {
                if (string.IsNullOrEmpty(Price))
                    return;
                TotalPrice = Amount * double.Parse(Price);
            }
            else
            {
                if (string.IsNullOrEmpty(HcPrice))
                    return;
                TotalPrice = Amount * double.Parse(HcPrice);
            }
        }

        private void CalculateAmount()
        {
            foreach (var usage in UsageDb.GetUsages())
            {
                Regex reg = new Regex(usage.Reg);
                if (reg.IsMatch(UsageName))
                {
                    if (!string.IsNullOrEmpty(Days) && !string.IsNullOrEmpty(UsageName))
                    {
                        int total = UsagesFunction.CheckUsage(usage, int.Parse(Days));
                        if (total > 0 && Dosage != string.Empty)
                            Amount = Math.Ceiling(double.Parse(Dosage) * total);
                        else
                            Amount = 0;
                    }
                }
            }
        }

        public object Clone()
        {
            DeclareMedicine declareMedicine = new DeclareMedicine()
            {
                Id = Id,
                Name = Name,
                ChiName = ChiName,
                EngName = EngName,
                IsControlMed = IsControlMed,
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
            };
            return declareMedicine;
        }
    }
}