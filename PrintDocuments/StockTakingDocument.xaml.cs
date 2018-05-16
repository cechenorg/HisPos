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
        public string PageNumber => ThisPage + " / " + TotalPage;
        private List<Product> printCollection;
        public List<Product> PrintCollection
        {
            get { return printCollection; }
            set
            {
                printCollection = value;
                NotifyPropertyChanged("PrintCollection");
            }
        }

        public string ThisPage { get; private set; }
        public string TotalPage { get; private set; }
        public string UserName { get; private set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public StockTakingDocument(List<Product> products, string userName, int thisPage, int totalPage)
        {
            InitializeComponent();
            DataContext = this;
            PrintCollection = products;
            ThisPage = thisPage.ToString();
            TotalPage = totalPage.ToString();
            UserName = userName;
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
