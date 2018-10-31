using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Controls;
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
        public int TotalProductCount { get; private set; }
        public string ThisPage { get; private set; }
        public string TotalPage { get; private set; }
        public string UserName { get; private set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public StockTakingDocument(List<Product> products, string userName, int totalProductCount, int thisPage, int totalPage, SortDescription sortDescription)
        {
            InitializeComponent();
            DataContext = this;
            PrintCollection = products;
            ThisPage = thisPage.ToString();
            TotalPage = totalPage.ToString();
            UserName = userName;
            TotalProductCount = totalProductCount;
            PrintData.Items.SortDescriptions.Add(sortDescription);
            PrintData.Items.SortDescriptions.Add(new SortDescription(PrintData.Columns[0].SortMemberPath, ListSortDirection.Ascending));
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
