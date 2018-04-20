using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace His_Pos.Class.StoreOrder
{
    public class Category : INotifyPropertyChanged
    {
        public Category(string category = "")
        {
            switch (category)
            {
                case "進":
                    CategoryName = "進貨";
                    break;
                case "退":
                    CategoryName = "退貨";
                    break;
                case "調":
                    CategoryName = "調貨";
                    break;
                default:
                    CategoryName = "";
                    break;
            }
        }
        private string categoryName;

        public event PropertyChangedEventHandler PropertyChanged;

        public string CategoryName {
            get
            {
                return categoryName;
            }
            set
            {
                categoryName = value;
                NameChanged(value);
                NotifyPropertyChanged("CategoryName");
            }
        }

        private void NameChanged(string value)
        {
            switch (value)
            {
                case "進貨":
                    CategoryColor = Brushes.Green;
                    break;
                case "退貨":
                    CategoryColor = Brushes.Red;
                    break;
                case "調貨":
                    CategoryColor = Brushes.Blue;
                    break;
                default:
                    CategoryColor = Brushes.Transparent;
                    break;
            }
        }
        public Brush categoryColor;
        public Brush CategoryColor {
            get
            {
                return categoryColor;
            }
            set
            {
                categoryColor = value;
                NotifyPropertyChanged("CategoryColor");
            }
        }

        private void NotifyPropertyChanged(string info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }
    }
}
