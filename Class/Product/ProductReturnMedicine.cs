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
    public class ProductReturnMedicine: AbstractClass.Product, IProductReturn, INotifyPropertyChanged, ICloneable
    {
        public ProductReturnMedicine(DataRow row) : base(row)
        {
            Stock = new InStock(row);

            Note = row[""].ToString();
            ReturnAmount = Double.Parse(row[""].ToString());
            BatchNumber = row[""].ToString();
            BatchLimit = Double.Parse(row[""].ToString());
            ReturnPrice = Double.Parse(row[""].ToString());
            ReturnTotalPrice = Double.Parse(row[""].ToString());
        }

        public string Note { get; set; }
        public InStock Stock { get; set; }
        public double ReturnAmount { get; set; }
        public string BatchNumber { get; set; }
        public double BatchLimit { get; set; }
        public double ReturnPrice { get; set; }
        public double ReturnTotalPrice { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }

        public object Clone()
        {
            throw new NotImplementedException();
        }
    }
}
