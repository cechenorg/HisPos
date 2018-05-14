using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
