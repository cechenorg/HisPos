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
using System.Windows.Navigation;
using System.Windows.Shapes;
using His_Pos.Class.StoreOrder;

namespace His_Pos.H2_STOCK_MANAGE.ProductPurchase.TradeControl
{
    /// <summary>
    /// ReturnControl.xaml 的互動邏輯
    /// </summary>
    public partial class ReturnControl : UserControl
    {
        public ReturnControl()
        {
            InitializeComponent();

            DataContext = this;
        }

        internal void SetDataContext(StoreOrder storeOrderData)
        {
            
        }

        internal void ClearControl()
        {

        }
    }
}
