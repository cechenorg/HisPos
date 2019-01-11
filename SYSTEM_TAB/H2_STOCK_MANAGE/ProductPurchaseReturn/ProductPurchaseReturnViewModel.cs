using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace His_Pos.SYSTEM_TAB.H2_STOCK_MANAGE.ProductPurchaseReturn
{
    public class ProductPurchaseReturnViewModel : ViewModelBase
    {
        public RelayCommand AddOrderCommand { get; set; }
        public RelayCommand DeleteOrderCommand { get; set; }
        public RelayCommand SaveOrderCommand { get; set; }
        public RelayCommand ToNextStatusCommand { get; set; }
        public RelayCommand AllProcessingOrderToDoneCommand { get; set; }

        public ProductPurchaseReturnViewModel()
        {

        }
    }
}
