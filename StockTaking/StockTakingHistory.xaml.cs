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
using System.Windows.Shapes;
using His_Pos.Class.StockTakingOrder;

namespace His_Pos.StockTaking
{
    /// <summary>
    /// StockTakingHistory.xaml 的互動邏輯
    /// </summary>
    public partial class StockTakingHistory : Window, INotifyPropertyChanged
    {
        private ObservableCollection<StockTakingOverview> stockTakingOverviewCollection;

        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<StockTakingOverview> StockTakingOverviewCollection
        {
            get { return stockTakingOverviewCollection; }
            set
            {
                stockTakingOverviewCollection = value;
                NotifyPropertyChanged("StockTakingOverviewCollection");
            }
        }

        public StockTakingHistory(ObservableCollection<StockTakingOverview> stockTakingOverviews)
        {
            InitializeComponent();
            DataContext = this;
            StockTakingOverviewCollection = stockTakingOverviews;
        }

        private void NotifyPropertyChanged(string info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }

        private void StockTakingHistory_OnDeactivated(object sender, EventArgs e)
        {
            Close();
        }
    }
}
