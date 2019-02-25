using System;
using System.Collections.Generic;
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
using His_Pos.Class;
using His_Pos.FunctionWindow;

namespace His_Pos.SYSTEM_TAB.H2_STOCK_MANAGE.ProductManagement.ProductDetail
{
    /// <summary>
    /// StockTakingNoLastPriceWindow.xaml 的互動邏輯
    /// </summary>
    public partial class StockTakingNoLastPriceWindow : Window
    {
        #region ----- Define Variables -----
        public double Price { get; set; }
        public bool ConfirmClicked { get; set; } = false;
        #endregion

        public StockTakingNoLastPriceWindow()
        {
            InitializeComponent();
        }

        #region ----- Define Functions -----
        private void Confirm_OnClick(object sender, RoutedEventArgs e)
        {
            if(!IsLastPriceValid()) return;

            ConfirmClicked = true;
            Close();
        }
        private bool IsLastPriceValid()
        {
            if (Price == 0)
            {
                MessageWindow.ShowMessage("單價不可為0!", MessageType.ERROR);
                return false;
            }
            else if (Price < 0)
            {
                MessageWindow.ShowMessage("單價不可小於0!", MessageType.ERROR);
                return false;
            }

            return true;
        }
        #endregion

    }
}
