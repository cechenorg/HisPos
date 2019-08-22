using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using His_Pos.Class;
using His_Pos.FunctionWindow;
using His_Pos.NewClass.Product.ProductGroupSetting;
using His_Pos.SYSTEM_TAB.H2_STOCK_MANAGE.ProductManagement.ProductDetail.SharedWindow.ProductGroupSettingWindow;

namespace His_Pos.SYSTEM_TAB.H2_STOCK_MANAGE.ProductManagement.ProductDetail.SharedControl.GroupControl
{
    public class GroupInventoryControlViewModel : ViewModelBase
    {
        #region ----- Define Commands -----
        public RelayCommand ShowProductGroupWindowCommand { get; set; }
        #endregion

        #region ----- Define Variables -----
        private string medicineID;
        private string wareHouseID;
        private double inventory;
        private ProductGroupSettings productGroupSettingCollection;
        
        public ProductGroupSettings ProductGroupSettingCollection
        {
            get { return productGroupSettingCollection; }
            set { Set(() => ProductGroupSettingCollection, ref productGroupSettingCollection, value); }
        }
        #endregion

        public GroupInventoryControlViewModel()
        {
            ShowProductGroupWindowCommand = new RelayCommand(ShowProductGroupWindowAction);
        }

        #region ----- Define Actions -----
        private void ShowProductGroupWindowAction()
        {
            if (inventory < 0)
            {
                MessageWindow.ShowMessage("欲調整商品群組 需先解決負庫問題", MessageType.ERROR);
                return;
            }

            ProductGroupSettingWindow productGroupSettingWindow = new ProductGroupSettingWindow(ProductGroupSettingCollection, wareHouseID, inventory);
            productGroupSettingWindow.ShowDialog();

            //待修
        }
        #endregion

        #region ----- Define Functions -----
        public void ReloadData(string medID, string wareID, double totalInventory)
        {
            inventory = totalInventory;
            medicineID = medID;
            wareHouseID = wareID;

            ProductGroupSettingCollection = ProductGroupSettings.GetProductGroupSettingsByID(medicineID, wareHouseID);
        }
        #endregion
    }
}
