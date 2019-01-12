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

        public RelayCommand AddOrderCommand { get; set; }
        public RelayCommand DeleteOrderCommand { get; set; }
        public RelayCommand SaveOrderCommand { get; set; }
        public RelayCommand ToNextStatusCommand { get; set; }
        public RelayCommand AllProcessingOrderToDoneCommand { get; set; }
        
        public StoreOrders StoreOrderCollection { get; set; }
        public StoreOrder CurrentStoreOrder { get; set; }

        public ProductPurchaseReturnViewModel()
        {
            CurrentStoreOrder = new StoreOrder();
        }
    }
}
