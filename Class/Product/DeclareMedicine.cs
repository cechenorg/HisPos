using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using His_Pos.Interface;

namespace His_Pos.Class.Product
{
    public class DeclareMedicine : Medicine, ITrade, IDeletable
    {
        public DeclareMedicine()
        {
        }

        public DeclareMedicine(DataRow dataRow): base(dataRow)
        {
            IsControlMed = Boolean.Parse((dataRow["HISMED_CONTROL"].ToString() == "") ? "False" : dataRow["HISMED_CONTROL"].ToString());
            IsFrozMed = Boolean.Parse((dataRow["HISMED_FROZ"].ToString() == "") ? "False" : dataRow["HISMED_FROZ"].ToString());
            PaySelf = false;
            HcPrice = double.Parse(dataRow["HISMED_PRICE"].ToString());
            Ingredient = dataRow["HISMED_INGREDIENT"].ToString();
            MedicalCategory = new Medicate(dataRow);
            Stock = new InStock(dataRow);
            Price = dataRow["PRO_SELL_PRICE"].ToString();
            TotalPrice = 0;
        }

        public InStock Stock { get; set; }
        public double Cost { get; set; }
        public double TotalPrice { get; set; }
        public double Amount { get; set; }
        public string Price { get; set; }
        public string CountStatus { get; set; }
        public string FocusColumn { get;set; }
        public int Days { get; set; }

        public void CalculateData(string inputSource)
        {
            throw new NotImplementedException();
        }

        private string source;

        public event PropertyChangedEventHandler PropertyChanged;

        public string Source
        {
            get { return source; }
            set
            {
                source = value;
                NotifyPropertyChanged("Source");
            }
        }
    }
}
