using His_Pos.Class.Manufactory;
using His_Pos.Class.Product;
using His_Pos.InventoryManagement;
using His_Pos.ProductPurchase;
using His_Pos.Struct.Product;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace His_Pos.H2_STOCK_MANAGE.InventoryManagement
{
    /// <summary>
    /// MergeStockWindow.xaml 的互動邏輯
    /// </summary>
    public partial class MergeStockWindow : Window
    {

        private string targetProId = string.Empty;
        public MergeStockWindow()
        {
            InitializeComponent();
            InitData();
            DataContext = this;
        }
        private void InitData() {
           
            LabelSourceProduct.Content = OtcDetail.Instance.InventoryOtc.Id;
            LabelSourceProductName.Content = OtcDetail.Instance.InventoryOtc.Name;
            LabelSourceStock.Content = OtcDetail.Instance.InventoryOtc.StockValue;
        }

        private void LabelTargetProduct_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (sender is null) return;

            TextBox textBox = sender as TextBox;

            if (e.Key == Key.Enter)
            {
                NewItemDialog newItemDialog = new NewItemDialog(InventoryManagementView.Instance.ProductCollection, "", LabelTargetProduct.Text, OtcDetail.Instance.InventoryOtc.WareHouse);
                LabelTargetStock.Content = "0";
                LabelTargetProductName.Content = string.Empty;
                if (newItemDialog.ConfirmButtonClicked)
                {
                    LabelTargetProductName.Content = ((PurchaseProduct)newItemDialog.SelectedItem).Name.ToString();
                    LabelTargetStock.Content = ((PurchaseProduct)newItemDialog.SelectedItem).Inventory.ToString();
                    LabelMergeStock.Content = (Convert.ToInt32(LabelSourceStock.Content.ToString().Split('.')[0]) + Convert.ToInt32(LabelTargetStock.Content)).ToString();
                    targetProId = ((PurchaseProduct)newItemDialog.SelectedItem).Id.ToString();
                }
            }
        }

        private void ButtonSubnmmit_Click(object sender, RoutedEventArgs e)
        {
            ProductDb.MergeProduct(OtcDetail.Instance.InventoryOtc.Id, targetProId);
            MessageWindow messageWindow = new MessageWindow("併庫成功",Class.MessageType.SUCCESS);
            messageWindow.ShowDialog();
            OtcDetail.Instance.UpdateUi();
            Close();
        }
    }
}
