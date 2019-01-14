using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using His_Pos.ChromeTabViewModel;
using His_Pos.NewClass.StoreOrder;

namespace His_Pos.SYSTEM_TAB.H2_STOCK_MANAGE.ProductPurchaseReturn
{
    public class ProductPurchaseReturnViewModel : TabBase
    {
        public override TabBase getTab()
        {
            return this;
        }

        #region ----- Define Command -----
        public RelayCommand AddOrderCommand { get; set; }
        public RelayCommand DeleteOrderCommand { get; set; }
        public RelayCommand AddProductCommand { get; set; }
        public RelayCommand ToNextStatusCommand { get; set; }
        public RelayCommand AllProcessingOrderToDoneCommand { get; set; }
        #endregion

        #region ----- Define Variables -----
        public StoreOrders StoreOrderCollection { get; set; }
        public StoreOrder CurrentStoreOrder { get; set; }
        #endregion

        public ProductPurchaseReturnViewModel()
        {
            CurrentStoreOrder = new StoreOrder();

            AddOrderCommand = new RelayCommand(AddOrderAction);
            DeleteOrderCommand = new RelayCommand(DeleteOrderAction);
            ToNextStatusCommand = new RelayCommand(ToNextStatusAction);
        }

        #region ----- Define Actions -----
        private void AddOrderAction()
        {

        }
        private void DeleteOrderAction()
        {

        }
        private void ToNextStatusAction()
        {

        }
        #endregion

        #region ----- Define Functions -----

        #endregion
    }
}
