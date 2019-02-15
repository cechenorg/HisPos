﻿using System.Windows.Controls;

namespace His_Pos.SYSTEM_TAB.H2_STOCK_MANAGE.ProductPurchaseReturn
{
    /// <summary>
    /// ProductPurchaseReturnView.xaml 的互動邏輯
    /// </summary>
    public partial class ProductPurchaseReturnView : UserControl
    {
        public ProductPurchaseReturnView()
        {
            InitializeComponent();
        }

        private void StoreOrders_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataGrid dataGrid = sender as DataGrid;
            
            if(dataGrid?.SelectedItem is null) return;

            dataGrid.ScrollIntoView(dataGrid.SelectedItem);
        }
    }
}
