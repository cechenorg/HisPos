using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using His_Pos.Class.Product;
using His_Pos.FunctionWindow;
using His_Pos.Interface;

namespace His_Pos.SYSTEM_TAB.H2_STOCK_MANAGE.InventoryManagement
{
    /// <summary>
    /// MergeStockWindow.xaml 的互動邏輯
    /// </summary>
    public partial class MergeStockWindow : Window
    {
        private IInventory Inventory ;
        public ObservableCollection<ProductGroup> ProductGroups;
        private string targetProId = string.Empty;
        public MergeStockWindow(IInventory inventory)
        {
            InitializeComponent();
            Inventory = inventory;
            InitData();
            DataContext = this;
        }
        private void InitData() {
           
            LabelSourceProduct.Content = Inventory.Id;
            LabelSourceProductName.Content = Inventory.Name;
            LabelSourceStock.Content = Inventory.StockValue;
        }

        private void LabelTargetProduct_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            //if (sender is null) return;

            //TextBox textBox = sender as TextBox;

            //if (e.Key == Key.Enter)
            //{
            //    NewItemDialog newItemDialog = new NewItemDialog(InventoryManagementView.Instance.ProductCollection, "", LabelTargetProduct.Text, Inventory.WareHouseId);
            //    LabelTargetStock.Content = "0";
            //    LabelTargetProductName.Content = string.Empty;
            //    if (newItemDialog.ConfirmButtonClicked)
            //    {
            //        LabelTargetProductName.Content = ((PurchaseProduct)newItemDialog.SelectedItem).Name.ToString();
            //        LabelTargetStock.Content = ((PurchaseProduct)newItemDialog.SelectedItem).Inventory.ToString();
            //        LabelMergeStock.Content = (Convert.ToInt32(LabelSourceStock.Content.ToString().Split('.')[0]) + Convert.ToInt32(LabelTargetStock.Content)).ToString();
            //        targetProId = ((PurchaseProduct)newItemDialog.SelectedItem).Id.ToString();
            //    }
            //}
        }

        private void ButtonSubnmmit_Click(object sender, RoutedEventArgs e)
        {
            ///ProductDb.MergeProduct(Inventory.Id, targetProId);
            MessageWindow.ShowMessage("併庫成功",Class.MessageType.SUCCESS);
            
            Close();
        }
    }
}
