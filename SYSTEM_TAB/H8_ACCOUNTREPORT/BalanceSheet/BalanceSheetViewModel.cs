using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Command;
using His_Pos.ChromeTabViewModel;
using His_Pos.SYSTEM_TAB.H8_ACCOUNTREPORT.BalanceSheet.BalanceControl;

namespace His_Pos.SYSTEM_TAB.H8_ACCOUNTREPORT.BalanceSheet
{
    public class BalanceSheetViewModel: TabBase
    {
        public override TabBase getTab()
        {
            return this;
        }

        #region ----- Define ViewModels -----
        public MedPointViewModel MedPointViewModel { get; set; }
        public TransferViewModel TransferViewModel { get; set; }
        public PayableViewModel PayableViewModel { get; set; }
        public PayViewModel PayViewModel { get; set; }
        #endregion

        #region ----- Define Commands -----
        public RelayCommand<string> ChangeDetailCommand { get; set; }
        #endregion

        #region ----- Define Variables -----
        #endregion

        #region ----- Define Actions -----
        #endregion

        #region ----- Define Functions -----
        #endregion
    }
}
