using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using His_Pos.AbstractClass;

namespace His_Pos.PrintDocuments
{
    /// <summary>
    /// StockTakingDocument.xaml 的互動邏輯
    /// </summary>
    public partial class StockTakingDocument : UserControl, INotifyPropertyChanged
    {
        public string PrintDate => DateTime.Now.ToString();
        private ObservableCollection<Product> printCollection;
        public ObservableCollection<Product> PrintCollection
        {
            get { return printCollection; }
            set
            {
                printCollection = value;
                NotifyPropertyChanged("PrintCollection");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public StockTakingDocument(ObservableCollection<Product> products)
        {
            InitializeComponent();
            DataContext = this;
            PrintCollection = products;
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
