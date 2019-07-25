﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using His_Pos.ChromeTabViewModel;

namespace His_Pos.SYSTEM_TAB.P1_TRANSACTION.ProductTransaction
{
    public class ProductTransactionViewModel : TabBase
    {
        public override TabBase getTab()
        {
            return this;
        }

        #region ----- Define ViewModels -----
        public ProductTransactionTradeViewModel TradeViewModel { get; set; }
        public ProductTransactionCustomerViewModel CustomerViewModel { get; set; }
        #endregion

        #region ----- Define Commands -----
        #endregion

        #region ----- Define Variables -----
        #endregion

        #region ----- Define Actions -----
        #endregion

        #region ----- Define Functions -----
        #endregion
    }
}
