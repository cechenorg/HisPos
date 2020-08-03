using MahApps.Metro.Behaviours;
using System;
using System.Collections.Generic;
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

namespace His_Pos.SYSTEM_TAB.H2_STOCK_MANAGE.ProductPurchaseReturn.NormalView
{
    /// <summary>
    /// NormalView.xaml 的互動邏輯
    /// </summary>
    public partial class NormalView : UserControl
    {
        public NormalView()
        {
            InitializeComponent();
        }

        private void StoreOrders_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataGrid dataGrid = sender as DataGrid;

            if (dataGrid?.SelectedItem is null) return;

            dataGrid.ScrollIntoView(dataGrid.SelectedItem);
        }

        public void Reload(object sender,int i)
        {
            
        }
    }
}
