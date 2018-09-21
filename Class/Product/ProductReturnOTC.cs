using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using His_Pos.Interface;
using His_Pos.Struct.Product;

namespace His_Pos.Class.Product
{
    public class ProductReturnOTC : AbstractClass.Product, IProductReturn, INotifyPropertyChanged, ICloneable
    {
        public ProductReturnOTC(DataRow row) : base(row)
        {
            Stock = new InStock(row);

            Note = row["PRO_NOTE"].ToString();
            ReturnAmount = Double.Parse(row["AMOUNT"].ToString());
            BatchNumber = row["BATCHNUMBER"].ToString();
            BatchLimit = Double.Parse(row["BATCHLIMIT"].ToString());
            ReturnPrice = Double.Parse(row["PRICE"].ToString());
            ReturnTotalPrice = Double.Parse(row["TOTAL"].ToString());
        }

        public ProductReturnOTC(PurchaseProduct selectedItem)
        {
            Id = selectedItem.Id;
            Name = selectedItem.Name;
            ChiName = selectedItem.ChiName;
            EngName = selectedItem.EngName;

            Stock = new InStock(selectedItem);

            Note = "";
            ReturnAmount = 0;
            BatchNumber = "";
            BatchLimit = 0;
            ReturnPrice = 0;
            ReturnTotalPrice = 0;
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
