using System;
using System.Data;
using System.Text.RegularExpressions;
using His_Pos.Interface;
using His_Pos.Service;

namespace His_Pos.Class.Product
{
    public class DeclareMedicine : AbstractClass.Product, ITrade, IDeletable, ICloneable
    {
        public DeclareMedicine()
        {
            HcPrice = "";
            Ingredient = "";
            MedicalCategory = new Medicate();
            Cost = 0;
            Price = 0;
            TotalPrice = 0;
            Amount = 0;
            CountStatus = "";
            FocusColumn = "";
            Usage = new Usage();
            days = "";
            Position = "";
            IsBuckle = true;
            source = "";
        }

        public DeclareMedicine(DataRow dataRow,string type) : base(dataRow)
        {
            MedicalCategory = new Medicate();
            Usage = new Usage();
            if (type.Equals("DeclareFile"))
            {
                Ingredient = string.Empty;
                Cost = 0;
                Price = 0;
                TotalPrice = 0;
                Amount = 0;
                CountStatus = string.Empty;
                FocusColumn = string.Empty;
                Usage = new Usage();
                days = string.Empty;
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
                Ingredient = dataRow["HISMED_INGREDIENT"].ToString();
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
                    days = string.Empty;
                }
                else
                {
                    Dosage = string.IsNullOrEmpty(dataRow["HISDECDET_AMOUNT"].ToString())? string.Empty : dataRow["HISDECDET_AMOUNT"].ToString();
                    UsageName = string.IsNullOrEmpty(dataRow["HISFEQ_ID"].ToString())? string.Empty : dataRow["HISFEQ_ID"].ToString();
                    Days = string.IsNullOrEmpty(dataRow["HISDECDET_DRUGDAY"].ToString())? string.Empty : dataRow["HISDECDET_DRUGDAY"].ToString();
                    Position = string.IsNullOrEmpty(dataRow["HISWAY_ID"].ToString())? string.Empty : dataRow["HISWAY_ID"].ToString();
                    Amount = string.IsNullOrEmpty(dataRow["HISDECDET_QTY"].ToString())? 0 : Convert.ToDouble(dataRow["HISDECDET_QTY"].ToString());
                }

            }
            IsControlMed = int.Parse(dataRow["HISMED_CONTROL"].ToString());
            IsFrozMed = Boolean.Parse(dataRow["HISMED_FROZ"].ToString().Equals(string.Empty) ? "False" : dataRow["HISMED_FROZ"].ToString());
            HcPrice = dataRow["HISMED_PRICE"].ToString();
            Note = dataRow["HISMED_NOTE"].ToString().Equals(string.Empty) ? string.Empty : dataRow["HISMED_NOTE"].ToString();
            IsBuckle = true;
        }
        
        public int IsControlMed { get; set; }
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
        private double price;

        public double Price
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
        private void CalculateTotalPrice()
        {
            if (PaySelf)
            {
                TotalPrice = Amount * Price;
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
            if(MedicalCategory.Form.Equals(""))
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
                IsBuckle = IsBuckle,
            };
            return declareMedicine;
        }
    }
}