using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.SYSTEM_TAB.H2_STOCK_MANAGE.ProductManagement.ProductDetail.SharedWindow.ProductGroupSettingWindow {
    public class ProductGroupSettingWindowViewModel : ObservableObject {
        #region Var
        private bool isSplit;
        public bool IsSpilt {
            get { return isSplit; }
            set { Set(() => IsSpilt, ref isSplit, value); }
        }
        #endregion


        public ProductGroupSettingWindowViewModel() { }
    }
}
