using System;
using System.ComponentModel;
using System.Data;
using His_Pos.Struct.Product;

namespace His_Pos.Class
{
    public class InStock : INotifyPropertyChanged
    {
        private string onTheWayAmount;
        public string OnTheWayAmount
        {
            get { return onTheWayAmount; }
            set
            {
                onTheWayAmount = value;
                NotifyPropertyChanged("OnTheWayAmount");
            }
        }

        private double inventory;

        public InStock() { }

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
            OnTheWayAmount = dataRow["PRO_ONTHEWAY"].ToString();
            Inventory = Double.Parse((dataRow["PRO_INVENTORY"].ToString() == "")
                ? "0"
                : dataRow["PRO_INVENTORY"].ToString());
        }

        public InStock(PurchaseProduct selectedItem)
        {
            BasicAmount = selectedItem.BasicAmount;
            SafeAmount = selectedItem.SafeAmount;
            Inventory = selectedItem.Inventory;
            OnTheWayAmount = selectedItem.OnTheWayAmount;
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
