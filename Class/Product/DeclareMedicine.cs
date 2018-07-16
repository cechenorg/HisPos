using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using His_Pos.Interface;
using His_Pos.Service;

namespace His_Pos.Class.Product
{
    public class DeclareMedicine : AbstractClass.Product, ITrade, IDeletable
    {
        public DeclareMedicine()
        {
        }

        public DeclareMedicine(DataRow dataRow) : base(dataRow)
        {
            IsControlMed = Boolean.Parse((dataRow["HISMED_CONTROL"].ToString() == "") ? "False" : dataRow["HISMED_CONTROL"].ToString());
            IsFrozMed = Boolean.Parse((dataRow["HISMED_FROZ"].ToString() == "") ? "False" : dataRow["HISMED_FROZ"].ToString());
            PaySelf = false;
            HcPrice = double.Parse(dataRow["HISMED_PRICE"].ToString());
            Ingredient = dataRow["HISMED_INGREDIENT"].ToString();
            MedicalCategory = new Medicate(dataRow);
            Stock = new InStock(dataRow);
            Cost = 0;
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
        public bool PaySelf { get; set; }
        public double HcPrice { get; set; }
        public string Ingredient { get; set; }
        public Medicate MedicalCategory { get; set; }
        public InStock Stock { get; set; }
        public double Cost { get; set; }
        public string Price { get; set; }
        public double TotalPrice { get; set; }
        public double amount;

        public double Amount
        {
            get { return amount; }
            set
            {
                amount = value;
                NotifyPropertyChanged("Amount");
            }
        }

        public string CountStatus { get; set; }
        public string FocusColumn { get; set; }
        public Usage Usage { get; set; }

        public string UsageName
        {
            get { return Usage.Name; }
            set
            {
                Usage.Name = value;
                CalculateTotalPrice();
                NotifyPropertyChanged("UsageName");
            }
        }

        public string Dosage
        {
            get { return MedicalCategory.Dosage; }
            set
            {
                MedicalCategory.Dosage = value;
                if (double.TryParse(value, out _))
                    CalculateTotalPrice();
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
                    CalculateTotalPrice();
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
            foreach (var usage in UsageDb.GetUsages())
            {
                Regex reg = new Regex(usage.Reg);
                if (reg.IsMatch(UsageName))
                {
                    if (!string.IsNullOrEmpty(Days) && !string.IsNullOrEmpty(UsageName))
                    {
                        int total = UsagesFunction.CheckUsage(usage, int.Parse(Days));
                        if (total > 0 && Dosage != string.Empty)
                        {
                            Amount = Math.Ceiling(double.Parse(Dosage) * total);
                        }
                        else
                        {
                            Amount = 0;
                        }
                    }
                }
            }
        }
    }
}