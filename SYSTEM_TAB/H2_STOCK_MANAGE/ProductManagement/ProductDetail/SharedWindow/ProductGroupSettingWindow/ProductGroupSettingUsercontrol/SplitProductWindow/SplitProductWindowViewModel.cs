using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using His_Pos.Class;
using His_Pos.FunctionWindow;
using His_Pos.NewClass.Product.ProductGroupSetting.SplitProduct;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.SYSTEM_TAB.H2_STOCK_MANAGE.ProductManagement.ProductDetail.SharedWindow.ProductGroupSettingWindow.ProductGroupSettingUsercontrol.SplitProductWindow {
   public class SplitProductWindowViewModel : ViewModelBase{
        #region Var
        private string proID;
        public string ProID
        {
            get => proID;
            set { Set(() => ProID, ref proID, value); }
        }
        private SplitProducts splitProductCollection = new SplitProducts();
        public SplitProducts SplitProductCollection
        {
            get => splitProductCollection;
            set { Set(() => SplitProductCollection, ref splitProductCollection, value); }
        }
        public RelayCommand SplitProductGroupCommand { get; set; }
        #endregion
        public SplitProductWindowViewModel(string proID) {
            ProID = proID;
            SplitProductCollection.GetDataByProID(proID);
            SplitProductGroupCommand = new RelayCommand(SplitProductGroupAction);
        }
        #region Function
        private void SplitProductGroupAction() {
            ConfirmWindow confirmWindow = new ConfirmWindow("是否拆出此商品庫存?", "拆庫確認");
            if (((bool)confirmWindow.DialogResult) == true)
            {
                //SplitProductCollection.SplitProductInventory(proID); 
                MessageWindow.ShowMessage(proID + "拆庫成功", MessageType.SUCCESS);
            }
        }
        #endregion
    }
}
