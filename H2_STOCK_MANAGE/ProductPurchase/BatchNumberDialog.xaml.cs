using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using His_Pos.H2_STOCK_MANAGE.ProductPurchase.TradeControl;
using His_Pos.Struct.StoreOrder;

namespace His_Pos.H2_STOCK_MANAGE.ProductPurchase
{
    /// <summary>
    /// BatchNumberDialog.xaml 的互動邏輯
    /// </summary>
    public partial class BatchNumberDialog : Window
    {
        public Collection<ReturnControl.BatchNumOverview> BatchNumOverviews { get; set; }
        public bool IsConfirmClicked { get; set; } = false;
        
        public BatchNumberDialog(Collection<ReturnControl.BatchNumOverview> batchNumOverviews)
        {
            InitializeComponent();
            DataContext = this;

            BatchNumOverviews = batchNumOverviews;
        }

        private void Confirm_Click(object sender, RoutedEventArgs e)
        {
            IsConfirmClicked = true;
            Close();
        }
    }
}
