using System;
using GalaSoft.MvvmLight.Command;
using His_Pos.ChromeTabViewModel;

namespace His_Pos.SYSTEM_TAB.H2_STOCK_MANAGE.ProductPurchaseRecord
{
    class ProductPurchaseRecordViewModel : TabBase
    {
        public override TabBase getTab()
        {
            return this;
        }

        #region ----- Define Commands -----
        public RelayCommand SearchOrderCommand { get; set; }
        public RelayCommand FilterOrderCommand { get; set; }
        #endregion

        #region ----- Define Variables -----
        public DateTime? SearchStartDate { get; set; }
        public DateTime? SearchEndDate { get; set; }
        public string SearchOrderID { get; set; }
        public string SearchID { get; set; }
        #endregion

        #region ----- Define Actions -----
        #endregion

        #region ----- Define Functions -----
        #endregion
    }
}
