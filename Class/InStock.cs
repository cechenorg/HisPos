using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using His_Pos.Struct.Product;

namespace His_Pos.Class
{
    public class InStock : INotifyPropertyChanged
    {
        private double inventory;
       

        public double Inventory
        {
            get { return inventory; }
            set
            {
                inventory = value;
                NotifyPropertyChanged("Inventory");
            }
        }

        private string safeAmount;
        public string SafeAmount
        {
            get { return safeAmount; }
            set
            {
                safeAmount = value;
                NotifyPropertyChanged("SafeAmount");
            }
        }

        private string basicAmount;

        public InStock(DataRow dataRow)
        {
            BasicAmount = dataRow["PRO_BASICQTY"].ToString();
            SafeAmount = dataRow["PRO_SAFEQTY"].ToString();
            Inventory = Double.Parse((dataRow["PRO_INVENTORY"].ToString() == "")
                ? "0"
                : dataRow["PRO_INVENTORY"].ToString());
        }

        public InStock(PurchaseProduct selectedItem)
        {
            BasicAmount = selectedItem.BasicAmount;
            SafeAmount = selectedItem.SafeAmount;
            Inventory = selectedItem.Inventory;
        }

        public string BasicAmount
        {
            get { return basicAmount; }
            set
            {
                basicAmount = value;
                NotifyPropertyChanged("BasicAmount");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }
    }
}
